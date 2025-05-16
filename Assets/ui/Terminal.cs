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
    private TextField commandInput;
    private Button ExecuteCommandButton;

    private bool isFirstStart = true;
    public bool isMapOpen = false;
    public bool isOpen = false;
    // Nouvelle liste pour sauvegarder les messages
    public List<string> messageHistory = new List<string>();

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

        ClearTerminalButton.clicked += ClearTerminal;
        ExitTerminalButton.clicked += ExitTerminal;
        OpenMapButton.clicked += DisplayMap;
        ExecuteCommandButton.clicked += ExecuteCommand;

        // Restaurer les messages affichés
        if (isFirstStart) {
            isFirstStart = false;
            StartCoroutine(StartingTerminal());
        }
        RestoreMessageHistory();
    }
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return)) ExecuteCommand();
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
            case "bite":
                AddMessageToTerminal("BIIITTTEEEE!!!");
                break;
            case "titouan":
                AddMessageToTerminal("<3");
                break;
            case "arcane":
                AddMessageToTerminal("Arcane, la série animée inspirée de l'univers de League of Legends, est une véritable œuvre d'art qui transcende les attentes. Sa narration captivante plonge les spectateurs dans un monde riche, avec des personnages profonds et nuancés comme Vi, Jinx, et Silco, dont les motivations complexes les rendent terriblement humains. L'animation est un chef-d'œuvre en soi, mélangeant un style visuel unique et des séquences d'action époustouflantes qui semblent tout droit sorties d'une peinture vivante. La bande-son, vibrante et immersive, amplifie chaque moment clé, donnant une profondeur émotionnelle supplémentaire. En outre, Arcane ne se contente pas de séduire les fans de League of Legends : elle s'adresse à un public plus large grâce à son exploration de thèmes universels tels que la famille, la trahison, et la lutte des classes. Ce mariage parfait entre esthétique, narration et émotion place Arcane parmi les meilleures séries jamais créées.");
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

    void ExitTerminal()
    {
        uIDocument.gameObject.SetActive(false);
        isOpen = false;
    }

    void DisplayMap() {
        AddMessageToTerminal("map", true);
        if (isMapOpen) {
            MapContainer.style.display = DisplayStyle.None;
            ScrollContainer.style.display = DisplayStyle.Flex;
            isMapOpen = false;
            OpenMapButton.text = "Open Map";
        } else {
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
