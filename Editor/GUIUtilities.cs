/*
* Written by Jesse Stiller: www.rollingkinetics.com
* License: Mozilla Public License Version 2.0 (https://www.mozilla.org/en-US/MPL/2.0/)
*/

using System;
using UnityEditor;
using UnityEngine;

namespace JesseStiller.Enjoined {
    public static class GUIUtilities {
        private static readonly GUIContent[] anchorTypesGUIContent = new GUIContent[] { new GUIContent("User"), new GUIContent("Automatic") };

        public static void DrawConnectedAnchorProperty(SerializedProperty connectedAnchor, SerializedProperty autoConfigureConnectedAnchor) {
            Rect controlRect = EditorGUILayout.GetControlRect();
            Rect fillRect = EditorGUI.PrefixLabel(controlRect, new GUIContent("Connected Anchor Mode"));
            fillRect.y -= 1f;
            // HACK: For some reason the x-axis position and width are being sub-pixel for some reason, so round them to whole numbers
            fillRect.x = Mathf.Round(fillRect.x);
            fillRect.width = Mathf.Round(fillRect.width);
            
            autoConfigureConnectedAnchor.boolValue = GUI.Toolbar(fillRect, autoConfigureConnectedAnchor.boolValue ? 1 : 0, anchorTypesGUIContent, EditorStyles.radioButton) == 1;
            using(new EditorGUI.DisabledGroupScope(autoConfigureConnectedAnchor.boolValue)) {
                EditorGUI.indentLevel = 1;
                EditorGUILayout.PropertyField(connectedAnchor);
                EditorGUI.indentLevel = 0;
            }
        }

        internal static void ToggleAndFill(Action<Rect> toggleControl, Action<Rect> fillControl, bool enableFillControl, bool enableToggle) {
            Rect controlRect = EditorGUILayout.GetControlRect();

            Rect toggleRect = new Rect(controlRect);
            toggleRect.xMax = EditorGUIUtility.labelWidth;
            toggleRect.yMin -= 1f;
            using(new EditorGUI.DisabledScope(enableToggle)) {
                toggleControl(toggleRect);
            }

            if(enableFillControl == false) {
                GUI.enabled = false;
            }
            Rect fillRect = new Rect(controlRect);
            fillRect.xMin = EditorGUIUtility.labelWidth + 14f;
            fillControl(fillRect);

            if(enableFillControl == false) {
                GUI.enabled = true;
            }
        }

        internal static void PropertyFieldWithoutHeader(SerializedProperty serializedProperty) {
            serializedProperty = serializedProperty.Copy();
            SerializedProperty endProperty = serializedProperty.GetEndProperty();
            bool enterChildren = true;
            while(serializedProperty.NextVisible(enterChildren) && !SerializedProperty.EqualContents(serializedProperty, endProperty)) { 
                EditorGUILayout.PropertyField(serializedProperty);
            }
        }
        
        internal static bool FullClickRegionFoldout(string header, bool folded) {
            Rect controlRect= EditorGUILayout.GetControlRect();
            controlRect.x += 3;
            Rect clickRect = new Rect(controlRect);
            float defaultLeftMargin = clickRect.xMin;
            clickRect.xMin = 0f;

            Rect labelRect = new Rect(controlRect);
            labelRect.x += 11f;

            GUI.Label(labelRect, header);
            
            if(Event.current.type == EventType.Repaint) {
                EditorStyles.foldout.Draw(new Rect(controlRect.x, clickRect.y, EditorGUIUtility.labelWidth - EditorGUI.indentLevel, clickRect.height), false, false, folded, false);
            }

            Event currentEvent = Event.current;
            if(currentEvent.type == EventType.MouseDown && clickRect.Contains(currentEvent.mousePosition)) {
                folded = !folded;
                currentEvent.Use();
            }
            return folded;
        }
    }
}