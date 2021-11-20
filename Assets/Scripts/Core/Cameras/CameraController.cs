using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Guinea.Core
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField] float m_speed;
        [SerializeField] float m_sensitivity;
        [SerializeField] Vector2 m_clampX;
        [SerializeField] bool m_invertX;
        [SerializeField] Vector2 m_clampY;
        [SerializeField] bool m_invertY;
        [SerializeField] bool m_clamp;
        [SerializeField] LayerMask m_collider;


        int m_invertFactorX;
        int m_invertFactorY;
        InputAction m_cameraNavigate;
        void Awake()
        {
            m_cameraNavigate = InputManager.Map.EntityBuilder.CameraNavigate;
            m_invertFactorX = m_invertX ? -1 : 1;
            m_invertFactorY = m_invertY ? -1 : 1;
        }

        void OnEnable()
        {
            InputManager.Map.EntityBuilder.CameraLook.performed += OnCameraLook;
            // m_cameraNavigate.Enable();
        }


        void OnDisable()
        {
            InputManager.Map.EntityBuilder.CameraLook.performed -= OnCameraLook;
            // m_cameraNavigate.Disable();
        }

        void Update()
        {
            Vector2 navigateValue = m_cameraNavigate.ReadValue<Vector2>();
            Vector3 direction = navigateValue.x * transform.right + navigateValue.y * transform.forward;
            if (direction.magnitude != 0f && !Physics.SphereCast(transform.position, 0.25f, direction, out RaycastHit hit, .25f, m_collider))
            {
                transform.Translate(m_speed * direction * Time.deltaTime, Space.World);
            }
        }

        private void CameraLook(Vector2 mouseDelta)
        {
            Vector3 cameraRotationEuler = transform.localRotation.eulerAngles;
            cameraRotationEuler += m_sensitivity * new Vector3(m_invertFactorX * mouseDelta.y, m_invertFactorY * mouseDelta.x, 0f);
            if (m_clamp)
            {
                cameraRotationEuler.x = ClampAngle(cameraRotationEuler.x, m_clampX.x, m_clampX.y);
                cameraRotationEuler.y = ClampAngle(cameraRotationEuler.y, m_clampY.x, m_clampY.y);
            }
            // transform.localRotation = Quaternion.Euler(cameraRotationEuler);
            transform.localEulerAngles = cameraRotationEuler;
        }

        private void OnCameraLook(InputAction.CallbackContext context)
        {
            Vector2 mouseDelta = InputManager.Map.EntityBuilder.CameraLook.ReadValue<Vector2>();
            CameraLook(mouseDelta);
        }

        static float ClampAngle(float angle, float from, float to)
        {
            // accepts e.g. -80, 80
            if (angle < 0f) angle = 360 + angle;
            if (angle > 180f) return Mathf.Max(angle, 360 + from);
            return Mathf.Min(angle, to);
        }
    }
}