using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class Support : NetworkBehaviour
{
    LinkedList<Controllable> controllables;
    LinkedListNode<Controllable> current;

    public override void OnNetworkSpawn()
    {

        if(!IsOwner)
        {
            return;
        }
        var foundControllables = FindObjectsByType<Controllable>(FindObjectsSortMode.None);
        controllables = new LinkedList<Controllable>(foundControllables);
        current = controllables.First;
        Cursor.lockState = CursorLockMode.Locked;
        SwitchCurrentOwnerOfObjectServerRpc(current.Value.gameObject.GetComponent<NetworkObject>());


        
    }

    private void Update()
    {
        if(!IsOwner)
        {
            return;
        }
        SwitchCurrent();
        current.Value.Control();
        

    }
    private void SwitchCurrent()

    {
        if(Input.GetKeyDown(KeyCode.K))
        {
            current.Value.StopControlling();
            SwitchCurrentOwnerOfObjectServerRpc(current.Value.gameObject.GetComponent<NetworkObject>());
            current = current.Next ?? current.List.First;
            SwitchCurrentOwnerOfObjectServerRpc(current.Value.gameObject.GetComponent<NetworkObject>());
        }
        else if(Input.GetKeyDown(KeyCode.J))
        {
            current.Value.StopControlling();
            SwitchCurrentOwnerOfObjectServerRpc(current.Value.gameObject.GetComponent<NetworkObject>());
            current = current.Previous ?? current.List.Last;
            SwitchCurrentOwnerOfObjectServerRpc(current.Value.gameObject.GetComponent<NetworkObject>());
        }
    }
    

    [ServerRpc (RequireOwnership = false)]
    void SwitchCurrentOwnerOfObjectServerRpc(NetworkObjectReference myObject)
    {
        if(myObject.TryGet(out NetworkObject networkObject)){
            if(networkObject.IsOwnedByServer){
                networkObject.ChangeOwnership(OwnerClientId);
            }
            else
            {
                networkObject.RemoveOwnership();
            }
        }
    }   

}                                                                 
