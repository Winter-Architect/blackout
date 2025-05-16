using UnityEngine;

public class PressurePlate : MonoBehaviour
{
    public GameObject wall;

    private void OnTriggerEnter()
    {
        if (wall != null)
        {
            Destroy(wall);
        }
    }
}
