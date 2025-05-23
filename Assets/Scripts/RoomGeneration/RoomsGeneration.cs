using System;
using Unity.AI.Navigation;
using UnityEngine;
using TMPro;
using Unity.Netcode;
using Unity.VisualScripting;
using Unity.Netcode.Components;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.AI;
using Random = System.Random;

public class RoomsGeneration : NetworkBehaviour
{
    public Rooms roomPrefabs;
    public Room endRoom;    
    public GameObject zombPrefab;
    public GameObject turretPrefab;   
    public GameObject spikeyPrefab;
    public GameObject slimePrefab;
    public int numberOfRooms = 20;
    private Room previousPreviousRoom;
    
    public GameObject DoorPrefab;
    [SerializeField] public Queue<NetworkObject> GeneratedRooms = new Queue<NetworkObject>() ;

     private string LastRoomDirection = null;
     private bool lastRoomIsStairs = false;
     private float totalWeight = 0;

     private List<NetworkObject> alreadyGeneratedRooms = new List<NetworkObject>();
     
    private System.Random random = new System.Random(1);

     public override void OnNetworkSpawn() {
        if (!IsServer)
        {
            Debug.LogWarning("[Generation de salles] Ce script ne doit être exécuté que sur le serveur !");
            return;
        }
        
        StartGeneration();
     }

    void StartGeneration()
    {
        if (!NetworkManager.Singleton || !NetworkManager.Singleton.IsListening)
        {
            Debug.LogError("[Generation de salles] NetworkManager n'est pas prêt ! Les NetworkObjects ne peuvent pas être spawned.");
            return;
        }
        
        if (roomPrefabs.roomPrefabs.Length <= 1)
        {
            Debug.LogError("[Generation de salles] Probleme dans la liste : il n'y a pas assez de salles !!!!!");
            return;
        }

        for (int i = 0; i < roomPrefabs.roomPrefabs.Length; i++)
        {
            NetworkObject room = roomPrefabs.roomPrefabs[i];
            Debug.Log($"[Generation de salles] Salle {i} : {room.name} ajoutée");
            Room roomScript2 = room.GetComponent<Room>();

            if (roomScript2.GetComponent<RoomTrigger>() == null)
        {
            roomScript2.gameObject.AddComponent<RoomTrigger>();
        }

            totalWeight += roomScript2.Weight;
        }

        Room currRoom = GameObject.Find("StartingRoom").GetComponent<Room>();
        if (!currRoom) {
            Debug.LogError("[Generation de salles] La salle de départ n'a pas été initialisée (ou nom != StartingRoom) !");
            return;
        }
    }


    public void GenerateRoom(Room previousRoom, int id)
    {
        if (!IsServer) return;
        if (numberOfRooms <= 0) {
            GenerateLastRoom(previousRoom);
            return; 
        }
        PlayerPrefs.SetInt("CurrentRoomID", id + 1);
        if (GeneratedRooms.Count > 3) DeleteRoom();
        

        NetworkObject roomNetworkObject = GetRandomRoom(previousRoom.gameObject);
        if (roomNetworkObject == null) return;

        Room roomScript = roomNetworkObject.GetComponent<Room>();

        roomScript.RoomID = id + 1;
        var tmp = roomScript.GetComponentInChildren<TextMeshProUGUI>();
        if (tmp != null)
        {
            tmp.text = roomScript.RoomID < 9 ? "0" + (roomScript.RoomID + 1).ToString()
                                            : (roomScript.RoomID + 1).ToString();
        }

        var entry = roomScript.transform.Find("Entry");
        var exit = roomScript.transform.Find("Exit");
        if (entry) entry.GetComponent<BoxCollider>().enabled = false;
        if (exit) exit.GetComponent<BoxCollider>().enabled = false;

        numberOfRooms--;

        Debug.Log($"[{(IsServer ? "SERVER" : "CLIENT")}] Génération de la salle {roomScript.RoomID}, Position: {roomScript.transform.position}");

        if (GeneratedRooms.Count > 3)
        {
            NetworkObject[] roomsArray = GeneratedRooms.ToArray();
            NetworkObject previousRoomObject = roomsArray[0];
            Debug.Log(previousRoomObject.name);
            if (previousRoomObject != null)
            {
                Door door = previousRoomObject.GetComponentInChildren<Door>();
                if (door != null)
                {
                    door.CloseDoor();
                }
            }
        }



        BuildNavmeshLinks(roomScript);
        SpawnEnemies(roomScript);
        try
        {
            DeSpawnAll(GeneratedRooms.ToArray()[1].GetComponent<Room>());
        }
        catch (Exception)
        {
        }
    }

