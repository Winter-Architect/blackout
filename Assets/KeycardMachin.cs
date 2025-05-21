using System;
using Blackout.Inventory;
using Unity.Netcode;
using UnityEngine;
using System.Collections;
using Unity.VisualScripting;

public class KeycardMachin : NetworkBehaviour
{
    
    public Animator Animator;
    public bool debounce = false;
    public string Open;
    public AudioSource Audio; // Reference to AudioSource
    private Agent agent;
    public Door DoorCondition;
    private void Start()
    {
        agent = FindFirstObjectByType<Agent>();
        
    }
    IEnumerator WaitForAnimation()
    {
        AnimatorStateInfo animState = Animator.GetCurrentAnimatorStateInfo(0);
        yield return new WaitForSeconds(animState.length); // Waits for animation to finish
        DoorCondition.Condition = true;
        debounce = false;
    }
    public void DoSomething()
    {
        Debug.Log(agent.currentlyEquippedItempublic?.name);
        if (debounce == false && agent.currentlyEquippedItempublic?.name == "keycard variant(Clone)" )
        {

            
            debounce = true;

            //InventoryController.Instance.RemoveItemFromInv(agent.currentlyEquippedItempublic.GetComponent<Item>());
            
            
            
            Animator.SetBool(Open, true);
            if (Audio != null && !Audio.isPlaying)
            {
                Audio.Play();
            }
            StartCoroutine(WaitForAnimation());
            
            
            debounce = false;
        }
    }
}
