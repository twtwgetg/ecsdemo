using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
#if !UNITY_DISABLE_MANAGED_COMPONENTS
public class MeshBase : MonoBehaviour
{
    public GameObject prefab;

    class Baker : Baker<MeshBase>
    {
        public override void Bake(MeshBase authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponentObject(entity, new PrefabCWrapper
            {
                xprefab = authoring.prefab
                ,pos = authoring.transform.position

            });
        }
    }
}

 

// Wrapper class to resolve CS0452 error  
public class PrefabCWrapper:IComponentData
{
    public GameObject xprefab;
    public Vector3 pos;
}
#endif