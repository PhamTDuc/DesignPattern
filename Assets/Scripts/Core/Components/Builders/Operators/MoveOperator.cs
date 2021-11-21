using System.Collections.Generic;
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
        Placement m_cachePlacement;
        Placement[] m_objectPlacements;
        ComponentBase m_componentBase;
        Transform m_obj;
        List<ComponentBase> m_attachedComponents;

        private static SharedInteraction s_sharedInteraction;
        private static Option[] options = { Option.UNDO };
        public override Option[] Options => options;

        public static void Initialize(SharedInteraction sharedInteraction)
        {
            s_sharedInteraction = sharedInteraction;
        }

        public override IOperator.Result Invoke(Context context, InputActionMap ev)
        {
            m_z = Utils.Cam.WorldToScreenPoint(context.@object.position).z;
            m_offset = context.@object.position - Utils.Cam.ScreenToWorldPoint(Utils.MousePointOnScreen(m_z));
            m_obj = context.@object;
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
                Debug.Log("FINISHED!!");
                return IOperator.Result.FINISHED;
            }

            if (ev["Unselect"].triggered)
            {
                Debug.Log("CANCELLED!!");
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
                    context.@object.position = placement.GetWorldPosition(hit.point);
                    context.@object.rotation = placement.Rotation;
                    s_sharedInteraction.ChangeSelectedColor(Color.green);
                }
                else // *Can hit Ground
                {
                    context.@object.position = hit.point;
                    context.@object.rotation = m_prevRot;
                    m_cachePlacement = null;
                    s_sharedInteraction.HideGridView();
                    s_sharedInteraction.ChangeSelectedColor(Color.red);
                }
            }
            else
            {
                context.@object.position = m_offset + Utils.Cam.ScreenToWorldPoint(Utils.MousePointOnScreen(m_z));
                context.@object.rotation = m_prevRot;
                m_cachePlacement = null;
                s_sharedInteraction.HideGridView();
                s_sharedInteraction.ChangeSelectedColor(Color.red);
            }

            return IOperator.Result.RUNNING_MODAL;
        }

        public override IOperator.Result Execute(Context context)
        {
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
                Debug.Log($"MoveOperator::SetActivePlacement({enabled})");
                foreach (Placement placement in m_objectPlacements)
                {
                    placement.gameObject.SetActive(enabled);
                }
            }
        }
    }
}