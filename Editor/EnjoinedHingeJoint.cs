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
        private SerializedProperty connectedAnchor, autoConfigureConnectedAnchor, useSpring, spring;

        private void OnEnable() {
            connectedAnchor = serializedObject.FindProperty("m_ConnectedAnchor");
            autoConfigureConnectedAnchor = serializedObject.FindProperty("m_AutoConfigureConnectedAnchor");
            useSpring = serializedObject.FindProperty("m_UseSpring");
            spring = serializedObject.FindProperty("m_Spring");
        }

        public override void OnInspectorGUI() {
            base.OnInspectorGUI();

            GUILayout.Space(20f);

            /**
            * Currently the only difference is that the connectedAnchor vector field is greyed out if autoConfigureConnectedAnchor is true
            */
            serializedObject.Update();
            SerializedProperty iterator = serializedObject.GetIterator();
            for(bool enterChildren = true; iterator.NextVisible(enterChildren); enterChildren = false) {
                switch(iterator.propertyPath) {
                    case "m_AutoConfigureConnectedAnchor":
                        break;
                    case "m_ConnectedAnchor":
                        GUIUtilities.DrawConnectedAnchorProperty(connectedAnchor, autoConfigureConnectedAnchor);
                        break;
                    case "m_UseSpring":
                        useSpring.boolValue = EditorGUILayout.ToggleLeft("Spring", useSpring.boolValue);
                        break;
                    case "m_Spring":
                        EditorGUI.indentLevel = 1;
                        SubPropertyOperator subPropertyOperator = new SubPropertyOperator(iterator);
                        while(subPropertyOperator.MoveNext()) {
                            Debug.Log(subPropertyOperator.iterator.propertyPath);
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
                    default:
                        EditorGUILayout.PropertyField(iterator, true, null);
                        break;
                }
            }
            serializedObject.ApplyModifiedProperties();
        }
    }
}