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
        }
    }
}