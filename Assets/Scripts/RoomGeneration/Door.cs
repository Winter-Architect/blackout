using UnityEngine;

public class Door : MonoBehaviour
{

    public Animator Animator;
    public string PlayerTag;
    public string OpenCloseAnnimBoolName;

    void OnTriggerEnter(Collider other)
    {        
        if (other.CompareTag(PlayerTag))
        {
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