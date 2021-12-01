using UnityEngine;

namespace Guinea.Core
{
    public static class Utils
    {
        static readonly LayerMask s_selectableLayer = LayerMask.GetMask("Default");
        static readonly LayerMask s_placementEnvLayer = LayerMask.GetMask("Placement", "Environment", "Ground");
        static readonly LayerMask s_groundLayer = LayerMask.GetMask("Ground");
        static readonly string s_selectableTag = "Selectable";
        static readonly float s_lowerLimit = 4.0f;
        static Camera s_camera;

        public static float RayCastLength => 200f;

        public static LayerMask SelectableLayer => s_selectableLayer;
        public static LayerMask PlacementEnvLayer => s_placementEnvLayer;
        public static LayerMask GroundLayer => s_groundLayer;
        public static string SelectableTag => s_selectableTag;
        public static Camera Cam
        {
            get
            {
                if (s_camera == null)
                {
                    s_camera = Camera.main;
                }
                return s_camera;
            }
        }

        public static void SwitchCamera(Camera camera) => s_camera = camera;
        public static Vector3 MousePointOnScreen(float z)
        {
            Vector3 mousePos = InputManager.Map.UI.Point.ReadValue<Vector2>();
            mousePos.z = z;
            return mousePos;
        }

        public static Ray ScreenPointToRay()
        {
            Vector3 mousePos = InputManager.Map.UI.Point.ReadValue<Vector2>();
            mousePos.z = Cam.nearClipPlane;
            return Cam.ScreenPointToRay(mousePos);
        }

        public static Vector3 GetTargetFromMouse()
        {
            Vector3 mousePos = InputManager.Map.UI.Point.ReadValue<Vector2>();
            mousePos.z = Cam.nearClipPlane;
            Ray ray = Cam.ScreenPointToRay(mousePos);

            if (Physics.Raycast(ray, out RaycastHit hit, 200))
            {
                if (hit.distance > s_lowerLimit)
                {
                    return hit.point;
                }
            }
            return Cam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Cam.farClipPlane));
        }

        public static Vector3 GetTargetFromCenterCam()
        {
            Ray ray = Cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
            if (Physics.Raycast(ray, out RaycastHit hit, 200))
            {
                return hit.point;
            }
            return Cam.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, Cam.farClipPlane));
        }

        public static int CeilingMaxMagnitude(float value)
        {
            return (int)(Mathf.Sign(value) * Mathf.CeilToInt(Mathf.Abs(value)));
        }

        public static int FloorMinMagnitude(float value)
        {
            return (int)(Mathf.Sign(value) * Mathf.FloorToInt(Mathf.Abs(value)));
        }

        public static float ClampAngle(float angle, float from, float to)
        {
            float start = (from + to) * 0.5f - 180;
            float floor = Mathf.FloorToInt((angle - start) / 360) * 360;
            from += floor;
            to += floor;
            return Mathf.Clamp(angle, from, to);
        }
    }
}