    public void DeSpawnAll(Room room)
    {
        NavMeshAgent[] navMeshAgents = room.GetComponentsInChildren<NavMeshAgent>(true);
        foreach (NavMeshAgent navMesh in navMeshAgents)
        {
            Debug.Log("delete Agent");
            NetworkObject netObj = navMesh.GetComponentInParent<NetworkObject>();
            netObj.Despawn();
        }
        NavMeshSurface[] navMeshSurf = room.GetComponentsInChildren<NavMeshSurface>(true);
        foreach (NavMeshSurface navMesh in navMeshSurf)
        {
        }
        NavMeshLink[] navmeshLinks = room.GetComponentsInChildren<NavMeshLink>(true);
        foreach (NavMeshLink navMesh in navmeshLinks)
        {
            Debug.Log("delete Links");
            navMesh.enabled = false;
            navMesh.gameObject.SetActive(false);
        }
    }

    private void BuildNavmeshLinks(Room room)
    {
        NavMeshLink[] navmeshLinks = room.GetComponentsInChildren<NavMeshLink>(true);
        foreach (NavMeshLink navMesh in navmeshLinks)
        {
            Debug.Log("Found Navmesh");
            navMesh.enabled = true;
            navMesh.gameObject.SetActive(true);
        }
        
    }

