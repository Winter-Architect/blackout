using UnityEngine;

public class Flashlight : MonoBehaviour , IActionItem
{
    

    public void PrimaryAction(Agent agent)
    {
        Debug.Log("Tried to shine");
    }
}
