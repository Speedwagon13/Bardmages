using UnityEngine;

namespace Bardmages.AI {

    /// <summary>
    /// 
    /// </summary>
    class VectorUtil {

        /// <summary>
        /// Converts a 3D vector to a 2D xz vector.
        /// </summary>
        /// <returns>The converted xz vector.</returns>
        /// <param name="vector">The 3D vector to convert.</param>
        internal static Vector2 GetDirection2D(Vector3 vector) {
            return new Vector2(vector.x, vector.z);
        }
    }
}