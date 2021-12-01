using UnityEngine;
using System.Collections.Generic;

namespace Guinea.Core.Components
{
    public class Entity : MonoBehaviour, IWeapon, IMove, IDestructible
    {
        [SerializeField] WheelCollider m_wheelColliderPrefab;
        [SerializeField] Transform m_emptyPrefab;
        [SerializeField] Transform m_wheelsCollider;
        [SerializeField] Transform m_visualWheels;
        [SerializeField] bool m_shoot; // TEST: IWeapon.Shoot() for multiple weapons
        List<Wheel> m_wheels;
        List<IWeapon> m_weapons;
        [SerializeField] int m_maxHealth;// TEST: [SerializeField] for testing only 
        [SerializeField] int m_currentHealth;// TEST: [SerializeField] for testing only 

        public int MaxHealth => m_maxHealth;
        public int CurrentHealth => m_currentHealth;

        private void AddWheel(ComponentBase component)
        {
            Wheel wheel = component.GetComponent<Wheel>();
            WheelCollider wheelCollider = Instantiate(m_wheelColliderPrefab, wheel.transform.position, transform.rotation);
            wheelCollider.transform.SetParent(m_wheelsCollider);

            Transform pivot = Instantiate(m_emptyPrefab, wheel.transform.position, transform.rotation);
            pivot.SetParent(m_visualWheels);
            wheel.transform.SetParent(pivot);
            wheel.Init(wheelCollider, pivot);

            if (m_wheels == null)
            {
                m_wheels = new List<Wheel>();
            }
            m_wheels.Add(wheel);
        }

        private void AddWeapon(ComponentBase component)
        {
            IWeapon weapon = component.GetComponent<IWeapon>();
            if (m_weapons == null)
            {
                m_weapons = new List<IWeapon>();
            }

            component.transform.SetParent(transform);
            m_weapons.Add(weapon);
        }

        public void HandleComponent(ComponentBase component)
        {
            switch (component.Type)
            {
                case ComponentType.WHEEL:
                    AddWheel(component);
                    break;
                case ComponentType.WEAPON:
                    AddWeapon(component);
                    break;
                default:
                    break;
            }
        }

        public void Init()
        {
            // * Setup Health
            ComponentBase[] components = GetComponentsInChildren<ComponentBase>();
            foreach (ComponentBase component in components)
            {
                m_maxHealth += component.MaxHealth;
                component.Heal(component.MaxHealth);
            }
        }

        public void Move(float steerAngle, float motorTorque, float brakeTorque = 0f)
        {
            if (m_wheels == null) return;
            foreach (Wheel wheel in m_wheels)
            {
                wheel.Move(steerAngle, motorTorque, brakeTorque);
            }
        }
        public void Shoot(bool pressed)
        {
            if (m_weapons == null) return;
            foreach (IWeapon weapon in m_weapons)
            {
                weapon.Shoot(pressed);
            }
        }

        public void Reload()
        {
            if (m_weapons == null) return;
            foreach (IWeapon weapon in m_weapons)
            {
                weapon.Reload();
            }
        }

        public void AimTo(Vector3 target)
        {
            if (m_weapons == null) return;
            foreach (IWeapon weapon in m_weapons)
            {
                weapon.AimTo(target);
            }
        }

        public void Heal(int value)
        {
            m_currentHealth = Mathf.Min(m_currentHealth + value, m_maxHealth);
        }

        public void GetDamage(int damage, RaycastHit hit)
        {
            m_currentHealth = Mathf.Max(m_currentHealth - damage, 0);
        }
    }
}