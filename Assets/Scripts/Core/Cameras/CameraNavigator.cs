using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using Zenject;

namespace Guinea.Core
{
    public class CameraNavigator : MonoBehaviour, IBeginDragHandler, IDragHandler
    {
        [SerializeField] Transform m_cameraTripod;
        [SerializeField] Transform m_camera;
        SettingManager m_settingManager;
        int m_invert;

        [Inject]
        void Initialize(SettingManager settingManager)
        {
            m_settingManager = settingManager;
            Commons.Logger.Log("CameraNavigator::Initialize()");
        }

        void OnValidate()
        {
            Commons.Logger.Assert(m_cameraTripod != null && m_camera != null, "cameraTripod and camera variables must be assigned!");
        }

        void OnEnable()
        {
            InputManager.Map.EntityBuilder.CameraZoom.performed += OnCameraZoom;
        }

        void OnDisable()
        {
            InputManager.Map.EntityBuilder.CameraZoom.performed -= OnCameraZoom;
        }

        private void OnCameraZoom(InputAction.CallbackContext context)
        {
            Vector2 scrollWheel = context.ReadValue<Vector2>();
            float multiplier = 0.01f; // * ScrollWheel is Pixel/Frame, using multiplier to make ZoomStep smaller 
            Vector3 translation = new Vector3(0f, 0f, multiplier * m_settingManager.SettingValues.zoomSpeed * scrollWheel.y);
            m_camera.Translate(translation, Space.Self);
        }
        public void OnBeginDrag(PointerEventData eventData)
        {
            Plane plane = new Plane(m_camera.forward, m_camera.position);
            float cursorDis = plane.GetDistanceToPoint(eventData.pointerCurrentRaycast.worldPosition);
            float navigatorDis = plane.GetDistanceToPoint(transform.position);
            if (cursorDis < navigatorDis)
            {
                m_invert = 1;
            }
            else
            {
                m_invert = -1;
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            m_camera.RotateAround(m_cameraTripod.position, Vector3.up, m_settingManager.SettingValues.cameraSpeed * m_invert * eventData.delta.x);
        }
    }
}