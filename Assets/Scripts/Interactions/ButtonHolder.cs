using System.Collections;
using UnityEngine;

public class ButtonHolder : MonoBehaviour, IInteractable
{
    [SerializeField] private Animator codeAnimator;

    [SerializeField] private Animator codepadAnimator;
    [SerializeField] private GameObject myCanvas;

    public void acceptInteraction(IInteractor interactor)
    {
        Debug.Log("I HAVE BEEN INTERACTED");
        codeAnimator.SetTrigger("Move");
        codepadAnimator.SetTrigger("Move");
        StartCoroutine("changeCameraPerspective");
    }

    IEnumerator changeCameraPerspective()
    {
        myCanvas.SetActive(true);
        yield return new WaitForSeconds(5f);
        myCanvas.SetActive(false);
    }

    public bool canAcceptInteraction(IInteractor interactor)
    {
        return true;
    }


}
