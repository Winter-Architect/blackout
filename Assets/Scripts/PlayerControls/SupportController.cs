using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UIElements;
using Cursor = UnityEngine.Cursor;

public class SupportController : NetworkBehaviour
{
    [SerializeField] private GameObject currentlyControlled;
    [SerializeField] private UIDocument uiDocument;
    public GameObject CURRENTCAMERA;
    private bool IsControllableCamera(ControllableObject myObject)
    {
        return myObject.Id == "Camera";
    }

    private Outline AddOutlineToObjectOrGetOutline(GameObject objectToOutline)
    {
        if(objectToOutline.TryGetComponent<Outline>(out Outline outline)){
            return outline;
        }
        else{
            var line = objectToOutline.AddComponent<Outline>();
            return line;
        }
        
    }

    private void DestroyOutlineOnObject(GameObject myObject)
    {
        if(myObject.TryGetComponent<Outline>(out Outline outline)){
            Destroy(outline);
        }
        

    }

    public void SwitchCurrentlyControlled(GameObject objectToControl)
    {
        
        if(IsControllableCamera(currentlyControlled.GetComponent<ControllableObject>()) && IsControllableCamera(objectToControl.GetComponent<ControllableObject>()))
        {
            currentlyControlled.GetComponentInChildren<ControllableObject>().childCamera.gameObject.SetActive(false);
            DestroyOutlineOnObject(currentlyControlled);
            objectToControl.GetComponentInChildren<ControllableObject>().childCamera.gameObject.SetActive(true);
        }
        currentlyControlled = objectToControl;

        GiveCurrentCameraToSpectatorServerRpc(currentlyControlled.GetComponent<NetworkObject>(), 2);
        
        


    }

    public override void OnNetworkSpawn()
    {    

       
    

        base.OnNetworkSpawn();
        if(!IsOwner){
            return;
        }
        Cursor.lockState = CursorLockMode.Locked;
        uiDocument.enabled = true;
        StartCoroutine(DelayedStart());

    }

    IEnumerator DelayedStart()
    {
        //Figure out how to avoid using this
        yield return new WaitForSeconds(0.5f);
            TutorialManager.Instance.StartTutorial("player2");
            currentlyControlled = GameObject.FindGameObjectWithTag("FIRSTCAM");

            

            currentlyControlled.GetComponentInChildren<ControllableObject>().childCamera.gameObject.SetActive(true);
            var outline = AddOutlineToObjectOrGetOutline(currentlyControlled);
            outline.OutlineMode = Outline.Mode.OutlineAll;
            outline.OutlineColor = Color.yellow;
            outline.OutlineWidth = 5f;

            GiveCurrentCameraToSpectatorServerRpc(currentlyControlled.GetComponent<NetworkObject>(), 2);
    }

    void Update()
    {
        if(!IsOwner){
            return;
        }
        ControlCurrentObject();
        
    }

    [ServerRpc (RequireOwnership = false)]
    void SwitchCurrentOwnerOfObjectServerRpc(NetworkObjectReference myObject)
    {
        if(myObject.TryGet(out NetworkObject networkObject)){
            //IF DOESNT WORK REMOVE CONDITION
            if(networkObject.IsOwnedByServer){
                networkObject.ChangeOwnership(OwnerClientId);
            }
            
        }

    }

    [ServerRpc (RequireOwnership = false)]

    void GiveCurrentCameraToSpectatorServerRpc(NetworkObjectReference myCamera, ulong spectatorClientId)
    {

        Debug.Log("SERVERRPCSSSSSS");

        GetCurrentCameraToSpectatorClientRpc(myCamera, new ClientRpcParams
    {
        Send = new ClientRpcSendParams
        {
            TargetClientIds = new[] { spectatorClientId }
        }
    });
        
    }

    [ClientRpc]
    void GetCurrentCameraToSpectatorClientRpc(NetworkObjectReference myCamera, ClientRpcParams clientRpcParams = default){
        Debug.Log("CLIENTRPC");
        Debug.Log(OwnerClientId);
        Debug.Log(OwnerClientId);
        myCamera.TryGet(out NetworkObject networkObject);
        if(Spectator.Instance is not null){
            Spectator.Instance.SwitchCurrentCamera(networkObject.gameObject.transform.GetChild(1).gameObject.GetComponentInChildren<Camera>(true));
            
        }
        
        
    }


    private void ControlCurrentObject()
    {
        if(!currentlyControlled){
            return;
        }
        SwitchCurrentOwnerOfObjectServerRpc(currentlyControlled.GetComponent<NetworkObject>());
        switch (currentlyControlled.GetComponent<ControllableObject>().Id){
            case "Camera" :
                ControllableCamera controllableCamera = currentlyControlled.GetComponent<ControllableCamera>();
                controllableCamera.Control();
                CURRENTCAMERA = controllableCamera.myCameraTransform.gameObject;
                if(controllableCamera.toControl)
                {
                    SwitchCurrentlyControlled(controllableCamera.toControl);
                    controllableCamera.toControl = null;
                    

                }
                break;

            default :
                break;
        }

    }
}
