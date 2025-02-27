
using System.Collections;
using UnityEngine;

public class CollectableItem : BaseInteractable, IInteractable
{
    public Item item;

    public override void AcceptInteraction(IInteractionHandler handler)
    {
        handler.InteractWith(this);
        OnInteract?.Invoke();
    }

    void Start()
    {
        OnInteract.AddListener(() => gameObject.SetActive(false));
    }


}
