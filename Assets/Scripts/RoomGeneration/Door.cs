using Unity.VisualScripting;
using UnityEngine;

public class Door : MonoBehaviour
{

    public Animator Animator;
    public string PlayerTag;
    public string OpenCloseAnnimBoolName;
    public AudioSource DoorAudio; // Reference to AudioSource
    private bool doorOpen = false;
    public bool Condition = false;
    void OnTriggerEnter(Collider other)
    {        
        if (doorOpen == false && other.CompareTag(PlayerTag) && Condition == true)
        {
            Animator.SetBool(OpenCloseAnnimBoolName, true);
            
            // Play sound when door opens
            if (DoorAudio != null && !DoorAudio.isPlaying)
            {
                DoorAudio.Play();
            }
            doorOpen = true;
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
