using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.UniversalDelegates;
using UnityEngine;
using static StateSystem; 

#if !UNITY_DISABLE_MANAGED_COMPONENTS
public partial struct createsystem : ISystem
{
    private EntityQuery _XrQuery;

    // 将字段设置为 readonly
    private static GameObject temp;

    public void OnCreate(ref SystemState state)
    {
        // 缓存查询，避免每帧重新创建
        _XrQuery = SystemAPI.QueryBuilder()
            .WithAll<Mr>() // 必须拥有 Mr 组件
            .Build();

         Main.RegistEvent("event_create", begin);
    }
    object begin(object p)
    {
        temp = xpg;
        return null;
    }
    static GameObject xpg;

    public void OnUpdate(ref SystemState state)
    {
        // 获取 GameObject 引用
        GameObject gb = ProcessAndDestroyEntities(ref state);

        // 如果存在有效的 GameObject，则创建新的实体
        if (gb != null)
        {
             xpg = gb; 
        }

        if (temp != null)
        {
            CreateEntities(ref state, temp);
            temp= null;
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
    Actor createSolider(enum_team _team)
    {
        var x = new Actor
        {
            rot = Quaternion.identity, // 默认旋转
            sca = Vector3.one, // 默认缩放
            team = _team,
            life = 3,
            skilltime = 0,
            deadTime = 0, 
            state = enum_state.Idle,
            time = Random.Range(0, 2f), // 随机时间
        };
        return x;
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

        Vector3 pos = new Vector3(-50, 0, 0);
        for (int i = 0; i < 2000; i++)
        {
            // 创建一个新的实体
            var entity = state.EntityManager.CreateEntity();
            // 添加随机偏移
            Vector3 randomOffset = new Vector3(Random.Range(-0.5f, 0.5f), 0, Random.Range(-0.5f, 0.5f));
            Vector3 formatpos = pos + new Vector3(i % 30 * 2, 0, i / 30 * 2) + randomOffset;
            // 初始化 msxExp 组件
            var msxExpComponent = createSolider(enum_team.blue);

            // 添加 MovementComponent 到实体
            state.EntityManager.AddComponent<Actor>(entity);
            state.EntityManager.SetComponentData(entity, msxExpComponent);

            state.EntityManager.AddComponent<TPosition>(entity);
            state.EntityManager.SetComponentData(entity, new TPosition { pos = formatpos });

            var tstate = new TState { enemy = Entity.Null, moveTime = 3, targpos = Vector3.positiveInfinity };
            state.EntityManager.AddComponent<TState>(entity);
            state.EntityManager.SetComponentData(entity, tstate);
        }

        pos = new Vector3(50, 0, 0);
        for (int i = 0; i < 2000; i++)
        {
            // 创建一个新的实体
            var entity = state.EntityManager.CreateEntity();

            // 添加随机偏移
            Vector3 randomOffset = new Vector3(Random.Range(-0.5f, 0.5f), 0, Random.Range(-0.5f, 0.5f));

            Vector3 formatpos = pos + new Vector3(i % 30 * 2, 0, i / 30 * 2) + randomOffset;
            // 初始化 msxExp 组件
            var msxExpComponent = createSolider(enum_team.red);
            // 添加 MovementComponent 到实体
            state.EntityManager.AddComponent<Actor>(entity);
            state.EntityManager.SetComponentData(entity, msxExpComponent);

            state.EntityManager.AddComponent<TPosition>(entity);
            state.EntityManager.SetComponentData(entity, new TPosition { pos = formatpos });

            var tstate = new TState { enemy = Entity.Null, moveTime = 3, targpos = Vector3.positiveInfinity };
            state.EntityManager.AddComponent<TState>(entity);
            state.EntityManager.SetComponentData(entity, tstate);
        }
    }
}
#endif
