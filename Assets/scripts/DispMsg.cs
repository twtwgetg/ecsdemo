using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DispMsg : MonoBehaviour
{
    public string str_event = "event_";
    public object obj;
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Button>().onClick.AddListener(() =>
        {
            Main.DispEvent(str_event,obj);
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
