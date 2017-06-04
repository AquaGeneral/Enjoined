/*
* Written by Jesse Stiller: www.rollingkinetics.com
* License: Mozilla Public License Version 2.0 (https://www.mozilla.org/en-US/MPL/2.0/)
*/

using System;
using UnityEditor;
using UnityEngine;

namespace JesseStiller.Enjoined {
    public static class GUIUtilities {
        public static void DrawConnectedAnchorProperty(SerializedProperty connectedAnchor, SerializedProperty autoConfigureConnectedAnchor) {
            Rect controlRect2 = EditorGUILayout.GetControlRect();
            Rect fillRect2 = EditorGUI.PrefixLabel(controlRect2, new GUIContent("Connected Anchor Mode"));
            autoConfigureConnectedAnchor.boolValue = GUI.Toolbar(fillRect2, autoConfigureConnectedAnchor.boolValue ? 1 : 0, new GUIContent[] { new GUIContent("User"), new GUIContent("Automatic") }, EditorStyles.radioButton) == 1;
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
    }
}