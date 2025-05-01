#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;
using BuildingTool.Runtime.Utilities;

namespace BuildingTool.Editor.Builder3D.Handlers
{
    /// <summary>
    /// Handles the creation, visualization and positioning of the ghost preview in the 3D scene.
    /// Automatically updates based on external position data and applies a lightweight transparent material.
    /// </summary>
    public class PreviewGhostHandler
    {
        #region Fields -------------------------------------------------------

        private readonly ColorConfig m_colorConfig;
        private GameObject m_previewInstance;
        private Material m_ghostMaterial;
        private Quaternion m_rotation = Quaternion.identity;
        private bool m_isCurrentPositionValid = true;

        #endregion

        #region Properties ---------------------------------------------------

        public Vector3 Position => this.m_previewInstance != null ? this.m_previewInstance.transform.position : Vector3.zero;
        public Quaternion Rotation => this.m_rotation;

        #endregion

        #region Constructor --------------------------------------------------

        /// <summary>
        /// Creates a new preview handler using the specified color configuration.
        /// </summary>
        /// <param name="colorConfig">Reference to the ColorConfig ScriptableObject.</param>
        public PreviewGhostHandler(ColorConfig colorConfig)
        {
            this.m_colorConfig = colorConfig;
            this.CreateGhostMaterial();
        }

        #endregion

        #region Public Methods -----------------------------------------------

        /// <summary>
        /// Updates the ghost preview position and rotation based on provided world position.
        /// </summary>
        /// <param name="prefab">The prefab to preview.</param>
        /// <param name="worldPosition">Calculated position where the ghost should appear.</param>
        public void UpdatePreview(GameObject prefab, Vector3 worldPosition)
        {
            if (prefab == null)
            {
                this.ClearPreview();
                return;
            }

            if (this.m_previewInstance == null || this.m_previewInstance.name != prefab.name)
                this.SpawnPreviewInstance(prefab);

            this.m_previewInstance.transform.position = worldPosition;
            this.m_previewInstance.transform.rotation = this.m_rotation;

            this.m_isCurrentPositionValid = this.CheckForOverlap();
            this.UpdateGhostColor(this.m_isCurrentPositionValid);

            SceneView.RepaintAll();
        }

        /// <summary>
        /// Rotates the preview instance by the given angle on Y.
        /// </summary>
        /// <param name="angle">Rotation angle in degrees.</param>
        public void RotatePreview(float angle)
        {
            this.m_rotation *= Quaternion.Euler(0f, angle, 0f);
        }

        /// <summary>
        /// Clears the current preview instance from the scene.
        /// </summary>
        public void ClearPreview()
        {
            if (this.m_previewInstance != null)
            {
                GameObject.DestroyImmediate(this.m_previewInstance);
                this.m_previewInstance = null;
            }
        }

        /// <summary>
        /// Returns true if the current preview does not collide with anything.
        /// </summary>
        public bool IsValidPlacement()
        {
            return this.m_isCurrentPositionValid;
        }

        #endregion

        #region Private Methods ----------------------------------------------

        private void SpawnPreviewInstance(GameObject prefab)
        {
            this.ClearPreview();

            this.m_previewInstance = GameObject.Instantiate(prefab);
            this.m_previewInstance.name = prefab.name;
            this.m_previewInstance.hideFlags = HideFlags.HideAndDontSave;

            foreach (var renderer in this.m_previewInstance.GetComponentsInChildren<Renderer>())
            {
                renderer.sharedMaterial = this.m_ghostMaterial;
            }
        }

        private void CreateGhostMaterial()
        {
            Shader shader = Shader.Find("Unlit/Color");
            if (shader == null)
            {
                BTDebug.LogError("Failed to find 'Unlit/Color' shader for ghost material.");
                return;
            }

            this.m_ghostMaterial = new Material(shader)
            {
                color = this.m_colorConfig.ColorGhostValid
            };
            this.m_ghostMaterial.SetFloat("_Mode", 3); // Transparent
            this.m_ghostMaterial.renderQueue = 3000;
        }

        private void UpdateGhostColor(bool isValid)
        {
            if (this.m_ghostMaterial == null) return;

            this.m_ghostMaterial.color = isValid
                ? this.m_colorConfig.ColorGhostValid
                : this.m_colorConfig.ColorGhostInvalid;
        }

        private bool CheckForOverlap()
        {
            if (!this.GetOrientedOverlapBox(this.m_previewInstance, out Vector3 center, out Vector3 size, out Quaternion rotation))
                return true;

            Vector3 halfExtents = size * 0.5f;

            Collider[] hits = Physics.OverlapBox(center, halfExtents, rotation);
            hits = System.Array.FindAll(hits, h =>
                h != null && !IsChildOf(h.gameObject, this.m_previewInstance)
            );

            if (hits.Length > 0)
            {
                string names = string.Join(", ", System.Array.ConvertAll(hits, h => h.name));
                BTDebug.LogWarning($"Invalid placement: overlaps with {hits.Length} object(s): {names}");
                return false;
            }

            return true;
        }

        private bool GetOrientedOverlapBox(GameObject root, out Vector3 center, out Vector3 size, out Quaternion rotation)
        {
            center = Vector3.zero;
            size = Vector3.one;
            rotation = Quaternion.identity;

            if (root == null) return false;

            Renderer[] renderers = root.GetComponentsInChildren<Renderer>();
            if (renderers.Length == 0) return false;

            Bounds localBounds = new Bounds(Vector3.zero, Vector3.zero);
            bool first = true;

            foreach (Renderer renderer in renderers)
            {
                Transform t = renderer.transform;
                MeshFilter mf = renderer.GetComponent<MeshFilter>();
                if (mf == null || mf.sharedMesh == null)
                    continue;

                // Get bounds of the mesh in its local space
                Bounds meshBounds = mf.sharedMesh.bounds;

                // Transform mesh bounds center to local space of the root
                Vector3 localCenter = root.transform.InverseTransformPoint(t.TransformPoint(meshBounds.center));
                Vector3 localSize = Vector3.Scale(meshBounds.size, t.lossyScale);

                if (first)
                {
                    localBounds = new Bounds(localCenter, localSize);
                    first = false;
                }
                else
                {
                    localBounds.Encapsulate(new Bounds(localCenter, localSize));
                }
            }

            if (first)
                return false;

            center = root.transform.TransformPoint(localBounds.center);
            size = localBounds.size * 0.90f;
            rotation = root.transform.rotation;
            
            return true;
        }

        private bool IsChildOf(GameObject obj, GameObject potentialParent)
        {
            Transform current = obj.transform;
            while (current != null)
            {
                if (current == potentialParent.transform)
                    return true;
                current = current.parent;
            }
            return false;
        }

        #endregion

        #region Gizmos -------------------------------------------------------

        /// <summary>
        /// Draws the overlap box preview used for collision validation.
        /// Uses reduced size and current ghost rotation.
        /// </summary>
        public void DrawDebugOverlapBox()
        {
            if (!this.GetOrientedOverlapBox(this.m_previewInstance, out Vector3 center, out Vector3 size, out Quaternion rotation))
                return;

            Handles.color = this.m_isCurrentPositionValid
                ? this.m_colorConfig.ColorGhostValid
                : this.m_colorConfig.ColorGhostInvalid;

            using (new Handles.DrawingScope(Matrix4x4.TRS(center, rotation, Vector3.one)))
            {
                Handles.DrawWireCube(Vector3.zero, size);
            }
        }

        #endregion
    }
}

#endif
