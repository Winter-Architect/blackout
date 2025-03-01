using System.Collections;
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
            surface.layerMask = LayerMask.GetMask("Walkable", "Climbable");

            surface.BuildNavMesh();

            StartCoroutine(UpdateNavMeshLinks());
        }
    }

    private IEnumerator UpdateNavMeshLinks()
    {
        yield return new WaitForSeconds(0.1f);

        NavMeshLink[] links = FindObjectsOfType<NavMeshLink>();
        
        foreach (NavMeshLink link in links)
        {
            Vector3 originalPosition = link.transform.position;

            link.transform.position += Vector3.up * 0.001f; 
            link.UpdateLink(); 
            link.transform.position = originalPosition; 
            link.UpdateLink();
        }
    }
}