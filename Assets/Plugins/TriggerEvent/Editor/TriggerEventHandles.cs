#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using PsychoGarden.Utils;

namespace PsychoGarden.TriggerEvents
{
    /// <summary>
    /// Utility class for smart drawing of TriggerEvent connections in the scene.
    /// </summary>
    public static class TriggerEventHandles
    {
        private static Dictionary<Transform, List<Transform>> targetToOrigins = new();

        private const float parallelTolerance = 0.1f; // Tolerance to consider "parallel" origins (in meters)

        /// <summary>
        /// Rebuilds the internal mapping from targets to origins.
        /// </summary>
        /// <param name="triggerEventsCache">The list of (owner, triggerEvent, cachedColor) tuples.</param>
        public static void RebuildTargetMapping(List<(GameObject owner, TriggerEvent triggerEvent, Color cachedColor)> triggerEventsCache)
        {
            targetToOrigins.Clear();

            foreach (var (owner, triggerEvent, _) in triggerEventsCache)
            {
                if (triggerEvent == null || owner == null)
                    continue;

                int count = triggerEvent.GetPersistentEventCount();
                for (int i = 0; i < count; i++)
                {
                    var targetObj = triggerEvent.GetPersistentTarget(i);
                    Transform targetTransform = SafeGetTargetTransform(targetObj);
                    if (targetTransform == null)
                        continue;

                    if (!targetToOrigins.ContainsKey(targetTransform))
                        targetToOrigins[targetTransform] = new List<Transform>();

                    targetToOrigins[targetTransform].Add(owner.transform);
                }
            }
        }

        /// <summary>
        /// Draws the connections from a TriggerEvent's origin Transform to all its persistent targets.
        /// Automatically decides between drawing straight lines or Bezier curves based on connections.
        /// </summary>
        /// <param name="triggerEvent">The TriggerEvent instance containing persistent targets.</param>
        /// <param name="origin">The Transform that acts as the starting point of the connections.</param>
        public static void DrawConnectionGizmos(TriggerEvent triggerEvent, Transform origin)
        {
            if (triggerEvent == null || origin == null)
                return;

            int count = triggerEvent.GetPersistentEventCount();
            if (count == 0)
                return;

            for (int i = 0; i < count; i++)
            {
                var targetObj = triggerEvent.GetPersistentTarget(i);
                Transform targetTransform = SafeGetTargetTransform(targetObj);
                if (targetTransform == null)
                    continue;

                // Draw a smart connection from origin to the resolved target
                DrawSmartConnection(origin, targetTransform, triggerEvent.editorColor);
            }
        }

        /// <summary>
        /// Draws a smart connection between two points.
        /// Tangents rotate around the origin-target axis in regular angular steps.
        /// Supports configurable angleStep and tangent distance.
        /// </summary>
        /// <param name="origin">The origin Transform.</param>
        /// <param name="target">The target Transform.</param>
        /// <param name="color">The color of the connection.</param>
        /// <param name="angleStep">Degrees between each connection (default 30).</param>
        /// <param name="baseTangentDistance">Base tangent offset factor (default 0.15).</param>
        public static void DrawSmartConnection(
            Transform origin,
            Transform target,
            Color color,
            float angleStep = 30f,
            float baseTangentDistance = 0.05f)
        {
            if (!origin || !target)
                return;

            Vector3 originPos = origin.position;
            Vector3 targetPos = target.position;

            Handles.color = color;

            if (targetToOrigins.TryGetValue(target, out var origins))
            {
                // Group origins by proximity
                List<Transform> nearbyOrigins = new List<Transform>();

                foreach (var other in origins)
                {
                    if (origin == null || !origin || other == null || !other)
                        continue;

                    if (Vector3.Distance(other.position, origin.position) < parallelTolerance)
                    {
                        nearbyOrigins.Add(other);
                    }
                }

                nearbyOrigins.Sort((a, b) => origins.IndexOf(a).CompareTo(origins.IndexOf(b)));
                int nearbyIndex = nearbyOrigins.IndexOf(origin);

                if (nearbyIndex == 0)
                {
                    // First nearby origin ➔ straight line
                    Handles.DrawLine(originPos, targetPos);
                }
                else
                {
                    // Other nearby origins ➔ rotate around axis at regular angles

                    Vector3 direction = (targetPos - originPos).normalized;
                    Vector3 up = Vector3.up;

                    // Compute rotation base
                    Vector3 right = Vector3.Cross(up, direction);
                    if (right == Vector3.zero)
                        right = Vector3.Cross(Vector3.forward, direction); // fallback

                    right.Normalize();
                    up = Vector3.Cross(direction, right).normalized; // recalculate correct up vector

                    float distance = Vector3.Distance(originPos, targetPos) * baseTangentDistance;
                    float startingAngle = 90f; // Start at 90 degrees (top)

                    int stepsPerCircle = Mathf.RoundToInt(360f / angleStep);
                    int fullTurns = nearbyIndex / stepsPerCircle;
                    int stepIndex = nearbyIndex % stepsPerCircle;

                    float angle = startingAngle + stepIndex * angleStep;

                    // Apply rotation
                    Quaternion rotation = Quaternion.AngleAxis(angle, direction);
                    Vector3 rotatedRight = rotation * right;

                    float radius = distance + (fullTurns * distance); // Increase radius each full turn

                    Vector3 startTangent = originPos + rotatedRight * radius;
                    Vector3 endTangent = targetPos + rotatedRight * radius;

                    Handles.DrawBezier(originPos, targetPos, startTangent, endTangent, color, null, 2f);
                }
            }
            else
            {
                // Fallback if no mapping
                Handles.DrawLine(originPos, targetPos);
            }

            // Draw a marker
            HandlesExtensions.DrawWireSphere(targetPos, 0.05f);
        }

        /// <summary>
        /// Safely extracts a Transform from a given UnityEngine.Object.
        /// Handles Transform, Component, and GameObject cases, and ensures the target is valid.
        /// </summary>
        /// <param name="targetObj">The object from which to extract the Transform.</param>
        /// <returns>A valid Transform if possible; otherwise, null.</returns>
        public static Transform SafeGetTargetTransform(Object targetObj)
        {
            if (targetObj == null || targetObj.Equals(null))
                return null;

            Transform targetTransform = null;

            if (targetObj is Transform t)
            {
                targetTransform = t;
            }
            else if (targetObj is Component c)
            {
                targetTransform = c.transform;
            }
            else if (targetObj is GameObject g)
            {
                targetTransform = g.transform;
            }

            if (targetTransform == null || targetTransform.Equals(null) || targetTransform.gameObject == null)
                return null;

            return targetTransform;
        }
    }
}
#endif
