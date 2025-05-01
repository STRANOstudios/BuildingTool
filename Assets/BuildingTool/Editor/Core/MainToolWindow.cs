#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

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
            // Open builder first — it will be the main anchor
            var builder = EditorWindow.CreateWindow<BuildingTool3DEditorWindow>();
            builder.titleContent = new GUIContent("Builder Tool");
            builder.Show();

            // Dock other windows next to the builder
            var pack = EditorWindow.CreateWindow<PackCreatorWindow>(
                desiredDockNextTo: new[] { builder.GetType() });
            pack.titleContent = new GUIContent("Pack Manager");
            pack.Show();

            var settings = EditorWindow.CreateWindow<SettingsWindow>(
                desiredDockNextTo: new[] { builder.GetType() });
            settings.titleContent = new GUIContent("Settings");
            settings.Show();
        }

        #endregion
    }
}

#endif
