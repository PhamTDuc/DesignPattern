using UnityEngine;

namespace Guinea.Core
{
    public static class Utils
    {
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