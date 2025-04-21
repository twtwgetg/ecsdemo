using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class msx : MonoBehaviour
{
    public GameObject zs; 
    class Baker:Baker<msx>
    {
        public override void Bake(msx authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponentObject(entity, new Mr
            {
                zs = authoring.zs
            });
        }
    }
}

public class Mr : IComponentData
{
    public GameObject zs;
}
public class msxExp:IComponentData
{
    public Mesh mesh;
    public zhanshi zs;
    public Material mat;
    public Vector3 pos, sca;
    public Quaternion rot;
}