using UnityEngine;

namespace Guinea.Core.Components
{
    public class Frame : MonoBehaviour
    {
        [SerializeField] float m_maxMotorTorque;
        [Range(0f, 1f)]
        [SerializeField] float m_maxBrakeRatio;
    }
}