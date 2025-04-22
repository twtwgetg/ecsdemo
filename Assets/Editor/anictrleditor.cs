using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
[CustomEditor(typeof(anictrl))]
public class anictrleditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("attack"))
        {
            
            var cx = target as anictrl;
            cx.attack();
        }
    }
}
