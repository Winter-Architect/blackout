using UnityEngine;
using UnityEngine.UIElements;

public class CameraHUD : MonoBehaviour
{
    private UIDocument uIDocument;
    private VisualElement ui;
    private Button OpenTerminalButton;
    private bool isTermOpen = false;

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

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T)) OpenTerminal();
    }

    void OpenTerminal()
    {
        terminal.gameObject.SetActive(!isTermOpen);
        isTermOpen = !isTermOpen;
    }
}