using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[Serializable]
public class LodMesh
{
    public Mesh hight;
    public Mesh middle, low;
}
[Serializable]
public class ActionData
{
    //当前动作所有的模型
    public LodMesh[] ml;
}
[Serializable]
public class TAction
{
    public ActionData data;
    public string name;
    public int framecount = 60;

    internal void getMesh()
    {
#if UNITY_EDITOR
        var path = Application.dataPath + "/UMS_LODS";
        var dir = new System.IO.DirectoryInfo(path);
        List<LodMesh> pm = new List<LodMesh>();
        for(int i = 0; i < framecount; i++)
        {
            LodMesh acd = new LodMesh();

            var hpath = string.Format("Assets/BakedMeshes/Warrior_Attack_{0}.asset", i);
            acd.hight = UnityEditor.AssetDatabase.LoadAssetAtPath<Mesh>(hpath);

            var dirpath = string.Format("Warrior_{0}_{1}", name, i);
            var cx = string.Format(path + "/{0}/{1}_combined_static/01_unnamed.mesh", dirpath, dirpath);
            var apath ="Assets/"+ cx.Replace("\\", "/").Replace(Application.dataPath, "");
            acd.middle = UnityEditor.AssetDatabase.LoadAssetAtPath<Mesh>(apath);
            cx = string.Format(path + "/{0}/{1}_combined_static/02_unnamed.mesh", dirpath, dirpath);
            apath = "Assets/" + cx.Replace("\\", "/").Replace(Application.dataPath, "");
            acd.low = UnityEditor.AssetDatabase.LoadAssetAtPath<Mesh>(apath);
            pm.Add(acd);
       
        }
        data.ml = pm.ToArray();
#endif
    }
    /**
     * 根据时间获取当前动作的模型
     */
    internal Mesh getMeshByTime(float time)
    {
        int framecount =(int)( time * 60);
        framecount %= data.ml.Length;
        return data.ml[framecount].hight;
    }
}
public class zhanshi : MonoBehaviour
{
    public TAction[] actions;

    public void getmesh()
    {
        for(int i = 0; i < actions.Length; i++)
        {
            actions[i].getMesh();
        } 
    }
    TAction getAni(string name)
    {
        for(int i= 0; i < actions.Length; i++)
        {
            if(actions[i].name == name)
            {
                return actions[i];
            }
        }
        return null;
    }
    internal Mesh getMesh(string ani, float time)
    {
        var ac = getAni(ani);
        if (ac != null)
        {
            return  ac.getMeshByTime(time);
        }
        return null;
    }
     
}
