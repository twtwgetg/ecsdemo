using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

#if !UNITY_DISABLE_MANAGED_COMPONENTS  
public partial struct createsystem : ISystem
{
    private NativeHashMap<int, NativeList<Matrix4x4>> _meshInstanceMap;
    private EntityQuery _msxExpQuery; 
    private EntityQuery _XrQuery;
    public void OnCreate(ref SystemState state)
    {
        // 初始化 NativeHashMap，用于存储 Mesh 和对应的矩阵列表
        _meshInstanceMap = new NativeHashMap<int, NativeList<Matrix4x4>>(10, Allocator.Persistent);
        _msxExpQuery = SystemAPI.QueryBuilder().WithAll<msxExp>().Build();
        _XrQuery = SystemAPI.QueryBuilder()
        .WithAll<Mr>() // 必须拥有 msxExp 组件     
        .Build();
    }
    public void OnDestroy(ref SystemState state)
    {
        // 释放 NativeHashMap 和内部的 NativeList
        foreach (var kvp in _meshInstanceMap)
        {
            kvp.Value.Dispose();
        }
        _meshInstanceMap.Dispose();
    }
    public void OnUpdate(ref SystemState state)
    {
        // 清空 NativeHashMap 中的矩阵列表
        foreach (var kvp in _meshInstanceMap)
        {
            kvp.Value.Clear();
        }

 

        GameObject gb = null;
        // 遍历查询到的实体并绘制网格  
        foreach (var entity in _XrQuery.ToEntityArray(Allocator.Temp))
        {
            // 获取 msxExp 组件  
            var dt = SystemAPI.ManagedAPI.GetComponent<Mr>(entity);
            gb = dt.zs;
            state.EntityManager.RemoveComponent<Mr>(entity); // 修复：使用 EntityManager.RemoveComponent 替代  
        }
        if (gb != null)
        {
            _XrQuery= SystemAPI.QueryBuilder()
            .WithAll<Mr>() // 必须拥有 msxExp 组件     
            .Build();
            for (int i = 0; i < 10000; i++)
            {
                // 创建一个新的实体  
                var entity = state.EntityManager.CreateEntity();

                // 创建并初始化 msxExp 组件  
                var msxExpComponent = new msxExp
                {
                    pos = new Vector3(i % 100*2, 0, i / 100 * 2), // 设置位置，排列成 10x10 的网格  
                    rot = Quaternion.identity, // 默认旋转  
                    sca = Vector3.one, // 默认缩放  
                    zs = gb.GetComponent<zhanshi>(),
                    time = Random.Range(0, 2f),  
                };

                // 为实体添加 msxExp 组件  
                state.EntityManager.AddComponentObject(entity, msxExpComponent);
            }

        }

 
        foreach (var entity in _msxExpQuery.ToEntityArray(Allocator.Temp))
        {
            // 获取 msxExp 组件  
            var dt = SystemAPI.ManagedAPI.GetComponent<msxExp>(entity);
            dt.time += Time.deltaTime;
            // 创建变换矩阵  
            Matrix4x4 matrix4X4 = Matrix4x4.TRS(dt.pos, dt.rot, dt.sca);
            var dis = Vector3.Distance(dt.pos, mcam.transform.position);
            int lod = 0;
            if (dis > 30)
            {
                lod= 2;
            }
            else if (lod > 5)
            {
                lod = 1;
            }
            else
            {
                lod = 0;
            }
            var m = dt.zs.getMesh("Attack", dt.time, lod);

            // 如果 NativeHashMap 中没有该 Mesh，则添加
            int mesh = m.GetInstanceID();
            cache[mesh] = m;

            if (!_meshInstanceMap.ContainsKey(mesh))
            {
                _meshInstanceMap[mesh] = new NativeList<Matrix4x4>(Allocator.Persistent);
            }

            // 将矩阵添加到对应的列表中
            _meshInstanceMap[mesh].Add(matrix4X4);
        }
        //MaterialPropertyBlock props = new MaterialPropertyBlock();

        foreach (var kvp in _meshInstanceMap)
        {
            Graphics.DrawMeshInstanced(cache[kvp.Key], 0, mat, kvp.Value.AsArray().ToArray()
                , kvp.Value.Length, null,
                UnityEngine.Rendering.ShadowCastingMode.Off);
             
        }
    }
    static Dictionary<int, Mesh> cache = new Dictionary<int, Mesh>();
    static Camera _cam;
    static Camera mcam
    {
        get
        {
            if (_cam == null)
            {
                _cam = Camera.main;
            }
            return _cam;
        }
    }
    static Material _mat;
    static Material mat
    {
        get
        {
            if (_mat == null)
            {
                _mat = Resources.Load<Material>("MAT_Warrior_Red");// new Material(Shader.Find("Standard"));
                _mat.enableInstancing = true;
            }
            return _mat;
        }
    }
}
#endif
