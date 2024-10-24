using System.Collections;
using System.Collections.Generic;
using UnityEditor.Timeline;
using UnityEngine;

public class Chainsystem : MonoBehaviour
{
     private int combonumber;

    private float time;

    [SerializeField] private float sec;

    public void Combo()
    {
        combonumber++;

        time = 0;
    }
    private void ComboReset()
    {
        time = 0;

        combonumber = 0;

        Debug.Log("Combo rest");

   
    }

    private void Update()
    {
        time += Time.deltaTime;
        if (time >= sec)
        {
            ComboReset();
        }

        if (Input.GetKeyDown(KeyCode.Z))
        {
            Combo();
        }  
    }
}
