using System.Collections.Generic;
using UnityEngine;

namespace BuildingTool.Runtime.Utilities
{
    /// <summary>
    /// Provides color transformation utilities for different types of color blindness.
    /// Allows simulation and correction of colors to improve accessibility.
    /// </summary>
    public static class ColorBlindCorrection
    {
        public enum ColorBlindMode
        {
            Normal,
            Achromatomaly,
            Achromatopsia,
            Deuteranomaly,
            Deuteranopia,
            Protanomaly,
            Protanopia,
            Tritanomaly,
            Tritanopia
        }

        private static readonly Dictionary<ColorBlindMode, (Vector3 red, Vector3 green, Vector3 blue)> Presets = new()
        {
            { ColorBlindMode.Normal,        (new Vector3(100, 0, 0), new Vector3(0, 100, 0), new Vector3(0, 0, 100)) },
            { ColorBlindMode.Achromatomaly, (new Vector3(61.8f, 32.0f, 6.2f), new Vector3(16.3f, 77.5f, 6.2f), new Vector3(16.3f, 32.0f, 51.6f)) },
            { ColorBlindMode.Achromatopsia, (new Vector3(29.9f, 58.7f, 11.4f), new Vector3(29.9f, 58.7f, 11.4f), new Vector3(29.9f, 58.7f, 11.4f)) },
            { ColorBlindMode.Deuteranomaly, (new Vector3(80.0f, 20.0f, 0f), new Vector3(25.8f, 74.2f, 0f), new Vector3(0f, 14.2f, 85.8f)) },
            { ColorBlindMode.Deuteranopia,  (new Vector3(62.5f, 37.5f, 0f), new Vector3(70.0f, 30.0f, 0f), new Vector3(0f, 30.0f, 70.0f)) },
            { ColorBlindMode.Protanomaly,   (new Vector3(81.7f, 18.3f, 0f), new Vector3(33.3f, 66.7f, 0f), new Vector3(0f, 12.5f, 87.5f)) },
            { ColorBlindMode.Protanopia,    (new Vector3(56.667f, 43.333f, 0f), new Vector3(55.833f, 44.167f, 0f), new Vector3(0f, 24.167f, 75.833f)) },
            { ColorBlindMode.Tritanomaly,   (new Vector3(96.7f, 3.3f, 0f), new Vector3(0f, 73.3f, 26.7f), new Vector3(0f, 18.3f, 81.7f)) },
            { ColorBlindMode.Tritanopia,    (new Vector3(95.0f, 5.0f, 0f), new Vector3(0f, 43.333f, 56.667f), new Vector3(0f, 47.5f, 52.5f)) }
        };

        /// <summary>
        /// Transforms a color based on a given color blindness mode preset.
        /// </summary>
        /// <param name="original">The original color.</param>
        /// <param name="mode">The color blindness simulation mode.</param>
        /// <returns>Corrected color value.</returns>
        public static Color Apply(Color original, ColorBlindMode mode)
        {
            var (red, green, blue) = Presets.TryGetValue(mode, out var preset) ? preset : Presets[ColorBlindMode.Normal];

            float r = (original.r * red.x + original.g * red.y + original.b * red.z) / 100f;
            float g = (original.r * green.x + original.g * green.y + original.b * green.z) / 100f;
            float b = (original.r * blue.x + original.g * blue.y + original.b * blue.z) / 100f;

            return new Color(r, g, b, original.a);
        }
    }
}
