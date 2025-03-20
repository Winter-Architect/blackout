using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class locker : MonoBehaviour
{
    public Animator Animator;
    public string Open;
    public bool open = false;
    public bool debounce = false;
    public AudioSource Audio; // Reference to AudioSource
    IEnumerator WaitForAnimation()
    {
        AnimatorStateInfo animState = Animator.GetCurrentAnimatorStateInfo(0);
        yield return new WaitForSeconds(animState.length); // Waits for animation to finish
        debounce = false;
    }
    public void DoSomething()
    {
        
        if (debounce == false)
        {
            debounce = true;
            if (open)
            {
                Animator.SetBool(Open, false);
                open = false;
                if (Audio != null && !Audio.isPlaying)
                {
                    Audio.Play();
                }
            }
            else
            {
                Animator.SetBool(Open, true);
                open = true;
                if (Audio != null && !Audio.isPlaying)
                {
                    Audio.Play();
                }
            }

            StartCoroutine(WaitForAnimation());
        }

    }
}
