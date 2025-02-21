using UnityEngine;

public class InteractableButton : BaseInteractable, IInteractable
{

    void Start()
    {
        OnInteract.AddListener(() => Debug.Log("Button pressed"));
    }

}
