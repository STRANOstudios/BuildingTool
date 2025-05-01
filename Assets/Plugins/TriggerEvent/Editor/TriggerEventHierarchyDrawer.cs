using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace PsychoGarden.TriggerEvents
{
#if UNITY_EDITOR
    /// <summary>
    /// Draws visual indicators in the Hierarchy and SceneView for TriggerEvents.
    /// </summary>
    [InitializeOnLoad]
    public static class TriggerEventHierarchyDrawer
    {
        private static List<(GameObject owner, TriggerEvent triggerEvent, Color cachedColor)> triggerEventsCache = new();
        private static double lastCacheTime;
        private const double cacheRefreshInterval = 2.0; // seconds

        // Static constructor to register callbacks
        static TriggerEventHierarchyDrawer()
        {
            EditorApplication.hierarchyWindowItemOnGUI += HierarchyWindowOnGUI;
            SceneView.duringSceneGui += DrawSceneView;
            EditorApplication.update += () =>
            {
                SceneView.RepaintAll();
                UnityEditorInternal.InternalEditorUtility.RepaintAllViews();
                RefreshCacheIfNeeded();
            };
        }

        /// <summary>
        /// Refreshes the cache of all TriggerEvents in the scene periodically.
        /// </summary>
        private static void RefreshCacheIfNeeded()
        {
            if (EditorApplication.timeSinceStartup - lastCacheTime < cacheRefreshInterval)
                return;

            triggerEventsCache.Clear();

            foreach (var go in Object.FindObjectsByType<GameObject>(FindObjectsSortMode.None))
            {
                if (go == null)
                    continue;

                var components = go.GetComponents<MonoBehaviour>();
                foreach (var comp in components)
                {
                    if (comp == null)
                        continue;

                    var fields = comp.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                    foreach (var field in fields)
                    {
                        if (field.FieldType == typeof(TriggerEvent))
                        {
                            if (field.GetValue(comp) is TriggerEvent triggerEvent && triggerEvent != null)
                            {
                                triggerEvent.CheckAuto(comp);
                                triggerEventsCache.Add((go, triggerEvent, triggerEvent.editorColor));
                            }
                        }
                    }
                }
            }

            TriggerEventHandles.RebuildTargetMapping(triggerEventsCache);

            lastCacheTime = EditorApplication.timeSinceStartup;
        }

        /// <summary>
        /// Draws colored rectangles in the Hierarchy next to GameObjects linked by TriggerEvents.
        /// </summary>
        private static void HierarchyWindowOnGUI(int instanceID, Rect selectionRect)
        {
            if (!TriggerEventSettings.ShowConnections)
                return;

            GameObject go = EditorUtility.InstanceIDToObject(instanceID) as GameObject;
            if (go == null)
                return;

            foreach (var (owner, triggerEvent, cachedColor) in triggerEventsCache)
            {
                if (triggerEvent == null || owner == null)
                    continue;

                if (!TriggerEventDrawer.EvaluateShowConnections(triggerEvent))
                    continue;

                if (!TriggerEventDrawer.EvaluateShouldDrawFor(triggerEvent, owner))
                    continue;

                int count = triggerEvent.GetPersistentEventCount();
                for (int i = 0; i < count; i++)
                {
                    var targetObj = triggerEvent.GetPersistentTarget(i);
                    if (targetObj == go || (targetObj is Component component && component.gameObject == go))
                    {
                        Rect rect = new Rect(selectionRect.x - 20f, selectionRect.y + 2f, 5f, selectionRect.height - 4f);
                        EditorGUI.DrawRect(rect, cachedColor);

                        // Stop after drawing for the first matching target
                        return;
                    }
                }
            }
        }

        /// <summary>
        /// Draws connection lines in the SceneView for all TriggerEvents.
        /// </summary>
        private static void DrawSceneView(SceneView sceneView)
        {
            foreach (var (owner, triggerEvent, cachedColor) in triggerEventsCache)
            {
                if (triggerEvent == null || owner == null)
                    continue;

                if (!TriggerEventDrawer.EvaluateShowConnections(triggerEvent))
                    continue;

                if (!TriggerEventDrawer.EvaluateShouldDrawFor(triggerEvent, owner))
                    continue;

                TriggerEventHandles.DrawConnectionGizmos(triggerEvent, owner.transform);
            }
        }

        /// <summary>
        /// Determines if the connections for a specific TriggerEvent should be shown.
        /// </summary>
        public static bool ShouldShowConnections(TriggerEvent triggerEvent)
        {
            if (triggerEvent == null)
                return TriggerEventSettings.ShowConnections;

            return triggerEvent.editorShowConnections;
        }

        /// <summary>
        /// Determines if a specific TriggerEvent should be drawn for a given owner.
        /// </summary>
        public static bool ShouldDrawFor(TriggerEvent triggerEvent, GameObject owner)
        {
            if (triggerEvent == null)
                return ShouldDrawForGlobal(owner);

            return triggerEvent.editorDisplayMode switch
            {
                TriggerEvent.DisplayMode.All => true,
                TriggerEvent.DisplayMode.None => false,
                TriggerEvent.DisplayMode.OnSelected => Selection.activeGameObject == owner,
                _ => true,
            };
        }

        /// <summary>
        /// Determines if any TriggerEvent should be drawn for a GameObject based on global settings.
        /// </summary>
        private static bool ShouldDrawForGlobal(GameObject owner)
        {
            return (TriggerEvent.DisplayMode)TriggerEventSettings.DisplayMode switch
            {
                TriggerEvent.DisplayMode.All => true,
                TriggerEvent.DisplayMode.None => false,
                TriggerEvent.DisplayMode.OnSelected => Selection.activeGameObject == owner,
                _ => true,
            };
        }
    }
#endif
}
