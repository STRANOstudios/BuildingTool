using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using BuildingTool.Runtime.Utilities;

namespace BuildingTool.Editor.Builder3D.Handlers
{
    /// <summary>
    /// Draws a list of prefabs with previews and size information, allowing full-row selection.
    /// Includes scroll handling and foldout per category.
    /// </summary>
    public class PrefabSelectionDrawer
    {
        #region Constants -------------------------------------------------

        private const float ItemHeight = 68f;
        private const float MaxListHeight = 300f;

        #endregion

        #region State -----------------------------------------------------

        private readonly System.Action<GameObject, string> m_onSelected;
        private readonly Dictionary<string, bool> m_foldoutStates = new();
        private readonly Dictionary<string, Vector2> m_scrollPositions = new();

        #endregion

        #region Constructor -----------------------------------------------

        /// <summary>
        /// Initializes the drawer with a callback invoked when a prefab is selected.
        /// </summary>
        /// <param name="onSelected">Callback to execute on prefab selection.</param>
        public PrefabSelectionDrawer(System.Action<GameObject, string> onSelected)
        {
            this.m_onSelected = onSelected;
        }

        #endregion

        #region Public Methods --------------------------------------------

        /// <summary>
        /// Renders a section of prefab cards, each within a foldout. Adds scrolling if list exceeds max height.
        /// </summary>
        /// <param name="category">Section title / prefab category (e.g., Floors).</param>
        /// <param name="prefabs">List of GameObjects to display.</param>
        public void DrawPrefabSection(string category, List<GameObject> prefabs)
        {
            if (!this.m_foldoutStates.ContainsKey(category))
                this.m_foldoutStates[category] = true;

            if (!this.m_scrollPositions.ContainsKey(category))
                this.m_scrollPositions[category] = Vector2.zero;

            this.m_foldoutStates[category] = EditorGUILayout.Foldout(
                this.m_foldoutStates[category],
                $"{category} ({prefabs.Count})",
                true
            );

            if (!this.m_foldoutStates[category])
                return;

            float totalHeight = prefabs.Count * ItemHeight;
            bool useScroll = totalHeight > MaxListHeight;

            if (useScroll)
            {
                this.m_scrollPositions[category] = EditorGUILayout.BeginScrollView(
                    this.m_scrollPositions[category],
                    GUILayout.Height(MaxListHeight)
                );

                this.DrawPrefabItems(prefabs, category);

                EditorGUILayout.EndScrollView();
            }
            else
            {
                this.DrawPrefabItems(prefabs, category);
            }

            EditorGUILayout.Space(6);
        }

        #endregion

        #region Private Methods -------------------------------------------

        private void DrawPrefabItems(List<GameObject> prefabs, string category)
        {
            foreach (GameObject prefab in prefabs)
            {
                if (prefab == null) continue;

                Texture2D preview = AssetPreview.GetAssetPreview(prefab) ?? AssetPreview.GetMiniThumbnail(prefab);
                Bounds bounds = this.GetBounds(prefab);

                Rect rect = EditorGUILayout.BeginHorizontal("box", GUILayout.Height(64));
                {
                    if (GUI.Button(rect, GUIContent.none))
                    {
                        this.m_onSelected?.Invoke(prefab, category);
                        BTDebug.LogInfo($"Selected prefab: {prefab.name}");
                    }

                    GUILayout.Label(preview, GUILayout.Width(64), GUILayout.Height(64));
                    GUILayout.Space(8);

                    GUILayout.BeginVertical();
                    GUILayout.Label(prefab.name, EditorStyles.boldLabel);
                    GUILayout.Label($"Size: {bounds.size.x:F2}, {bounds.size.y:F2}, {bounds.size.z:F2}", EditorStyles.miniLabel);
                    GUILayout.EndVertical();
                }
                EditorGUILayout.EndHorizontal();
            }
        }

        private Bounds GetBounds(GameObject go)
        {
            Renderer[] renderers = go.GetComponentsInChildren<Renderer>();
            if (renderers.Length == 0)
                return new Bounds(Vector3.zero, Vector3.zero);

            Bounds bounds = renderers[0].bounds;
            for (int i = 1; i < renderers.Length; i++)
                bounds.Encapsulate(renderers[i].bounds);

            return bounds;
        }

        #endregion
    }
}
