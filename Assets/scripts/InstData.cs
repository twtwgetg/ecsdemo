using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class InstData : IComponentData, IDisposable
{
    public GameObject Instance;

    public void Dispose()
    {
        UnityEngine.Object.DestroyImmediate(Instance);
    }
}