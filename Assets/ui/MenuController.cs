using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class MenuController : MonoBehaviour
{
    public VisualElement ui;
    public VisualElement Buttons;
    public VisualElement playPanel;
    public Button playButton;
    public Button settingsButton;
    public Button exitButton;
    public Button hostButton;
    public Button exitPlayPanelButton;
    public TextField codeField;

    private void Awake()
    {
        ui = GetComponent<UIDocument>().rootVisualElement;
    }

    private void OnEnable()
    {
        Buttons = ui.Q<VisualElement>("Buttons");
        playButton = ui.Q<Button>("Play");
        settingsButton = ui.Q<Button>("Settings");
        exitButton = ui.Q<Button>("Exit");
        
        
        playButton.text = "Play";
        settingsButton.text = "Settings";
        exitButton.text = "Exit";
        
        Buttons.style.display = DisplayStyle.Flex;
        
        playButton.clicked += OnPlaybutton;
        exitButton.clicked += OnExitClicked;
        
        
        /* Play Panel*/
        playPanel = ui.Q<VisualElement>("PlayPanel");
        hostButton = ui.Q<Button>("Host");
        exitPlayPanelButton = ui.Q<Button>("ExitPlayPanel");
        codeField = ui.Q<TextField>("Code");
        
        hostButton.text = "Host Game";
        exitPlayPanelButton.text = "Back";
        
        playPanel.style.display = DisplayStyle.None;

        exitPlayPanelButton.clicked += ExitPlayPanel;
        hostButton.clicked += HostGameClicked;

        codeField.RegisterValueChangedCallback(evt =>
        {
            Debug.Log("Texte tapé : " + evt.newValue);

        });
    }

    private void OnPlaybutton()
    {
        playPanel.style.display = DisplayStyle.Flex;
        Buttons.style.display = DisplayStyle.None;
       // gameObject.SetActive(false);
    }

    private void ExitPlayPanel()
    {
        playPanel.style.display = DisplayStyle.None;
        Buttons.style.display = DisplayStyle.Flex;
    }

    private void HostGameClicked()
    {
        gameObject.SetActive(false);
    }

    private void OnExitClicked()
    {
        Application.Quit();
    }
}
