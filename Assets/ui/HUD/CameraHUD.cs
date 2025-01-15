using UnityEngine;
using UnityEngine.UIElements;

public class Camera : MonoBehaviour
{
    private UIDocument uIDocument;
    private VisualElement ui;
    private Button OpenTerminalButton;

    [SerializeField] private GameObject terminal;

    void Awake()
    {
        uIDocument = GetComponent<UIDocument>();
        ui = uIDocument.rootVisualElement;
    }

    void OnEnable()
    {
        OpenTerminalButton = ui.Q<Button>("OpenTerminal");
        OpenTerminalButton.clicked += OpenTerminal;
    }

    void OpenTerminal()
    {
        Debug.Log("Open Terminal");
        terminal.gameObject.SetActive(true);
        // terminal.rootVisualElement.Focus();
    }
}