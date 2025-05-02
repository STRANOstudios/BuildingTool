#if UNITY_EDITOR

using System;
using UnityEditor;
using UnityEngine;

namespace BuildingTool.Runtime.Utilities
{
    /// <summary>
    /// Provides intelligent anchor-based snapping for modular placement.
    /// Automatically detects nearby objects using overlap boxes and aligns anchors.
    /// </summary>
    public static class SmartSnapUtility
    {
        public enum AnchorPosition
        {
            BottomLeft,
            BottomCenter,
            BottomRight
        }

        /// <summary>
        /// Computes the snapped position of the preview object to align its closest bottom anchor
        /// with the nearest anchor of a nearby object using expanded OverlapBox detection.
        /// </summary>
        public static Vector3 ComputeSmartSnappedPosition(GameObject previewObject, float detectionRange)
        {
            if (!BTMath.TryGetOrientedOverlapBox(previewObject, out Vector3 center, out Vector3 size, out Quaternion rotation))
                return previewObject.transform.position;

            Vector3 halfExtents = size;
            halfExtents.x += detectionRange;
            halfExtents.z += detectionRange;
            halfExtents.y *= 0.9f;

            Collider[] hits = Physics.OverlapBox(center, halfExtents, rotation);
            hits = System.Array.FindAll(hits, h =>
                h != null && !IsChildOf(h.gameObject, previewObject)
            );

            if (hits.Length == 0)
                return previewObject.transform.position;

            GameObject closestTarget = null;
            float closestDistance = float.MaxValue;
            Vector3 bestPreviewAnchor = Vector3.zero;
            Vector3 bestTargetAnchor = Vector3.zero;

            Bounds previewBounds = BTMath.GetLocalBounds(previewObject);
            Vector3[] previewAnchors = GetAnchorWorldPoints(previewObject, previewBounds);

            foreach (Collider hit in hits)
            {
                GameObject target = GetPrefabRoot(hit.gameObject);
                if (target == previewObject || !target.activeInHierarchy)
                    continue;

                Bounds targetBounds = BTMath.GetLocalBounds(target);
                Vector3[] targetAnchors = GetAnchorWorldPoints(target, targetBounds);

                foreach (Vector3 pa in previewAnchors)
                {
                    foreach (Vector3 ta in targetAnchors)
                    {
                        float dist = Vector3.Distance(pa, ta);
                        if (dist < closestDistance)
                        {
                            closestDistance = dist;
                            closestTarget = target;
                            bestPreviewAnchor = pa;
                            bestTargetAnchor = ta;
                        }
                    }
                }
            }

            if (closestTarget == null)
                return previewObject.transform.position;

            Vector3 offset = previewObject.transform.position - bestPreviewAnchor;
            Vector3 finalPosition = bestTargetAnchor + offset;
            finalPosition.y = previewObject.transform.position.y;

            return finalPosition;
        }

        private static GameObject GetPrefabRoot(GameObject go)
        {
            var prefabRoot = PrefabUtility.GetOutermostPrefabInstanceRoot(go);
            if (prefabRoot != null)
                return prefabRoot;

            return go.transform.root.gameObject;
        }

        private static Vector3[] GetAnchorWorldPoints(GameObject obj, Bounds localBounds)
        {
            Transform t = obj.transform;
            Vector3[] anchors = new Vector3[3];

            Vector3 extents = localBounds.extents;
            Vector3 center = localBounds.center;

            Vector3 offsetBL = new Vector3(-extents.x, 0, 0);
            Vector3 offsetBC = Vector3.zero;
            Vector3 offsetBR = new Vector3(extents.x, 0, 0);

            anchors[0] = t.TransformPoint(offsetBL);
            anchors[1] = t.TransformPoint(offsetBC);
            anchors[2] = t.TransformPoint(offsetBR);

            return anchors;
        }

        /// <summary>
        /// Checks whether the specified GameObject is a descendant of the given potential parent.
        /// This includes direct and indirect (nested) children within the hierarchy.
        /// </summary>
        /// <param name="obj">The GameObject to test for hierarchy relationship.</param>
        /// <param name="potentialParent">The GameObject that may be an ancestor in the hierarchy.</param>
        /// <returns>True if <paramref name="obj"/> is a child (or nested child) of <paramref name="potentialParent"/>; otherwise, false.</returns>

        public static bool IsChildOf(GameObject obj, GameObject potentialParent)
        {
            Transform current = obj.transform;
            while (current != null)
            {
                if (current == potentialParent.transform)
                    return true;
                current = current.parent;
            }
            return false;
        }

        /// <summary>
        /// Draws the overlap detection box for debug purposes.
        /// </summary>
        public static void DrawSmartSnapBox(GameObject previewObject, float expandXZ = 1.5f)
        {
            if (!BTMath.TryGetOrientedOverlapBox(previewObject, out Vector3 center, out Vector3 size, out Quaternion rotation))
                return;

            Vector3 adjusted = size;
            adjusted.x += expandXZ;
            adjusted.z += expandXZ;
            adjusted.y *= 0.9f;

            Handles.color = Color.cyan;
            using (new Handles.DrawingScope(Matrix4x4.TRS(center, rotation, Vector3.one)))
            {
                Handles.DrawWireCube(Vector3.zero, adjusted);
            }
        }
    }
}

#endif
