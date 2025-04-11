using UnityEngine;

public class Switches : MonoBehaviour
{
    
    public bool Active = false;
    

    public void DoSomething()
    {
        if (Active)
        {
            GetComponent<MeshRenderer>().material.color = Color.black;
            Active = false;
        }
        else
        {
            GetComponent<MeshRenderer>().material.color = Color.white;
            Active = true;
        }
    }
}
