#if UNITY_EDITOR

using UnityEditor;

using BuildingTool.Editor.Builder3D.EditorWindows;
using BuildingTool.Editor.PackCreator.EditorWindows;
using BuildingTool.Editor.Settings.EditorWindows;

namespace BuildingTool.Editor.Core
{
    /// <summary>
    /// Opens and docks all Building Tool editor windows together in a unified layout.
    /// </summary>
    public class MainToolWindow : EditorWindow
    {
        #region Menu -------------------------------------------------------

        [MenuItem("Tools/Building Tool/Launch Tool", priority = -1)]
        public static void Open()
        {
            // Open Builder window first and dock others to it
            BuildingTool3DEditorWindow builder = EditorWindow.GetWindow<BuildingTool3DEditorWindow>();
            builder.titleContent = new UnityEngine.GUIContent("Builder Tool");

            PackCreatorWindow pack = EditorWindow.CreateWindow<PackCreatorWindow>();
            pack.titleContent = new UnityEngine.GUIContent("Pack Manager");

            SettingsWindow settings = EditorWindow.CreateWindow<SettingsWindow>();
            settings.titleContent = new UnityEngine.GUIContent("Settings");

            // Dock pack and settings to builder
            DockWindowTo(pack, builder);
            DockWindowTo(settings, builder);

            builder.Show();
        }

        #endregion

        #region Docking ----------------------------------------------------

        private static void DockWindowTo(EditorWindow target, EditorWindow anchor)
        {
            var anchorParent = typeof(EditorWindow).GetProperty("m_Parent", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.GetValue(anchor);
            var targetParentField = typeof(EditorWindow).GetField("m_Parent", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (anchorParent != null && targetParentField != null)
            {
                targetParentField.SetValue(target, anchorParent);
            }
        }

        #endregion
    }
}

#endif
