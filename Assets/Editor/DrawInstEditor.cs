using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
[CustomEditor(typeof(DrawInst))]
public class DrawInstEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if(GUILayout.Button("getmodel"))
        {
            var cx = target as DrawInst;
            cx.getMesh();
        }
    }
}
