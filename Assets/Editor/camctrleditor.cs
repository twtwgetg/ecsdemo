using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
[CustomEditor(typeof(CameraController))]
public class camctrleditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("显示view"))
        {
            (target as CameraController).showview();

        }
        if (GUILayout.Button("显示透视矩阵"))
        {
            (target as CameraController).showproj();

        }
    }
}
