public interface IInteractable
{
    public bool canAcceptInteraction(IInteractor interactor);
    public void acceptInteraction(IInteractor interactor);

}
