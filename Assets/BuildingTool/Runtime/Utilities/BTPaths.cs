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
    }
}
