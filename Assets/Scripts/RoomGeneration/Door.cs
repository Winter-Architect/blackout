using Unity.VisualScripting;
using UnityEngine;

public class Door : MonoBehaviour
{

    public Animator Animator;
    public string PlayerTag;
    public string OpenCloseAnnimBoolName;
    public bool CanOpenDoor = true;
    public AudioSource DoorAudio; // Reference to AudioSource
    void OnTriggerEnter(Collider other)
    {        
        if (CanOpenDoor && other.CompareTag(PlayerTag))
        {
            Animator.SetBool(OpenCloseAnnimBoolName, true);
            
            // Play sound when door opens
            if (DoorAudio != null && !DoorAudio.isPlaying)
            {
                DoorAudio.Play();
            }
        }
    }

    // void OnTriggerExit(Collider other)
    // {
        
    //     if (other.CompareTag(PlayerTag))
    //     {
    //         Animator.SetBool(OpenCloseAnnimBoolName, false);
    //     }
    // }
}
