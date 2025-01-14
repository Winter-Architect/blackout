using Unity.Netcode;
using UnityEngine;

public class Spectator : NetworkBehaviour
{
    public static Spectator Instance;

    private Camera currentCamera;
    private Camera player1Cam;

    public int followedPlayer = 0;


    private void Update()
    {
        if(!IsOwner){
            return;
        }
        if(followedPlayer == 0){
            player1Cam.gameObject.SetActive(true);
            currentCamera.gameObject.SetActive(false);
        }
        else{
            player1Cam.gameObject.SetActive(false);
            currentCamera.gameObject.SetActive(true);
        }
        if(Input.GetKeyDown(KeyCode.Mouse0)){
            followedPlayer = 1 - followedPlayer;
        }

    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (IsOwner) 
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject); 
            }
            else
            {
                Destroy(gameObject);
            }
            player1Cam = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<Camera>(true);
        }
    }

    public void SwitchCurrentCamera(Camera newCamera)
    {
        if (currentCamera != null)
        {
            currentCamera.gameObject.SetActive(false);
        }

        currentCamera = newCamera;
        currentCamera.gameObject.SetActive(true);

        Debug.Log($"Switched to camera: {newCamera.name}");
    }
}
