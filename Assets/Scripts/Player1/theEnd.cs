using UnityEngine;

public class temp : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            var agent = other.GetComponent<Agent>();
            if (agent != null)
            {
                agent.isGameWon.Value = true;
            }
        }
    }
}
