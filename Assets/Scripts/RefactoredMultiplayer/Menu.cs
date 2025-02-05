using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using Unity.Netcode;
using UnityEngine.Audio;
using System;
using Unity.Netcode.Transports.UTP;
using System.Net;
using System.Net.Sockets;
using UnityEngineInternal;

public class Menu : MonoBehaviour
{


    [SerializeField] private AudioMixer masterMixer;
    [SerializeField] private UIDocument LobbyUI;
    public UIDocument menuUI;
    public VisualElement ui;
    public VisualElement Buttons;
    public VisualElement playPanel;
    public VisualElement networkPanel;
    public VisualElement lanPanel;
    public Button hostLanButton;
    public Button joinLanButton;
    public TextField IpField;
    public Button exitLanPanel;

    public Button lanButton;
    public Button onlineButton;
    public Button exitNetworkPanelButton;
    public Button playButton;
    public Button settingsButton;
    public Button exitButton;
    public Button hostButton;
    public Button exitPlayPanelButton;
    public TextField codeField;
    public SliderInt volumeSlider;
    public Button LowQButton;
    public Button MedQButton;
    public Button HighQButton;
    public VisualElement settingsPanel;
    public Button exitSettingsButton;    
    private Dictionary<string, Button> qualityButtons;
    private const string SELECTED_CLASS = "qualityButtonsSelected";
    private const string DEFAULT_CLASS = "qualityButtons";
    
    public void Awake()
    {
        menuUI = gameObject.GetComponent<UIDocument>();
        menuUI.enabled = true;
        ui = menuUI.rootVisualElement;
        LobbyUI.rootVisualElement.visible = false;
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

        LowQButton = ui.Q<Button>("LowQuality");
        MedQButton = ui.Q<Button>("MediumQuality");
        HighQButton = ui.Q<Button>("HighQuality");

        qualityButtons = new Dictionary<string, Button>
        {
            { "Low", ui.Q<Button>("LowQuality") },
            { "Medium", ui.Q<Button>("MediumQuality") },
            { "High", ui.Q<Button>("HighQuality") }
        };
        
        foreach (var kvp in qualityButtons)
        {
            int qualityLevel = kvp.Key == "Low" ? 0 : kvp.Key == "Medium" ? 1 : 2;
            kvp.Value.clicked += () => SetQuality(kvp.Key, qualityLevel);
        }
        
        playPanel = ui.Q<VisualElement>("PlayPanel");
        hostButton = ui.Q<Button>("Host");
        exitPlayPanelButton = ui.Q<Button>("ExitPlayPanel");
        codeField = ui.Q<TextField>("Code");
        
        hostButton.text = "Host Game";
        exitPlayPanelButton.text = "Back";
        
        playPanel.style.display = DisplayStyle.None;

        exitPlayPanelButton.clicked += ExitPlayPanel;
        hostButton.clicked += HostGameClicked;

        networkPanel = ui.Q<VisualElement>("NetworkPanel");
        lanButton = ui.Q<Button>("LAN");
        onlineButton = ui.Q<Button>("Online");
        exitNetworkPanelButton = ui.Q<Button>("ExitNetworkPanel");

        lanButton.text = "LAN";
        onlineButton.text = "Online";
        exitNetworkPanelButton.text = "Back";

        exitNetworkPanelButton.clicked += ExitNetworkPanel;
        onlineButton.clicked += OnOnlineButton;

        lanPanel = ui.Q<VisualElement>("LanPanel");
        hostLanButton = ui.Q<Button>("HostLan");
        joinLanButton = ui.Q<Button>("PlayLan");
        exitLanPanel = ui.Q<Button>("ExitLanPanel");
        IpField = ui.Q<TextField>("IP");

        lanPanel.style.display = DisplayStyle.None;

        hostLanButton.text = "Host";
        joinLanButton.text = "Join";
        exitLanPanel.text = "Back";

        lanButton.clicked += OnLanButton;

        hostLanButton.clicked += OnHostLan;
        joinLanButton.clicked += OnJoinLan;
        exitLanPanel.clicked += OnExitLan;
        


        codeField.RegisterValueChangedCallback(evt =>
        {
            string text = evt.newValue;
            if (text.Length == 6)
            {
                try
                {
                    codeField.maxLength = 20;
                    codeField.isReadOnly = true;
                    codeField.value = "loading ...";
                    codeField.style.color = new StyleColor(new Color32(144, 190, 109, 255));
                    JoinGame(text);
                    StartCoroutine(RevertTextAfterDelay(3f, ""));
                    codeField.style.color = new StyleColor(new Color32(230, 57, 70, 255));
                    Debug.Log("code valide");
                }
                catch(Exception e)
                {
                    codeField.maxLength = 50;
                    codeField.isReadOnly = true;
                    codeField.value = "this code does not exist";
                    codeField.style.color = new StyleColor(new Color32(230, 57, 70, 255));
                    Debug.Log("code invalide");
                    StartCoroutine(RevertTextAfterDelay(3f, ""));
                    StartCoroutine(GoBackMenuUIAfterDelay(3f));

                    Debug.Log(e.Data);
                }
            }
        });
        
    }

