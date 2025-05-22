using UnityEngine;

public class InteractableButton : BaseInteractable, IInteractable
{

    public override void AcceptInteraction(IInteractionHandler handler)
    {
        handler.InteractWith(this);
        OnInteract?.Invoke();
    }

    void Start()
    {
        // OnInteract.AddListener(() => Debug.Log("Button pressed"));
    }

}
