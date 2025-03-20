using UnityEngine;

public class MiniDoorScript : MonoBehaviour
{
    public Animator Animator;
    public string PlayerTag;
    public string Open;
    public AudioSource DoorAudio; // Reference to AudioSource
    void OnTriggerEnter(Collider other)
    {        
        if (other.CompareTag(PlayerTag))
        {
            Animator.SetBool(Open, true);
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
