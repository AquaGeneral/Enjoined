﻿/*
* Written by Jesse Stiller: www.rollingkinetics.com
* License: Mozilla Public License Version 2.0 (https://www.mozilla.org/en-US/MPL/2.0/)
*/

using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace JesseStiller.Enjoined {
    [CanEditMultipleObjects]
    [CustomEditor(typeof(ConfigurableJoint))]
    public class EnjoinedConfigurableJoint : Editor {
        private readonly string[] xyzStrings = new string[] { "X", "Y", "Z" };

        private ConfigurableJoint joint;
        private Dictionary<string, SerializedProperty> properties = new Dictionary<string, SerializedProperty>();
        
        private void OnEnable() {
            joint = (ConfigurableJoint)serializedObject.targetObject;

            SerializedProperty iterator = serializedObject.GetIterator();
            while(iterator.Next(true)) {
                // Remove the initial "m_" prefix
                properties.Add(iterator.propertyPath.Remove(0, 2), iterator.Copy());
            }
        }

        public override void OnInspectorGUI() {
            serializedObject.Update();

            DrawSerializedProperties("ConnectedBody", "Anchor", "Axis");
            
            GUIUtilities.DrawConnectedAnchorProperty(properties["ConnectedAnchor"], properties["AutoConfigureConnectedAnchor"]);

            EditorGUILayout.PropertyField(properties["SecondaryAxis"]);

            EditorGUILayout.LabelField("Linear Limit", EditorStyles.boldLabel);
            MultiPropertyField("Linear Motion", xyzStrings, properties["XMotion"], properties["YMotion"], properties["ZMotion"]);

            /**
            * Linear Limit
            */
            EditorGUI.indentLevel = 1;
            if(joint.xMotion != ConfigurableJointMotion.Limited && joint.yMotion != ConfigurableJointMotion.Limited && joint.zMotion != ConfigurableJointMotion.Limited) {
                EditorGUILayout.HelpBox("The following linear limits are only used when at least one axis of Linear Motion is set to Limited", MessageType.Info);
            }

            EditorGUI.BeginChangeCheck();
            SoftJointLimit linearLimitSoftJointLimit = new SoftJointLimit();
            SoftJointLimitSpring linearLimitSpring = new SoftJointLimitSpring();
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

            EditorGUILayout.LabelField("Angular Limits", EditorStyles.boldLabel);

            MultiPropertyField("Angular Motion", xyzStrings, properties["AngularXMotion"], properties["AngularYMotion"], properties["AngularZMotion"]);

            /**
            * X-axis Angular Limit
            */
            EditorGUILayout.LabelField("Angular X Limit");
            EditorGUI.indentLevel = 1;
            GUIUtilities.MinMaxWithFloatFields("Angle", properties["LowAngularXLimit.limit"], properties["HighAngularXLimit.limit"], -180f, 180f, 3);
            // TODO: Does the max bounciness and contact distance values actually ever get used? Is the lower/high angular limit distinction only for the joint angle limit?
            GUIUtilities.MinMaxWithFloatFields("Bounciness", properties["LowAngularXLimit.bounciness"], properties["HighAngularXLimit.bounciness"], 0f, 1f, 10);
            MinMaxPropertyFieldsControl("Contact Distance", properties["LowAngularXLimit.contactDistance"], properties["HighAngularXLimit.contactDistance"]);
            EditorGUILayout.PropertyField(properties["AngularXLimitSpring.spring"], new GUIContent("Spring Force"));
            DrawSerializedProperties("AngularXLimitSpring.damper");
            EditorGUI.indentLevel = 0;

            /**
            * Y-axis Angular Limit
            */
            EditorGUILayout.LabelField("Angular Y Limit");
            EditorGUI.indentLevel = 1;
            GUIUtilities.MinMaxWithFloatFields("Angle", properties["AngularYLimit.limit"], properties["AngularYLimit.limit"], -180f, 180f, 3);
            GUIUtilities.MinMaxWithFloatFields("Bounciness", properties["AngularYLimit.bounciness"], properties["AngularYLimit.bounciness"], -180f, 180f, 3);
            MinMaxPropertyFieldsControl("Contact Distance", properties["AngularYLimit.contactDistance"], properties["AngularYLimit.contactDistance"]);
            EditorGUI.indentLevel = 0;

            EditorGUILayout.LabelField("Angular Z Limit");
            EditorGUI.indentLevel = 1;
            GUIUtilities.MinMaxWithFloatFields("Angle", properties["AngularZLimit.limit"], properties["AngularZLimit.limit"], -180f, 180f, 3);
            GUIUtilities.MinMaxWithFloatFields("Bounciness", properties["AngularZLimit.bounciness"], properties["AngularZLimit.bounciness"], -180f, 180f, 3);
            MinMaxPropertyFieldsControl("Contact Distance", properties["AngularZLimit.contactDistance"], properties["AngularZLimit.contactDistance"]);
            EditorGUI.indentLevel = 0;

            EditorGUILayout.LabelField("Angular YZ Spring");
            EditorGUI.indentLevel = 1;
            EditorGUILayout.PropertyField(properties["AngularYZLimitSpring.spring"], new GUIContent("Spring Force"));
            DrawSerializedProperties("AngularYZLimitSpring.damper");
            EditorGUI.indentLevel = 0;

            EditorGUILayout.LabelField("Positional Drive", EditorStyles.boldLabel);

            DrawSerializedProperties("TargetPosition", "TargetVelocity");
            PropertyFieldsWithoutFoldout("XDrive", "YDrive", "ZDrive");

            /**
            * Rotation Drive
            */
            EditorGUILayout.LabelField("Rotational Drive", EditorStyles.boldLabel);
            DrawSerializedProperties("TargetAngularVelocity");
            MultiPropertyField("Target Rotation", new string[] { "X", "Y", "Z", "W" }, 
                properties["TargetRotation.x"], properties["TargetRotation.y"], properties["TargetRotation.z"], properties["TargetRotation.w"]);
            DrawSerializedProperties("RotationDriveMode");
            EditorGUI.indentLevel = 1;
            PropertyFieldsWithoutFoldout("AngularXDrive", "AngularYZDrive", "SlerpDrive");

            EditorGUI.indentLevel = 0;

            EditorGUILayout.LabelField("Miscellaneous", EditorStyles.boldLabel);

            DrawSerializedProperties("ProjectionMode", "ProjectionDistance", "ProjectionAngle", "ConfiguredInWorldSpace", "SwapBodies", "BreakForce", "BreakTorque", "EnableCollision", "EnablePreprocessing", "MassScale", "ConnectedMassScale");

            serializedObject.ApplyModifiedProperties();
        }

        private void PropertyFieldsWithoutFoldout(params string[] serializedPropertyPaths) {
            foreach(string serializedPropertyPath in serializedPropertyPaths) {
                SerializedProperty serializedProperty = properties[serializedPropertyPath].Copy();
                SerializedProperty endProperty = serializedProperty.GetEndProperty();
                bool enterChildren = true;
                EditorGUILayout.LabelField(serializedProperty.displayName);
                EditorGUI.indentLevel++;
                while(serializedProperty.NextVisible(enterChildren) && !SerializedProperty.EqualContents(serializedProperty, endProperty)) {
                    EditorGUILayout.PropertyField(serializedProperty);
                }
                EditorGUI.indentLevel--;
            }
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

        private static void MultiPropertyField(string label, string[] propertyLabels, params SerializedProperty[] properties) {
            Debug.Assert(propertyLabels.Length == properties.Length);

            Rect controlRect = EditorGUILayout.GetControlRect();

            Rect fillRect = EditorGUI.PrefixLabel(controlRect, new GUIContent(label));
            float propertyCellWidth = (fillRect.width - (properties.Length - 1f) * 2f) / properties.Length;

            float lastLabelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 13f;

            Rect cellRect = new Rect(fillRect.x - 1, fillRect.y, propertyCellWidth, fillRect.height);
            for(int i = 0; i < properties.Length; i++) {
                EditorGUI.PropertyField(cellRect, properties[i], new GUIContent(propertyLabels[i]));
                cellRect.x += propertyCellWidth + 2f;
            }

            EditorGUIUtility.labelWidth = lastLabelWidth;
        }
    }
}