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
        _XrQuery = SystemAPI.QueryBuilder()
        .WithAll<Mr>() // 必须拥有 msxExp 组件     
        .Build();


    }

    public void OnUpdate(ref SystemState state)
    {
        GameObject gb = null;
        // 遍历查询到的实体并绘制网格  
        foreach (var entity in _XrQuery.ToEntityArray(Allocator.Temp))
        {
            // 获取 msxExp 组件  
            var dt = SystemAPI.ManagedAPI.GetComponent<Mr>(entity);
            gb = dt.zs;
            state.EntityManager.RemoveComponent<Mr>(entity); 
            //如何移除entity?
            state.EntityManager.DestroyEntity(entity);
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
    }
     
}
#endif
