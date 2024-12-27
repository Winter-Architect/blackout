using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using Unity.Netcode;
using UnityEngine.Audio;

public class MenuController : MonoBehaviour
{
    [SerializeField] private AudioMixer masterMixer;
    
    public UIDocument UIDocument;
    
    public VisualElement ui;
    public VisualElement Buttons;
    public VisualElement playPanel;
    public Button playButton;
    public Button settingsButton;
    public Button exitButton;
    public Button hostButton;
    public Button exitPlayPanelButton;
    public TextField codeField;
    public SliderInt volumeSlider;

    public VisualElement settingsPanel;
    public Button exitSettingsButton;

    private string[] codesTests = new[] { "1234", "1111" }; // liste temporaire à remplacer avec une liste des codes actifs pour rejoindre les parties en cours
    

    public void Awake()
    {
        UIDocument = gameObject.GetComponent<UIDocument>();
        UIDocument.enabled = true;
        ui = UIDocument.rootVisualElement;
        masterMixer.SetFloat("MasterVolume", PlayerPrefs.GetFloat("SavedMasterVolume"));
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
        settingsButton.clicked += OnSettingsClicked;
        
        
        /* Settings */
        settingsPanel = ui.Q<VisualElement>("SettingsPanel");
        settingsPanel.style.display = DisplayStyle.None;
        exitSettingsButton = ui.Q<Button>("ExitSettingsPanel");
        volumeSlider = ui.Q<SliderInt>("VolumeSlider");
        
        exitSettingsButton.text = "Back";
        
        exitSettingsButton.clicked += OnSettingsClosed;
        
        volumeSlider.value = (int)(PlayerPrefs.GetFloat("SavedMasterVolume") * 100);


        volumeSlider.RegisterValueChangedCallback(evt =>
        {
            SetVolume(evt.newValue);
        });
        
        
        
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
            string text = evt.newValue;
            if (text.Length == 4)
            {
                if (codesTests.Contains(text))
                {
                    codeField.maxLength = 20;
                    codeField.isReadOnly = true;
                    codeField.value = "loading ...";
                    codeField.style.color = new StyleColor(new Color32(144, 190, 109, 255));
                    
                    var transport = NetworkManager.Singleton.GetComponent<Unity.Netcode.Transports.UTP.UnityTransport>();
                    transport.ConnectionData.Address = text;
                    transport.ConnectionData.Port = 7777;

                    NetworkManager.Singleton.StartClient();
                    Debug.Log("code valide");
                }
                else
                {
                    codeField.maxLength = 50;
                    codeField.isReadOnly = true;
                    codeField.value = "this code does not exist";
                    codeField.style.color = new StyleColor(new Color32(230, 57, 70, 255));
                    Debug.Log("code invalide");
                    StartCoroutine(RevertTextAfterDelay(3f, ""));
                }
            }
        });
        
    }


    private IEnumerator RevertTextAfterDelay(float delay, string text)
    {
        yield return new WaitForSeconds(delay); 
        codeField.value = text; 
        codeField.maxLength = 4;
        codeField.isReadOnly = false;
    }

    private void OnPlaybutton()
    {
        playPanel.style.display = DisplayStyle.Flex;
        Buttons.style.display = DisplayStyle.None;
    }

    private void ExitPlayPanel()
    {
        playPanel.style.display = DisplayStyle.None;
        Buttons.style.display = DisplayStyle.Flex;
    }

    private void HostGameClicked()
    {
        gameObject.SetActive(false);/*
        NetworkManager.Singleton.StartHost();   

        isReady = true;
        DisplayPlayers.Instance.SetPlayerReadyServerRpc(NetworkManager.Singleton.LocalClientId, isReady);
        Debug.Log("Host");*/
    }

    private void OnExitClicked()
    {
        Application.Quit();
    }

    private void OnSettingsClicked()
    {
        settingsPanel.style.display = DisplayStyle.Flex;
        Buttons.style.display = DisplayStyle.None;
    }
    private void OnSettingsClosed()
    {
        settingsPanel.style.display = DisplayStyle.None;
        Buttons.style.display = DisplayStyle.Flex;
    }

    public void SetVolume(float value)
    {
        if (value < 1) value = .0001f;
        PlayerPrefs.SetFloat("SavedMasterVolume", value);
        masterMixer.SetFloat("MasterVolume", Mathf.Log10(value / 100) * 20f);
    }
}
