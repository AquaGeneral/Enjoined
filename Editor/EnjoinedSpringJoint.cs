/*
* Written by Jesse Stiller: www.rollingkinetics.com
* License: Mozilla Public License Version 2.0 (https://www.mozilla.org/en-US/MPL/2.0/)
*/

using UnityEditor;
using UnityEngine;

namespace JesseStiller.Enjoined {
    [CanEditMultipleObjects]
    [CustomEditor(typeof(SpringJoint))]
    public class EnjoinedSpringJoint : Editor {
        private SerializedProperty connectedAnchor, autoConfigureConnectedAnchor;

        private void OnEnable() {
            connectedAnchor = serializedObject.FindProperty("m_ConnectedAnchor");
            autoConfigureConnectedAnchor = serializedObject.FindProperty("m_AutoConfigureConnectedAnchor");
        }

        public override void OnInspectorGUI() {
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
                    default:
                        EditorGUILayout.PropertyField(iterator, true, null);
                        break;
                }
            }
            serializedObject.ApplyModifiedProperties();
        }
    }
}