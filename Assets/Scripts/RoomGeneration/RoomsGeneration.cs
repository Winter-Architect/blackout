using Unity.AI.Navigation;
using UnityEngine;
using TMPro;
using Unity.Netcode;
using System.Collections.Generic; // Ajoute ceci en haut

public class RoomsGeneration : NetworkBehaviour
{
    public Rooms roomPrefabs;
    public Room endRoom;
    public int numberOfRooms = 20; // Nombre total de salles à générer
    public GameObject DoorPrefab;
     private string LastRoomDirection = null;
     private bool lastRoomIsStairs = false;
     private float totalWeight = 0;
     
    private System.Random random = new System.Random(0);
    public NavMeshSurface navMeshSurface;
    private List<Room> generatedRooms = new List<Room>(); // Ajoute cette ligne

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
            Debug.Log($"[Generation de salles] Salle {i} : {room.name}");
            Room roomScript2 = room.GetComponent<Room>();
            totalWeight += roomScript2.Weight;
        }

        Room currRoom = GameObject.Find("StartingRoom").GetComponent<Room>();
        if (!currRoom) {
            Debug.LogError("[Generation de salles] La salle de départ n'a pas été initialisée (ou nom != StartingRoom) !");
            return;
        }

        generatedRooms.Clear();
        generatedRooms.Add(currRoom);

        // Génère seulement deux salles au début
        for (int i = 0; i < 2 && i < numberOfRooms; i++)
        {
            currRoom = GenerateRoom(currRoom, i);
            generatedRooms.Add(currRoom);
        }

        // Ne génère pas la salle de fin tout de suite

        BakeNavMesh();
    }

    // Appelle cette méthode pour générer la salle suivante
    public void GenerateNextRoom()
    {
        if (generatedRooms.Count - 1 >= numberOfRooms)
        {
            // Génère la salle de fin si toutes les salles sont faites
            if (endRoom != null)
            {
                Room currRoom = generatedRooms[generatedRooms.Count - 1];
                GameObject roomObject = Instantiate(endRoom.gameObject);
                NetworkObject roomNetworkObject = roomObject.GetComponent<NetworkObject>();
                if (roomNetworkObject != null && !roomNetworkObject.IsSpawned)
                {
                    roomNetworkObject.Spawn();
                }

                Transform entryPoint = roomObject.transform.Find("Entry");
                Transform lastRoomExit = currRoom.transform.Find("Exit");

                if (entryPoint == null || lastRoomExit == null)
                {
                    Debug.LogError("[Generation de salles] La salle de fin ne contient pas d'Entry ou la salle précédente ne contient pas d'Exit !");
                    return;
                }

                float angleDifference = lastRoomExit.eulerAngles.y - entryPoint.eulerAngles.y;
                roomObject.transform.Rotate(Vector3.up, angleDifference);

                Vector3 offset = entryPoint.position - roomObject.transform.position;
                roomObject.transform.position = lastRoomExit.position - offset;

                entryPoint.GetComponent<BoxCollider>().enabled = false;
            }
            else
            {
                Debug.LogWarning("[Generation de salles] La salle de fin n'a pas été initialisée !");
            }
            BakeNavMesh();
            return;
        }

        Room lastRoom = generatedRooms[generatedRooms.Count - 1];
        Room newRoom = GenerateRoom(lastRoom, generatedRooms.Count - 1);
        generatedRooms.Add(newRoom);
        BakeNavMesh();
    }

    Room GenerateRoom(Room PreviousRoom, int id)
    {
        NetworkObject roomNetworkObject = GetRandomRoom(PreviousRoom.gameObject);
        Room roomScript = roomNetworkObject.GetComponent<Room>();

        Transform entryPoint = roomScript.transform.Find("Entry");
        Transform exitPoint = roomScript.transform.Find("Exit");

        if (entryPoint == null || exitPoint == null)
        {
            Debug.LogError($"[Generation de salles] La salle {roomScript.name} ne contient pas d'Entry ou d'Exit !");
            return null;
        }

        Transform lastRoomExit = PreviousRoom.transform.Find("Exit");

        if (lastRoomExit != null)
        {
            float angleDifference = lastRoomExit.eulerAngles.y - entryPoint.eulerAngles.y;
            
            roomScript.transform.Rotate(Vector3.up, angleDifference);

            entryPoint = roomScript.transform.Find("Entry"); 

            Vector3 offset = entryPoint.position - roomScript.transform.position;
            roomScript.transform.position = lastRoomExit.position - offset;
        }

        entryPoint.GetComponent<BoxCollider>().enabled = false;
        exitPoint.GetComponent<BoxCollider>().enabled = false;

        roomScript.RoomID = id+1;

        TextMeshProUGUI tmp = roomScript.GetComponentInChildren<TextMeshProUGUI>();
        if (tmp != null)
        {
            tmp.text = roomScript.RoomID < 9 ? "0" + (roomScript.RoomID + 1).ToString() : (roomScript.RoomID + 1).ToString();

        } else
        {
            Debug.LogWarning("TextMeshProUGUI non trouvé dans les enfants de 'room'");
        }
        
        BakeNavMesh();

        if (roomScript.GetComponent<RoomTrigger>() == null)
        {
            roomScript.gameObject.AddComponent<RoomTrigger>();
        }

        return roomScript;
    }

    NetworkObject GetRandomRoom(GameObject PreviousRoom) {

        Room roomScript = null;
        NetworkObject selectedRoomPrefab = null;        

        float randomWeight = (float)(random.NextDouble() * totalWeight);
        float currentWeight = 0;
        for (int i = 0; i < roomPrefabs.roomPrefabs.Length; i++)
        {
            NetworkObject roomPrefab = roomPrefabs.roomPrefabs[i];
            Room tempScript = roomPrefab.GetComponent<Room>();

            currentWeight += tempScript.Weight;
             if (currentWeight >= randomWeight) {
                selectedRoomPrefab = roomPrefab;
                roomScript = tempScript;
                break;
            }
        }

        if (selectedRoomPrefab == null) {
            Debug.LogError("[Generation de salles] Aucune salle n'a été trouvée !");
            return GetRandomRoom(PreviousRoom);
        }

         if (roomScript == null)
        {
            Debug.LogError($"[Generation de salles] La salle {selectedRoomPrefab.name} ne contient pas de script Room !");
            return null;
        }
        
        string direction = roomScript.isTurningLeft ? "left" : roomScript.isTurningRight ? "right" : null;
        bool isStairs = roomScript.isStairs;
           
        if (
           direction != null && direction == LastRoomDirection ||
           PreviousRoom.name == selectedRoomPrefab.name ||
           isStairs && lastRoomIsStairs
        ) 
                return GetRandomRoom(PreviousRoom);

        if (selectedRoomPrefab == null)
        {
            Debug.LogError("[Generation de salles] Le prefab sélectionné est null !");
            return null;
        }

        NetworkObject room = Instantiate(selectedRoomPrefab);
        var instanceNetworkObject = room.GetComponent<NetworkObject>();
        if (instanceNetworkObject != null && !instanceNetworkObject.IsSpawned)
        {
            instanceNetworkObject.Spawn();
        } else Debug.LogWarning($"[Generation de salles] NetworkObject non trouvé ou déjà spawn dans la salle générée ! Nom : {room.name}");

        if (direction != null) {
            LastRoomDirection = direction;
        }
        lastRoomIsStairs = isStairs;
         return room;
    }
    
    void BakeNavMesh()
    {
        if (navMeshSurface is not null)
        {
            navMeshSurface.BuildNavMesh();
        }
    }
}