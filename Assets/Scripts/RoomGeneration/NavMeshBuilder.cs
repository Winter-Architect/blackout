using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

public class NavMeshBuilder : MonoBehaviour
{
    private NavMeshSurface surface;

    void Awake()
    {
        surface = GetComponent<NavMeshSurface>();
    }

    public void BuildNavMesh()
    {
        if (surface != null)
        {
            Debug.Log($"Building NavMesh for room: {gameObject.name}");
            
            // Allow building on vertical surfaces
            surface.layerMask = LayerMask.GetMask("Walkable", "Climbable");

            surface.BuildNavMesh();
        }
        else
        {
            Debug.LogWarning($"No NavMeshSurface found on {gameObject.name}");
        }
    }
}