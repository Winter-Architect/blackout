using UnityEngine;
using UnityEngine.UIElements;

public class CameraHUD : MonoBehaviour
{
    private UIDocument uIDocument;
    private VisualElement ui;
    private Button OpenTerminalButton;
    public bool isTermOpen = false;

    [SerializeField] private GameObject terminal;
    private Terminal terminalscript;

    void Awake()
    {
        uIDocument = terminal.gameObject.GetComponent<UIDocument>();
        ui = uIDocument.rootVisualElement;
        terminalscript = terminal.gameObject.GetComponent<Terminal>();
    }

    void OnEnable()
    {
       try {
         OpenTerminalButton = ui.Q<Button>("OpenTerminal");
        OpenTerminalButton.clicked += OpenTerminal;
       } catch {}
    }

    void Update()
    {
        if (!isTermOpen) if (Input.GetKeyDown(KeyCode.T)) OpenTerminal();
        terminalscript.isOpen = isTermOpen;
    }

    void OpenTerminal()
    {
        bool willBeOpen = !isTermOpen;
        terminal.gameObject.SetActive(willBeOpen);
        isTermOpen = willBeOpen;
    }

    // Ajoute cette méthode pour fermer explicitement le terminal depuis l'extérieur
    public void CloseTerminal()
    {
        terminal.gameObject.SetActive(false);
        isTermOpen = false;
    }
}