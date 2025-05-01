#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;
using BuildingTool.Runtime.Utilities;
using System.IO;

namespace BuildingTool.Editor.Settings.EditorWindows
{
    /// <summary>
    /// Window for configuring tool colors, save paths and debug visibility.
    /// Accessible via Window > Building Tool > Settings.
    /// </summary>
    public class SettingsWindow : EditorWindow
    {
        #region Fields -----------------------------------------------------

        private ColorConfig m_colorConfig;
        private SerializedObject m_colorConfigSerialized;

        private string m_savedPrefabDirectory;
        private bool m_showDebugLogs;

        #endregion

        #region Unity ------------------------------------------------------

        [MenuItem("Tools/Building Tool/Settings", priority = 0)]
        public static void Open()
        {
            var window = GetWindow<SettingsWindow>();
            window.titleContent = new GUIContent("Building Tool Settings");
            window.minSize = new Vector2(450, 400);
            window.Show();
        }

        private void OnEnable()
        {
            this.m_colorConfig = AssetDatabase.LoadAssetAtPath<ColorConfig>(BTPaths.ColorConfigAsset);
            if (this.m_colorConfig == null)
            {
                this.m_colorConfig = ScriptableObject.CreateInstance<ColorConfig>();
                Directory.CreateDirectory("Assets/BuildingTool/Data");
                AssetDatabase.CreateAsset(this.m_colorConfig, BTPaths.ColorConfigAsset);
                AssetDatabase.SaveAssets();
            }

            this.m_colorConfigSerialized = new SerializedObject(this.m_colorConfig);
            this.m_savedPrefabDirectory = BTPaths.SavedPrefabDirectory;
            this.m_showDebugLogs = BTDebug.Enabled;
        }

        private void OnGUI()
        {
            DrawSavePath();
            EditorGUILayout.Space(12);

            DrawDebugToggle();
            EditorGUILayout.Space(12);

            DrawColorSettings();
        }

        #endregion

        #region GUI Blocks -------------------------------------------------

        private void DrawSavePath()
        {
            EditorGUILayout.LabelField("Saved Prefab Directory", EditorStyles.boldLabel);
            EditorGUILayout.BeginHorizontal();
            this.m_savedPrefabDirectory = EditorGUILayout.TextField(this.m_savedPrefabDirectory);

            if (GUILayout.Button("...", GUILayout.Width(30)))
            {
                string selected = EditorUtility.OpenFolderPanel("Select Prefab Save Directory", "Assets", "");
                if (!string.IsNullOrEmpty(selected))
                {
                    if (selected.StartsWith(Application.dataPath))
                    {
                        this.m_savedPrefabDirectory = "Assets" + selected.Substring(Application.dataPath.Length);
                    }
                    else
                    {
                        EditorUtility.DisplayDialog("Invalid Folder", "Please select a folder inside the Assets directory.", "OK");
                    }
                }
            }
            EditorGUILayout.EndHorizontal();

            if (this.m_savedPrefabDirectory != BTPaths.SavedPrefabDirectory)
            {
                BTPaths.SetPrefabSaveDirectory(this.m_savedPrefabDirectory);
            }
        }

        private void DrawDebugToggle()
        {
            EditorGUILayout.LabelField("Debug Log Visibility", EditorStyles.boldLabel);

            BTDebug.Enabled = EditorGUILayout.ToggleLeft("Enable Logging Globally", BTDebug.Enabled);
            BTDebug.ShowSuccess = EditorGUILayout.ToggleLeft("Show Success Logs", BTDebug.ShowSuccess);
            BTDebug.ShowInfo = EditorGUILayout.ToggleLeft("Show Info Logs", BTDebug.ShowInfo);
            BTDebug.ShowWarning = EditorGUILayout.ToggleLeft("Show Warning Logs", BTDebug.ShowWarning);
            BTDebug.ShowError = EditorGUILayout.ToggleLeft("Show Error Logs", BTDebug.ShowError);
        }

        private void DrawColorSettings()
        {
            EditorGUILayout.LabelField("Color Config", EditorStyles.boldLabel);
            EditorGUILayout.Space(4);

            this.m_colorConfigSerialized.Update();

            SerializedProperty prop = this.m_colorConfigSerialized.GetIterator();
            bool enterChildren = true;

            while (prop.NextVisible(enterChildren))
            {
                enterChildren = false;
                if (prop.name == "m_Script") continue;

                EditorGUILayout.PropertyField(prop, true);
            }

            this.m_colorConfigSerialized.ApplyModifiedProperties();
        }

        #endregion
    }
}

#endif
