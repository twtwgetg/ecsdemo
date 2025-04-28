using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.UniversalDelegates;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;
using static BombSystem;
using static UnityEngine.EventSystems.EventTrigger;

#if !UNITY_DISABLE_MANAGED_COMPONENTS  
public partial struct StateSystem : ISystem
{
    private EntityQuery _msxExpQuery;
    private float speed; // Removed readonly to allow assignment  
    private NativeHashMap<int3, Entity> _occupiedPositions;
    public void OnCreate(ref SystemState state)
    {
        // Initialize the field in the OnCreate method  
        speed = 3f;
        // 初始化占位管理
        _occupiedPositions = new NativeHashMap<int3, Entity>(10000, Allocator.Persistent);
        // 查询所有拥有 TState 和 msxExp 的实体
        _msxExpQuery = SystemAPI.QueryBuilder()
            .WithAll<TState, Actor>()
            .Build();
    }
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        // Replace the invalid ToComponentArray call with a valid approach  
        using (var entities = _msxExpQuery.ToEntityArray(Allocator.Temp))
        {
            foreach (var entity in entities)
            {
                // Retrieve the required components for the ProcAi method  
                var dt = state.EntityManager.GetComponentData<TState>(entity);
                var src = state.EntityManager.GetComponentData<Actor>(entity);
                var srcpos = state.EntityManager.GetComponentData<TPosition>(entity); 
                ProcAi(entity, ref state, ref dt, ref src,ref srcpos);
            }
        }
    }
    [BurstCompile]
    void procDie(ref SystemState state, Entity entity, ref Actor src)
    {
        if (src.state != enum_state.Dead)
        {
            src.state = enum_state.Dead;
            src.time = 0;
            // 释放占位
            var srcpos = state.EntityManager.GetComponentData<TPosition>(entity);
            _occupiedPositions.Remove(floorToInt(srcpos.pos));
        }
        else
        {
            //已经开始播放死亡动作
            if (src.deadTime > 1)
            {
                //把自己敌人清空
                var en = state.EntityManager.GetComponentData<TState>(entity);
                if(state.EntityManager.Exists(en.enemy))
                {
                    //如果对手还活着
                    var tag = state.EntityManager.GetComponentData<Actor>(en.enemy);
                    tag.enemycount--;
                    state.EntityManager.SetComponentData<Actor>(en.enemy, tag);
                }

                //播放死亡动作结束，删除Monster  
                state.EntityManager.DestroyEntity(entity);
            }
            else
            {
                src.deadTime += Time.deltaTime;
            }
        }
    }
    [BurstCompile]
    void GetNewTarget(ref SystemState state,Entity entity, ref TState dt, ref Actor src,ref TPosition srcpos)
    {
        //如果没有目标位置，那么找一个敌人，并将它的位置设置为目标位置  
        dt.enemy = getTarget(entity, ref state);
        if (dt.enemy != Entity.Null)
        {
            dt.targpos = state.EntityManager.GetComponentData<TPosition>(dt.enemy).pos + getRandomVec();
            var act_enemy = state.EntityManager.GetComponentData<Actor>(dt.enemy);
            act_enemy.enemycount += 1;
            state.EntityManager.SetComponentData<Actor>(dt.enemy, act_enemy);
        }
        else
        {
            src.state = enum_state.Idle;
            dt.ClearTaget();
        }
    }
    const float cd_attack = 0.8f;
    [BurstCompile]
    void ProcAttacking(Entity entity, ref SystemState state, ref TState dt, ref Actor src,ref TPosition srcpos, Actor enemy)
    {
        // 占用目标位置
         _occupiedPositions.TryAdd(floorToInt(srcpos.pos), entity);
        src.state = enum_state.Attack;
        src.skilltime += Time.deltaTime;
        if (src.skilltime > cd_attack)
        {
            src.skilltime = 0;
            //目标被攻击后，将对方设置成敌人
            var enemystate = state.EntityManager.GetComponentData<TState>(dt.enemy);
            enemystate.setEnemy(entity, ref state);
            state.EntityManager.SetComponentData<TState>(dt.enemy, enemystate);
            //减生命
            enemy.life -= 1;
            state.EntityManager.SetComponentData<Actor>(dt.enemy, enemy);
            if (enemy.life <= 0)
            {
                //敌人死亡，清除目标
                src.state = enum_state.Idle;
                dt.ClearTaget();
                _occupiedPositions.Remove(floorToInt(srcpos.pos));
            }
            else
            {
                //敌人没死亡，那么重新校验一下位置
            }
        }
        else
        {
            //不够成伤害
        }
    }
    [BurstCompile]
    private int3 floorToInt(Vector3 position)
    {
        return new int3(
            Mathf.FloorToInt(position.x),
            Mathf.FloorToInt(position.y),
            Mathf.FloorToInt(position.z)
        );
    }
    [BurstCompile]
    void arrivedTaget(ref SystemState state, Entity entity, ref TState dt, ref Actor src, ref TPosition srcpos)
    {
        if (state.EntityManager.Exists(dt.enemy))
        {
            //如果敌人存在，那么看是否已经死亡
            var enemy = state.EntityManager.GetComponentData<Actor>(dt.enemy);
            TPosition tagpos = state.EntityManager.GetComponentData<TPosition>(dt.enemy);
            if (enemy.life <= 0)
            {
                //敌人已经死亡 
                dt.ClearTaget();
                src.state = enum_state.Idle;
            }
            else
            {
                //如果已经到达目标位置,开始攻击
                if (Vector3.Distance(srcpos.pos, tagpos.pos) < 2 )
                { 
                    ProcAttacking(entity, ref state, ref dt, ref src, ref srcpos,enemy);
                }
                else
                {
                    dt.targpos = state.EntityManager.GetComponentData<TPosition>(dt.enemy).pos;
                }
            }
        }
        else
        {
            src.state = enum_state.Idle;
            dt.targpos = Vector3.positiveInfinity;
        }
    }
    [BurstCompile]
    void procLife(ref SystemState state, Entity entity, ref TState dt, ref Actor src,ref TPosition srcpos)
    {
        //先看有没有目标位置
        if(float.IsPositiveInfinity( dt.targpos.x))
        {
            GetNewTarget(ref state,entity,ref dt, ref src,ref srcpos);
        }
        else
        {
            //如果有目标位置，那么看是否已经到达目标位置
            if (Vector3.Distance(srcpos.pos, dt.targpos) < 2)
            { 
                arrivedTaget(ref state, entity, ref dt, ref src, ref srcpos);
            }
            else
            {
                procNotArrived(ref state, entity, ref src,ref dt,ref srcpos);
            }
        } 
    }
    [BurstCompile]
    void procNotArrived(ref SystemState state, Entity entity, ref Actor src, ref TState dt, ref TPosition srcpos)
    {
        dt.moveTime -= Time.deltaTime;
        if (dt.moveTime < 0)
        {
            dt.moveTime = UnityEngine.Random.Range(4, 5);
            //过个3秒钟，更新一下目标位置
            if (state.EntityManager.Exists(dt.enemy))
            {
                //判断对手还活着，重新校验一下位置
                var tagpos = state.EntityManager.GetComponentData<TPosition>(dt.enemy);
                dt.targpos = tagpos.pos + getRandomVec();

                // 检查目标位置是否被占用
                int3 targetPos = floorToInt(dt.targpos);
                int step = 2;
                while(_occupiedPositions.ContainsKey(targetPos))
                {
                    // 如果目标位置被占用，重新选择目标位置
                    dt.targpos = srcpos.pos + getRandomVec()*step;
                    step += 1;
                    targetPos = floorToInt(dt.targpos);
                }

            }
            else
            {
                //如果对手已经死亡，先冲到目的地再说,到地方再重新查找新的敌人
            }
        } 
        src.state = enum_state.Run;
        var diff = dt.targpos - srcpos.pos;
        diff.y = 0;
        src.rot = Quaternion.LookRotation(diff);
        srcpos.pos = Vector3.MoveTowards(srcpos.pos, dt.targpos, Time.deltaTime * speed);
    }

    [BurstCompile]
    private Vector3 getRandomVec()
    {
        return new Vector3(getRandomPos(), 0, getRandomPos());
    }

    [BurstCompile]
    float getRandomPos()
    {
        return UnityEngine.Random.Range(-0.5f, 0.6f);
    }
    [BurstCompile]
    void ProcAi(Entity entity, ref SystemState state,ref TState dt, ref Actor src, ref TPosition srcpos)
    { 
        if (src.life <= 0)
        {
            procDie(ref state, entity, ref src);
        }
        else
        {
            procLife(ref state, entity,ref dt, ref src,ref srcpos); 
        }

        if (state.EntityManager.Exists(entity))
        {
            state.EntityManager.SetComponentData<Actor>(entity, src);
            state.EntityManager.SetComponentData<TState>(entity, dt);
            state.EntityManager.SetComponentData<TPosition>(entity, srcpos);
        }
    }


    /**  
     * 获取目标  
     * 1阵营不同  
     * 2距离最近  
     * 3距离最短  
     */
    [BurstCompile]
    private Entity getTarget(Entity entity, ref SystemState state)
    {
        Entity target = Entity.Null; 
        var self = state.EntityManager.GetComponentData<TPosition>(entity);
        var src = state.EntityManager.GetComponentData<Actor>(entity);
        using (var entities = _msxExpQuery.ToEntityArray(Allocator.Temp))
        {
            float maxdis = 1000000;
            for (int i = 0; i < entities.Length; i++)
            {
                var tar = state.EntityManager.GetComponentData<Actor>(entities[i]);
                if (src.team == tar.team)
                {
                    continue;
                }
                if (tar.enemycount > 2)
                {
                    continue;
                }
                var tagpos = state.EntityManager.GetComponentData<TPosition>(entities[i]);
                float dis = Vector3.Distance(self.pos, tagpos.pos);
                if (dis < maxdis)
                {
                    maxdis = dis;
                    target = entities[i];
                }
            }
        }
        return target;
    }
     
}
#endif
