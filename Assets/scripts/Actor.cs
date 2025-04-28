using System;
using Unity.Entities;
using UnityEngine;
public enum enum_state
{
    None = 0,
    Attack = 1,
    Dead = 2,
    Idle = 3,
    Run = 4,
}
public struct Actor : IComponentData
{

    public Quaternion rot;
    public Vector3 sca;
    public float life;
    public float time;
    public float skilltime;
    public float deadTime;
    public enum_team team;
    public enum_state state;
    public int enemycount;//敌人数量，被敌人标记为目标，那么数量加1
}
public struct TPosition:IComponentData
{
    public Vector3 pos;
}


/**
 * 状态机
 * */
public struct TState : IComponentData
{
    public Entity enemy;
    internal float moveTime;

    private Vector3 _targpos;
    public Vector3 targpos
    {
        get
        {
            return _targpos;
        }
        set
        {
            _targpos = value;
        }
    } 
    
    internal void ClearTaget()
    {
        
        enemy = Entity.Null;
        targpos = Vector3.positiveInfinity;
    }

    internal void setEnemy(Entity _enemy, ref SystemState state)
    {
        enemy = _enemy;
        TPosition tagpos = state.EntityManager.GetComponentData<TPosition>(_enemy);
        targpos = tagpos.pos;
        moveTime = 3f;
    }
}
