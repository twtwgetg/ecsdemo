using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

#if !UNITY_DISABLE_MANAGED_COMPONENTS  
public partial struct DrawSystem : ISystem
{
    private NativeHashMap<int, NativeList<Matrix4x4>> _meshInstanceMap;
    private EntityQuery _msxExpQuery;
    public void OnCreate(ref SystemState state)
    {
        // 初始化 NativeHashMap，用于存储 Mesh 和对应的矩阵列表
        _meshInstanceMap = new NativeHashMap<int, NativeList<Matrix4x4>>(10, Allocator.Persistent);
        _msxExpQuery = SystemAPI.QueryBuilder().WithAll<msxExp>().Build();
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
                lod = 2;
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
