using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using TMPro;
public class GameOverScreen : MonoBehaviour
{
    public UIDocument uiDocument;
    private VisualElement root;
    private Label scoreText;
    private Label Text;
    private Button LobbyButton;
    private Button DocumentsButton;
    private VisualElement Container;
    [SerializeField] private UIDocument LobbyUI;
    [SerializeField] private UIDocument DocumentUI;


    void Awake()
    {
        root = uiDocument.rootVisualElement;
        scoreText = root.Q<Label>("Score");
        Text = root.Q<Label>("Text");
        LobbyButton = root.Q<Button>("Lobby");
        DocumentsButton = root.Q<Button>("Documents");
        Container = root.Q<VisualElement>("Container");
    }
    void OnEnable()
    {
        LobbyButton.clicked += OpenLobby;
        DocumentsButton.clicked += OpenDocuments;
    }

    private void OpenLobby()
    {
        Debug.Log("Bouton Lobby cliqué");
        if (LobbyUI != null)
           { Debug.Log("LobbyUI != null");
        LobbyUI.sortingOrder = 99;}
        if (uiDocument != null){
            uiDocument.sortingOrder = 0;
        Debug.Log("uiDocument != null");}
    }

    private void OpenDocuments()
    {
        Debug.Log("Bouton Documents cliqué");
        if (DocumentUI != null){
            DocumentUI.sortingOrder = 99;
        Debug.Log("DocumentUI != null");}
        if (uiDocument != null){
            uiDocument.sortingOrder = 0;
        Debug.Log("uiDocument != null");}
    }
}