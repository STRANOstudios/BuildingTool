namespace BuildingTool.Runtime.Utilities
{
    /// <summary>
    /// Contains centralized path definitions used across the BuildingTool system.
    /// All asset and directory paths should be referenced from this static class.
    /// </summary>
    public static class BTPaths
    {
        /// <summary>
        /// Base path for generated data assets (e.g. ScriptableObjects).
        /// </summary>
        public const string DataRoot = "Assets/BuildingTool/Data";

        /// <summary>
        /// Asset path for the PackManager ScriptableObject.
        /// </summary>
        public const string PackManagerAsset = DataRoot + "/PackManager.asset";

        /// <summary>
        /// Asset path for the ColorConfig ScriptableObject.
        /// </summary>
        public const string ColorConfigAsset = DataRoot + "/ColorConfig.asset";

        /// <summary>
        /// Directory path for generated prefab assets.
        /// </summary>
        private static string m_savedPrefabDirectory = "Assets/Levels/Prefabs/BuildingTool/GeneratedPrefabs/";

        public static string SavedPrefabDirectory => m_savedPrefabDirectory;

        public static void SetPrefabSaveDirectory(string path)
        {
            m_savedPrefabDirectory = path;
        }

        /// <summary>
        /// Directory path for generated prefab assets.
        /// </summary>
        public const string LogoDirectory = "Assets/BuildingTool/Editor/Resources/logo.png";
    }
}