    private void SetQuality(string selectedQuality, int qualityLevel)
    {
        QualitySettings.SetQualityLevel(qualityLevel, true);

        foreach (var kvp in qualityButtons)
        {
            var button = kvp.Value;
            bool isSelected = kvp.Key == selectedQuality;

            if (isSelected)
            {
                button.RemoveFromClassList(DEFAULT_CLASS);
                button.AddToClassList(SELECTED_CLASS);
            }
            else
            {
                button.RemoveFromClassList(SELECTED_CLASS);
                button.AddToClassList(DEFAULT_CLASS);
            }
        }
    }

    private IEnumerator GoBackMenuUIAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay); 
        Debug.Log("SALUT");
        LobbyUI.rootVisualElement.visible = false;
        menuUI.rootVisualElement.visible = true;
    }

    private IEnumerator RevertTextAfterDelay(float delay, string text)
    {
        yield return new WaitForSeconds(delay); 
        codeField.value = text; 
        codeField.maxLength = 6;
        codeField.isReadOnly = false;
        
    }

    private void OnLanButton()
    {
        lanPanel.style.display = DisplayStyle.Flex;
        networkPanel.style.display = DisplayStyle.None;

    }
    public static string GetLocalIPv4()
    {
        string localIP = string.Empty;

        try
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());

            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    localIP = ip.ToString();
                    break;
                }
            }

            if (string.IsNullOrEmpty(localIP))
            {
                Debug.LogError("Aucune adresse IPv4 trouvée.");
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Erreur lors de la récupération de l'IP locale : {ex.Message}");
        }

        return localIP;
    }
    private void OnHostLan()
    {
        try
        {
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetConnectionData(GetLocalIPv4(), 7777, "127.0.0.1");
            NetworkManager.Singleton.StartHost();
            LobbyUI.rootVisualElement.visible = true;
            menuUI.rootVisualElement.visible = false;
        }
        catch(Exception)
        {
            Debug.LogError("Could not host Session on" + GetLocalIPv4() + " 7777");
            LobbyUI.rootVisualElement.visible = false;
            menuUI.rootVisualElement.visible = true;
        }

    }

    private void OnJoinLan()
    {
        
        NetworkManager.Singleton.GetComponent<UnityTransport>().SetConnectionData(IpField.text, 7777, null);

        NetworkManager.Singleton.StartClient();

        LobbyUI.rootVisualElement.visible = true;
        menuUI.rootVisualElement.visible = false;


    }
    private void OnExitLan()
    {
        lanPanel.style.display = DisplayStyle.None;
        Buttons.style.display = DisplayStyle.Flex;
    }

    private void OnPlaybutton()
    {
        networkPanel.style.display = DisplayStyle.Flex;
        Buttons.style.display = DisplayStyle.None;
    }

    private void ExitPlayPanel()
    {
        playPanel.style.display = DisplayStyle.None;
        Buttons.style.display = DisplayStyle.Flex;
    }

    private void ExitNetworkPanel()
    {
        networkPanel.style.display = DisplayStyle.None;
        Buttons.style.display = DisplayStyle.Flex;
    }

    private void OnOnlineButton()
    {
        playPanel.style.display = DisplayStyle.Flex;
        networkPanel.style.display = DisplayStyle.None;
    }



    public void JoinGame(string myCode)
    {
        LobbyUI.rootVisualElement.Q<Label>().text = "";
        LobbyUI.rootVisualElement.visible = true;
        menuUI.rootVisualElement.visible = false;
        TestRelay.Instance.JoinRelay(myCode);
        
        
    }


    public async void HostGame()
    {
        await TestRelay.Instance.CreateRelay();
        
    }

    private void HostGameClicked()
    {
        LobbyUI.rootVisualElement.visible = true;
        menuUI.rootVisualElement.visible = false;
        HostGame();
        
        

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
