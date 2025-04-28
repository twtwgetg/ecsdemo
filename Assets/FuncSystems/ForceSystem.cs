using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using static BombSystem;
#if !UNITY_DISABLE_MANAGED_COMPONENTS
public partial struct ForceSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        var _msxExpQuery = SystemAPI.QueryBuilder().WithAll<TPhysic, TPosition>().Build();

        using (var entities = _msxExpQuery.ToEntityArray(Allocator.Temp))
        {
            foreach (var entity in entities)
            {
                var dt = state.EntityManager.GetComponentData<TPhysic>(entity);
                var tr = state.EntityManager.GetComponentData<TPosition>(entity);
                if (tr.pos.y <0)
                {
                    tr.pos.y = 0;

                    //移除组件
                    state.EntityManager.RemoveComponent<TPhysic>(entity); 
                }
                else
                {
                     
                    dt.force.y -= 9.8f * Time.deltaTime;
                    state.EntityManager.SetComponentData(entity, dt);
                    tr.pos+= dt.force * Time.deltaTime;
                }
                state.EntityManager.SetComponentData(entity, tr);
            }
        }
    }
}
#endif