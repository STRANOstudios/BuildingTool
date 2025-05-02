#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;
using UnityEditor.SceneManagement;
using BuildingTool.Runtime.Utilities;
using BuildingTool.Editor.Builder3D.Utilities;

namespace BuildingTool.Editor.Builder3D.Handlers
{
    /// <summary>
    /// Handles all scene input events for prefab placement, rotation, and cancellation.
    /// Works with PreviewGhostHandler to control placement behavior in the scene view.
    /// </summary>
    public class PlacementInputHandler
    {
        #region Fields -------------------------------------------------------

        private readonly PreviewGhostHandler m_ghostHandler;
        private readonly System.Func<GameObject> m_getSelectedPrefab;
        private readonly System.Func<string> m_getSelectedCategory;
        private readonly System.Action m_clearSelectionCallback;
        private float m_planeHeight;
        private bool m_smartSnapEnabled = true;

        #endregion

        #region Constructor --------------------------------------------------

        /// <summary>
        /// Initializes a new input handler for controlling prefab placement in the scene.
        /// </summary>
        /// <param name="ghostHandler">Ghost preview controller.</param>
        /// <param name="selectedPrefabGetter">Delegate to access the current prefab selection.</param>
        /// <param name="selectedCategoryGetter">Function to get current selected category (e.g., "Floors").</param>
        /// <param name="clearSelectionCallback">Callback to clear the current prefab selection.</param>
        public PlacementInputHandler(
            PreviewGhostHandler ghostHandler,
            System.Func<GameObject> selectedPrefabGetter,
            System.Func<string> selectedCategoryGetter,
            System.Action clearSelectionCallback)
        {
            this.m_ghostHandler = ghostHandler;
            this.m_getSelectedPrefab = selectedPrefabGetter;
            this.m_getSelectedCategory = selectedCategoryGetter;
            this.m_clearSelectionCallback = clearSelectionCallback;
        }

        #endregion

        #region Public Methods -----------------------------------------------

        /// <summary>
        /// Updates the plane height used for placement projection.
        /// </summary>
        /// <param name="height">Height of the construction plane (Y).</param>
        public void SetPlaneHeight(float height)
        {
            this.m_planeHeight = height;
        }

        /// <summary>
        /// Processes all relevant user input events in the scene view.
        /// </summary>
        /// <param name="sceneView">Reference to the current SceneView context.</param>
        public void HandleInput(SceneView sceneView)
        {
            GameObject prefab = this.m_getSelectedPrefab.Invoke();
            if (prefab == null)
            {
                this.m_ghostHandler.ClearPreview();
                return;
            }

            Event e = Event.current;
            Vector2 mousePos = e.mousePosition;

            Vector3 worldPos = BTMath.IntersectMouseWithHorizontalPlane(mousePos, this.m_planeHeight);
            worldPos = this.ApplyGlobalSnap(worldPos);

            this.m_ghostHandler.UpdatePreview(prefab, worldPos);

            if (e.type == EventType.ScrollWheel)
            {
                float direction = Mathf.Sign(e.delta.y);
                this.m_ghostHandler.RotatePreview(direction * 15f);
                e.Use();
            }

            if (e.type == EventType.MouseDown && e.button == 0)
            {
                bool isIgnoringCollisions = Event.current.shift;

                if (isIgnoringCollisions || this.m_ghostHandler.IsValidPlacement())
                {
                    string category = this.m_getSelectedCategory.Invoke();
                    Transform parent = HierarchyOrganizer.GetPlacementParent(this.m_planeHeight, category);

                    GameObject placed = PrefabUtility.InstantiatePrefab(prefab, parent) as GameObject;
                    placed.transform.SetPositionAndRotation(this.m_ghostHandler.Position, this.m_ghostHandler.Rotation);

                    Undo.RegisterCreatedObjectUndo(placed, "Place Building Module");
                    EditorSceneManager.MarkSceneDirty(placed.scene);

                    BTDebug.LogSuccess($"Placed prefab: {placed.name} under Level({this.m_planeHeight}m)/{category}");
                }
                else
                {
                    BTDebug.LogWarning("Cannot place prefab: position is invalid (overlapping objects).");
                }

                e.Use();
            }

            if (e.type == EventType.MouseDown && e.button == 1)
            {
                this.m_clearSelectionCallback?.Invoke();
                this.m_ghostHandler.ClearPreview();
                BTDebug.LogInfo("Selection cleared.");
                e.Use();
            }

            bool isAltHeld = Event.current.alt;
            m_smartSnapEnabled = !isAltHeld;
        }

        #endregion

        #region Private Methods ----------------------------------------------

        private Vector3 ApplyGlobalSnap(Vector3 position)
        {
            if (!EditorSnapSettings.snapEnabled)
                return position;

            Vector3 snap = EditorSnapSettings.move;

            GameObject ghost = this.m_ghostHandler.CurrentPreview;
            if (ghost != null)
            {
                // Draw debugging gizmo for smart snap
                //SmartSnapUtility.DrawSmartSnapBox(ghost, 3f); // Only Debug

                ghost.transform.position = position;

                if (m_smartSnapEnabled)
                {
                    Vector3 smartSnapped = SmartSnapUtility.ComputeSmartSnappedPosition(ghost, 3f);

                    if (smartSnapped != ghost.transform.position)
                    {
                        return smartSnapped;
                    }
                }

                position = new Vector3(
                    Mathf.Round(position.x / snap.x) * snap.x,
                    Mathf.Round(position.y / snap.y) * snap.y,
                    Mathf.Round(position.z / snap.z) * snap.z
                );
            }

            return position;
        }

        #endregion
    }
}

#endif
