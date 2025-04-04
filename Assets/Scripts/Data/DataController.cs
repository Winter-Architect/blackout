using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Collections;
using System.Text;

public class DataController : MonoBehaviour
{

    private string USER_URL = "https://api.nocteln.fr/blackout/users";
    private string userId = null;
    void Start() {
     userId = SystemInfo.deviceUniqueIdentifier;
        StartCoroutine(AddPlayerRequest(userId));
    }

    void OnApplicationQuit() {
        if (userId == null) return;
        StartCoroutine(RemovePlayerRequest(userId));
    }


    IEnumerator AddPlayerRequest(string playerId)
    {
        string jsonData = "{\"id\":\"" + playerId + "\"}"; // Création du JSON
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);

        using (UnityWebRequest req = new UnityWebRequest(USER_URL, "POST"))
        {
            req.uploadHandler = new UploadHandlerRaw(bodyRaw);
            req.downloadHandler = new DownloadHandlerBuffer();
            req.SetRequestHeader("Content-Type", "application/json"); // Indiquer que c'est du JSON

            yield return req.SendWebRequest();

            if (req.result == UnityWebRequest.Result.ConnectionError || req.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("Error: " + req.error);
            }
            else
            {
                // Debug.Log("Response: " + req.downloadHandler.text);
            }
        }
    }

    IEnumerator RemovePlayerRequest(string playerId)
    {
        string jsonData = "{\"id\":\"" + playerId + "\"}"; // Création du JSON
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);

        using (UnityWebRequest req = new UnityWebRequest(USER_URL, "DELETE"))
        {
            req.uploadHandler = new UploadHandlerRaw(bodyRaw);
            req.downloadHandler = new DownloadHandlerBuffer();
            req.SetRequestHeader("Content-Type", "application/json"); // Indiquer que c'est du JSON

            yield return req.SendWebRequest();

            if (req.result == UnityWebRequest.Result.ConnectionError || req.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("Error: " + req.error);
            }
            else
            {
                Debug.Log("Response: " + req.downloadHandler.text);
            }
        }
    }

    IEnumerator GetUsersData(){
        using (UnityWebRequest req = UnityWebRequest.Get(USER_URL))
        {
            yield return req.SendWebRequest();
            if (req.result == UnityWebRequest.Result.ConnectionError)
            {
                Debug.Log("Error while sending request" + req.error);
            }
            else
            {
                Debug.Log("Received: " + req.downloadHandler.text);
            }
        }
    }
}
