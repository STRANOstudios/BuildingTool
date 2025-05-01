using UnityEngine;
using PsychoGarden.Utils;

namespace BuildingTool.Runtime.Utilities
{
    /// <summary>
    /// Defines color settings for visual elements in the Building Tool.
    /// This ScriptableObject allows customization of colors used for plane visualization,
    /// selection highlights, ghost previews, and other UI elements within the editor tools.
    /// </summary>
    [CreateAssetMenu(fileName = "ColorConfig", menuName = "BuildingTool/ColorConfig", order = 1)]
    public class ColorConfig : ScriptableObject
    {
        #region Color Fields -----------------------------------------------

        [SerializeField]
        private Color m_colorPlane = new Color(1f, 1f, 1f, 0.05f); // Semi-transparent white

        [SerializeField]
        private Color m_colorOutline = new Color(1f, 1f, 1f, 1f); // Solid white

        [SerializeField]
        private Color m_colorSelection = Color.yellow;

        [Line]
        [SerializeField]
        private Color m_colorGhostValid = new Color(0f, 1f, 0f, 0.3f); // Transparent green

        [SerializeField]
        private Color m_colorGhostInvalid = new Color(1f, 0f, 0f, 0.3f); // Transparent red

        [Line]
        [SerializeField]
        private ColorBlindCorrection.ColorBlindMode m_colorBlindMode = ColorBlindCorrection.ColorBlindMode.Normal;

        #endregion

        #region Properties -------------------------------------------------

        public Color ColorPlane => AdjustColor(this.m_colorPlane);
        public Color ColorOutline => AdjustColor(this.m_colorOutline);
        public Color ColorSelection => AdjustColor(this.m_colorSelection);
        public Color ColorGhostValid => AdjustColor(this.m_colorGhostValid);
        public Color ColorGhostInvalid => AdjustColor(this.m_colorGhostInvalid);

        #endregion

        #region Methods ----------------------------------------------------

        private Color AdjustColor(Color input)
        {
            return ColorBlindCorrection.Apply(input, this.m_colorBlindMode);
        }

        #endregion
    }
}