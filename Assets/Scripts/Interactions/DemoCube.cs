using UnityEngine;

public class DemoCube : MonoBehaviour
{
    public void DoSomething()
    {
        GetComponent<MeshRenderer>().material.color = Color.black;
    }

}
