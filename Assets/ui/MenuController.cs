using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class MenuController : MonoBehaviour
{
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

    private string[] codesTests = new[] { "1234", "1111" }; // liste temporaire à remplacer avec une liste des codes actifs pour rejoindre les parties en cours
    

    public void Awake()
    {
        UIDocument = gameObject.GetComponent<UIDocument>();
        UIDocument.enabled = true;
        ui = UIDocument.rootVisualElement;
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
            string text = evt.newValue;
            if (text.Length == 4)
            {
                if (codesTests.Contains(text))
                {
                    codeField.maxLength = 20;
                    codeField.isReadOnly = true;
                    codeField.value = "loading ...";
                    codeField.style.color = new StyleColor(new Color32(144, 190, 109, 255));
                    Debug.Log("code valide");
                }
                else
                {
                    codeField.maxLength = 50;
                    codeField.isReadOnly = true;
                    codeField.value = "this code does not exit";
                    codeField.style.color = new StyleColor(new Color32(230, 57, 70, 255));
                    Debug.Log("code invalid");
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
