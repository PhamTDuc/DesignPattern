using UnityEngine;
using UnityEngine.InputSystem;
using Guinea.Core;
using Guinea.Core.Components;

namespace Guinea.Client
{
    public class PlayerController : MonoBehaviour
    {
        IMove m_move;
        IWeapon m_weapon;
        [SerializeField] float m_maxTorque;
        [SerializeField] float m_maxSteerAngle;
        [SerializeField] float m_maxBrakeTorque;

        void Awake()
        {
            m_move = GetComponent<IMove>();
            m_weapon = GetComponent<IWeapon>();
        }

        void OnEnable()
        {
            InputManager.Map.EntityBuilder.Disable();
            InputManager.Map.Player.Enable();
        }

        void OnDisable()
        {
            InputManager.Map.Player.Disable();
        }

        void Update()
        {
            Vector2 input = InputManager.Map.Player.Movement.ReadValue<Vector2>();
            m_move.Move(m_maxSteerAngle * input.x, m_maxBrakeTorque * input.y, 0f);
            m_weapon.AimTo(Utils.GetTargetFromMouse());
        }

        private void AimTo(InputAction.CallbackContext context)
        {
            m_weapon.AimTo(Utils.GetTargetFromMouse());
        }
    }
}