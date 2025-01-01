using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class SupportController : NetworkBehaviour
{
    [SerializeField] private GameObject currentlyControlled;
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


    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if(!IsOwner){
            return;
        }
        Cursor.lockState = CursorLockMode.Locked;

        StartCoroutine(DelayedStart());

    }

    IEnumerator DelayedStart()
    {
        //Figure out how to avoid using this
        yield return new WaitForSeconds(0.5f);
            currentlyControlled = GameObject.FindGameObjectWithTag("Controllables");

            currentlyControlled.GetComponentInChildren<ControllableObject>().childCamera.gameObject.SetActive(true);
            var outline = AddOutlineToObjectOrGetOutline(currentlyControlled);
            outline.OutlineMode = Outline.Mode.OutlineAll;
            outline.OutlineColor = Color.yellow;
            outline.OutlineWidth = 5f;
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
