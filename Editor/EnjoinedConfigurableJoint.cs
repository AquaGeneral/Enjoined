/*
* Written by Jesse Stiller: www.rollingkinetics.com
* License: Mozilla Public License Version 2.0 (https://www.mozilla.org/en-US/MPL/2.0/)
*/

using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace JesseStiller.Enjoined {
    [CanEditMultipleObjects]
    [CustomEditor(typeof(ConfigurableJoint))]
    public class EnjoinedConfigurableJoint : Editor {
        private bool linearLimitFoldoutState = false;
        
        private ConfigurableJoint joint;

        private Dictionary<string, SerializedProperty> properties = new Dictionary<string, SerializedProperty>();

        private void OnEnable() {
            joint = (ConfigurableJoint)serializedObject.targetObject;

            SerializedProperty iterator = serializedObject.GetIterator();
            while(iterator.Next(true)) {
                // Remove the initial "m_" prefix
                properties.Add(iterator.propertyPath.Remove(0, 2), iterator.Copy());
            }

            FieldInfo[] fields = typeof(EnjoinedConfigurableJoint).GetFields(BindingFlags.NonPublic | BindingFlags.Instance);
            foreach(FieldInfo fieldInfo in fields) {
                if(fieldInfo.FieldType != typeof(SerializedProperty)) continue;

                string propertyPath = "m_" + char.ToUpper(fieldInfo.Name[0]) + fieldInfo.Name.Substring(1);
                fieldInfo.SetValue(this, serializedObject.FindProperty(propertyPath));
            }
        }

        public override void OnInspectorGUI() {
            serializedObject.Update();

            DrawSerializedProperties("ConnectedBody", "Anchor", "Axis");
            
            GUIUtilities.DrawConnectedAnchorProperty(properties["ConnectedAnchor"], properties["AutoConfigureConnectedAnchor"]);

            EditorGUILayout.PropertyField(properties["SecondaryAxis"]);

            MultiPropertyField("Linear Motion", new GUIContent[] { new GUIContent("X"), new GUIContent("Y"), new GUIContent("Z") }, properties["XMotion"], properties["YMotion"], properties["ZMotion"]);

            /**
            * Linear Limit
            */
            if(joint.xMotion != ConfigurableJointMotion.Limited && joint.yMotion != ConfigurableJointMotion.Limited && joint.zMotion != ConfigurableJointMotion.Limited &&
                (joint.linearLimit.limit != 0f || joint.linearLimit.bounciness != 0f || joint.linearLimit.contactDistance != 0f || joint.linearLimitSpring.spring != 0f ||
                joint.linearLimitSpring.damper != 0f)) {
                EditorGUILayout.HelpBox("The following linear limits are only used when at least one axis of Linear Motion is set to Limited", MessageType.Info);
            }

            EditorGUI.BeginChangeCheck();
            SoftJointLimit linearLimitSoftJointLimit = new SoftJointLimit();
            SoftJointLimitSpring linearLimitSpring = new SoftJointLimitSpring();
            EditorGUI.indentLevel = 1;
            linearLimitSoftJointLimit.limit = EditorGUILayout.FloatField("Displacement Limit", joint.linearLimit.limit);
            linearLimitSoftJointLimit.bounciness = EditorGUILayout.FloatField("Bounciness", joint.linearLimit.bounciness);
            linearLimitSoftJointLimit.contactDistance = EditorGUILayout.FloatField("Contact Distance", joint.linearLimit.contactDistance);
            linearLimitSpring.spring = EditorGUILayout.FloatField("Spring Force", joint.linearLimitSpring.spring);
            linearLimitSpring.damper = EditorGUILayout.FloatField("Damper", joint.linearLimitSpring.damper);
            EditorGUI.indentLevel = 0;
            if(EditorGUI.EndChangeCheck()) {
                Undo.RecordObject(joint, "Inspector");
                joint.linearLimit = linearLimitSoftJointLimit;
                joint.linearLimitSpring = linearLimitSpring;
            }

            MultiPropertyField("Angular Motion", new GUIContent[] { new GUIContent("X"), new GUIContent("Y"), new GUIContent("Z") }, properties["AngularXMotion"], properties["AngularYMotion"], properties["AngularZMotion"]);

            /**
            * X-axis Angular Limit
            */
            EditorGUILayout.PrefixLabel("X-axis Angular Limit");
            EditorGUI.indentLevel = 1;
            GUIUtilities.MinMaxWithFloatFields("Angle", properties["LowAngularXLimit.limit"], properties["HighAngularXLimit.limit"], -180f, 180f, 3);
            // TODO: Does the max bounciness and contact distance values actually ever get used? Is the lower/high angular limit distinction only for the joint angle limit?
            GUIUtilities.MinMaxWithFloatFields("Bounciness", properties["LowAngularXLimit.bounciness"], properties["HighAngularXLimit.bounciness"], 0f, 1f, 10);
            MinMaxPropertyFieldsControl("Contact Distance", properties["LowAngularXLimit.contactDistance"], properties["HighAngularXLimit.contactDistance"]);
            EditorGUILayout.PropertyField(properties["AngularXLimitSpring.spring"], new GUIContent("Spring Force"));
            DrawSerializedProperties("AngularXLimitSpring.damper");
            EditorGUI.indentLevel = 0;

            //DrawSerializedProperties(angularXLimitSpring, lowAngularXLimit, highAngularXLimit, angularYLimit, angularYZLimitSpring, angularYLimit, angularZLimit);
            EditorGUILayout.PrefixLabel("Y-axis Angular Limit");
            EditorGUI.indentLevel = 1;

            EditorGUI.indentLevel = 0;

            /**
            * Rotation Drive
            */
            DrawSerializedProperties("RotationDriveMode");
            EditorGUI.indentLevel = 1;
            if((RotationDriveMode)properties["RotationDriveMode"].enumValueIndex == RotationDriveMode.XYAndZ) {
                DrawSerializedProperties("AngularXDrive", "AngularYZDrive");
            } else {
                DrawSerializedProperties("SlerpDrive");
            }
            EditorGUI.indentLevel = 0;

            DrawSerializedProperties("ProjectionMode", "ProjectionDistance", "ProjectionAngle", "ConfiguredInWorldSpace", "SwapBodies", "BreakForce", "BreakTorque", "EnableCollision", "EnablePreprocessing", "MassScale", "ConnectedMassScale");

            serializedObject.ApplyModifiedProperties();



            GUILayout.Space(40f);

            DrawDefaultInspector();
        }
        
        private void DrawSerializedProperties(params string[] propertyNames) {
            foreach(string propertyName in propertyNames) {
                EditorGUILayout.PropertyField(properties[propertyName], true, null);
            }
        }

        private static void MinMaxPropertyFieldsControl(string label, SerializedProperty minProperty, SerializedProperty maxProperty) {
            Rect controlRect = EditorGUILayout.GetControlRect();

            Rect fillRect = EditorGUI.PrefixLabel(controlRect, new GUIContent(label));

            Rect minRect = new Rect(fillRect.x, fillRect.y, fillRect.width * 0.5f - 5f, fillRect.height);
            GUI.Label(new Rect(minRect.x, minRect.y, 25f, minRect.height), "min");
            minRect.xMin += 19f;
            EditorGUI.PropertyField(minRect, maxProperty, GUIContent.none);

            Rect maxRect = new Rect(fillRect.x + fillRect.width * 0.5f + 5f, fillRect.y, fillRect.width * 0.5f - 5f, fillRect.height);
            GUI.Label(new Rect(maxRect.x, maxRect.y, 28f, maxRect.height), "max");
            maxRect.xMin += 21f;
            EditorGUI.PropertyField(maxRect, maxProperty, GUIContent.none);
        }

        private static void MultiPropertyField(string label, GUIContent[] propertyLabels, params SerializedProperty[] properties) {
            Debug.Assert(propertyLabels.Length == properties.Length);

            Rect controlRect = EditorGUILayout.GetControlRect();

            Rect fillRect = EditorGUI.PrefixLabel(controlRect, new GUIContent(label));
            float propertyCellWidth = (fillRect.width - (properties.Length - 1f) * 2f) / properties.Length;

            float lastLabelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 13f;

            Rect cellRect = new Rect(fillRect.x - 1, fillRect.y, propertyCellWidth, fillRect.height);
            for(int i = 0; i < properties.Length; i++) { 
                EditorGUI.PropertyField(cellRect, properties[i], propertyLabels[i]);
                cellRect.x += propertyCellWidth + 2f;
            }

            EditorGUIUtility.labelWidth = lastLabelWidth;
        }
    }
}