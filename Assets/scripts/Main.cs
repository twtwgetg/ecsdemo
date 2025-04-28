using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour
{
    public delegate object registfun(object parm);
    // Start is called before the first frame update
    public static Main inst;
    private void Awake()
    {
        inst = this;
    }
    void Start()
    {
        DispEvent("gamebegin");
    }
    static Dictionary<string,List< registfun>> evs = new();
    public static object DispEvent(string ev,object parm=null){

        if(evs.ContainsKey(ev)){
            for(int i = 0; i < evs[ev].Count; i++)
            {
                var p = evs[ev][i](parm);
                if (p != null)
                {
                    return p;
                }
            }
            Debug.LogError("没有处理消息" + ev);
            return null;
        }
        else{
            return null;
        }
    }
    public static void RegistEvent(string ev,registfun fun){
        if(evs.ContainsKey(ev)){
            //Debug.LogError("已经注册消息" + ev);
        }
        else
        {
            evs[ev] = new List<registfun>();
        }
        evs[ev].Add(fun);
    }
    public static void RemoveEvent(string ev,registfun fun){
        if(evs.ContainsKey(ev)){
            evs[ev].Remove(fun);
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
