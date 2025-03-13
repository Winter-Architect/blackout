using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Collections;

public class DataController : MonoBehaviour
{

    private string URL = "http://nocteln.fr:4000/blackout/users";

    // UnityWebRequest request = UnityWebRequest.Get(URL);
    
    // function MakeRequest() {
    //     yield return request.SendWebRequest();
    
    // if (request.result == UnityWebRequest.Result.ConnectionError)
    // {
    //     Debug.Log("Error while sending request");
    // }
    // else
    // {
    //     Debug.Log("Received: " + request.downloadHandler.text);
    // }
    // }

    void Start() {
        StartCoroutine(GetData());
    }

    IEnumerator GetData(){
        using (UnityWebRequest req = UnityWebRequest.Get(URL))
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
