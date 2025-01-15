using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class Terminal : MonoBehaviour
{
    private UIDocument uIDocument;
    private VisualElement ui;
    private VisualElement BoutonsContainer;
    private VisualElement TextContainer;

    private Button ButtonUp;
    private Button ButtonDown;
    private Button ButtonLeft;
    private Button ButtonRight;
    private Button ClearTerminalButton;
    private Button ExitTerminalButton;


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
        Debug.Log(ui);
        BoutonsContainer = ui.Q<VisualElement>("ButtonsZoneContainer");
        TextContainer = ui.Q<VisualElement>("TextZoneContainer").Q<ScrollView>();

        ButtonUp = BoutonsContainer.Q<Button>("ButtonUp");
        ButtonDown = BoutonsContainer.Q<Button>("ButtonDown");
        ButtonLeft = BoutonsContainer.Q<Button>("ButtonLeft");
        ButtonRight = BoutonsContainer.Q<Button>("ButtonRight");

        ClearTerminalButton = BoutonsContainer.Q<Button>("ClearTerminal");      
        ExitTerminalButton = BoutonsContainer.Q<Button>("ExitTerminal");  

        ButtonUp.clicked += () => ButtonArrowClicked(ButtonUp);
        ButtonDown.clicked += () => ButtonArrowClicked(ButtonDown);
        ButtonLeft.clicked += () => ButtonArrowClicked(ButtonLeft);
        ButtonRight.clicked += () => ButtonArrowClicked(ButtonRight);
        
        ClearTerminalButton.clicked += ClearTerminal;
        ExitTerminalButton.clicked += ExitTerminal;

        //uIDocument.enabled = false;
    }

    void ButtonArrowClicked(Button button) {
        Debug.Log(button.name);
    }

    void ClearTerminal() {
        TextContainer.Clear();
    }

    void ExitTerminal() {
        uIDocument.gameObject.SetActive(false);
    }
}