using UnityEngine;
using UnityEngine.UI;

public class ProximityPrompt : MonoBehaviour
{
    public GameObject promptUI; // Assign a UI element in the Inspector
    private bool canInteract = false;
    
    private Agent playerAgent; // Reference to the player's Agent script

    void Start()
    {
        if (promptUI != null)
            promptUI.SetActive(false);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // Ensure the player has the "Player" tag
        {
            canInteract = true;
            if (promptUI != null)
                promptUI.SetActive(true);

            // Get the Agent component from the player
            playerAgent = other.GetComponent<Agent>();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            canInteract = false;
            if (promptUI != null)
                promptUI.SetActive(false);
            playerAgent = null; // Reset reference when leaving
        }
    }

    void Update()
    {
        if (canInteract && Input.GetKeyDown(KeyCode.E)) // Press "E" to interact
        {
            Interact();
        }
    }

    void Interact()
    {
        if (playerAgent != null)
        {
            playerAgent.hasKey = true; // Set hasKey to true inside the Agent script
            Debug.Log("Player picked up the key!");
        }
    }
}