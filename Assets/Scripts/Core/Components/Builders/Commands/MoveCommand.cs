using UnityEngine;
using System.Collections.Generic;


namespace Guinea.Core.Components
{
    public class MoveCommand : ICommand
    {
        ComponentBase m_component;
        Vector3 m_prevPos;
        Quaternion m_prevRot;

        // Vector3 m_currentPos;
        // Quaternion m_currentRot;

        List<ComponentBase> m_attachedComponents;

        public MoveCommand(ComponentBase component, Vector3 prevPos, Quaternion prevRot, List<ComponentBase> attachedComponents)
        {
            m_component = component;
            m_prevPos = prevPos;
            m_prevRot = prevRot;
            // m_currentPos = component.transform.position;
            // m_currentRot = component.transform.rotation;
            m_attachedComponents = attachedComponents;
        }

        public void Undo()
        {
            m_component.transform.position = m_prevPos;
            m_component.transform.rotation = m_prevRot;
            m_component.ResetAttachedComponents(m_attachedComponents);
        }
    }
}