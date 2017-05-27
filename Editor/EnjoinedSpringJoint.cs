/*
* Written by Jesse Stiller: www.rollingkinetics.com
* License: Mozilla Public License Version 2.0
*/

using System;
using UnityEditor;
using UnityEngine;

[CanEditMultipleObjects]
[CustomEditor(typeof(SpringJoint))]
public class EnjoinedSpringJoint : Editor {
    private SerializedProperty connectedBody, anchor, autoConfigureConnectedAnchor, connectedAnchor, spring, damper, 
        minDistance, maxDistance, tolerance, breakForce, breakTorque, enableCollision, enablePreprocessing;

    private void OnEnable() {
        connectedBody                = serializedObject.FindProperty("_connectedBody");
        anchor                       = serializedObject.FindProperty("_anchor");
        autoConfigureConnectedAnchor = serializedObject.FindProperty("_autoConfigureConnectedAnchor");
        connectedAnchor              = serializedObject.FindProperty("_connectedAnchor");
        spring                       = serializedObject.FindProperty("spring");
        damper                       = serializedObject.FindProperty("damper");
        minDistance                  = serializedObject.FindProperty("minDistance");
        maxDistance                  = serializedObject.FindProperty("maxDistance");
        tolerance                    = serializedObject.FindProperty("tolerance");
        breakForce                   = serializedObject.FindProperty("breakForce");
        breakTorque                  = serializedObject.FindProperty("breakTorque");
        enableCollision              = serializedObject.FindProperty("enableCollision");
        enablePreprocessing          = serializedObject.FindProperty("enablePreprocessing");
    }

    //public override void OnInspectorGUI() {
    //    EditorGUILayout.PropertyField(connectedBody);
    //    EditorGUILayout.PropertyField(anchor);
    //    EditorGUILayout.PropertyField(autoConfigureConnectedAnchor);
    //    using(new EditorGUI.DisabledScope(autoConfigureConnectedAnchor.boolValue)) {
    //        EditorGUILayout.PropertyField(connectedAnchor);
    //    }
    //}

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
