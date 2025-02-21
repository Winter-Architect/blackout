public interface IInteractor
{
    public bool CanInteract(IInteractable interactable);
    public void InteractWith(IInteractable interactable);
}