    public void SpawnEnemies(Room room)
    {
        Debug.Log("In SpawnEnemies");
        var allTransforms = room.GetComponentsInChildren<Transform>(true);
    
        var zombPaths = allTransforms
            .Where(t => t.name.StartsWith("ZombNode"))
            .ToList();

        var turretPositions = allTransforms
            .Where(t => t.name.StartsWith("TurretNode"))
            .ToList();        
        
        var spikeyPositions = allTransforms
            .Where(t => t.name.StartsWith("SpikeyNode"))
            .ToList();

        var slimePositions = allTransforms
            .Where(t => t.name.StartsWith("SlimeNode"))
            .ToList();
        
        foreach (var zombPathRoot in zombPaths)
        {
            List<Transform> path = new List<Transform>();
            foreach (Transform child in zombPathRoot)
            {
                NavMeshHit hit;
                if (NavMesh.SamplePosition(child.position, out hit, 2.0f, NavMesh.AllAreas))
                {
                    child.position = hit.position;
                }

                path.Add(child);
            }

            GameObject zombObj = Instantiate(zombPrefab, path[0].position, Quaternion.identity);
            var zombNetworkObj = zombObj.GetComponent<NetworkObject>();
            if (zombNetworkObj != null) zombNetworkObj.Spawn();
            zombNetworkObj.transform.SetParent(room.transform);
            ZombZomb zomb = zombObj.GetComponent<ZombZomb>();
            NavMeshHit hitBis;
            if (NavMesh.SamplePosition(path[0].position, out hitBis, 2.0f, NavMesh.AllAreas)) path[0].position = hitBis.position;
            zombObj.GetComponent<NavMeshAgent>().Warp(path[0].position);
            zomb.InitializePath(path.OrderBy(t => t.name).ToList());
            
        }

        foreach (var turretNode in turretPositions)
        {
            GameObject turretObj = Instantiate(turretPrefab, turretNode.position, turretNode.rotation);
            var turretNetworkObj = turretObj.GetComponent<NetworkObject>();
            if (turretNetworkObj != null) turretNetworkObj.Spawn();
            turretNetworkObj.transform.SetParent(room.transform);
            TurretEnemy turret = turretObj.GetComponent<TurretEnemy>();
            NavMeshHit hit;
            if (NavMesh.SamplePosition(turretNode.position, out hit, 2.0f, NavMesh.AllAreas)) turretNode.position = hit.position;
            turretObj.GetComponent<NavMeshAgent>().Warp(turretNode.position);
            turret.isRaycastLaser = (turretNode.GetChild(0).name == "isLaser");

        }
        
        foreach (var spikeyNode in spikeyPositions)
        {
            GameObject spikeyObj = Instantiate(spikeyPrefab, spikeyNode.position, spikeyNode.rotation);
            var spikeyNetworkObj = spikeyObj.GetComponent<NetworkObject>();
            if (spikeyNetworkObj != null) spikeyNetworkObj.Spawn();
            spikeyNetworkObj.transform.SetParent(room.transform);
            SpikeyEnemy spikey = spikeyObj.GetComponent<SpikeyEnemy>();
            spikeyObj.GetComponent<NavMeshAgent>().Warp(spikeyNode.position);
            NavMeshHit hit;
            if (NavMesh.SamplePosition(spikeyNode.position, out hit, 2.0f, NavMesh.AllAreas)) spikeyNode.position = hit.position;
            spikey.Initialized(spikeyNode.GetChild(0));
        }
        
        foreach (var slimeNode in slimePositions)
        {
            NavMeshHit hit;
            if (NavMesh.SamplePosition(slimeNode.position, out hit, 2.0f, NavMesh.AllAreas)) slimeNode.position = hit.position;
            GameObject slimeObj = Instantiate(slimePrefab, slimeNode.position, slimeNode.rotation);
            var slimeNetworkObj = slimeObj.GetComponent<NetworkObject>();
            if (slimeNetworkObj != null) slimeNetworkObj.Spawn();
            slimeNetworkObj.transform.SetParent(room.transform);
            slimeObj.GetComponent<NavMeshAgent>().Warp(slimeNode.position);
            RottenSlime slime = slimeObj.GetComponent<RottenSlime>();
            slime.Initialized();
        }
    }

    NetworkObject GetRandomRoom(GameObject PreviousRoom)
    {
        Room roomScript = null;
        NetworkObject selectedRoomPrefab = null;

        float randomWeight = (float)(random.NextDouble() * totalWeight);
        float currentWeight = 0;
        for (int i = 0; i < roomPrefabs.roomPrefabs.Length; i++)
        {
            NetworkObject roomPrefab = roomPrefabs.roomPrefabs[i];
            Room tempScript = roomPrefab.GetComponent<Room>();

            currentWeight += tempScript.Weight;
            if (currentWeight >= randomWeight)
            {
                selectedRoomPrefab = roomPrefab;
                roomScript = tempScript;
                break;
            }
        }

        if (selectedRoomPrefab == null || roomScript == null)
        {
            Debug.LogWarning("[Generation de salles] Retry: Aucun prefab valide.");
            return GetRandomRoom(PreviousRoom);
        }

        if (alreadyGeneratedRooms.Contains(selectedRoomPrefab))
        {
            Debug.LogWarning("[Generation de salles] Retry: Salle déjà générée.");
            return GetRandomRoom(PreviousRoom);
        }

        string direction = roomScript.isTurningLeft ? "left" :
                        roomScript.isTurningRight ? "right" : null;
        bool isStairs = roomScript.isStairs;

        if ((direction != null && direction == LastRoomDirection) ||
            PreviousRoom.name == selectedRoomPrefab.name ||
            (isStairs && lastRoomIsStairs))
        {
            return GetRandomRoom(PreviousRoom);
        }

        NetworkObject roomInstance = Instantiate(selectedRoomPrefab);

        Transform entryPoint = roomInstance.transform.Find("Entry");
        Transform lastRoomExit = PreviousRoom.transform.Find("Exit");

        if (entryPoint == null || lastRoomExit == null)
        {
            Debug.LogError("[Generation de salles] Missing Entry or Exit transform.");
            Destroy(roomInstance.gameObject);
            return null;
        }

        float angleDifference = lastRoomExit.eulerAngles.y - entryPoint.eulerAngles.y;
        roomInstance.transform.Rotate(Vector3.up, angleDifference);

        entryPoint = roomInstance.transform.Find("Entry");

        Vector3 offset = entryPoint.position - roomInstance.transform.position;
        roomInstance.transform.position = lastRoomExit.position - offset;

        roomInstance.Spawn();

        alreadyGeneratedRooms.Add(selectedRoomPrefab);
        foreach (var door in roomInstance.GetComponentsInChildren<Door>(true))
        {
            var netObj = door.GetComponent<NetworkObject>();
            if (netObj != null && !netObj.IsSpawned)
            {
                netObj.Spawn();
            }
        }

        GeneratedRooms.Enqueue(roomInstance);

        if (direction != null)
            LastRoomDirection = direction;
        lastRoomIsStairs = isStairs;

        return roomInstance;
    }
    

