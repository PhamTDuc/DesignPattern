using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Guinea.Core.Inventory;

namespace Guinea.Core.Components
{
    public class ComponentBase : MonoBehaviour, IEnumerable
    {
        [SerializeField] ComponentType m_type;
        [SerializeField] ItemType m_itemType;
        [SerializeField] AttachType m_attachType;
        [SerializeField] float m_weight;

        List<ComponentBase> m_attachedComponents;
        // event Action OnDetach;

        public ComponentType Type => m_type;
        public ItemType ItemType => m_itemType;
        public AttachType AttachType => m_attachType;
        public float Weight => m_weight;
        public void AddComponent(ComponentBase component)
        {
            if (m_attachedComponents == null)
            {
                m_attachedComponents = new List<ComponentBase>();
            }
            m_attachedComponents.Add(component);
            // OnDetach += component.OnDetachCallback;
            Commons.Logger.Log($"ComponentBase::AddComponent(): {component.gameObject.name} to {gameObject.name}");
        }

        public void DetachAllComponents(out List<ComponentBase> childrenComponents)
        {
            if (m_attachedComponents == null)
            {
                childrenComponents = null;
                return;
            }

            // OnDetach?.Invoke();
            // OnDetach = null;
            childrenComponents = new List<ComponentBase>(m_attachedComponents);
            m_attachedComponents.Clear();
        }

        public void ResetAttachedComponents(List<ComponentBase> newAttachedComponents)
        {
            m_attachedComponents = newAttachedComponents;
        }

        void OnDetachCallback()
        {
            Commons.Logger.Log($"Detach component {gameObject.name}");
        }

        public IEnumerator GetEnumerator()
        {
            return m_attachedComponents?.GetEnumerator();
        }
    }
}