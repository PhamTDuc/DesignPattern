using UnityEngine;
using System.Collections.Generic;

namespace Guinea.Core.Components
{
    public class Entity : MonoBehaviour, IWeapon
    {
        [SerializeField] WheelCollider m_wheelColliderPrefab;
        [SerializeField] Transform m_emptyPrefab;
        [SerializeField] Transform m_wheelsCollider;
        [SerializeField] Transform m_visualWheels;
        [SerializeField] float steerAngle;
        [SerializeField] float motorTorque;
        [SerializeField] Transform m_target; // Test IWeapon.AimTo() for multiple weapons
        [SerializeField] bool m_shoot; // Test IWeapon.Shoot() for multiple weapons
        List<Wheel> m_wheels;
        List<IWeapon> m_weapons;

        void FixedUpdate()
        {
            Move(steerAngle, motorTorque, 0f);
            AimTo(m_target.transform.position);
            Shoot(m_shoot);
        }

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

        public void Move(float steerAngle, float motorTorque, float brakeTorque = 0f)
        {
            if (m_wheels == null) return;
            foreach (Wheel wheel in m_wheels)
            {
                wheel.Move(steerAngle, motorTorque, brakeTorque);
            }
        }

        public void CleanUp()
        {
            Placement[] placements = GetComponentsInChildren<Placement>();
            foreach (Placement placement in placements)
            {
                placement.gameObject.SetActive(false);
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
    }
}