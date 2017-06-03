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
            Rect fillRect = EditorGUI.PrefixLabel(controlRect, new GUIContent("Connected Anchor"));
            fillRect.x -= 1;
            float autoLabelWidth = GUI.skin.toggle.CalcSize(new GUIContent("Auto")).x;

            bool newAutoConfigure = EditorGUI.ToggleLeft(
                new Rect(fillRect.x - autoLabelWidth, controlRect.y, autoLabelWidth, controlRect.height), 
                "Auto", autoConfigureConnectedAnchor.boolValue);

            if(autoConfigureConnectedAnchor.boolValue != newAutoConfigure) {
                autoConfigureConnectedAnchor.boolValue = newAutoConfigure;

                // Forcibly reset the connectedAnchor point if it's set to auto configure.
                if(newAutoConfigure == true) {
                    Joint joint = (Joint)connectedAnchor.serializedObject.targetObject;
                    joint.autoConfigureConnectedAnchor = true; // The property must be set to true right now for connectedAnchor to be updated.
                    connectedAnchor.vector3Value = joint.connectedAnchor;
                }
            }
            
            using(new EditorGUI.DisabledScope(autoConfigureConnectedAnchor.boolValue)) {
                EditorGUI.PropertyField(fillRect, connectedAnchor, GUIContent.none);
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