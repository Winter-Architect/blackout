using System;
using Unity.AI.Navigation;
using Unity.Netcode;
using UnityEngine;

public class RoomTrigger : NetworkBehaviour
{
    private bool triggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (triggered || !IsServer) return;
        if (other.CompareTag("Player"))
        {
            Debug.Log("trigger");
            triggered = true;
            var generator = FindFirstObjectByType<RoomsGeneration>();
            var currentRoom = GetComponent<Room>();

            if (generator != null && currentRoom != null)
            {
                generator.GenerateRoom(currentRoom, currentRoom.RoomID);
            }

            NavMeshLink[] navmeshLinks = GetComponentsInChildren<NavMeshLink>(true);
            foreach (NavMeshLink navMesh in navmeshLinks)
            {
                Debug.Log("Found Navmesh");
                navMesh.enabled = true;
                navMesh.gameObject.SetActive(true);
            }
        }
    }
}