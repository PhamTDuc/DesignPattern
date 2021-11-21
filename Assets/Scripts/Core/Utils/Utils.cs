using UnityEngine;

namespace Guinea.Core
{
    public static class Utils
    {
        static readonly LayerMask s_selectableLayer = LayerMask.GetMask("Default");
        static readonly LayerMask s_placementEnvLayer = LayerMask.GetMask("Placement", "Environment");
        static readonly string s_selectableTag = "Selectable";
        static Camera s_camera;

        public static float RayCastLength => 200f;

        public static LayerMask SelectableLayer => s_selectableLayer;
        public static int PlacementEnvLayer => s_placementEnvLayer;
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

        public static int CeilingMaxMagnitude(float value)
        {
            return (int)(Mathf.Sign(value) * Mathf.CeilToInt(Mathf.Abs(value)));
        }

        public static int FloorMinMagnitude(float value)
        {
            return (int)(Mathf.Sign(value) * Mathf.FloorToInt(Mathf.Abs(value)));
        }
    }
}