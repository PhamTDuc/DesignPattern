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
        [SerializeField] Vector2 m_clampPitch;
        [SerializeField] float m_smoothTime;

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
            Vector3 direction = target - m_aiming.position;

            Vector3 pitchVector = Vector3.ProjectOnPlane(direction, m_aiming.right);
            float pitch = Vector3.SignedAngle(m_aiming.forward, pitchVector, m_aiming.right);
            // m_aiming.RotateAround(m_aiming.position, m_aiming.right, pitch); // TODO: Using clamp angles
            pitch = Utils.ClampAngle(m_aiming.localEulerAngles.x + pitch, m_clampPitch.x, m_clampPitch.y);
            float velocity = 0f;
            pitch = Mathf.SmoothDampAngle(m_aiming.localEulerAngles.x, pitch, ref velocity, m_smoothTime);
            m_aiming.localEulerAngles = new Vector3(pitch, 0f, 0f);

            Vector3 yawVector = Vector3.ProjectOnPlane(direction, transform.up);
            float yaw = Vector3.SignedAngle(transform.forward, yawVector, transform.up);
            transform.RotateAround(transform.position, transform.up, yaw); // TODO: Using clamp angles
        }
    }
}