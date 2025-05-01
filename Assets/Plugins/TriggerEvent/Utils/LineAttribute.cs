using UnityEngine;

namespace PsychoGarden.Utils
{
    /// <summary>
    /// Attribute to draw a horizontal line in the Unity Inspector.
    /// </summary>
    /// <summary>
    /// Attribute to draw a horizontal line in the Unity Inspector.
    /// </summary>
    public class LineAttribute : PropertyAttribute
    {
        public float Thickness { get; private set; }
        public float Padding { get; private set; }
        public Color Color { get; private set; }

        // Default constructor
        public LineAttribute()
        {
            Thickness = 1f;
            Padding = 10f;
            Color = new Color(0.5f, 0.5f, 0.5f, 1f);
        }

        // Constructor with thickness and padding
        public LineAttribute(float thickness, float padding)
        {
            Thickness = thickness;
            Padding = padding;
            Color = new Color(0.5f, 0.5f, 0.5f, 1f);
        }

        // Constructor with Color
        public LineAttribute(float thickness, float padding, float r, float g, float b)
        {
            Thickness = thickness;
            Padding = padding;
            Color = new Color(r, g, b, 1f);
        }

        // Constructor with full Color object
        public LineAttribute(float thickness, float padding, Color color)
        {
            Thickness = thickness;
            Padding = padding;
            Color = color == default ? new Color(0.5f, 0.5f, 0.5f, 1f) : color;
        }
    }
}
