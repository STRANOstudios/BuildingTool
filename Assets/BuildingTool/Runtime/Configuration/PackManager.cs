using System.Collections.Generic;
using UnityEngine;

namespace BuildingTool.Runtime.Configuration
{
    /// <summary>
    /// Centralized container that stores and manages all available modular packs.
    /// This ScriptableObject is used as a single point of reference for both editor tools
    /// and runtime systems that require access to the library of building modules.
    /// </summary>
    [CreateAssetMenu(fileName = "PackManager", menuName = "BuildingTool/PackManager", order = 0)]
    public class PackManager : ScriptableObject
    {
        [SerializeField]
        private List<Pack> m_packs = new();

        public List<Pack> Packs => this.m_packs;
    }
}
