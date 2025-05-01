using System;
using System.Reflection;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

#if UNITY_EDITOR
namespace PsychoGarden.TriggerEvents
{
    /// <summary>
    /// Custom property drawer for TriggerEvent, allowing local override of visualization settings.
    /// </summary>
    [CustomPropertyDrawer(typeof(TriggerEvent), true)]
    public class TriggerEventDrawer : PropertyDrawer
    {
        private object unityEventDrawer;
        private MethodInfo unityEventDrawerOnGUIMethod;
        private MethodInfo unityEventDrawerGetHeightMethod;
        private bool initialized = false;

        /// <summary>
        /// Draws the custom inspector GUI for TriggerEvent.
        /// </summary>
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.serializedObject.isEditingMultipleObjects)
            {
                EditorGUI.HelpBox(position, "Multi-object editing not supported yet.", MessageType.Info);
                return;
            }

            Initialize(property);

            SerializedProperty colorProp = property.FindPropertyRelative("editorColor");
            SerializedProperty showConnectionsProp = property.FindPropertyRelative("editorShowConnections");
            SerializedProperty displayModeProp = property.FindPropertyRelative("editorDisplayMode");

            float lineHeight = EditorGUIUtility.singleLineHeight;
            float padding = 2f;

            // Calculate rectangles
            Rect showRect = new Rect(position.x, position.y, position.width, lineHeight);
            Rect colorRect = new Rect(position.x, showRect.yMax + padding, position.width, lineHeight);
            Rect modeRect = new Rect(position.x, colorRect.yMax + padding, position.width, lineHeight);
            Rect separatorRect = new Rect(position.x, modeRect.yMax + padding, position.width, 1f);
            Rect eventRect = new Rect(position.x, separatorRect.yMax + padding * 2, position.width, position.height - (lineHeight * 3 + padding * 5));

            // Draw override toggle
            EditorGUI.BeginChangeCheck();
            bool overrideSettings = EditorGUI.Toggle(showRect, new GUIContent("Override Global Settings"), showConnectionsProp.boolValue);
            if (EditorGUI.EndChangeCheck())
            {
                showConnectionsProp.boolValue = overrideSettings;
                property.serializedObject.ApplyModifiedProperties();
            }

            // Draw color and display mode fields if override is active
            EditorGUI.BeginDisabledGroup(!overrideSettings);

            if (colorProp != null)
                colorProp.colorValue = EditorGUI.ColorField(colorRect, new GUIContent("Connection Color"), colorProp.colorValue);

            if (displayModeProp != null)
                EditorGUI.PropertyField(modeRect, displayModeProp, new GUIContent("Display Mode"));

            EditorGUI.EndDisabledGroup();

            // Draw a separator line
            EditorGUI.DrawRect(separatorRect, new Color(0.3f, 0.3f, 0.3f, 1f));

            // Draw the actual UnityEvent using Unity internal drawer
            if (unityEventDrawer != null && unityEventDrawerOnGUIMethod != null)
            {
                unityEventDrawerOnGUIMethod.Invoke(unityEventDrawer, new object[] { eventRect, property, label });
            }
            else
            {
                EditorGUI.PropertyField(eventRect, property, label, true);
            }
        }

        /// <summary>
        /// Calculates the total height needed for the TriggerEvent property.
        /// </summary>
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            Initialize(property);

            float lineHeight = EditorGUIUtility.singleLineHeight + 2f;
            float baseHeight = (lineHeight * 3) + 8f; // Toggle + Color + Dropdown + Padding

            if (unityEventDrawer != null && unityEventDrawerGetHeightMethod != null)
            {
                baseHeight += (float)unityEventDrawerGetHeightMethod.Invoke(unityEventDrawer, new object[] { property, label });
            }
            else
            {
                baseHeight += EditorGUI.GetPropertyHeight(property, label, true);
            }

            return baseHeight;
        }

        /// <summary>
        /// Initializes the internal UnityEventDrawer via reflection.
        /// </summary>
        private void Initialize(SerializedProperty property)
        {
            if (initialized)
                return;

            var unityEditorInternal = typeof(Editor).Assembly.GetType("UnityEditorInternal.UnityEventDrawer");
            if (unityEditorInternal != null)
            {
                try
                {
                    unityEventDrawer = Activator.CreateInstance(unityEditorInternal, true);
                }
                catch
                {
                    Debug.LogWarning("[TriggerEvent] Could not instantiate UnityEventDrawer — using fallback editor");
                }

                unityEventDrawerOnGUIMethod = unityEditorInternal.GetMethod(
                    "OnGUI",
                    BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
                    null,
                    new Type[] { typeof(Rect), typeof(SerializedProperty), typeof(GUIContent) },
                    null
                );
                unityEventDrawerGetHeightMethod = unityEditorInternal.GetMethod(
                    "GetPropertyHeight",
                    BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
                    null,
                    new Type[] { typeof(SerializedProperty), typeof(GUIContent) },
                    null
                );
            }

            initialized = true;
        }

        /// <summary>
        /// Determines if the TriggerEvent connections should be shown.
        /// </summary>
        public static bool ShouldShowConnections(TriggerEvent triggerEvent)
        {
            return triggerEvent != null && triggerEvent.editorShowConnections && triggerEvent.editorDisplayMode != TriggerEvent.DisplayMode.None;
        }

        /// <summary>
        /// Determines if a specific TriggerEvent should draw based on its local settings.
        /// </summary>
        public static bool ShouldDrawFor(TriggerEvent triggerEvent, GameObject owner)
        {
            if (triggerEvent == null)
                return false;

            return triggerEvent.editorDisplayMode switch
            {
                TriggerEvent.DisplayMode.All => true,
                TriggerEvent.DisplayMode.None => false,
                TriggerEvent.DisplayMode.OnSelected => Selection.activeGameObject == owner,
                _ => true,
            };
        }

        /// <summary>
        /// Evaluates if connections should be shown, taking into account global fallback.
        /// </summary>
        public static bool EvaluateShowConnections(TriggerEvent triggerEvent)
        {
            if (triggerEvent == null)
                return TriggerEventSettings.ShowConnections;

            if (triggerEvent.editorShowConnections)
            {
                // Override ON, use local value
                return true;
            }
            else
            {
                // Override OFF, use global settings
                return TriggerEventSettings.ShowConnections;
            }
        }

        /// <summary>
        /// Evaluates if a specific TriggerEvent should draw for a given owner, considering global settings.
        /// </summary>
        public static bool EvaluateShouldDrawFor(TriggerEvent triggerEvent, GameObject owner)
        {
            if (triggerEvent == null)
                return ShouldDrawForGlobal(owner);

            if (triggerEvent.editorShowConnections)
            {
                return triggerEvent.editorDisplayMode switch
                {
                    TriggerEvent.DisplayMode.All => true,
                    TriggerEvent.DisplayMode.None => false,
                    TriggerEvent.DisplayMode.OnSelected => Selection.activeGameObject == owner,
                    _ => true,
                };
            }
            else
            {
                return ShouldDrawForGlobal(owner);
            }
        }

        /// <summary>
        /// Evaluates global settings to determine if any TriggerEvent should be drawn.
        /// </summary>
        private static bool ShouldDrawForGlobal(GameObject owner)
        {
            return (TriggerEvent.DisplayMode)TriggerEventSettings.DisplayMode switch
            {
                TriggerEvent.DisplayMode.All => true,
                TriggerEvent.DisplayMode.None => false,
                TriggerEvent.DisplayMode.OnSelected => Selection.activeGameObject == owner,
                _ => true,
            };
        }
    }
}
#endif
