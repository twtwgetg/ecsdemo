using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
[CustomEditor(typeof(zhanshi))]
public class zhanshieditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("生成"))
        {
            var cx = target as zhanshi;
            cx.getmesh();
        }
    }
}
