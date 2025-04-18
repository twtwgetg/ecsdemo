using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testcreateandhide : MonoBehaviour
{
    private void Awake()
    {
        var tx = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        tx.hideFlags = HideFlags.HideAndDontSave;
    }
}
