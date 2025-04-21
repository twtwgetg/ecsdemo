using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
[CustomEditor(typeof(createavator))]

public class createprefabeditor :Editor
{
    // Start is called before the first frame update
    
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("clear"))
        {
            var cx = target as createavator;
            cx.transform.Clear();
        }
        if (GUILayout.Button("create"))
        {
           var cx =  target as createavator;
            for(int i = 0; i < cx.x; i++)
            {
                for(int j= 0; j < cx.y; j++)
                {
                    GameObject.Instantiate(cx.prefab, new Vector3(i, 0, j), Quaternion.identity,cx.transform);
                }
            }
        }
    }
}
