using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PrototypePlayerSpawner : NetworkBehaviour
{

    public static PrototypePlayerSpawner Instance;

    public GameObject agentPrefab;
    public GameObject supportPrefab;
    public GameObject spectatorPrefab;

    private Dictionary<ulong, NetworkObject> spawnedPlayers = new Dictionary<ulong, NetworkObject>();

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void RequestSpawn(string role)
    {
        if(IsClient)
        {
            RequestSpawnServerRpc(role);
        }
    }
    [ServerRpc(RequireOwnership = false)]
    private void RequestSpawnServerRpc(string role, ServerRpcParams rpcParams = default)
    {
        ulong clientId = rpcParams.Receive.SenderClientId;

        if(spawnedPlayers.ContainsKey(clientId))
        {
            Debug.LogWarning($"Client {clientId} is already spawned");
            return;
        }

        GameObject prefabToSpawn = GetPrefab(role);

        if(prefabToSpawn == null)
        {
            Debug.LogError($"Invalid role {role}");
        }

        GameObject playerObject = Instantiate(prefabToSpawn);
        NetworkObject netObj = playerObject.GetComponent<NetworkObject>();
        netObj.SpawnWithOwnership(clientId);

        spawnedPlayers.Add(clientId, netObj);

    }

    private GameObject GetPrefab(string role)
    {
        return role switch
        {
            "Agent" => agentPrefab,
            "Support" => supportPrefab,
            "Spectator" => spectatorPrefab,

            _ => null
        };
    }

    public void HandlePlayerDisconnect(ulong clientId)
    {
        if(spawnedPlayers.TryGetValue(clientId, out NetworkObject netObject))
        {
            netObject.Despawn(true);
            spawnedPlayers.Remove(clientId);
        }
    }







}
