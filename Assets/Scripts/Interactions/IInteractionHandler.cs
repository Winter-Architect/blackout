using UnityEngine;

public interface IInteractionHandler
{
    void InteractWith(InteractableButton button);
    void InteractWith(BaseInteractable baseInteractable);
    void InteractWith(CollectableItem item);
}
