using UnityEngine;
using UnityEngine.UIElements;

public class Terminal : MonoBehaviour
{
    private UIDocument uIDocument;
    private VisualElement ui;
    private VisualElement BoutonsContainer;
    private VisualElement TextContainer;   
    private ScrollView ScrollContainer;
    private Button ClearTerminalButton;
    private Button ExitTerminalButton;
    private Button OpenMapButton;
    private TextField commandInput;
    private Button ExecuteCommandButton;


    void Awake()
    {
        
        if(TryGetComponent<UIDocument>(out uIDocument))
        {
            uIDocument.gameObject.SetActive(true);
        }
        else
        {
            Debug.LogError("No UIDocument found on Terminal");
        }
    }

    void OnEnable()
    {
        ui = uIDocument.rootVisualElement;
        
        BoutonsContainer = ui.Q<VisualElement>("ButtonsZoneContainer");
        TextContainer = ui.Q<VisualElement>("TextZoneContainer");
        ScrollContainer = TextContainer.Q<ScrollView>();

        commandInput = TextContainer.Q<TextField>("ExecuteCommand");
        ExecuteCommandButton = commandInput.Q<Button>("ExcuteCommand");

        ClearTerminalButton = BoutonsContainer.Q<Button>("ClearTerminal");      
        ExitTerminalButton = BoutonsContainer.Q<Button>("ExitTerminal");  
        OpenMapButton = BoutonsContainer.Q<Button>("OpenMap");

        ClearTerminalButton.clicked += ClearTerminal;
        ExitTerminalButton.clicked += ExitTerminal;
        OpenMapButton.clicked += () => AddMessageToTerminal("BITE");
        ExecuteCommandButton.clicked += ExecuteCommand;
    }

    void ExecuteCommand() {
        AddMessageToTerminal(commandInput.text);
        commandInput.value = "";
    }

    void ClearTerminal() {
        ScrollContainer.Clear();
    }

    void ExitTerminal() {
        uIDocument.gameObject.SetActive(false);
    }

    public void AddMessageToTerminal(string message) {
        var scrollPos = ScrollContainer.scrollOffset;
        var textElement = new Label();
        textElement.text = message;
        textElement.AddToClassList("text");
        ScrollContainer.Add(textElement);
        scrollPos.y += 250;
        ScrollContainer.scrollOffset = scrollPos;
    }
}
