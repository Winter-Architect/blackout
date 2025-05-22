using System.Collections;
using System.Collections.Generic; // Nécessaire pour utiliser List
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class Terminal : MonoBehaviour
{
    private UIDocument uIDocument;
    private VisualElement ui;
    private VisualElement BoutonsContainer;
    private VisualElement TextContainer;   
    private VisualElement MapContainer;
    private ScrollView ScrollContainer;
    private Button ClearTerminalButton;
    private Button ExitTerminalButton;
    private Button OpenMapButton;
    private Button HelpButton;
    private TextField commandInput;
    private Button ExecuteCommandButton;

    private bool isFirstStart = true;
    public bool isMapOpen = false;
    public bool isOpen = false;
    public bool helpButtonClicked = false;
    private Sprite mapSprite;
    // Nouvelle liste pour sauvegarder les messages
    public List<string> messageHistory = new List<string>();
    private Support support;
    private void Start()
    {
        support = FindFirstObjectByType<Support>();
        
    }

    void Awake() 
    {
        if (TryGetComponent<UIDocument>(out uIDocument))
        {
            uIDocument.gameObject.SetActive(true);
            isOpen = true;
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
        MapContainer = TextContainer.Q<VisualElement>("MapContainer");

        commandInput = TextContainer.Q<TextField>("ExecuteCommand");
        ExecuteCommandButton = commandInput.Q<Button>("ExcuteCommand");

        ClearTerminalButton = BoutonsContainer.Q<Button>("ClearTerminal");      
        ExitTerminalButton = BoutonsContainer.Q<Button>("ExitTerminal");  
        OpenMapButton = BoutonsContainer.Q<Button>("OpenMap");
        HelpButton = BoutonsContainer.Q<Button>("Help");

        ClearTerminalButton.clicked += ClearTerminal;
        ExitTerminalButton.clicked += ExitTerminal;
        OpenMapButton.clicked += DisplayMap;
        ExecuteCommandButton.clicked += ExecuteCommand;
        HelpButton.clicked += ListAllCommands;

        // Restaurer les messages affichés
        if (isFirstStart)
        {
            isFirstStart = false;
            StartCoroutine(StartingTerminal());
        }
        RestoreMessageHistory();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return)) ExecuteCommand();
        
        if (support != null)
        {
            mapSprite = support.currentRoom.Map;
        }
    }

    void ExecuteCommand() {
        if (commandInput.value == "") return;
        AddMessageToTerminal(commandInput.text, true);
        switch (commandInput.value) {
            case "clear":
                ClearTerminal();
                break;
            case "exit":
                ExitTerminal();
                break;
            case "map":
                DisplayMap();
                break;
            case "help":
                ListAllCommands();
                break;
            default:
                AddMessageToTerminal("Command \"" + commandInput.value + "\" unknown");
                break;
        }
        commandInput.value = "";
    }

    void ClearTerminal() {
        ScrollContainer.Clear();
        messageHistory.Clear();
        AddMessageToTerminal("clear", true);
    }

    void ListAllCommands()
    {
        AddMessageToTerminal("List of all the commands :\n>>> clear: clear the terminal\n>>> exit: exit the terminal\n>>> map: display the map of the current room");
        helpButtonClicked = true;
    }

    void ExitTerminal()
    {
        // Cherche le CameraHUD parent ou via une référence
        CameraHUD cameraHUD = FindFirstObjectByType<CameraHUD>();
        if (cameraHUD != null)
        {
            cameraHUD.CloseTerminal();
        }
        else
        {
            // fallback si jamais CameraHUD n'est pas trouvé
            gameObject.SetActive(false);
        }
    }

    void DisplayMap() {
        AddMessageToTerminal("map", true);
        if (!isMapOpen && mapSprite == null)
        {
            AddMessageToTerminal("No map found for this room");
            return;
        }
        if (isMapOpen)
        {
            MapContainer.style.display = DisplayStyle.None;
            ScrollContainer.style.display = DisplayStyle.Flex;
            isMapOpen = false;
            OpenMapButton.text = "Open Map";
        }
        else
        {
            MapContainer.style.backgroundImage = new StyleBackground(mapSprite);
            MapContainer.style.display = DisplayStyle.Flex;
            ScrollContainer.style.display = DisplayStyle.None;
            isMapOpen = true;
            OpenMapButton.text = "Close Map";
        }
    }

    public void AddMessageToTerminal(string message, bool isACommand = false) {
        // Ajouter à l'historique
        message = isACommand ? message : ">>> " + message;
        messageHistory.Add(message);

        var scrollPos = ScrollContainer.scrollOffset;
        var textElement = new Label();
        textElement.text = message;
        textElement.AddToClassList("text");
        ScrollContainer.Add(textElement);

        scrollPos.y += 250;
        ScrollContainer.scrollOffset = scrollPos;
    }

    private void RestoreMessageHistory()
    {
        ScrollContainer.Clear(); // Réinitialiser le contenu visuel
        foreach (string message in messageHistory)
        {
            var textElement = new Label();
            textElement.text = message;
            textElement.AddToClassList("text");
            ScrollContainer.Add(textElement);
        }
    }

    IEnumerator StartingTerminal() {
        if (messageHistory.Count == 0) // Ne lancer que si c'est la première fois
        {
            AddMessageToTerminal("Starting terminal...");
            yield return new WaitForSeconds(2);
            AddMessageToTerminal("Loading packages...");
            yield return new WaitForSeconds(2);
            AddMessageToTerminal("Loaded successfully!");
        }
    }
}
