using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using TMPro;
using UnityEngine.SceneManagement;
using Unity.Services.Lobbies.Models;
public class GameOverScreen : MonoBehaviour
{
    public UIDocument uiDocument;
    private VisualElement root;
    private Label scoreText;
    private Label Text;
    private Button LobbyButton;
    private Button DocumentsButton;


    void Awake()
    {
        root = uiDocument.rootVisualElement;
        scoreText = root.Q<Label>("Score");
        Text = root.Q<Label>("Text");
        LobbyButton = root.Q<Button>("Lobby");
        DocumentsButton = root.Q<Button>("Documents");
    }
    void OnEnable()
    {
        LobbyButton.clicked += OpenLobby;
        DocumentsButton.clicked += OpenDocuments;

        scoreText.text = PlayerPrefs.GetInt("CurrentRoomID") - 1 + " Room(s) cleared\n";
    }

    private void OpenLobby()
    {
        if (Unity.Netcode.NetworkManager.Singleton != null)
        {
            Unity.Netcode.NetworkManager.Singleton.Shutdown();
        }
        SceneManager.LoadScene("StartMenu");
    }

    private void OpenDocuments()
    {
        // Lance le chargement de la scène et abonne-toi à l'événement sceneLoaded
        if (Unity.Netcode.NetworkManager.Singleton != null)
        {
            Unity.Netcode.NetworkManager.Singleton.Shutdown();
        }
        SceneManager.sceneLoaded += OnStartMenuLoaded;
        SceneManager.LoadScene("StartMenu");
    }

    private void OnStartMenuLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("Scene loaded: " + scene.name);
        if (scene.name == "StartMenu")
        {
            // Cherche le GameObject qui contient DocumentUI et le script Document
            var docObj = GameObject.FindFirstObjectByType<Document>();
            if (docObj != null)
            {
                docObj.OpenDocumentUI(); // Appelle une méthode publique à créer dans ton script Document
                Debug.Log("Document UI opened");
            }
            else
            {
                Debug.LogWarning("Document script not found in the scene.");
            }
            // Désabonne-toi pour éviter des appels multiples
            SceneManager.sceneLoaded -= OnStartMenuLoaded;
        }
    }
    
}
