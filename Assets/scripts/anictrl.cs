using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum enum_team
{
    red,
    blue,
}
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

    public void run()
    {
        ani.SetBool("isMoving", true);
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
