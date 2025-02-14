using UnityEngine;

public class Door : MonoBehaviour
{

    public Animator Animator;
    public string PlayerTag;
    public string OpenCloseAnnimBoolName;

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("Door Triggered");
        
        if (other.CompareTag(PlayerTag))
        {
            Debug.Log("Player Detected");
            Animator.SetBool(OpenCloseAnnimBoolName, true);
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
