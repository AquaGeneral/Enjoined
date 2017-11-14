/*
* Written by Jesse Stiller: www.rollingkinetics.com
* License: Mozilla Public License Version 2.0 (https://www.mozilla.org/en-US/MPL/2.0/)
*/

using UnityEditor;
using UnityEngine;

namespace JesseStiller.Enjoined {
    [CanEditMultipleObjects]
    [CustomEditor(typeof(HingeJoint))]
    public class EnjoinedHingeJoint : Editor {
        private SerializedProperty connectedAnchor, autoConfigureConnectedAnchor, useSpring, spring, useMotor, motor, useLimits, limits;

        private void OnEnable() {
            connectedAnchor = serializedObject.FindProperty("m_ConnectedAnchor");
            autoConfigureConnectedAnchor = serializedObject.FindProperty("m_AutoConfigureConnectedAnchor");
            useSpring = serializedObject.FindProperty("m_UseSpring");
            spring = serializedObject.FindProperty("m_Spring");
            useMotor = serializedObject.FindProperty("m_UseMotor");
            motor = serializedObject.FindProperty("m_Motor");
            useLimits = serializedObject.FindProperty("m_UseLimits");
            limits = serializedObject.FindProperty("m_Limits");
        }

        public override void OnInspectorGUI() {
            serializedObject.Update();
            SerializedProperty iterator = serializedObject.GetIterator();
            for(bool enterChildren = true; iterator.NextVisible(enterChildren); enterChildren = false) {
                SubPropertyOperator subPropertyOperator;

                switch(iterator.propertyPath) {
                    case "m_AutoConfigureConnectedAnchor":
                        break;
                    case "m_ConnectedAnchor":
                        GUIUtilities.DrawConnectedAnchorProperty(connectedAnchor, autoConfigureConnectedAnchor);
                        break;
                    case "m_UseSpring":
                        useSpring.boolValue = EditorGUILayout.Toggle("Spring", useSpring.boolValue);
                        break;
                    case "m_Spring":
                        EditorGUI.indentLevel = 1;
                        subPropertyOperator = new SubPropertyOperator(iterator);
                        while(subPropertyOperator.MoveNext()) {
                            switch(subPropertyOperator.iterator.propertyPath) {
                                case "m_Spring.spring":
                                    EditorGUILayout.PropertyField(subPropertyOperator.iterator, new GUIContent("Force"));
                                    break;
                                default:
                                    EditorGUILayout.PropertyField(subPropertyOperator.iterator);
                                    break;
                            }
                        }
                        EditorGUI.indentLevel = 0;
                        break;
                    case "m_UseMotor":
                        useMotor.boolValue = EditorGUILayout.Toggle("Motor", useMotor.boolValue);
                        break;
                    case "m_Motor":
                        EditorGUI.indentLevel = 1;
                        GUIUtilities.PropertyFieldWithoutHeader(motor);
                        EditorGUI.indentLevel = 0;
                        break;
                    case "m_UseLimits":
                        useLimits.boolValue = EditorGUILayout.Toggle("Limits", useLimits.boolValue);
                        break;
                    case "m_Limits":
                        EditorGUI.indentLevel = 1;
                        subPropertyOperator = new SubPropertyOperator(iterator);
                        while(subPropertyOperator.MoveNext()) {
                            EditorGUILayout.PropertyField(subPropertyOperator.iterator);

                            // Clamp the min and max limit properties only to -180 to 180
                            switch(subPropertyOperator.iterator.propertyPath) {
                                case "m_Limits.min":
                                case "m_Limits.max":
                                    subPropertyOperator.iterator.floatValue = Mathf.Clamp(subPropertyOperator.iterator.floatValue, -180f, 180f);
                                    break;
                            }
                        }
                        EditorGUI.indentLevel = 0;
                        break;
                    default:
                        EditorGUILayout.PropertyField(iterator, true, null);
                        break;
                }
            }
            serializedObject.ApplyModifiedProperties();
        }
    }
}