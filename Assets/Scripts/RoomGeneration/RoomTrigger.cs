using Unity.AI.Navigation;
using Unity.Netcode;
using UnityEngine;

public class RoomTrigger : NetworkBehaviour
{
    private bool triggered = false;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("trigger entered " + other.name + " " + triggered);
        if (triggered || !IsServer) return;
        if (other.CompareTag("Player"))
        {
            Debug.Log("trigger player entered");
            triggered = true;
            var generator = FindFirstObjectByType<RoomsGeneration>();
            var currentRoom = GetComponent<Room>();

            if (generator != null && currentRoom != null)
            {
                Debug.Log("staring room generation");
                generator.GenerateRoom(currentRoom, currentRoom.RoomID);
                Debug.Log("room generated");
            }
            NavMeshLink[] navmeshLinks = GetComponentsInChildren<NavMeshLink>(true);
            foreach (NavMeshLink navMesh in navmeshLinks)
            {
                Debug.Log("Found Navmesh");
                navMesh.enabled = false;
                navMesh.enabled = true;
                navMesh.gameObject.SetActive(false);
                navMesh.gameObject.SetActive(true);
            }
        }
    }
}