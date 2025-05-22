using UnityEngine;



public class Ending : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Agent player = other.GetComponent<Agent>();
            if (player != null) player.isGameWon.Value = true;
        }
    }
}