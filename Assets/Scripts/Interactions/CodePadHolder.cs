using UnityEngine;

public class CodePadHolder : MonoBehaviour//, IInteractable
{

    [SerializeField] private GameObject codeUI;
    public void acceptInteraction(IInteractor interactor)
    {
        codeUI.SetActive(true);

    }

    public bool canAcceptInteraction(IInteractor interactor)
    {
        return true;
    }
}
