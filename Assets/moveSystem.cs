using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

#if !UNITY_DISABLE_MANAGED_COMPONENTS  
public partial struct moveSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        var noLoadingQuery = SystemAPI.QueryBuilder()
               .WithAll<Transform>() // 必须拥有 PrefabCWrapper 组件   
               .Build();


        foreach (var entity in noLoadingQuery.ToEntityArray(Allocator.Temp))
        {
             var transform = SystemAPI.ManagedAPI.GetComponent<Transform>(entity);

             transform.localPosition+=new Vector3(0,0,0.01f);
        }
    }
}
#endif