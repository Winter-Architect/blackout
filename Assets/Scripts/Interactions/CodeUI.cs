using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class CodeUI : MonoBehaviour
{
    [SerializeField] private TMP_InputField myText;

    [SerializeField] private GameObject portal;

    [SerializeField] private Animator lightAnimator;
    private string code = "0285";

    void OnEnable()
    {
        myText = GetComponentInChildren<TMP_InputField>();
        EventSystem.current.SetSelectedGameObject(myText.gameObject, null);
        myText.OnPointerClick(new PointerEventData(EventSystem.current));
    }

    void Update(){
        if(Input.GetKeyDown(KeyCode.Return))
        {
            if(myText.text == code)
            {
                Debug.Log("Congrats you won !");
                portal.GetComponent<Portal>().setTraverse(true);
                lightAnimator.SetTrigger("Light");
            }
        }
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            gameObject.SetActive(false);
        }
    }
}
