using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public abstract class BaseInteractable : MonoBehaviour, IInteractable
{
    protected SphereCollider myCheckTrigger;
    protected bool canInteract = false;
    protected List<IInteractor> interactorsInRange = new List<IInteractor>();
    protected Outline myOutline;

    public UnityEvent OnInteract;
    [SerializeField] protected float interactionRange;

    public abstract void AcceptInteraction(IInteractionHandler handler);
    void Awake()
    {
        myCheckTrigger = gameObject.AddComponent<SphereCollider>();
        myCheckTrigger.isTrigger = true;
        myCheckTrigger.radius = interactionRange;
    }
    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.TryGetComponent<IInteractor>(out IInteractor myInteractor) && !canInteract)
        {
            canInteract = true;
            myOutline = gameObject.AddComponent<Outline>();
            interactorsInRange.Add(myInteractor);
        }   
    }
    void OnTriggerExit(Collider other)
    {
        if(other.gameObject.TryGetComponent<IInteractor>(out IInteractor myInteractor) && canInteract)
        {
            canInteract = false;
            interactorsInRange.Remove(myInteractor);
            if(interactorsInRange.Count == 0)
            {
                if(myOutline != null)
                {
                    Destroy(myOutline);
                }
            }
        }  
    }




}
