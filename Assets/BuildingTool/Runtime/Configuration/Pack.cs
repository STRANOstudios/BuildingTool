using System.Collections.Generic;
using UnityEngine;

namespace BuildingTool.Runtime.Configuration
{
    /// <summary>
    /// Represents a modular asset collection used to define a specific building style or theme.
    /// A Pack contains references to categorized prefabs such as floors, walls, and roofs,
    /// which can be used by building tools to assemble modular structures.
    /// </summary>
    [System.Serializable]
    public class Pack
    {
        [SerializeField]
        private string m_name = "New Pack";

        [SerializeField]
        private List<GameObject> m_floors = new();

        [SerializeField]
        private List<GameObject> m_walls = new();

        [SerializeField]
        private List<GameObject> m_roofs = new();

        public string Name => this.m_name;
        public List<GameObject> Floors => this.m_floors;
        public List<GameObject> Walls => this.m_walls;
        public List<GameObject> Roofs => this.m_roofs;

        public void Rename(string newName)
        {
            this.m_name = newName;
        }
    }
}
