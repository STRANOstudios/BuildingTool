#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace PsychoGarden.TriggerEvents
{
    /// <summary>
    /// Editor window for managing global TriggerEvent settings.
    /// </summary>
    public class TriggerEventSettingsWindow : EditorWindow
    {
        /// <summary>
        /// Opens the TriggerEvent settings window from the Unity menu.
        /// </summary>
        [MenuItem("Tools/Psycho Garden/TriggerEvent Settings")]
        public static void OpenWindow()
        {
            GetWindow<TriggerEventSettingsWindow>("TriggerEvent Settings");
        }

        /// <summary>
        /// Draws the window GUI for modifying global settings.
        /// </summary>
        private void OnGUI()
        {
            GUILayout.Label("Global Visualization Settings", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            // Toggle to show or hide global connections
            TriggerEventSettings.ShowConnections = EditorGUILayout.Toggle(
                new GUIContent("Show Global Connections", "Enable or disable drawing connections globally."),
                TriggerEventSettings.ShowConnections);

            // Dropdown to choose the global display mode
            TriggerEventSettings.DisplayMode = (int)(TriggerEvent.DisplayMode)EditorGUILayout.EnumPopup(
                new GUIContent("Global Display Mode", "Controls when connections are shown globally."),
                (TriggerEvent.DisplayMode)TriggerEventSettings.DisplayMode);

            EditorGUILayout.Space(10);

            // Button to reset all settings to default values
            if (GUILayout.Button("Reset to Defaults"))
            {
                TriggerEventSettings.ResetToDefaults();
            }
        }
    }
}
#endif
