using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

// 定义一个系统，用于创建一万个实体  
public class ECSInstanceRenderingDemo : MonoBehaviour
{
    public Mesh mesh;
    public Material material;

    private void Start()
    {
        // 获取默认世界的实体管理器  
        var world = World.DefaultGameObjectInjectionWorld;
        var entityManager = world.EntityManager;

        // 创建一个实体原型，包含所需的组件  
        var entityArchetype = entityManager.CreateArchetype(
            typeof(LocalTransform),
            typeof(RenderMesh), // 替换为 RenderMesh  
            typeof(LocalToWorld)
        );

        // 创建一万个实体  
        const int instanceCount = 10000;
        for (int i = 0; i < instanceCount; i++)
        {
            // 创建实体  
            var entity = entityManager.CreateEntity(entityArchetype);

            // 设置实体的位置  
            float x = (i % 100) * 1.5f;
            float z = (i / 100) * 1.5f;
            entityManager.SetComponentData(entity, LocalTransform.FromPosition(new float3(x, 0, z)));

            // 设置实体的渲染网格和材质  
            entityManager.SetSharedComponentManaged(entity, new RenderMesh
            {
                mesh = mesh,
                material = material
            });
        }
    }
}
