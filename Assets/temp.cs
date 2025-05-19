using UnityEngine;

public class temp : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player entered the trigger!");
            // Mettre la vie du player à 0
            var agent = other.GetComponent<Agent>();
            if (agent != null)
            {
                agent.Health = 0;
            Debug.Log("Player entered the trigger!!!!!!!!!!!!!");
            }
        }
    }
}
