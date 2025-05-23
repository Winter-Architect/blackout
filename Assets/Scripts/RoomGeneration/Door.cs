using Unity.VisualScripting;
using System.Collections; 
using UnityEngine;
using Unity.Netcode;

public class Door : MonoBehaviour
{
    public Animator Animator;
    public string PlayerTag;
    public string OpenCloseAnnimBoolName;
    public AudioSource DoorAudio;
    private bool doorOpen = false;
    public bool Condition = false;
    public bool CanBeOpen = true;
    
    public GameObject monsterPrefab;
    public Transform spawnPoint; 
    public Transform exitDoor;  
    
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(PlayerTag))
        {
            if (!doorOpen && Condition && CanBeOpen)
            {
                Animator.SetBool(OpenCloseAnnimBoolName, true);
                if (DoorAudio != null && !DoorAudio.isPlaying)
                {
                    DoorAudio.Play();
                }
                foreach (var doorCollider in gameObject.GetComponents<Collider>())
                {
                    doorCollider.enabled = false;
                }
                doorOpen = true;
            }
        }
    }

    private IEnumerator MoveMonsterToExit(GameObject monster)
    {
        float speed = 5f; 
        float step;        

        while (Vector3.Distance(monster.transform.position, exitDoor.position) > 0.1f)
        {
            step = speed * Time.deltaTime;

            monster.transform.position = Vector3.MoveTowards(monster.transform.position, exitDoor.position, step);

            yield return null; 
        }

        Destroy(monster);
    }

    public void CloseDoor() {
        Animator.SetBool(OpenCloseAnnimBoolName, false);
        CanBeOpen = false;
    }

    [ServerRpc(RequireOwnership = false)]
    public void RequestOpenDoorServerRpc(ulong playerId)
    {
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