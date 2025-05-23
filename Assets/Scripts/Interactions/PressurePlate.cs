using UnityEngine;

public class PressurePlate : MonoBehaviour
{
    public GameObject wall; 

    //deletes the "wall" whenever the player touches the pressure plate. This script took many hours to make due to its complexity, as you can probably tell.
    private void OnTriggerEnter(Collider other) 
    {
       if (other.CompareTag("Player"))
       {
           if (wall != null)
            {
                Destroy(wall);
            }
       }
    }
}
