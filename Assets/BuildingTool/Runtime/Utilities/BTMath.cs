using UnityEditor;
using UnityEngine;

namespace BuildingTool.Runtime.Utilities
{
    /// <summary>
    /// Provides helper math utilities for Building Tool operations such as cursor-plane intersections.
    /// </summary>
    public static class BTMath
    {
        /// <summary>
        /// Calculates the intersection point between a GUI ray and a horizontal plane.
        /// </summary>
        /// <param name="mousePosition">Mouse position in GUI coordinates.</param>
        /// <param name="planeHeight">The Y height of the target plane.</param>
        /// <returns>The point of intersection on the plane, or Vector3.zero if none found.</returns>
        public static Vector3 IntersectMouseWithHorizontalPlane(Vector2 mousePosition, float planeHeight)
        {
            Ray ray = HandleUtility.GUIPointToWorldRay(mousePosition);
            Plane plane = new Plane(Vector3.up, new Vector3(0f, planeHeight, 0f));

            if (plane.Raycast(ray, out float enter))
            {
                return ray.GetPoint(enter);
            }

            return Vector3.zero;
        }
    }
}
