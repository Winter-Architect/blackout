using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class LobbyManager : MonoBehaviour
{
    


    

    [SerializeField] private Button hostButton;
    [SerializeField] private Button joinButton;
    [SerializeField] private TMPro.TMP_InputField codeInputField;
    [SerializeField] private Button startGameButton;
    [SerializeField] private Button readyButton;

    [SerializeField] private bool isReady = false;

    void Start()
    {
        hostButton.onClick.AddListener(HostGame);
        joinButton.onClick.AddListener(JoinGame);
        startGameButton.onClick.AddListener(StartGame);
        readyButton.onClick.AddListener(Ready);
    }

    public async void HostGame()
    {
        await TestRelay.Instance.CreateRelay();
        isReady = true;
        DisplayPlayers.Instance.SetPlayerReadyServerRpc(NetworkManager.Singleton.LocalClientId, isReady);
    }
    public void JoinGame()
    {
        string myCode = codeInputField.text;

        TestRelay.Instance.JoinRelay(myCode);

    }
    public void StartGame(){
        //Start the game I guess ?
        if(DisplayPlayers.Instance.CanStart()){
            Debug.Log("Started game !");
            NetworkManager.Singleton.SceneManager.LoadScene("GameScene", UnityEngine.SceneManagement.LoadSceneMode.Single);

        }
        else{
            Debug.Log("Players must be all ready");
        }
    }
    public void Ready(){
        //Be ready !
        isReady = !isReady;
        DisplayPlayers.Instance.SetPlayerReadyServerRpc(NetworkManager.Singleton.LocalClientId, isReady);

    }

}
