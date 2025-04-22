using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Burst;
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
        ClearMeshInstanceMap();

        // 遍历查询到的实体并更新矩阵
        UpdateMeshInstances(ref state);

        // 渲染所有实例化的网格
        RenderMeshInstances();
    }

    /// <summary>
    /// 清空 NativeHashMap 中的矩阵列表
    /// </summary>
    [BurstCompile(CompileSynchronously = true)]
    private void ClearMeshInstanceMap()
    {
        foreach (var kvp in _meshInstanceMap)
        {
            kvp.Value.Clear();
        }
    }

    /// <summary>
    /// 遍历实体并更新网格实例数据
    /// </summary>
    [BurstCompile]
    private void UpdateMeshInstances(ref SystemState state)
    {
        foreach (var entity in _msxExpQuery.ToEntityArray(Allocator.Temp))
        {
            // 获取 msxExp 组件
            var dt = SystemAPI.ManagedAPI.GetComponent<msxExp>(entity);
            dt.time += Time.deltaTime;

            // 计算 LOD 和变换矩阵
            int lod = CalculateLOD(dt.pos);
            Matrix4x4 matrix4X4 = Matrix4x4.TRS(dt.pos, dt.rot, dt.sca);

            // 获取对应的 Mesh
            var m = dt.zs.getMesh("Attack", dt.time, lod);
            int meshID = m.GetInstanceID();
            cache[meshID] = m;

            // 如果 NativeHashMap 中没有该 Mesh，则添加
            if (!_meshInstanceMap.ContainsKey(meshID))
            {
                _meshInstanceMap[meshID] = new NativeList<Matrix4x4>(Allocator.Persistent);
            }

            // 将矩阵添加到对应的列表中
            _meshInstanceMap[meshID].Add(matrix4X4);
        }
    }

    /// <summary>
    /// 根据距离计算 LOD
    /// </summary>
    [BurstCompile(CompileSynchronously = true)]
    private int CalculateLOD(Vector3 position)
    {
        float distance = Vector3.Distance(position, mcam.transform.position);
        if (distance > 30)
        {
            return 2;
        }
        else if (distance > 5)
        {
            return 1;
        }
        else
        {
            return 0;
        }
    }

    /// <summary>
    /// 渲染所有实例化的网格
    /// </summary>
    private void RenderMeshInstances()
    {
        foreach (var kvp in _meshInstanceMap)
        {
            var mesh = cache[kvp.Key];
            var matrices = kvp.Value.AsArray();
           // Graphics.DrawMeshInstancedIndirect(mesh, 0, mat, matrices ,null,);
            // 使用 Graphics.DrawMeshInstanced 渲染
            Graphics.DrawMeshInstanced(mesh, 0, mat, matrices.ToArray(), matrices.Length, null,
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
                _mat = Resources.Load<Material>("MAT_Warrior_Red"); // new Material(Shader.Find("Standard"));
                _mat.enableInstancing = true;
            }
            return _mat;
        }
    }
}
#endif
