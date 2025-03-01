using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerSpawner : NetworkBehaviour
{
    [SerializeField] private GameObject firstPlayerPrefab;
    [SerializeField] private GameObject secondPlayerPrefab;

    [SerializeField] private GameObject thirdPlayerPrefab;

    public override void OnNetworkSpawn()
    {
        if(IsServer)
        {
            NetworkManager.SceneManager.OnLoadComplete += OnSceneLoaded;

        }
    }

    private async void OnSceneLoaded(ulong clientId, string sceneName, LoadSceneMode loadSceneMode)
    {
        if (!IsServer) return;

        await Task.Delay(200);

        SpawnPlayers();

        NetworkManager.SceneManager.OnLoadComplete -= OnSceneLoaded;
    }


    async void SpawnPlayers()
    {
        var clients = NetworkManager.Singleton.ConnectedClientsIds;
        int index = 0;
        foreach(ulong clientId in clients)
        {
            GameObject playerPrefab;
            if(index == 0){
                playerPrefab = firstPlayerPrefab;
            }
            else if (index == 1){
                playerPrefab = secondPlayerPrefab;
            }
            else{
                playerPrefab = thirdPlayerPrefab;
            }
            Vector3 spawnPoint = new Vector3(0, 2f, 0);
            GameObject playerInstance = Instantiate(playerPrefab, spawnPoint, Quaternion.identity);
            NetworkObject networkObject = playerInstance.GetComponent<NetworkObject>();
            networkObject.SpawnWithOwnership(clientId);

            index++;

            await Task.Yield();

        }
    }
}