 private void GenerateLastRoom(Room previousRoom) {
    if (!IsServer) return;
    if (GeneratedRooms.Count > 2) DeleteRoom();
    
    if (endRoom == null) {
        Debug.LogError("[Generation de salles] La salle de fin n'a pas été initialisée !");
        return;
    }
    
    NetworkObject roomInstance = Instantiate(endRoom.GetComponent<NetworkObject>());
    Room roomScript = roomInstance.GetComponent<Room>();
    
    roomScript.RoomID = previousRoom.RoomID + 1;
    var tmp = roomScript.GetComponentInChildren<TextMeshProUGUI>();
    if (tmp != null)
    {
        tmp.text = roomScript.RoomID < 9 ? "0" + (roomScript.RoomID + 1).ToString()
                                        : (roomScript.RoomID + 1).ToString();
    }
    
    Transform entryPoint = roomInstance.transform.Find("Entry");
    Transform lastRoomExit = previousRoom.transform.Find("Exit");

    if (entryPoint == null || lastRoomExit == null)
    {
        Debug.LogError("[Generation de salles] La salle de fin ne contient pas d'Entry ou la salle précédente ne contient pas d'Exit !");
        Destroy(roomInstance.gameObject);
        return;
    }

    float angleDifference = lastRoomExit.eulerAngles.y - entryPoint.eulerAngles.y;
    roomInstance.transform.Rotate(Vector3.up, angleDifference);

    entryPoint = roomInstance.transform.Find("Entry");
    
    Vector3 offset = entryPoint.position - roomInstance.transform.position;
    roomInstance.transform.position = lastRoomExit.position - offset;
    
    if (entryPoint) entryPoint.GetComponent<BoxCollider>().enabled = false;
    var exitPoint = roomInstance.transform.Find("Exit");
    if (exitPoint) exitPoint.GetComponent<BoxCollider>().enabled = false;
    
    roomInstance.Spawn();
    
    GeneratedRooms.Enqueue(roomInstance);
    
    Debug.Log($"[{(IsServer ? "SERVER" : "CLIENT")}] Génération de la dernière salle {roomScript.RoomID}, Position: {roomScript.transform.position}");
    
}
    void DeleteRoom() {
        if (GeneratedRooms.Count <= 1) {
            Debug.LogWarning("Aucune salle à retirer (hors salle de départ) !");
            return;
        }
        var room = GeneratedRooms.Dequeue();
        if (room == null) {
            Debug.LogWarning("Room null !");
            return;
        }
        if (!room.IsSpawned) {
            Debug.LogWarning($"Room {room.gameObject.name} n'est pas spawn, impossible de despawn.");
            return;
        }
        DeSpawnAll(room.GetComponent<Room>());
        room.Despawn(true);
        Debug.LogWarning($"Removed {room.gameObject.name}");
    }
}