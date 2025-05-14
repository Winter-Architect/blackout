using Unity.AI.Navigation;
using UnityEngine;
using TMPro;
using Unity.Netcode;
using Unity.VisualScripting;
using Unity.Netcode.Components;
using System.Collections.Generic;

public class RoomsGeneration : NetworkBehaviour
{
    public Rooms roomPrefabs;
    public Room endRoom;
    public int numberOfRooms = 20; // Nombre total de salles à générer

    public GameObject DoorPrefab;
    [SerializeField] public Queue<NetworkObject> GeneratedRooms = new Queue<NetworkObject>() ;

     private string LastRoomDirection = null;
     private bool lastRoomIsStairs = false;
     private float totalWeight = 0;
     
    private System.Random random = new System.Random(0);
     public NavMeshSurface navMeshSurface;

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
        
        random = new System.Random((int)System.DateTime.Now.Ticks);
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

       // GeneratedRooms.Enqueue(currRoom.GetComponent<NetworkObject>());

        
        BakeNavMesh();
    }


    public void GenerateRoom(Room previousRoom, int id)
    {
        if (!IsServer) return;
        if (numberOfRooms <= 0) {
            GenerateLastRoom(previousRoom);
            return; 
        }
        if (GeneratedRooms.Count > 2) DeleteRoom();
        

        NetworkObject roomNetworkObject = GetRandomRoom(previousRoom.gameObject);
        if (roomNetworkObject == null) return;

        Room roomScript = roomNetworkObject.GetComponent<Room>();

        // Update Room ID and visual label
        roomScript.RoomID = id + 1;
        var tmp = roomScript.GetComponentInChildren<TextMeshProUGUI>();
        if (tmp != null)
        {
            tmp.text = roomScript.RoomID < 9 ? "0" + (roomScript.RoomID + 1).ToString()
                                            : (roomScript.RoomID + 1).ToString();
        }

        // Disable colliders on entry/exit
        var entry = roomScript.transform.Find("Entry");
        var exit = roomScript.transform.Find("Exit");
        if (entry) entry.GetComponent<BoxCollider>().enabled = false;
        if (exit) exit.GetComponent<BoxCollider>().enabled = false;

        numberOfRooms--;

        Debug.Log($"[{(IsServer ? "SERVER" : "CLIENT")}] Génération de la salle {roomScript.RoomID}, Position: {roomScript.transform.position}");

        BakeNavMesh();
    }


    NetworkObject GetRandomRoom(GameObject PreviousRoom)
    {
        Room roomScript = null;
        NetworkObject selectedRoomPrefab = null;

        // Pick random weighted prefab
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

        // Retry if selection failed or not suitable
        if (selectedRoomPrefab == null || roomScript == null)
        {
            Debug.LogWarning("[Generation de salles] Retry: Aucun prefab valide.");
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

        // ✅ Instantiate but don't spawn yet
        NetworkObject roomInstance = Instantiate(selectedRoomPrefab);

        // ✅ Get Entry/Exit and position/rotate before Spawn()
        Transform entryPoint = roomInstance.transform.Find("Entry");
        Transform lastRoomExit = PreviousRoom.transform.Find("Exit");

        if (entryPoint == null || lastRoomExit == null)
        {
            Debug.LogError("[Generation de salles] Missing Entry or Exit transform.");
            Destroy(roomInstance.gameObject);
            return null;
        }

        // Apply rotation to match Exit-Entry alignment
        float angleDifference = lastRoomExit.eulerAngles.y - entryPoint.eulerAngles.y;
        roomInstance.transform.Rotate(Vector3.up, angleDifference);

        // Update entryPoint reference (rotation changed)
        entryPoint = roomInstance.transform.Find("Entry");

        // Align position
        Vector3 offset = entryPoint.position - roomInstance.transform.position;
        roomInstance.transform.position = lastRoomExit.position - offset;

        // ✅ Only now — spawn the networked object
        roomInstance.Spawn();

        
        GeneratedRooms.Enqueue(roomInstance);

        // Sync state
        if (direction != null)
            LastRoomDirection = direction;
        lastRoomIsStairs = isStairs;

        return roomInstance;
    }
    
    void BakeNavMesh()
    {
        if (navMeshSurface is not null)
        {
            navMeshSurface.BuildNavMesh();
        }
    }

 private void GenerateLastRoom(Room previousRoom) {
    if (!IsServer) return;
    if (GeneratedRooms.Count > 2) DeleteRoom();
    
    if (endRoom == null) {
        Debug.LogError("[Generation de salles] La salle de fin n'a pas été initialisée !");
        return;
    }
    
    // Instantier mais ne pas spawner encore
    NetworkObject roomInstance = Instantiate(endRoom.GetComponent<NetworkObject>());
    Room roomScript = roomInstance.GetComponent<Room>();
    
    // Mettre à jour l'ID de la salle et le label visuel
    roomScript.RoomID = previousRoom.RoomID + 1;
    var tmp = roomScript.GetComponentInChildren<TextMeshProUGUI>();
    if (tmp != null)
    {
        tmp.text = roomScript.RoomID < 9 ? "0" + (roomScript.RoomID + 1).ToString()
                                        : (roomScript.RoomID + 1).ToString();
    }
    
    // Récupérer les points d'entrée/sortie et positionner/tourner avant Spawn()
    Transform entryPoint = roomInstance.transform.Find("Entry");
    Transform lastRoomExit = previousRoom.transform.Find("Exit");

    if (entryPoint == null || lastRoomExit == null)
    {
        Debug.LogError("[Generation de salles] La salle de fin ne contient pas d'Entry ou la salle précédente ne contient pas d'Exit !");
        Destroy(roomInstance.gameObject);
        return;
    }

    // Appliquer la rotation pour aligner l'entrée avec la sortie précédente
    float angleDifference = lastRoomExit.eulerAngles.y - entryPoint.eulerAngles.y;
    roomInstance.transform.Rotate(Vector3.up, angleDifference);

    // Mettre à jour la référence entryPoint (la rotation a changé)
    entryPoint = roomInstance.transform.Find("Entry");
    
    // Aligner la position
    Vector3 offset = entryPoint.position - roomInstance.transform.position;
    roomInstance.transform.position = lastRoomExit.position - offset;
    
    // Désactiver les colliders sur l'entrée/sortie
    if (entryPoint) entryPoint.GetComponent<BoxCollider>().enabled = false;
    var exitPoint = roomInstance.transform.Find("Exit");
    if (exitPoint) exitPoint.GetComponent<BoxCollider>().enabled = false;
    
    // Spawn APRÈS positionnement
    roomInstance.Spawn();
    
    // Ajouter à la file des salles générées
    GeneratedRooms.Enqueue(roomInstance);
    
    Debug.Log($"[{(IsServer ? "SERVER" : "CLIENT")}] Génération de la dernière salle {roomScript.RoomID}, Position: {roomScript.transform.position}");
    
    // Mettre à jour le NavMesh
    BakeNavMesh();
}
    void DeleteRoom() {
        if (GeneratedRooms.Count <= 1) { // On garde toujours la salle de départ
            Debug.LogWarning("Aucune salle à retirer (hors salle de départ) !");
            return;
        }
        // On saute la salle de départ
        var room = GeneratedRooms.Dequeue();
        if (room == null) {
            Debug.LogWarning("Room null !");
            return;
        }
        if (!room.IsSpawned) {
            Debug.LogWarning($"Room {room.gameObject.name} n'est pas spawn, impossible de despawn.");
            return;
        }
        room.Despawn(true);
        Debug.LogWarning($"Removed {room.gameObject.name}");
    }
}
