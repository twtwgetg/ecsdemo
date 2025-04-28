using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

#if !UNITY_DISABLE_MANAGED_COMPONENTS  
public partial struct BombSystem : ISystem
{
    private EntityQuery _msxExpQuery;

    public void OnCreate(ref SystemState state)
    {
        // 查询所有拥有 msxExp 组件的实体  
    }

    /// <summary>  
    /// 模拟爆炸效果  
    /// </summary>  
    /// <param name="pos">爆炸中心点</param>  
    private void Bomb(Vector3 pos, ref SystemState state)
    {
        // 获取所有拥有 msxExp 组件的实体  
        _msxExpQuery = SystemAPI.QueryBuilder().WithAll<TPosition>().Build();
        using (var entities = _msxExpQuery.ToEntityArray(Allocator.Temp))
        {
            foreach (var entity in entities)
            {
                // 获取 msxExp 组件  
                var dt = state.EntityManager.GetComponentData<TPosition>(entity);

                // 计算实体与爆炸点的距离  
                var dis = Vector3.Distance(dt.pos, pos);

                if (dis < 5) // 5米范围内的实体会被炸飞  
                {
                    // 计算爆炸方向  
                    Vector3 direction = (dt.pos - pos);
                    direction.y = direction.magnitude;

                    state.EntityManager.AddComponentData(entity, new TPhysic()
                    {
                        force = direction,
                    });
                }
            }
        }
    }
    public struct TPhysic : IComponentData
    {
        public Vector3 force;
    }
    static Vector3 pickpos;
    public void OnUpdate(ref SystemState state)
    {
        // 检测鼠标左键点击  
#if UNITY_EDITOR
        bool ctrl = Input.GetKey(KeyCode.LeftControl);
#else
        bool ctrl=true;  
#endif
        if (ctrl && Input.GetMouseButtonDown(0))
        {
            pickpos = Input.mousePosition;
        }
        if(ctrl && Input.GetMouseButtonUp(0))
        {
            float dis = Vector3.Distance(Input.mousePosition, pickpos);
            if (dis < 4)
            {
                // 从摄像机发射一条射线  
                var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                // 检测射线是否击中地面  
                if (Physics.Raycast(ray, out var hit, 1000, 1 << LayerMask.NameToLayer("ground")))
                {
                    // 在击中点触发爆炸  
                    Bomb(hit.point, ref state);
                }
            }
        }   
      
    }
}
#endif
