using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UnitShow : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Main.RegistEvent("unit_changed", (a) =>
        {
            GetComponent<TextMeshProUGUI>().text ="unit:"+ a.ToString();
            return 1;
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
