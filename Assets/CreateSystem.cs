using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

#if !UNITY_DISABLE_MANAGED_COMPONENTS
public partial struct createsystem : ISystem
{
    private EntityQuery _XrQuery;

    public void OnCreate(ref SystemState state)
    {
        // 缓存查询，避免每帧重新创建
        _XrQuery = SystemAPI.QueryBuilder()
            .WithAll<Mr>() // 必须拥有 Mr 组件
            .Build();
    }

    public void OnUpdate(ref SystemState state)
    {
        // 获取 GameObject 引用
        GameObject gb = ProcessAndDestroyEntities(ref state);

        // 如果存在有效的 GameObject，则创建新的实体
        if (gb != null)
        {
            CreateEntities(ref state, gb);
        }
    }

    /// <summary>
    /// 处理并销毁所有拥有 Mr 组件的实体
    /// </summary>
    /// <param name="state">系统状态</param>
    /// <returns>返回最后一个实体的 GameObject 引用</returns>
    private GameObject ProcessAndDestroyEntities(ref SystemState state)
    {
        GameObject gb = null;

        // 获取所有拥有 Mr 组件的实体
        using (var entities = _XrQuery.ToEntityArray(Allocator.Temp))
        {
            foreach (var entity in entities)
            {
                // 获取 Mr 组件
                var dt = SystemAPI.ManagedAPI.GetComponent<Mr>(entity);
                gb = dt.zs;

                // 销毁实体
                state.EntityManager.DestroyEntity(entity);
            }
        }

        return gb;
    }

    /// <summary>
    /// 创建 10000 个实体并初始化其组件
    /// </summary>
    /// <param name="state">系统状态</param>
    /// <param name="gb">用于初始化的 GameObject</param>
    private void CreateEntities(ref SystemState state, GameObject gb)
    {
        // 获取 GameObject 的 zhanshi 组件
        var zsComponent = gb.GetComponent<zhanshi>();

        // 创建 10000 个实体
        for (int i = 0; i < 10000; i++)
        {
            // 创建一个新的实体
            var entity = state.EntityManager.CreateEntity();

            // 初始化 msxExp 组件
            var msxExpComponent = new msxExp
            {
                pos = new Vector3(i % 100 * 2, 0, i / 100 * 2), // 设置位置，排列成 10x10 的网格
                rot = Quaternion.identity, // 默认旋转
                sca = Vector3.one, // 默认缩放
                zs = zsComponent,
                time = Random.Range(0, 2f), // 随机时间
            };

            // 为实体添加 msxExp 组件
            state.EntityManager.AddComponentObject(entity, msxExpComponent);
        }
    }
}
#endif
