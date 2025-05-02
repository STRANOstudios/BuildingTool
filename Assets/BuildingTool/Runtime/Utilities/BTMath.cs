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

        /// <summary>
        /// Computes an oriented overlap box (position, size, rotation) based on all child renderers of a GameObject.
        /// The bounds are returned slightly reduced to avoid false overlaps.
        /// </summary>
        /// <param name="root">The root GameObject.</param>
        /// <param name="center">World-space center of the overlap box.</param>
        /// <param name="size">World-space size of the overlap box.</param>
        /// <param name="rotation">Rotation of the box.</param>
        /// <param name="shrinkFactor">Factor to reduce bounds size (default = 0.90).</param>
        /// <returns>True if successful, false if object has no mesh bounds.</returns>
        public static bool TryGetOrientedOverlapBox(GameObject root, out Vector3 center, out Vector3 size, out Quaternion rotation, float shrinkFactor = 0.90f)
        {
            center = Vector3.zero;
            size = Vector3.one;
            rotation = Quaternion.identity;

            if (root == null)
                return false;

            Renderer[] renderers = root.GetComponentsInChildren<Renderer>();
            if (renderers.Length == 0)
                return false;

            Bounds localBounds = new Bounds(Vector3.zero, Vector3.zero);
            bool initialized = false;

            foreach (Renderer renderer in renderers)
            {
                Transform t = renderer.transform;
                MeshFilter mf = renderer.GetComponent<MeshFilter>();
                if (mf == null || mf.sharedMesh == null)
                    continue;

                Bounds meshBounds = mf.sharedMesh.bounds;
                Vector3 localCenter = root.transform.InverseTransformPoint(t.TransformPoint(meshBounds.center));
                Vector3 localSize = Vector3.Scale(meshBounds.size, t.lossyScale);

                Bounds transformed = new Bounds(localCenter, localSize);

                if (!initialized)
                {
                    localBounds = transformed;
                    initialized = true;
                }
                else
                {
                    localBounds.Encapsulate(transformed);
                }
            }

            if (!initialized)
                return false;

            center = root.transform.TransformPoint(localBounds.center);
            size = localBounds.size * shrinkFactor;
            rotation = root.transform.rotation;

            return true;
        }

        /// <summary>
        /// Computes the local bounds of the given GameObject, considering all its child renderers.
        /// </summary>
        /// <param name="go">GameObject to evaluate.</param>
        /// <returns>Combined local bounds.</returns>
        public static Bounds GetLocalBounds(GameObject go)
        {
            Renderer[] renderers = go.GetComponentsInChildren<Renderer>();
            if (renderers.Length == 0)
                return new Bounds(Vector3.zero, Vector3.zero);

            Transform root = go.transform;
            Bounds localBounds = new Bounds(Vector3.zero, Vector3.zero);
            bool initialized = false;

            foreach (Renderer renderer in renderers)
            {
                Transform t = renderer.transform;
                MeshFilter mf = renderer.GetComponent<MeshFilter>();
                if (mf == null || mf.sharedMesh == null)
                    continue;

                Bounds meshBounds = mf.sharedMesh.bounds;
                Vector3 localCenter = root.transform.InverseTransformPoint(t.TransformPoint(meshBounds.center));
                Vector3 localSize = Vector3.Scale(meshBounds.size, t.lossyScale);

                Bounds transformed = new Bounds(localCenter, localSize);

                if (!initialized)
                {
                    localBounds = transformed;
                    initialized = true;
                }
                else
                {
                    localBounds.Encapsulate(transformed);
                }
            }

            return localBounds;
        }

    }
}
