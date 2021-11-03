using System.Collections;
using UnityEngine;

namespace Guinea.Core.Components
{
    public class Turret : MonoBehaviour, IWeapon
    {
        [SerializeField] Ammunition m_ammunition;
        [SerializeField] ParticleSystem m_muzzleFlash;
        [SerializeField] LayerMask m_shootLayer;
        [SerializeField] Transform m_aiming;

        float m_timer = 0f;
        bool m_isReloading = false;
        bool m_canShoot = true;

        public void Shoot(bool pressed)
        {
            if (!m_ammunition.allowHolding && !pressed)
            {
                m_canShoot = true;
            }

            if (pressed && !m_isReloading && Time.time >= m_timer)
            {
                if (m_canShoot)
                {
                    m_timer = Time.time + 1.0f / m_ammunition.fireRate;
                    InternalShoot();
                    if (!m_ammunition.allowHolding) m_canShoot = false;
                }
            }
        }

        public void Reload()
        {
            StopAllCoroutines();
            StartCoroutine(InternalReloadCoroutine());
        }

        void InternalShoot()
        {
            // m_recoil?.GenerateRecoil();
            if (m_muzzleFlash != null)
            {
                m_muzzleFlash.Play();
            }

            if (Physics.Raycast(m_aiming.position, m_aiming.forward, out RaycastHit hit, m_ammunition.maxRange, m_shootLayer))
            {
                IDestructible destructible = hit.collider.GetComponent<IDestructible>();
                destructible?.GetDamage(m_ammunition.maxDamage, hit);
            }
        }

        IEnumerator InternalReloadCoroutine()
        {
            m_isReloading = true;
            yield return new WaitForSeconds(m_ammunition.reloadTime);
            m_isReloading = false;
        }
        public void AimTo(Vector3 target)
        {
            Vector3 direction = transform.root.InverseTransformDirection(target - m_aiming.position);
            Vector3 right = Vector3.Cross(direction, Vector3.up);
            Vector3 up = Vector3.Cross(direction, right);
            Quaternion rotation = Quaternion.LookRotation(direction, up);

            Quaternion aiming_rot = Quaternion.Euler(rotation.eulerAngles.x, 0f, 0f);
            Quaternion base_rot = Quaternion.Euler(0f, rotation.eulerAngles.y, 0f);

            m_aiming.localRotation = aiming_rot;
            transform.localRotation = base_rot;
        }
    }
}