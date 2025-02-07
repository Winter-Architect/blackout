using UnityEngine;
using UnityEngine.WSA;

public class RoomsGeneration : MonoBehaviour
{
    
    //public GameObject[] roomPrefabs; // Tes différentes salles

    public Rooms roomPrefabs;
    public int numberOfRooms = 20; // Nombre total de salles à générer

     private Transform lastRoomExit; 
     private string LastRoomDirection = null;
     private bool lastRoomIsStairs = false;
     private float totalWeight = 0;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        for (int i = 0; i < roomPrefabs.roomPrefabs.Length; i++)
        {
            GameObject room = roomPrefabs.roomPrefabs[i];
            Room roomScript2 = room.GetComponent<Room>();
            totalWeight += roomScript2.Weight;
        }

        GameObject currRoom = GameObject.Find("StartingRoom");
        if (!currRoom) {
            Debug.LogError("La salle de départ n'a pas été initialisée (ou nom != StartingRoom) !");
            return;
        }
        for (int i = 0; i < numberOfRooms; i++)
        {
            Debug.Log("Room " + i);
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
        GameObject room = null;
        //Instantiate(roomPrefabs.roomPrefabs[Random.Range(0, roomPrefabs.roomPrefabs.Length)]);
        

        Debug.Log("1");

        float randomWeight = UnityEngine.Random.Range(0f, totalWeight);
        float currentWeight = 0;
       for (int i = 0; i < roomPrefabs.roomPrefabs.Length; i++)
        {
            room = Instantiate(roomPrefabs.roomPrefabs[i]);
            Room roomScript2 = room.GetComponent<Room>();
            currentWeight += roomScript2.Weight;
            Destroy(room);
            if (currentWeight >= randomWeight)
            {
                room = Instantiate(roomPrefabs.roomPrefabs[i]);
                break;
            }
        }

        Debug.Log("2");

        if (room == null) {
            Debug.LogError("Aucune salle n'a été trouvée !");
            return GetRandomRoom(PreviousRoom);
        }

        Debug.Log("3");
        
        Room roomScript = room.GetComponent<Room>();
        
        string direction = roomScript.isTurningLeft ? "left" : roomScript.isTurningRight ? "right" : null;
        bool isStairs = roomScript.isStairs;

        if (
           direction != null && direction == LastRoomDirection ||
           PreviousRoom.name == room.name ||
              isStairs &&  lastRoomIsStairs
        ) {
            Destroy(room);
            Debug.Log("DIRECTION WRONG" + direction);
            return GetRandomRoom(PreviousRoom);
        }

        Debug.Log("Room: " + room.name);
        if (direction != null) {
            LastRoomDirection = direction;
        }
        lastRoomIsStairs = isStairs;
         return room;
    }
}