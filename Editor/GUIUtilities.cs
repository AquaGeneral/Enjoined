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
            Rect controlRect = EditorGUILayout.GetControlRect();

            GUIStyle labelZeroRightPadding = new GUIStyle(GUI.skin.label);
            labelZeroRightPadding.padding.right = 0;

            GUIStyle labelWithoutPadding = new GUIStyle(GUI.skin.label);
            labelWithoutPadding.padding.left = labelWithoutPadding.padding.right = 0;

            GUIStyle toggleWithoutPadding = new GUIStyle(GUI.skin.toggle);
            toggleWithoutPadding.padding.right = 0;

            float connectedAnchorLabelWidth = labelZeroRightPadding.CalcSize(new GUIContent("Connected Anchor (")).x;
            float rightBracketWidth = labelWithoutPadding.CalcSize(new GUIContent(")")).x;
            float autoLabelWidth = toggleWithoutPadding.CalcSize(new GUIContent("Auto")).x;

            GUI.Label(controlRect, "Connected Anchor (", labelZeroRightPadding);
            bool newAutoConfigure = GUI.Toggle(new Rect(controlRect.x + connectedAnchorLabelWidth, controlRect.y, autoLabelWidth, controlRect.height), autoConfigureConnectedAnchor.boolValue, "Auto", toggleWithoutPadding);
            GUI.Label(new Rect(controlRect.x + connectedAnchorLabelWidth + autoLabelWidth, controlRect.y, controlRect.width, controlRect.height), ")", labelWithoutPadding);
            
            if(autoConfigureConnectedAnchor.boolValue != newAutoConfigure) {
                autoConfigureConnectedAnchor.boolValue = newAutoConfigure;

                // Forcibly reset the connectedAnchor point if it's set to auto configure.
                if(newAutoConfigure == true) {
                    Joint joint = (Joint)connectedAnchor.serializedObject.targetObject;
                    joint.autoConfigureConnectedAnchor = true; // The property must be set to true right now for connectedAnchor to be updated.
                    connectedAnchor.vector3Value = joint.connectedAnchor;
                }
            }

            Rect fillRect = new Rect(controlRect);
            fillRect.xMin = EditorGUIUtility.labelWidth + 13f;
            using(new EditorGUI.DisabledGroupScope(autoConfigureConnectedAnchor.boolValue)) {
                EditorGUI.PropertyField(fillRect, connectedAnchor, GUIContent.none);
            }

            EditorGUILayout.Space();

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