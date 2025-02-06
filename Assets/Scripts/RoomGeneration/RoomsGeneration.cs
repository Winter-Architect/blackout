using UnityEngine;

public class RoomsGeneration : MonoBehaviour
{
    
    public GameObject[] roomPrefabs; // Tes différentes salles
    public int numberOfRooms = 20; // Nombre total de salles à générer

     private Transform lastRoomExit; 
     private string LastRoomDirection = null;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameObject currRoom = GameObject.Find("StartingRoom");
        if (!currRoom) {
            Debug.LogError("La salle de départ n'a pas été initialisée (ou nom != StartingRoom) !");
            return;
        }
        for (int i = 0; i < numberOfRooms; i++)
        {
           currRoom = GenerateRoom(currRoom);
        }
    }


     GameObject GenerateRoom(GameObject PreviousRoom)
    {

        GameObject room = GetRandomRoom(PreviousRoom);

        Transform entryPoint = room.transform.Find("Entry");
        Transform exitPoint = room.transform.Find("Exit");

        if (entryPoint == null || exitPoint == null)
        {
            Debug.LogError("Une salle ne contient pas d'Entry ou d'Exit !");
            return null;
        }

        Room roomScript = room.GetComponent<Room>();
        if (roomScript == null)
        {
            Debug.LogError("La salle ne contient pas de script Room !");
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

        return room;
    }


    GameObject GetRandomRoom(GameObject PreviousRoom) {
        // Choisir une salle aléatoire
        GameObject room = Instantiate(roomPrefabs[Random.Range(0, roomPrefabs.Length)]);
        
        Room roomScript = room.GetComponent<Room>();
        
        string direction = roomScript.isTurningLeft ? "left" : roomScript.isTurningRight ? "right" : null;
        bool isStairs = roomScript.isStairs;

//PreviousRoom.name == room.name ||
        if (
           direction != null && direction == LastRoomDirection
        ) {
            Destroy(room);
            Debug.Log("DIRECTION WRONG" + direction);
            return GetRandomRoom(PreviousRoom);
        }

        Debug.Log("Room: " + room.name);
        if (direction != null) {
            LastRoomDirection = direction;
        }
         return room;
    }
}