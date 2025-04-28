using Unity.Entities;
using UnityEngine;

public class Bootstrap : MonoBehaviour
{
    private World defaultWorld;

    void Start()
    {
        //defaultWorld = World.DefaultGameObjectInjectionWorld;
        //defaultWorld.GetOrCreateSystem<CreateSystem>();
    }

    void Update()
    {
        defaultWorld.Update();
    }

    void OnDestroy()
    {
        defaultWorld.Dispose();
    }
}