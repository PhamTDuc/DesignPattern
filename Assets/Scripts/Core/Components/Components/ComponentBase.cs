using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Guinea.Core.Inventory;

namespace Guinea.Core.Components
{
    public class ComponentBase : MonoBehaviour, IEnumerable, IDestructible
    {
        [SerializeField] ComponentType m_type;
        [SerializeField] ItemType m_itemType;
        [SerializeField] AttachType m_attachType;
        [SerializeField] int m_weight;
        [SerializeField] int m_maxHealth;
        [SerializeField] int m_currentHealth = 0; // TEST: [SerializeField] for testing only 
        bool isAttached = false;

        List<ComponentBase> m_attachedComponents;

        public ComponentType Type => m_type;
        public ItemType ItemType => m_itemType;
        public AttachType AttachType => m_attachType;
        public int Weight => m_weight;
        public bool IsAttached => isAttached;
        public bool HasChildren => m_attachedComponents != null && m_attachedComponents.Count > 0;

        public int MaxHealth => m_maxHealth;

        public int CurrentHealth => m_currentHealth;

        public void AddComponent(ComponentBase component)
        {
            if (m_attachedComponents == null)
            {
                m_attachedComponents = new List<ComponentBase>();
            }
            component.isAttached = true;
            m_attachedComponents.Add(component);
            Commons.Logger.Log($"ComponentBase::AddComponent(): {component.gameObject.name} to {gameObject.name}");
        }

        public void DetachAllComponents(out List<ComponentBase> childrenComponents)
        {
            if (m_attachedComponents == null)
            {
                childrenComponents = null;
                return;
            }

            foreach (ComponentBase component in m_attachedComponents)
            {
                component.isAttached = false;
            }

            childrenComponents = new List<ComponentBase>(m_attachedComponents);
            m_attachedComponents.Clear();
        }

        public IEnumerator GetEnumerator()
        {
            m_attachedComponents?.RemoveAll(component => component== null);
            return m_attachedComponents?.GetEnumerator();
        }

        public void CleanUp()
        {
            m_attachedComponents?.Clear();
            Placement[] placements = GetComponentsInChildren<Placement>();
            foreach (Placement placement in placements)
            {
                // placement.gameObject.SetActive(false);
                Destroy(placement.gameObject);
            }

            // if (m_type == ComponentType.WHEEL) // ! Colliders is used as triggered for used in Destructible instead disable it
            // {
            //     Collider[] colliders = GetComponents<Collider>();
            //     foreach (Collider collider in colliders)
            //     {
            //         collider.enabled = false;
            //     }
            // }
        }

        public void Heal(int value)
        {
            int m_prevHealth = m_currentHealth;
            m_currentHealth = Mathf.Min(m_currentHealth + value, m_maxHealth);
            Entity entity = GetComponentInParent<Entity>();
            entity.Heal(m_currentHealth - m_prevHealth);
        }

        public void GetDamage(int damage, RaycastHit hit)
        {
            int m_prevHealth = m_currentHealth;
            m_currentHealth = Mathf.Max(m_currentHealth - damage, 0);
            Entity entity = GetComponentInParent<Entity>();
            entity.GetDamage(m_prevHealth - m_currentHealth, hit);
        }
    }
}