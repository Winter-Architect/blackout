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
            var generator = FindObjectOfType<RoomsGeneration>();
            if (generator != null)
            {
                generator.GenerateNextRoom();
            }
        }
    }
}