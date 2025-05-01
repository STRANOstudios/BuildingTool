using UnityEngine;

namespace BuildingTool.Editor.Builder3D.Utilities
{
    public static class HierarchyOrganizer
    {
        public static Transform GetPlacementParent(float height, string category)
        {
            string levelName = $"Level ({height:0.##}m)";
            GameObject levelRoot = GameObject.Find(levelName);
            if (levelRoot == null)
                levelRoot = new GameObject(levelName);

            Transform categoryGroup = levelRoot.transform.Find(category);
            if (categoryGroup == null)
            {
                GameObject group = new GameObject(category);
                group.transform.SetParent(levelRoot.transform);
                categoryGroup = group.transform;
            }

            return categoryGroup;
        }
    }
}
