using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using Unity.Services.Matchmaker.Models;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class DisplayPlayers : NetworkBehaviour
{
    public NetworkList<bool> PlayersReady = new NetworkList<bool>();

    public static DisplayPlayers Instance;
    public NetworkList<ulong> PlayerIDs = new NetworkList<ulong>();

    [SerializeField] private List<Image> playerDisplays = new List<Image>();
    [SerializeField] private List<Image> readyDisplays = new List<Image>();
    void Awake()
    {
        if(Instance is null){
            Instance = this;
        }
        else{
            Destroy(gameObject);
        }
    }

    public bool CanStart()
    {
        return !PlayersReady.Contains(false);
    }
    public override void OnNetworkSpawn()
    {
        if(IsServer)
        {
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
        }

        PlayerIDs.OnListChanged += OnPlayerListChanged;
        PlayersReady.OnListChanged += OnPlayerListChanged;

    }

    private void OnClientConnected(ulong clientId)
    {
        PlayerIDs.Add(clientId);
        PlayersReady.Add(false);
    }

    private void OnClientDisconnected(ulong clientId)
    {
        int index = PlayerIDs.IndexOf(clientId);
        PlayerIDs.Remove(clientId);
        PlayersReady.RemoveAt(index);
    }

    public override void OnDestroy()
    {
        if (IsServer && NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnected;
        }

        PlayerIDs.OnListChanged -= OnPlayerListChanged;
        PlayersReady.OnListChanged -= OnPlayerListChanged;
    }

    private void OnPlayerListChanged(NetworkListEvent<ulong> changeEvent)
    {
        Debug.Log("Player list updated!");
        int i = 0;
        while(i < PlayerIDs.Count){
            playerDisplays[i].color = Color.white;
            
            i++;
        }
        for(int j = i; j < playerDisplays.Count; j++){
            playerDisplays[j].color = Color.black;
        }
    }

    private void OnPlayerListChanged(NetworkListEvent<bool> changeEvent)
    {
        Debug.Log("Player ready list updated!");
        int i = 0;
        while(i < PlayersReady.Count){
            if(PlayersReady[i]){
                Debug.Log(i + "is ready");
                readyDisplays[i].color = Color.white;
            }
            else
            {
                readyDisplays[i].color = Color.black;
            }
            i++;
        }
        for(int j = i; j < readyDisplays.Count; j++){
            readyDisplays[j].color = Color.black;
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void SetPlayerReadyServerRpc(ulong clientId, bool isReady)
    {
        int index = PlayerIDs.IndexOf(clientId);
        PlayersReady[index] = isReady;
    }

}
