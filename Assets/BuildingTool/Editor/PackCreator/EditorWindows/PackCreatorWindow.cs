#if UNITY_EDITOR

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using BuildingTool.Runtime.Configuration;
using BuildingTool.Runtime.Utilities;
using Unity.VisualScripting;

namespace BuildingTool.Editor.PackCreator.EditorWindows
{
    /// <summary>
    /// Editor window for creating, editing, and managing modular building packs.
    /// Allows users to define categorized prefab lists and persist them inside a PackManager asset.
    /// </summary>
    public class PackCreatorWindow : EditorWindow
    {
        #region Variables --------------------------------------------------

        private PackManager m_packManager;
        private int m_selectedIndex = -1;
        private string m_newPackName = "New Pack";
        private Vector2 m_scrollPosition;

        #endregion

        #region Menu & Lifecycle -------------------------------------------

        [MenuItem("Tools/Building Tool/Modular Pack Manager")]
        public static void Open()
        {
            PackCreatorWindow window = GetWindow<PackCreatorWindow>();
            window.titleContent = new GUIContent("Modular Pack Manager");
            window.minSize = new Vector2(400f, 300f);
            window.Show();
        }

        private void OnEnable()
        {
            this.m_packManager = ScriptableObjectLoader.LoadOrCreate<PackManager>(BTPaths.PackManagerAsset);
        }

        #endregion

        #region GUI --------------------------------------------------------

        private void OnGUI()
        {
            this.m_scrollPosition = EditorGUILayout.BeginScrollView(this.m_scrollPosition);

            if (this.m_packManager.Packs.Count > 0)
            {
                string[] options = this.GetPackNames();
                this.m_selectedIndex = EditorGUILayout.Popup("Select Pack", this.m_selectedIndex, options);

                if (this.m_selectedIndex >= 0 && this.m_selectedIndex < this.m_packManager.Packs.Count)
                {
                    this.DrawSelectedPack(this.m_packManager.Packs[this.m_selectedIndex]);
                }
            }

            EditorGUILayout.Space(8);
            EditorGUILayout.LabelField("Create New Pack", EditorStyles.boldLabel);
            this.m_newPackName = EditorGUILayout.TextField("Name", this.m_newPackName);

            if (GUILayout.Button("Create"))
            {
                this.CreateNewPack(this.m_newPackName);
            }

            EditorGUILayout.EndScrollView();
        }

        #endregion

        #region Pack Drawing -----------------------------------------------

        private void DrawSelectedPack(Pack pack)
        {
            string newName = EditorGUILayout.TextField("Pack Name", pack.Name);
            if (newName != pack.Name)
            {
                pack.Rename(newName);
                EditorUtility.SetDirty(this.m_packManager);
            }

            SerializedObject so = new SerializedObject(this.m_packManager);
            SerializedProperty packs = so.FindProperty("m_packs");
            SerializedProperty currentPack = packs.GetArrayElementAtIndex(this.m_selectedIndex);

            EditorGUILayout.PropertyField(currentPack.FindPropertyRelative("m_floors"), true);
            EditorGUILayout.PropertyField(currentPack.FindPropertyRelative("m_walls"), true);
            EditorGUILayout.PropertyField(currentPack.FindPropertyRelative("m_roofs"), true);

            so.ApplyModifiedProperties();

            EditorGUILayout.Space(4);
            if (GUILayout.Button("Delete This Pack"))
            {
                this.m_packManager.Packs.RemoveAt(this.m_selectedIndex);
                this.m_selectedIndex = -1;

                EditorUtility.SetDirty(this.m_packManager);
                AssetDatabase.SaveAssets();
            }
        }

        #endregion

        #region Helpers ----------------------------------------------------

        private void CreateNewPack(string packName)
        {
            Pack newPack = new Pack();
            newPack.Rename(packName);
            this.m_packManager.Packs.Add(newPack);

            this.m_selectedIndex = this.m_packManager.Packs.Count - 1;

            EditorUtility.SetDirty(this.m_packManager);
            AssetDatabase.SaveAssets();
        }

        private string[] GetPackNames()
        {
            List<string> names = new List<string>();
            foreach (Pack pack in this.m_packManager.Packs)
            {
                names.Add(pack.Name);
            }
            return names.ToArray();
        }

        #endregion
    }
}
#endif
