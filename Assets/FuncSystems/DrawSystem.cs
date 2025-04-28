using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.UniversalDelegates;
using Unity.VisualScripting;
using UnityEngine;

#if !UNITY_DISABLE_MANAGED_COMPONENTS
[BurstCompile]
public class MeshData
{
    public byte lod;
    public Mesh mesh;
}
public partial struct DrawSystem : ISystem
{
    private NativeHashMap<int, NativeList<Matrix4x4>> _red;
    private NativeHashMap<int, NativeList<Matrix4x4>> _blue;

    private EntityQuery _msxExpQuery;

    public void OnCreate(ref SystemState state)
    {
        // 初始化 NativeHashMap，用于存储 Mesh 和对应的矩阵列表
        _red = new NativeHashMap<int, NativeList<Matrix4x4>>(10, Allocator.Persistent);
        _blue = new NativeHashMap<int, NativeList<Matrix4x4>>(10, Allocator.Persistent);
        _msxExpQuery = SystemAPI.QueryBuilder().WithAll<Actor>().Build();
    }

    public void OnDestroy(ref SystemState state)
    {
        // 释放 NativeHashMap 和内部的 NativeList
        foreach (var kvp in _red)
        {
            kvp.Value.Dispose();
        }
        _red.Dispose();

        foreach (var kvp in _blue)
        {
            kvp.Value.Dispose();
        }
        _blue.Dispose();
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
        foreach (var kvp in _red)
        {
            kvp.Value.Clear();
        }
        foreach (var kvp in _blue)
        {
            kvp.Value.Clear();
        }
    }
    static int _count = 0;
    static int count
    {
        get
        {
            return _count;
        }
        set
        {
            if (_count != value)
            {
                _count = value;
                Main.DispEvent("unit_changed", count);
            }
        }
    }
    /// <summary>
    /// 遍历实体并更新网格实例数据
    /// </summary>
    [BurstCompile]
    private void UpdateMeshInstances(ref SystemState state)
    {
        var lst = _msxExpQuery.ToEntityArray(Allocator.Temp);
        count = lst.Length;
        foreach (var entity in lst)
        {
            // 获取 msxExp 组件
            var dt = state.EntityManager.GetComponentData<Actor>(entity);
            var pos = state.EntityManager.GetComponentData<TPosition>(entity);  
            // 修改 time 值
            dt.time += Time.deltaTime;

            if (dt.state == enum_state.Dead)
            {
                // 如果死亡了，那么停留在最后一帧
                if (dt.time > 0.99f)
                {
                    dt.time = 0.99f;
                }
            }

            // 将修改后的组件写回实体
            state.EntityManager.SetComponentData(entity, dt);

            // 计算 LOD 和变换矩阵
            int lod = CalculateLOD(pos.pos);
            Matrix4x4 matrix4X4 = Matrix4x4.TRS(pos.pos, dt.rot, dt.sca);

            // 获取对应的 Mesh
            var m = zs.getMesh(dt.state, dt.time, lod);
            int meshID = m.GetInstanceID();
            if (!cache.ContainsKey(meshID))
            {
                cache.Add(meshID, new MeshData());
            }
            cache[meshID].mesh = m;
            cache[meshID].lod = (byte)lod;

            if (dt.team == enum_team.red)
            {
                // 如果 NativeHashMap 中没有该 Mesh，则添加
                if (!_red.ContainsKey(meshID))
                {
                    _red[meshID] = new NativeList<Matrix4x4>(Allocator.Persistent);
                }

                // 将矩阵添加到对应的列表中
                _red[meshID].Add(matrix4X4);
            }
            else
            {
                if (!_blue.ContainsKey(meshID))
                {
                    _blue[meshID] = new NativeList<Matrix4x4>(Allocator.Persistent);
                }

                // 将矩阵添加到对应的列表中
                _blue[meshID].Add(matrix4X4);
            }
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

        foreach (var kvp in _red)
        {
            var mesh = cache[kvp.Key];
            var matrices = kvp.Value.AsArray();
            Graphics.DrawMeshInstanced(mesh.mesh, 0, matred, matrices.ToArray(), matrices.Length, null,
              mesh.lod < 2 ? UnityEngine.Rendering.ShadowCastingMode.On : UnityEngine.Rendering.ShadowCastingMode.Off);
        }
        foreach (var kvp in _blue)
        {
            var mesh = cache[kvp.Key];
            var matrices = kvp.Value.AsArray();
            Graphics.DrawMeshInstanced(mesh.mesh, 0, matblue, matrices.ToArray(), matrices.Length, null,
              mesh.lod < 2 ? UnityEngine.Rendering.ShadowCastingMode.On : UnityEngine.Rendering.ShadowCastingMode.Off);
        }
    }
    static zhanshi _zs = null;
    static zhanshi zs
    {
        get
        {
            if(_zs== null){
                _zs = Resources.Load("zhanshi").GetComponent<zhanshi>();
            }
            return _zs;
        }
    }
    
    static Dictionary<int, MeshData> cache = new Dictionary<int, MeshData>();
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
    static Material _matred;
    static Material matred
    {
        get
        {
            if (_matred == null)
            {
                _matred = Resources.Load<Material>("m2"); // new Material(Shader.Find("Standard"));
                _matred.enableInstancing = true;
            }
            return _matred;
        }
    }
    static Material _matblue;
    static Material matblue
    {
        get
        {
            if (_matblue == null)
            {
                _matblue = Resources.Load<Material>("m1"); // new Material(Shader.Find("Standard"));
                _matblue.enableInstancing = true;
            }
            return _matblue;
        }
    }

}
#endif
