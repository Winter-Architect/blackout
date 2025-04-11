using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class switchesManager : MonoBehaviour
{
    public List<Switches> Switches;
    public Door DoorCondition;
    
    // Update is called once per frame
    void Update()
    {
        if (Switches[0].Active == true)
        {
            DoorCondition.Condition = true;
        }
        else
        {
            DoorCondition.Condition = false;
        }
    }
}
