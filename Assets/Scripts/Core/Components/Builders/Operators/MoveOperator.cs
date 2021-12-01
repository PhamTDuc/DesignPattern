using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace Guinea.Core.Components
{
    public class MoveOperator : IOperator
    {
        Vector3 m_offset;
        float m_z;
        Vector3 m_prevPos;
        Quaternion m_prevRot;
        Vector3 m_currentPos;
        Quaternion m_currentRot;
        Placement m_cachePlacement;
        Placement[] m_objectPlacements;
        ComponentBase m_componentBase;
        Transform m_obj;
        List<ComponentBase> m_attachedComponents;
        private static SharedInteraction s_sharedInteraction;
        private static Collider s_groundCollider;
        private static Option[] options = { Option.UNDO };
        public override Option[] Options => options;

        [Inject]
        void Initialize(SharedInteraction sharedInteraction, Collider groundCollider)
        {
            Commons.Logger.Assert(s_sharedInteraction == null, "s_sharedInteraction is already initialized!");
            Commons.Logger.Assert(s_groundCollider == null, "s_groundCollider is already initialized!");
            s_sharedInteraction = sharedInteraction;
            s_groundCollider = groundCollider;
        }

        // * CleanUp when and preventing Assertion Error when switching Scene
        void OnDestroy()
        {
            s_sharedInteraction = null;
            s_groundCollider = null;
            m_attachedComponents = null;
            m_objectPlacements = null;
        }

        public override IOperator.Result Invoke(Context context, InputActionMap ev)
        {
            m_z = Utils.Cam.WorldToScreenPoint(context.@object.position).z;
            m_offset = context.@object.position - Utils.Cam.ScreenToWorldPoint(Utils.MousePointOnScreen(m_z));
            m_prevPos = context.@object.position;
            m_prevRot = context.@object.rotation;

            m_objectPlacements = context.@object.GetComponentsInChildren<Placement>(true);
            m_componentBase = context.@object.GetComponent<ComponentBase>();
            SetActiveObjectPlacements(false);
            Debug.Log($"MoveOperator::Invoke(): {context.@object}");

            return IOperator.Result.RUNNING_MODAL;
        }

        public override IOperator.Result Modal(Context context, InputActionMap ev)
        {
            if (ev["Select"].triggered)
            {
                Debug.Log("MoveOperator::FINISHED!!");
                return IOperator.Result.FINISHED;
            }

            if (ev["Unselect"].triggered)
            {
                Debug.Log("MoveOperator::CANCELLED!!");
                return IOperator.Result.CANCELLED;
            }

            Ray ray = Utils.Cam.ScreenPointToRay(Utils.MousePointOnScreen(m_z));

            if (Physics.Raycast(ray, out RaycastHit hit, 100f, Utils.PlacementEnvLayer))
            {
                Placement placement = hit.collider.GetComponent<Placement>();
                if (placement != null && placement.AttachType == m_componentBase.AttachType)
                {
                    if (placement != m_cachePlacement)
                    {
                        m_cachePlacement = placement;
                        s_sharedInteraction.SetGridViewFromPlacement(placement);
                    }
                    m_currentPos = placement.GetWorldPosition(hit.point);
                    m_currentRot = placement.Rotation;
                    s_sharedInteraction.ChangeSelectedColor(Color.green);
                }
                else // *Can hit Ground
                {
                    m_currentPos = hit.point;
                    m_currentRot = m_prevRot;
                    m_cachePlacement = null;
                    s_sharedInteraction.HideGridView();
                    s_sharedInteraction.ChangeSelectedColor(Color.red);
                }
            }
            else
            {
                m_currentPos = m_offset + Utils.Cam.ScreenToWorldPoint(Utils.MousePointOnScreen(m_z));
                m_currentRot = m_prevRot;
                m_cachePlacement = null;
                s_sharedInteraction.HideGridView();
                s_sharedInteraction.ChangeSelectedColor(Color.red);
            }
            context.@object.position = m_currentPos;
            context.@object.rotation = m_currentRot;
            return IOperator.Result.RUNNING_MODAL;
        }

        public override IOperator.Result Execute(Context context)
        {
            m_obj = context.@object;
            context.@object.position = m_currentPos;
            context.@object.rotation = m_currentRot;
            ComponentBase childComponent = context.@object.GetComponent<ComponentBase>();

            s_sharedInteraction.SetEntityRoot(childComponent);
            if (m_cachePlacement != null)
            {
                ComponentBase component = m_cachePlacement.GetComponentInParent<ComponentBase>();
                if (component != null && childComponent != null)
                {
                    component.AddComponent(childComponent);
                }

                m_cachePlacement = null;
                s_sharedInteraction.HideGridView();
            }

            SetActiveObjectPlacements(true);
            m_objectPlacements = null;
            m_componentBase.DetachAllComponents(out m_attachedComponents);
            ResetComponentPosition();
            Debug.Log("MoveOperator::Execute()");
            return IOperator.Result.FINISHED;
        }

        public override IOperator.Result Cancel(Context context)
        {
            m_obj.position = m_prevPos;
            m_obj.rotation = m_prevRot;
            m_cachePlacement = null;
            s_sharedInteraction.HideGridView();
            SetActiveObjectPlacements(true);
            m_objectPlacements = null;
            return IOperator.Result.CANCELLED;
        }

        private void SetActiveObjectPlacements(bool enabled)
        {
            if (m_objectPlacements != null)
            {
                // Debug.Log($"MoveOperator::SetActivePlacement({enabled})");
                foreach (Placement placement in m_objectPlacements)
                {
                    placement.gameObject.SetActive(enabled);
                }
            }
        }

        // * Call this method to make Entity is on always on ground
        private void ResetComponentPosition()
        {
            ComponentBase[] components = s_sharedInteraction.ComponentsContainer.GetComponentsInChildren<ComponentBase>()
            .Where(component => component.IsAttached == true || component.HasChildren).ToArray();
            if (components.Length == 0)
            {
                return;
            }

            Bounds bounds = new Bounds(components[0].transform.position, Vector3.zero);
            foreach (ComponentBase component in components)
            {
                Renderer[] renderers = component.GetComponentsInChildren<Renderer>();
                foreach (Renderer renderer in renderers)
                {
                    bounds.Encapsulate(renderer.bounds);
                }
            }

            Vector3 startPoint = bounds.center;
            startPoint.y += bounds.extents.y;
            Ray ray = new Ray(startPoint, Vector3.down);

            if (Physics.Raycast(ray, out RaycastHit hit, Utils.RayCastLength, Utils.GroundLayer))
            {
                float moveDistance = hit.distance - bounds.size.y;
                foreach (ComponentBase component in components)
                {
                    component.transform.Translate(moveDistance * Vector3.down, Space.World);
                }
                Commons.Logger.Log($"MoveOperator::ResetComponentPosition(): Movedistance {moveDistance}");
            }
        }

    }
}