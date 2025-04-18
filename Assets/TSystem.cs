using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using static Unity.Entities.EntitiesJournaling;

public partial struct TSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        // 创建一个查询，筛选出所有拥有 PrefabCWrapper 组件的实体
        var noLoadingQuery = SystemAPI.QueryBuilder()
               .WithAll<PrefabCWrapper>() // 必须拥有 PrefabCWrapper 组件   
               .Build();

        // 遍历查询结果中的所有实体
        foreach (var entity in noLoadingQuery.ToEntityArray(Allocator.Temp))
        {
            // 获取实体的 PrefabCWrapper 组件，并从中提取预制体
            var prefab = SystemAPI.ManagedAPI.GetComponent<PrefabCWrapper>(entity).xprefab;

            // 实例化预制体，创建一个新的游戏对象
            var instance = GameObject.Instantiate(prefab);

            // 设置实例的隐藏标志，防止其在场景中显示和保存
            instance.hideFlags = HideFlags.HideAndDontSave;

            // 将实例的 Transform 组件添加到 ECS 实体中，作为托管组件
            state.EntityManager.AddComponentObject(entity, instance.GetComponent<Transform>());

            // 为实体添加一个 InstData 组件，存储实例化的游戏对象
            state.EntityManager.AddComponentData(entity, new InstData { Instance = instance });

            // 移除实体的 PrefabCWrapper 组件，表示该实体已完成加载
            state.EntityManager.RemoveComponent<PrefabCWrapper>(entity);
        }
    }
}
