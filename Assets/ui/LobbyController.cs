using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UIElements;

public class LobbyController : NetworkBehaviour
{
    [SerializeField] private UIDocument lobbyUI;
    [SerializeField] private List<VisualElement> playerDisplays = new List<VisualElement>(); 
    [SerializeField] private List<VisualElement> readyCheckDisplays = new List<VisualElement>();

    public Button readyButton;
    public Button startButton;

    private NetworkList<PlayerInfo> playerInfo = new NetworkList<PlayerInfo>();

    public struct PlayerInfo: INetworkSerializable, IEquatable<PlayerInfo>
    {
        public ulong playerID;
        public bool isPlayerReady;

        public PlayerInfo(ulong playerID, bool isPlayerReady)
        {
            this.playerID = playerID;
            this.isPlayerReady = isPlayerReady;
        }

        public void toggleReady(){
            isPlayerReady = !isPlayerReady;
        }

        public bool Equals(PlayerInfo other)
        {
            return playerID == other.playerID && isPlayerReady == other.isPlayerReady;
        }

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref playerID);
            serializer.SerializeValue(ref isPlayerReady);
        }
    }

    private int FindClientIdPlayerInfoInConnectedPlayers(ulong clientId){
        for(int i = 0; i < playerInfo.Count; i++)
        {
            var player = playerInfo[i];
            if(player.playerID == clientId)
            {
                return i;
            }
        }
        return -1;
    }

    private void Awake()
    {
        lobbyUI.rootVisualElement.visible = false;

    }
    private void OnEnable()
    {
        var root = lobbyUI.rootVisualElement;
        var playerDisplays = root.Query<VisualElement>(className: "playerDisplay").ToList();
        var readyDisplays = root.Query<VisualElement>(className: "readyDisplay").ToList();


        readyButton = root.Q<Button>("Ready");
        startButton = root.Q<Button>("Launch");
        this.playerDisplays.AddRange(playerDisplays);
        this.readyCheckDisplays.AddRange(readyDisplays);

        readyButton.clicked += OnReadyClicked;
        startButton.clicked += OnStartClicked;
        ResetPlayerDisplays();

    }

    private void OnReadyClicked()
    {
        TogglePlayerReadyServerRpc();
    }

    private void OnStartClicked()
    {
        bool CanStart()
        {
            if(playerInfo.Count < 2)
            {
                return false;
            }
            foreach (var info in playerInfo)
            {
                if(info.isPlayerReady == false)
                {
                    return false;
                }
            }
            return true;
        }

        if(CanStart())
        {
            NetworkManager.Singleton.SceneManager.LoadScene("DemoScene", UnityEngine.SceneManagement.LoadSceneMode.Single);
        }
        else
        {
            Debug.Log("Not enough players or not all players ready");
        }
    }

    public override void OnNetworkSpawn()
    {
        if(IsServer)
        {
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
        }
        playerInfo.OnListChanged += OnPlayerListChanged;

    }

    private void OnPlayerListChanged(NetworkListEvent<PlayerInfo> changeEvent)
    {
        UpdatePlayerDisplays();
    }

    private void OnClientConnected(ulong clientId)
    {
        Debug.Log("Player Connected");
        playerInfo.Add(new PlayerInfo(clientId, false));
        UpdatePlayerDisplays();
    }

    private void OnClientDisconnected(ulong clientId)
    {
        playerInfo.RemoveAt(FindClientIdPlayerInfoInConnectedPlayers(clientId));
        UpdatePlayerDisplays();
        
    }

    public override void OnDestroy()
    {
        if(IsServer && NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
            
            NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnected;
        }

        playerInfo.OnListChanged -= OnPlayerListChanged;
    }


    private void UpdatePlayerDisplays()
    {
        ResetPlayerDisplays();
        int count = 0;
        foreach (var client in playerInfo)
        {
            try
            {

                if(client.isPlayerReady)
                {
                    readyCheckDisplays[count].style.unityBackgroundImageTintColor = Color.white;
                }
                else
                {
                    readyCheckDisplays[count].style.unityBackgroundImageTintColor = Color.black;
                }
                playerDisplays[count].style.unityBackgroundImageTintColor = Color.white;
                count ++;
            }
            catch(IndexOutOfRangeException)
            {
                Debug.Log("Too many players !");
            }
        }
    }
    private void ResetPlayerDisplays()
    {
        foreach(var display in playerDisplays)
        {
            display.style.unityBackgroundImageTintColor = Color.black;
        }
        foreach(var readyCheckDisplay in readyCheckDisplays)
        {
            readyCheckDisplay.style.unityBackgroundImageTintColor = Color.black;
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void TogglePlayerReadyServerRpc(ServerRpcParams rpcParams = default)
    {

        ulong senderClientId = rpcParams.Receive.SenderClientId;
        int playerIndex = FindClientIdPlayerInfoInConnectedPlayers(senderClientId);

        PlayerInfo player = playerInfo[playerIndex];
    
        player.toggleReady();
        
        playerInfo[playerIndex] = player;
    }


}
