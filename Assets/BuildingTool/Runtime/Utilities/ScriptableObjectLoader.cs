using System.IO;
using UnityEditor;
using UnityEngine;

namespace BuildingTool.Runtime.Utilities
{
    /// <summary>
    /// Provides utility methods for loading or creating ScriptableObject assets at specified paths.
    /// Ensures consistent logging and directory validation.
    /// </summary>
    public static class ScriptableObjectLoader
    {
        /// <summary>
        /// Loads a ScriptableObject of type T from a given asset path. If the asset does not exist, a new instance is created and saved.
        /// </summary>
        /// <typeparam name="T">The type of ScriptableObject to load or create.</typeparam>
        /// <param name="path">The relative asset path (e.g. "Assets/BuildingTool/Data/MyAsset.asset").</param>
        /// <returns>The loaded or newly created ScriptableObject instance.</returns>
        public static T LoadOrCreate<T>(string path) where T : ScriptableObject
        {
            T asset = AssetDatabase.LoadAssetAtPath<T>(path);

            if (asset == null)
            {
                asset = ScriptableObject.CreateInstance<T>();

                string directory = Path.GetDirectoryName(path);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                AssetDatabase.CreateAsset(asset, path);
                AssetDatabase.SaveAssets();

                BTDebug.LogInfo($"Created new {typeof(T).Name} at: {path}");
            }
            else
            {
                BTDebug.LogSuccess($"{typeof(T).Name} loaded from: {path}");
            }

            return asset;
        }
    }
}
