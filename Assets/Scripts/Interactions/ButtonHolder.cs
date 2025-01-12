using UnityEngine;

public class ButtonHolder : MonoBehaviour, IInteractable
{
    public void acceptInteraction(IInteractor interactor)
    {
        Debug.Log("I HAVE BEEN INTERACTED");
    }

    public bool canAcceptInteraction(IInteractor interactor)
    {
        return true;
    }


}
