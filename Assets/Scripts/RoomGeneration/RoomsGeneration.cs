using Unity.AI.Navigation;
using UnityEngine;

public class RoomsGeneration : MonoBehaviour
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

    void Awake()
    {
        if (roomPrefabs.roomPrefabs.Length <= 1)
        {
            Debug.LogError("[Generation de salles] Probleme dans la liste : il n'y a pas assez de salles !!!!!");
            return;
        }

        for (int i = 0; i < roomPrefabs.roomPrefabs.Length; i++)
        {
            GameObject room = roomPrefabs.roomPrefabs[i];
            Room roomScript2 = room.GetComponent<Room>();
            totalWeight += roomScript2.Weight;
        }

        GameObject currRoom = GameObject.Find("StartingRoom");
        if (!currRoom) {
            Debug.LogError("[Generation de salles] La salle de départ n'a pas été initialisée (ou nom != StartingRoom) !");
            return;
        }
        for (int i = 0; i < numberOfRooms; i++)
        {
           currRoom = GenerateRoom(currRoom);
        }

        if (endRoom != null)
        {
            GameObject room = Instantiate(endRoom.gameObject);
            Transform entryPoint = room.transform.Find("Entry");
            Transform lastRoomExit = currRoom.transform.Find("Exit");

            if (entryPoint == null || lastRoomExit == null)
            {
                Debug.LogError("[Generation de salles] La salle de fin ne contient pas d'Entry ou la salle précédente ne contient pas d'Exit !");
                return;
            }

            float angleDifference = lastRoomExit.eulerAngles.y - entryPoint.eulerAngles.y;
            room.transform.Rotate(Vector3.up, angleDifference);

            Vector3 offset = entryPoint.position - room.transform.position;
            room.transform.position = lastRoomExit.position - offset;

            entryPoint.GetComponent<BoxCollider>().enabled = false;
        } else Debug.LogWarning("[Generation de salles] La salle de fin n'a pas été initialisée !");
        
        BakeNavMesh();
    }


     GameObject GenerateRoom(GameObject PreviousRoom)
    {

        GameObject room = GetRandomRoom(PreviousRoom);

        Transform entryPoint = room.transform.Find("Entry");
        Transform exitPoint = room.transform.Find("Exit");

        if (entryPoint == null || exitPoint == null)
        {
            Debug.LogError($"[Generation de salles] La salle {room.name} ne contient pas d'Entry ou d'Exit !");
            return null;
        }

        Transform lastRoomExit = PreviousRoom.transform.Find("Exit");

        // Si ce n'est pas la première salle, ajuster sa rotation et sa position
        if (lastRoomExit != null)
        {
            // Calculer l'angle entre l'Exit précédent et l'Entry de la nouvelle salle
            float angleDifference = lastRoomExit.eulerAngles.y - entryPoint.eulerAngles.y;
            
            // Appliquer la rotation pour aligner l'Entry avec l'Exit précédent
            room.transform.Rotate(Vector3.up, angleDifference);

            // Recalculer Entry après rotation
            entryPoint = room.transform.Find("Entry"); 

            // Déplacer la salle pour que son Entry coïncide avec l'Exit précédent
            Vector3 offset = entryPoint.position - room.transform.position;
            room.transform.position = lastRoomExit.position - offset;
        }

        entryPoint.GetComponent<BoxCollider>().enabled = false;
        exitPoint.GetComponent<BoxCollider>().enabled = false;

        //GenerateDoor(room);
        
        BakeNavMesh();

        return room;
    }

    void GenerateDoor(GameObject Room) {
        GameObject door = Instantiate(DoorPrefab);
        door.transform.position = Room.transform.Find("Entry").position;
        door.transform.rotation = Room.transform.Find("Entry").rotation;
    }


    GameObject GetRandomRoom(GameObject PreviousRoom) {

        Room roomScript = null;
        GameObject selectedRoomPrefab = null;
        //Instantiate(roomPrefabs.roomPrefabs[Random.Range(0, roomPrefabs.roomPrefabs.Length)]);
        

        float randomWeight = (float)(random.NextDouble() * totalWeight);
        float currentWeight = 0;
       for (int i = 0; i < roomPrefabs.roomPrefabs.Length; i++)
        {
            GameObject roomPrefab = roomPrefabs.roomPrefabs[i];
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


        GameObject room = Instantiate(selectedRoomPrefab);
        
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