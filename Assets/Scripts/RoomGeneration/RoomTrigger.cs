using UnityEngine;

public class RoomTrigger : MonoBehaviour
{
    private bool triggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (triggered) return;
        if (other.CompareTag("Player"))
        {
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