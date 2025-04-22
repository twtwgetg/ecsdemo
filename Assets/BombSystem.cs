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
        _msxExpQuery = SystemAPI.QueryBuilder().WithAll<msxExp>().Build();
        using (var entities = _msxExpQuery.ToEntityArray(Allocator.Temp))
        {
            foreach (var entity in entities)
            {
                // 获取 msxExp 组件
                var dt = SystemAPI.ManagedAPI.GetComponent<msxExp>(entity);

                // 计算实体与爆炸点的距离
                var dis = Vector3.Distance(dt.pos, pos);

                if (dis < 5) // 5米范围内的实体会被炸飞
                {
                    // 计算爆炸力（距离越近，力越大）
                    //float force = Mathf.Lerp(10f, 0f, dis / 5f); // 最大力为10，距离5米时力为0

                    // 计算爆炸方向
                    Vector3 direction = (dt.pos - pos);
                    direction.y = direction.magnitude;

                    //// 应用爆炸力
                    //dt.pos += direction * force * Time.deltaTime;

                    //// 添加向上的力，模拟炸飞效果
                    //dt.pos += Vector3.up * force * 0.5f * Time.deltaTime;


                    state.EntityManager.AddComponentObject(entity, new TPhysic()
                    {
                        force = direction,  
                    });
                }
            }
        }
    }
    public class TPhysic : IComponentData
    {
        public Vector3 force;  
    }
    public void OnUpdate(ref SystemState state)
    {
        // 检测鼠标左键点击
        if (Input.GetMouseButtonDown(0))
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
#endif
