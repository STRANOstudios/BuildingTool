#if UNITY_EDITOR

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using BuildingTool.Runtime.Configuration;
using BuildingTool.Runtime.Utilities;
using BuildingTool.Editor.Builder3D.Handlers;

namespace BuildingTool.Editor.Builder3D.EditorWindows
{
    /// <summary>
    /// Main editor window for assembling modular buildings in a 3D scene.
    /// Includes prefab selection, plane height adjustment, and ghost preview.
    /// Designed to interact with modular Pack data and runtime construction logic.
    /// </summary>
    public class BuildingTool3DEditorWindow : EditorWindow
    {
        #region Variables --------------------------------------------------

        private PackManager m_packManager;
        private ColorConfig m_colorConfig;

        private int m_selectedPackIndex = -1;
        private float m_planeHeight = 0f;

        private Vector2 m_scrollPosition = Vector2.zero;

        private GameObject m_selectedPrefab;
        private string m_selectedCategory;

        private PrefabSelectionDrawer m_selectionDrawer;
        private PreviewGhostHandler m_previewGhost;
        private PlacementInputHandler m_inputHandler;

        private readonly Vector3[] m_planeVertices = new Vector3[]
        {
            new Vector3(-100, 0, -100),
            new Vector3(-100, 0, 100),
            new Vector3(100, 0, 100),
            new Vector3(100, 0, -100)
        };

        #endregion

        #region Menu & Lifecycle -------------------------------------------

        [MenuItem("Tools/Building Tool/3D Modular Building Tool")]
        public static void Open()
        {
            BuildingTool3DEditorWindow window = GetWindow<BuildingTool3DEditorWindow>();
            window.titleContent = new GUIContent("3D Modular Building Tool");
            window.minSize = new Vector2(600f, 400f);
            window.Show();
        }

        private void OnEnable()
        {
            SceneView.duringSceneGui += this.OnSceneGUI;

            this.m_packManager = ScriptableObjectLoader.LoadOrCreate<PackManager>(BTPaths.PackManagerAsset);
            this.m_colorConfig = ScriptableObjectLoader.LoadOrCreate<ColorConfig>(BTPaths.ColorConfigAsset);

            this.m_selectionDrawer = new PrefabSelectionDrawer((prefab, category) =>
            {
                this.m_selectedPrefab = prefab;
                this.m_selectedCategory = category;
            });

            this.m_previewGhost = new PreviewGhostHandler(this.m_colorConfig);

            this.m_inputHandler = new PlacementInputHandler(
                this.m_previewGhost,
                () => this.m_selectedPrefab,
                () => this.m_selectedCategory,
                () => this.m_selectedPrefab = null
            );
        }

        private void OnDisable()
        {
            SceneView.duringSceneGui -= this.OnSceneGUI;
            this.m_previewGhost.ClearPreview();
        }

        #endregion

        #region GUI --------------------------------------------------------

        private void OnGUI()
        {
            this.m_scrollPosition = EditorGUILayout.BeginScrollView(this.m_scrollPosition);

            if (this.m_packManager != null && this.m_packManager.Packs.Count > 0)
            {
                string[] packNames = this.GetPackNames();
                this.m_selectedPackIndex = EditorGUILayout.Popup("Selected Pack", this.m_selectedPackIndex, packNames);

                if (this.m_selectedPackIndex >= 0 && this.m_selectedPackIndex < this.m_packManager.Packs.Count)
                {
                    Pack pack = this.m_packManager.Packs[this.m_selectedPackIndex];

                    this.m_planeHeight = EditorGUILayout.Slider("Plane Height", this.m_planeHeight, -10f, 50f);

                    EditorGUILayout.Space(10);
                    this.m_selectionDrawer.DrawPrefabSection("Floors", pack.Floors);
                    this.m_selectionDrawer.DrawPrefabSection("Walls", pack.Walls);
                    this.m_selectionDrawer.DrawPrefabSection("Roofs", pack.Roofs);
                }
            }
            else
            {
                EditorGUILayout.HelpBox("No PackManager found or it's empty. Please create at least one pack.", MessageType.Warning);
            }

            EditorGUILayout.EndScrollView();
        }

        #endregion

        #region Scene GUI --------------------------------------------------

        private void OnSceneGUI(SceneView sceneView)
        {
            this.DrawPlane(sceneView);

            if (this.m_inputHandler != null)
            {
                this.m_inputHandler.SetPlaneHeight(this.m_planeHeight);
                this.m_inputHandler.HandleInput(sceneView);
            }

            this.m_previewGhost?.DrawDebugOverlapBox();

            this.Repaint(); // For continuous interaction
        }

        private void DrawPlane(SceneView sceneView)
        {
            Camera sceneCamera = sceneView.camera;
            Vector3 planeCenter = sceneCamera.transform.position + sceneCamera.transform.forward * 100f;
            planeCenter.y = this.m_planeHeight;

            Matrix4x4 matrix = Matrix4x4.TRS(planeCenter, Quaternion.identity, Vector3.one);
            Handles.matrix = matrix;

            Handles.DrawSolidRectangleWithOutline(
                this.m_planeVertices,
                this.m_colorConfig.ColorPlane,
                this.m_colorConfig.ColorOutline
            );
        }

        #endregion

        #region Helpers ----------------------------------------------------

        private string[] GetPackNames()
        {
            List<string> names = new List<string>();
            foreach (Pack pack in this.m_packManager.Packs)
            {
                names.Add(pack.Name);
            }
            return names.ToArray();
        }

        #endregion
    }
}
#endif
