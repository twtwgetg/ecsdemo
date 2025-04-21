using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

#if !UNITY_DISABLE_MANAGED_COMPONENTS  
public partial struct createsystem : ISystem
{

    public void OnUpdate(ref SystemState state)
    {
        var noLoadingQuery = SystemAPI.QueryBuilder()
            .WithAll<Mr>() // 必须拥有 msxExp 组件     
            .Build();

        GameObject gb = null;
        // 遍历查询到的实体并绘制网格  
        foreach (var entity in noLoadingQuery.ToEntityArray(Allocator.Temp))
        {
            // 获取 msxExp 组件  
            var dt = SystemAPI.ManagedAPI.GetComponent<Mr>(entity);
            gb = dt.zs;
            state.EntityManager.RemoveComponent<Mr>(entity); // 修复：使用 EntityManager.RemoveComponent 替代  
        }
        if (gb != null)
        {
            for (int i = 0; i < 100; i++)
            {
                // 创建一个新的实体  
                var entity = state.EntityManager.CreateEntity();

                // 创建并初始化 msxExp 组件  
                var msxExpComponent = new msxExp
                {
                    pos = new Vector3(i % 10, 0, i / 10), // 设置位置，排列成 10x10 的网格  
                    rot = Quaternion.identity, // 默认旋转  
                    sca = Vector3.one, // 默认缩放  
                    zs = gb.GetComponent<zhanshi>(),
                    time = Random.Range(0, 2f),
                    mesh = Resources.GetBuiltinResource<Mesh>("Cube.fbx"), // 使用内置立方体网格  
                    mat = new Material(Shader.Find("Standard")) // 使用标准材质  
                };

                // 为实体添加 msxExp 组件  
                state.EntityManager.AddComponentObject(entity, msxExpComponent);
            }

        }

        // 查询所有拥有 msxExp 组件的实体  
        noLoadingQuery = SystemAPI.QueryBuilder()
            .WithAll<msxExp>() // 必须拥有 msxExp 组件     
            .Build(); 
        Dictionary<Mesh ,List<Matrix4x4>> dic = new Dictionary<Mesh, List<Matrix4x4>>();
        // 遍历查询到的实体并绘制网格  
        foreach (var entity in noLoadingQuery.ToEntityArray(Allocator.Temp))
        {
            // 获取 msxExp 组件  
            var dt = SystemAPI.ManagedAPI.GetComponent<msxExp>(entity);
            dt.time += Time.deltaTime;
            // 创建变换矩阵  
            Matrix4x4 matrix4X4 = Matrix4x4.TRS(dt.pos, dt.rot, dt.sca);
            var m = dt.zs.getMesh("Idle", dt.time);
            if (!dic.ContainsKey(m))
            {
                dic[m] = new List<Matrix4x4>();
            }
            dic[m].Add(matrix4X4);

            // 绘制网格  
            //Graphics.DrawMesh(dt.zs.getMesh("Idle",dt.time), matrix4X4, dt.mat, 0);
        }
 
        foreach (var x in dic)
        {
            Graphics.DrawMeshInstanced(x.Key, 0, mat, x.Value.ToArray());
        }
    }
    static Material _mat;
    static Material mat
    {
        get
        {
            if (_mat == null)
            {
                _mat = new Material(Shader.Find("Standard"));
                _mat.enableInstancing = true;
            }
            return _mat;
        }
    }
}
#endif
