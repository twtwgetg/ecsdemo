using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class anictrl : MonoBehaviour
{
    Animator ani
    {
        get
        {
            return GetComponent<Animator>();
        }
    }
    public void attack()
    {
        ani.SetTrigger("Attack");
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
