#if UNITY_EDITOR
using UnityEditor;

namespace PsychoGarden.TriggerEvents
{
    /// <summary>
    /// Static helper class to manage global settings for TriggerEvent visualization using EditorPrefs.
    /// </summary>
    public static class TriggerEventSettings
    {
        private const string ShowConnectionsKey = "TriggerEvent_ShowConnections";
        private const string DisplayModeKey = "TriggerEvent_DisplayMode";

        /// <summary>
        /// Gets or sets whether TriggerEvent connections should be globally shown in the editor.
        /// </summary>
        public static bool ShowConnections
        {
            get => EditorPrefs.GetBool(ShowConnectionsKey, true);
            set => EditorPrefs.SetBool(ShowConnectionsKey, value);
        }

        /// <summary>
        /// Gets or sets the global display mode for TriggerEvent visualization.
        /// 0 = All, 1 = None, 2 = OnSelected.
        /// </summary>
        public static int DisplayMode
        {
            get => EditorPrefs.GetInt(DisplayModeKey, 0);
            set => EditorPrefs.SetInt(DisplayModeKey, value);
        }

        /// <summary>
        /// Resets all TriggerEvent settings to their default values.
        /// </summary>
        public static void ResetToDefaults()
        {
            EditorPrefs.SetBool(ShowConnectionsKey, true);
            EditorPrefs.SetInt(DisplayModeKey, 0);
        }
    }
}
#endif
