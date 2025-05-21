using Unity.VisualScripting;
using System.Collections; 
using UnityEngine;
using Unity.Netcode;

public class Door : MonoBehaviour
{
    public Animator Animator;
    public string PlayerTag;
    public string OpenCloseAnnimBoolName;
    public AudioSource DoorAudio; // Reference to AudioSource
    private bool doorOpen = false;
    public bool Condition = false;
    public bool CanBeOpen = true;
    
    public GameObject monsterPrefab;
    public Transform spawnPoint; // at this door
    public Transform exitDoor;   // the other door across the room
    
    void OnTriggerEnter(Collider other)
    {
        Debug.Log("Merde");
        if (other.CompareTag(PlayerTag))
        {
            if (!doorOpen && Condition && CanBeOpen)
            {
                Animator.SetBool(OpenCloseAnnimBoolName, true);
                doorOpen = true;
            }
        }
    }

    private IEnumerator MoveMonsterToExit(GameObject monster)
    {
        float speed = 5f;  // Set the movement speed for the monster
        float step;        // Variable to hold the calculated step size

        // While the monster hasn't reached the exit door
        while (Vector3.Distance(monster.transform.position, exitDoor.position) > 0.1f)
        {
            // Calculate the step size for each frame (speed * deltaTime)
            step = speed * Time.deltaTime;

            // Move the monster towards the exit door position
            monster.transform.position = Vector3.MoveTowards(monster.transform.position, exitDoor.position, step);

            // Yield control back to Unity, allowing it to process other things (like animations)
            yield return null; // This makes it run over multiple frames
        }

        // Once the monster reaches the exit, you can destroy it or perform other actions
        Destroy(monster);
    }

    public void CloseDoor() {
        Animator.SetBool(OpenCloseAnnimBoolName, false);
        CanBeOpen = false;
    }

    [ServerRpc(RequireOwnership = false)]
    public void RequestOpenDoorServerRpc(ulong playerId)
    {
        // Vérifie les conditions côté serveur
        if (!doorOpen && Condition && CanBeOpen)
        {
            OpenDoorClientRpc();
            doorOpen = true;
        }
    }

    [ClientRpc]
    private void OpenDoorClientRpc()
    {
        Animator.SetBool(OpenCloseAnnimBoolName, true);
        if (DoorAudio != null && !DoorAudio.isPlaying)
        {
            DoorAudio.Play();
        }
    }
}