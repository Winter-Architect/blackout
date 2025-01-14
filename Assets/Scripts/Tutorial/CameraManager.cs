using UnityEngine;
using UnityEngine.EventSystems;

public class CameraManager : MonoBehaviour
{

    public static CameraManager Instance;

    void Awake()
    {
        if(Instance is null) {
            Instance = this;
        }
        else{
            Destroy(gameObject);
        }
    }


}
