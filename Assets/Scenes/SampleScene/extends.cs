using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class extends
{
    // Start is called before the first frame update
    public static void Clear(this Transform tx)
    {
        while (tx.childCount > 0)
        {
            var x = tx.GetChild(0);
            x.transform.parent = null;
            if (Application.isPlaying)
            {
                GameObject.Destroy(x.gameObject);
            }
            else
            {
                GameObject.DestroyImmediate(x.gameObject);
            }
        }
    }
}
