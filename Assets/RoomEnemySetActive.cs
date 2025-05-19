using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

public class RoomEnemySetActive : MonoBehaviour
{
    private NavMeshSurface surface;
    private bool initialized = false;

    void Awake()
    {
        surface = GetComponent<NavMeshSurface>();
    }

    public void ActivateEnemies()
    {
        if (initialized) return;
        initialized = true;
        Debug.LogWarning("Trying to activate");
        foreach (Transform child in transform)
        {
            if (child.name.StartsWith("Enemy"))
            {
                child.gameObject.SetActive(true);

                NavMeshAgent agent = child.GetComponent<NavMeshAgent>();
                if (agent != null)
                {
                    agent.enabled = false;
                    agent.enabled = true;
                }
            }
        }
    }
}