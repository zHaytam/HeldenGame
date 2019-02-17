using UnityEngine;

namespace Assets.Utils.Extensions
{
    public static class VectorExtensions
    {

        public static Vector3 ChangeTo(this Vector3 vector, float x, float y = float.NaN, float z = float.NaN)
        {
            vector.x = x;

            if (!float.IsNaN(y))
                vector.y = y;

            if (!float.IsNaN(z))
                vector.z = z;

            return vector;
        }

        public static Vector3 Increment(this Vector3 vector, float x, float y = float.NaN, float z = float.NaN)
        {
            vector.x += x;

            if (!float.IsNaN(y))
                vector.y += y;

            if (!float.IsNaN(z))
                vector.z += z;

            return vector;
        }

    }
}
