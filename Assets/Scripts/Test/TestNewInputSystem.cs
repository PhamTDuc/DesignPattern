using UnityEngine;
using UnityEngine.InputSystem;
using Guinea.Core;

namespace Guinea.Test
{
    [RequireComponent(typeof(Rigidbody))]
    public class TestNewInputSystem : MonoBehaviour
    {
#if DEVELOPMENT
        [SerializeField] float m_maxVelocity;
        [SerializeField] bool m_blockWhenUI;

        Rigidbody m_rb;
        InputAction m_movement;


        void Awake()
        {
            m_rb = GetComponent<Rigidbody>();
            m_movement = InputManager.Map.Player.Movement;
        }

        void Update()
        {
            // if (m_blockWhenUI && EventSystem.current != null && EventSystem.current.IsPointerOverGameObject()) // * When UI overlayed, block click triggered behind 
            //     return;

            Vector2 input = m_movement.ReadValue<Vector2>();
            Vector3 direction = (input.x * transform.right + input.y * transform.forward).normalized;
            Move(direction);
        }

        void Move(Vector3 direction = default(Vector3))
        {
            m_rb.velocity = direction * m_maxVelocity;
        }
#endif
    }

}