using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
[Serializable]
public class MeshX
{
    public int count;
    public Mesh[] mesh;
    public float dis;//多少米 
    public void clear()
    {
        ml.Clear();
    }
    List<Matrix4x4> ml = new List<Matrix4x4>();
    public UnityEngine.Rendering.ShadowCastingMode sd;
    internal void Add(Matrix4x4 x)
    {
        ml.Add(x);
    }

    internal Matrix4x4[] getMatrix()
    {
        return ml.ToArray();
    }

    internal void getMesh()
    {

#if UNITY_EDITOR
        mesh = new Mesh[count];
        for (int i = 0; i < count; i++)
        {
            string path = string.Format("Assets/BakedMeshes/Warrior_Idle_{0}.asset", i);
            var xt = AssetDatabase.LoadAssetAtPath<Mesh>(path);
            if (xt!= null)
            {
                mesh[i] = AssetDatabase.LoadAssetAtPath<Mesh>(path);
            }
        }
#endif
    }

    internal Mesh getCurFrame()
    {
        //1秒是60帧率
        float idx = (dt * 60);
        int frameidx=Mathf.FloorToInt( idx) % mesh.Length;
        var urFrame = mesh[frameidx];
        return urFrame;
    }
    float dt;
    internal void update()
    {
        dt += Time.deltaTime;
    }
}
public class DrawInst : MonoBehaviour
{
    [SerializeField]
    public MeshX[] lodMeshes; // 不同细节级别的网格数组
    public Material  mat; 

    List<Matrix4x4> ml = new List<Matrix4x4>();
    void Start()
    {
        mainCamera= Camera.main;
        mat.enableInstancing = true;
     
        for (int i= 0; i < 100; i++)
        for(int j= 0; j < 100; j++)
        {
            ml.Add(Matrix4x4.TRS(new Vector3(i, 0, j), Quaternion.identity, Vector3.one));
        }
        //matrices = ml.ToArray();
    }
    Camera mainCamera;
    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < lodMeshes.Length; i++)
        {
            lodMeshes[i].update();
        }
        for (int i = 0; i < lodMeshes.Length; i++)
        {
            lodMeshes[i].clear();
        }
        for (int i = 0; i < ml.Count; i++)
        {
            var x = ml[i];
            Vector3 instancePosition = x.GetColumn(3);
            float distance = Vector3.Distance(mainCamera.transform.position, instancePosition);
            if(distance < lodMeshes[0].dis)
            {
                lodMeshes[0].Add(x);
            }
            else if(distance < lodMeshes[1].dis)
            {
                lodMeshes[1].Add(x);
            }
            else
            {
                lodMeshes[2].Add(x);
            }
        }

        for (int i = 0; i < lodMeshes.Length; i++)
        {
            var mr = lodMeshes[i].getMatrix();
            Graphics.DrawMeshInstanced(lodMeshes[i].getCurFrame(), 0, mat,mr,mr.Length,null, lodMeshes[i].sd);
        } 
    }

    public void getMesh()
    {
        for (int i = 0; i < lodMeshes.Length; i++)
        {
            var lm = lodMeshes[i];
            lm.getMesh();
        }

    }
}
