#if UNITY_EDITOR

using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using BuildingTool.Runtime.Utilities;

namespace BuildingTool.Editor.Builder3D.Handlers
{
    /// <summary>
    /// Utility class to collect all Level objects from the scene, merge them under a common root,
    /// recenter the group, and save the structure as a prefab to disk.
    /// </summary>
    public static class PrefabSaver
    {
        /// <summary>
        /// Generates a prefab from all Level-rooted objects in the scene and saves it to disk.
        /// </summary>
        public static void SaveStructureAsPrefab()
        {
            GameObject[] levelRoots = SceneManager.GetActiveScene()
                .GetRootGameObjects()
                .Where(go => go.name.StartsWith("Level"))
                .ToArray();

            if (levelRoots.Length == 0)
            {
                BTDebug.LogWarning("No Level objects found in the scene to save.");
                return;
            }

            GameObject structureRoot = new GameObject("StructureRoot_Temp");

            foreach (GameObject level in levelRoots)
            {
                GameObject copy = GameObject.Instantiate(level);
                copy.name = level.name;
                copy.transform.SetParent(structureRoot.transform, true);
            }

            // Recenter structure
            CenterAtOrigin(structureRoot);

            // Ensure folder exists
            Directory.CreateDirectory(BTPaths.SavedPrefabDirectory);

            string filename = $"Structure_{System.DateTime.Now:yyyyMMdd_HHmmss}.prefab";
            string path = Path.Combine(BTPaths.SavedPrefabDirectory, filename);

            string localPath = AssetDatabase.GenerateUniqueAssetPath(path);

            GameObject prefab = PrefabUtility.SaveAsPrefabAsset(structureRoot, localPath, out bool success);
            GameObject.DestroyImmediate(structureRoot);

            if (success)
                BTDebug.LogSuccess($"Structure prefab saved at: {localPath}");
            else
                BTDebug.LogError("Failed to save prefab.");
        }

        private static void CenterAtOrigin(GameObject root)
        {
            Renderer[] renderers = root.GetComponentsInChildren<Renderer>();
            if (renderers.Length == 0) return;

            // Compute global bounds of the full structure
            Bounds bounds = renderers[0].bounds;
            for (int i = 1; i < renderers.Length; i++)
                bounds.Encapsulate(renderers[i].bounds);

            Vector3 offset = bounds.center;
            offset.y = 0f; // Preserve vertical positions

            // Move each child accordingly
            foreach (Transform child in root.transform)
            {
                Vector3 pos = child.position;
                pos -= offset;
                pos.y += offset.y; // preserve original Y
                child.position = pos;
            }
        }
    }
}
#endif
