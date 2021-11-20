using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Guinea.Core.Components
{
    public class MoveOperator : MonoBehaviour, IOperator
    {
        [SerializeField] LayerMask m_placementLayer;
        Vector3 m_offset;
        float m_z;
        Camera m_camera;
        Vector3 m_prevPos;
        Quaternion m_prevRot;
        Placement m_cachePlacement;
        Placement[] m_objectPlacements;
        ComponentBase m_componentBase;
        List<ComponentBase> m_attachedComponents;

        SharedInteraction m_sharedInteraction;

        [Inject]
        void Initialize(SharedInteraction sharedInteraction)
        {
            m_sharedInteraction = sharedInteraction;
        }

        void Awake()
        {
            m_camera = Camera.main;
        }

        public IOperator.Result Invoke(Context context)
        {
            m_z = m_camera.WorldToScreenPoint(context.@object.position).z;
            m_offset = context.@object.position - m_camera.ScreenToWorldPoint(MousePointOnScreen());

            m_prevPos = context.@object.position;
            m_prevRot = context.@object.rotation;

            m_objectPlacements = context.@object.GetComponentsInChildren<Placement>();
            m_componentBase = context.@object.GetComponent<ComponentBase>();
            SetActiveObjectPlacements(false);

            return IOperator.Result.RUNNING_MODAL;
        }

        public IOperator.Result Execute(Context context)
        {
            if (!InputManager.Map.EntityBuilder.Point.triggered)
            {
                return IOperator.Result.RUNNING_MODAL;
            }

            Ray ray = m_camera.ScreenPointToRay(MousePointOnScreen());

            if (Physics.Raycast(ray, out RaycastHit hit, 100f, m_placementLayer))
            {
                Placement placement = hit.collider.GetComponent<Placement>();
                if (placement != null && placement.AttachType == m_componentBase.AttachType)
                {
                    if (placement != m_cachePlacement)
                    {
                        m_cachePlacement = placement;
                        m_sharedInteraction.SetGridViewFromPlacement(placement);
                    }
                    context.@object.position = placement.GetWorldPosition(hit.point);
                    context.@object.rotation = placement.Rotation;
                    m_sharedInteraction.ChangeSelectedColor(Color.green);
                }
                else // *Can hit Ground
                {
                    context.@object.position = hit.point;
                    context.@object.rotation = m_prevRot;
                    m_cachePlacement = null;
                    m_sharedInteraction.HideGridView();
                    m_sharedInteraction.ChangeSelectedColor(Color.red);
                }
            }
            else
            {
                context.@object.position = m_offset + m_camera.ScreenToWorldPoint(MousePointOnScreen());
                context.@object.rotation = m_prevRot;
                m_cachePlacement = null;
                m_sharedInteraction.HideGridView();
                m_sharedInteraction.ChangeSelectedColor(Color.red);
            }

            return IOperator.Result.RUNNING_MODAL;
        }

        public ICommand Finish(Context context)
        {
            ComponentBase childComponent = context.@object.GetComponent<ComponentBase>();

            m_sharedInteraction.SetEntityRoot(childComponent);
            if (m_cachePlacement != null)
            {
                ComponentBase component = m_cachePlacement.GetComponentInParent<ComponentBase>();
                if (component != null && childComponent != null)
                {
                    component.AddComponent(childComponent);
                }

                m_cachePlacement = null;
                m_sharedInteraction.HideGridView();
            }

            SetActiveObjectPlacements(true);
            m_objectPlacements = null;
            m_componentBase.DetachAllComponents(out m_attachedComponents);
            return new MoveCommand(childComponent, m_prevPos, m_prevRot, m_attachedComponents);
        }

        public IOperator.Result Cancel(Context context)
        {
            context.@object.position = m_prevPos;
            context.@object.rotation = m_prevRot;
            m_cachePlacement = null;
            m_sharedInteraction.HideGridView();
            SetActiveObjectPlacements(true);
            m_objectPlacements = null;
            return IOperator.Result.CANCELLED;
        }

        private void SetActiveObjectPlacements(bool enabled)
        {
            if (m_objectPlacements != null)
            {
                foreach (Placement placement in m_objectPlacements)
                {
                    placement.gameObject.SetActive(enabled);
                }
            }
        }

        private Vector3 MousePointOnScreen()
        {
            Vector3 mousePos = InputManager.Map.EntityBuilder.Point.ReadValue<Vector2>();
            mousePos.z = m_z;
            return mousePos;
        }
    }
}