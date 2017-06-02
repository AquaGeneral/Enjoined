/*
* Written by Jesse Stiller: www.rollingkinetics.com
* License: Mozilla Public License Version 2.0 (https://www.mozilla.org/en-US/MPL/2.0/)
*/

using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace JesseStiller.Enjoined {
    [CanEditMultipleObjects]
    [CustomEditor(typeof(ConfigurableJoint))]
    public class EnjoinedConfigurableJoint : Editor {
        // Joint properties
        private SerializedProperty connectedBody, axis, anchor, autoConfigureConnectedAnchor, connectedAnchor, breakForce, breakTorque, enableCollision, enablePreprocessing;

        // Configurable Joint properties
        private SerializedProperty projectionAngle, projectionDistance, projectionMode, slerpDrive, angularYZDrive, angularXDrive, rotationDriveMode, targetAngularVelocity, targetRotation, zDrive, yDrive, xDrive, targetVelocity, targetPosition, angularZLimit, angularYLimit, highAngularXLimit, lowAngularXLimit, linearLimit, angularYZLimitSpring, angularXLimitSpring, angularXMotion, angularYMotion, angularZMotion, zMotion, yMotion, xMotion, secondaryAxis, configuredInWorldSpace, swapBodies;

        private void OnEnable() {
            FieldInfo[] fields = typeof(EnjoinedConfigurableJoint).GetFields(BindingFlags.NonPublic | BindingFlags.Instance);
            foreach(FieldInfo fieldInfo in fields) {
                if(fieldInfo.FieldType != typeof(SerializedProperty)) continue;

                string propertyPath = "m_" + char.ToUpper(fieldInfo.Name[0]) + fieldInfo.Name.Substring(1);
                fieldInfo.SetValue(this, serializedObject.FindProperty(propertyPath));
            }
        }

        public override void OnInspectorGUI() {
            serializedObject.Update();

            DrawSerializedProperties(connectedBody, axis, anchor);
            
            GUIUtilities.DrawConnectedAnchorProperty(connectedAnchor, autoConfigureConnectedAnchor);

            EditorGUILayout.PropertyField(secondaryAxis);

            MultiPropertyField("Linear Motion", new GUIContent[] { new GUIContent("X"), new GUIContent("Y"), new GUIContent("Z") }, xMotion, yMotion, zMotion);

            MultiPropertyField("Angular Motion", new GUIContent[] { new GUIContent("X"), new GUIContent("Y"), new GUIContent("Z") }, angularXMotion, angularYMotion, angularZMotion);

            EditorGUILayout.Space();
            EditorGUILayout.Space();

            serializedObject.ApplyModifiedProperties();

            base.OnInspectorGUI();
        }
        
        private void DrawSerializedProperties(params SerializedProperty[] properties) {
            foreach(SerializedProperty property in properties) {
                EditorGUILayout.PropertyField(property);
            }
        }       

        private static void MultiPropertyField(string label, GUIContent[] propertyLabels, params SerializedProperty[] properties) {
            Debug.Assert(propertyLabels.Length == properties.Length);

            Rect controlRect = EditorGUILayout.GetControlRect();

            Rect fillRect = EditorGUI.PrefixLabel(controlRect, new GUIContent(label));
            float propertyCellWidth = (fillRect.width - (properties.Length - 1f) * 2f) / properties.Length;

            float lastLabelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 13f;

            Rect cellRect = new Rect(fillRect.x, fillRect.y, propertyCellWidth, fillRect.height);
            for(int i = 0; i < properties.Length; i++) { 
                EditorGUI.PropertyField(cellRect, properties[i], propertyLabels[i]);
                cellRect.x += propertyCellWidth + 2f;
            }

            EditorGUIUtility.labelWidth = lastLabelWidth;
        }
    }
}