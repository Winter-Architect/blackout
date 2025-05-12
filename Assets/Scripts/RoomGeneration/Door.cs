using System.Collections;
using UnityEngine;

public class Door : MonoBehaviour
{
    public Animator Animator;
    public string PlayerTag;
    public string OpenCloseAnnimBoolName;
    public AudioSource DoorAudio; // Reference to AudioSource
    private bool doorOpen = false;
    public bool Condition = false;
    
    public GameObject monsterPrefab;
    public Transform spawnPoint; // at this door
    public Transform exitDoor;   // the other door across the room

    void OnTriggerEnter(Collider other)
    {        
        if (doorOpen == false && other.CompareTag(PlayerTag) && Condition == true)
        {
            Animator.SetBool(OpenCloseAnnimBoolName, true);
            
            Agent agent = other.gameObject.GetComponent<Agent>();
            if (agent.shouldSpawnEntity)
            {
                agent.spawnTimer = 120;
                agent.shouldSpawnEntity = false;
                // Monster spawn logic and movement right here!
                if (monsterPrefab != null && spawnPoint != null && exitDoor != null)
                {
                    // Spawn the monster at the spawn point
                    GameObject spawnedMonster = Instantiate(monsterPrefab, spawnPoint.position, spawnPoint.rotation);

                    // Start the movement coroutine
                    StartCoroutine(MoveMonsterToExit(spawnedMonster)); // This works now
                }
            }
            else
            {
                agent.spawnTimer -= 10;
            }
            
            // Play sound when door opens
            if (DoorAudio != null && !DoorAudio.isPlaying)
            {
                DoorAudio.Play();
            }
            doorOpen = true;
        }
    }

    // This is the proper way to define the coroutine.
    private IEnumerator MoveMonsterToExit(GameObject monster)
    {
        float speed = 5f;

        while (Vector3.Distance(monster.transform.position, exitDoor.position) > 0.1f)
        {
            monster.transform.position = Vector3.MoveTowards(monster.transform.position, exitDoor.position, speed * Time.deltaTime);
            yield return null; // This yields the control back to Unity for the next frame.
        }

        // Destroy the monster after reaching the exit door (optional)
        Destroy(monster);
    }
}
