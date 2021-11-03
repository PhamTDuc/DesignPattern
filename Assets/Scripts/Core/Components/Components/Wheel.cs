using UnityEngine;

namespace Guinea.Core.Components
{
    public class Wheel : MonoBehaviour
    {
        [SerializeField] bool m_canSteer;
        [SerializeField] bool m_hasMotor;
        [SerializeField]float m_radius;
        WheelCollider m_wheelCollider;
        Transform m_pivot;


        public void Init(WheelCollider wheelCollider, Transform pivot)
        {
            m_wheelCollider = wheelCollider;
            m_pivot = pivot;

            m_wheelCollider.radius = m_radius;
        }

        public void Move(float steerAngle, float motorTorque, float brakeTorque = 0f)
        {
            if (m_canSteer)
            {
                m_wheelCollider.steerAngle = steerAngle;
            }

            if (m_hasMotor)
            {
                m_wheelCollider.motorTorque = motorTorque;
            }

            m_wheelCollider.brakeTorque = brakeTorque;

            UpdateVisualWheel();
        }

        private void UpdateVisualWheel()
        {
            Vector3 pos;
            Quaternion rot;
            m_wheelCollider.GetWorldPose(out pos, out rot);
            // transform.position = pos;
            // transform.localRotation = m_initialOrientation * Quaternion.AngleAxis(m_wheelCollider.steerAngle, m_wheelCollider.transform.right) * Quaternion.AngleAxis(m_rpm, m_wheelCollider.transform.up);
            m_pivot.position =pos;
            m_pivot.rotation = rot;
        }
    }
}
