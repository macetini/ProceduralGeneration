using UnityEngine;

namespace Assets.Scripts.Utils
{
    public static class Vector3Extensions
    {
        public static readonly Vector3 NaN = new(float.NaN, float.NaN, float.NaN);

        public static bool IsNaN(this Vector3 v)
        {
            var x = v.x;
            var y = v.y;
            var z = v.z;

            return float.IsNaN(x) || float.IsNaN(y) || float.IsNaN(z);
        }

        public static Vector3 RoundVec3ToInt(this Vector3 v)
        {
            return new Vector3(Mathf.RoundToInt(v.x), Mathf.RoundToInt(v.y), Mathf.RoundToInt(v.z));
        }
    }
}
