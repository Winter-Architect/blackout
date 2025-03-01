using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class Support : NetworkBehaviour
{
    private LinkedList<Controllable> _controllables = new LinkedList<Controllable>();

    private Controllable[] foundControllables;
    private Room[] foundRooms;

    private Room currentRoom;

    private Agent player1;

    public LinkedList<Controllable> Controllables
    {
        get => _controllables;
        set
        {
            _controllables = value;
            OnControllablesChanged?.Invoke();
        }
    }

    private LinkedListNode<Controllable> current;
    public event Action OnControllablesChanged;

    public override void OnNetworkSpawn()
    {
        if (!IsOwner) return;

        //var foundControllables = FindObjectsByType<Controllable>(FindObjectsSortMode.None);
        //Controllables = new LinkedList<Controllable>(foundControllables);
        //current = Controllables.First;

        Destroy(GameObject.FindGameObjectWithTag("Inventory"));

        foundControllables = FindObjectsByType<Controllable>(FindObjectsSortMode.None);

        foundRooms = FindObjectsByType<Room>(FindObjectsSortMode.None);

        player1 = FindFirstObjectByType<Agent>();

        foreach(var room in foundRooms)
        {
            if(room.ContainsPlayer(player1))
            {
                currentRoom = room;
                break;
            }
        }

        Controllables = new LinkedList<Controllable>(currentRoom.GetControllablesWithin(foundControllables));
        current = Controllables.First;
        Cursor.lockState = CursorLockMode.Locked;

        if (current != null)
        {
            SwitchCurrentOwnerOfObjectServerRpc(current.Value.gameObject.GetComponent<NetworkObject>());
        }
    }

    public void RecheckForRoom(){
        if(!IsOwner){
            return;
        }
        foreach(var room in foundRooms)
        {
            if(room.ContainsPlayer(player1))
            {
                currentRoom = room;
                break;
            }
        }

        current.Value.StopControlling();
        Controllables = new LinkedList<Controllable>(currentRoom.GetControllablesWithin(foundControllables));
        current = Controllables.First;
        Debug.Log(current.Value.gameObject.name);
        SwitchCurrentOwnerOfObjectServerRpc(current.Value.gameObject.GetComponent<NetworkObject>());

    }
    private void Update()
    {
        if (!IsOwner) return;

        if(player1 is null){
            return;
        }

        if (current != null)
        {
            SwitchCurrent();
            if(current.Value.gameObject.GetComponent<NetworkObject>().IsOwnedByServer){
                SwitchCurrentOwnerOfObjectServerRpc(current.Value.gameObject.GetComponent<NetworkObject>());
            }
            current.Value.Control();
        }



    }

    private void SwitchCurrent()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            if (current != null)
            {
                current.Value.StopControlling();
                SwitchCurrentOwnerOfObjectServerRpc(current.Value.gameObject.GetComponent<NetworkObject>());

                current = current.Next ?? current.List.First;

                if (current != null)
                {
                    SwitchCurrentOwnerOfObjectServerRpc(current.Value.gameObject.GetComponent<NetworkObject>());
                }
            }
        }
        else if (Input.GetKeyDown(KeyCode.J))
        {
            if (current != null)
            {
                current.Value.StopControlling();
                SwitchCurrentOwnerOfObjectServerRpc(current.Value.gameObject.GetComponent<NetworkObject>());

                current = current.Previous ?? current.List.Last;

                if (current != null)
                {
                    SwitchCurrentOwnerOfObjectServerRpc(current.Value.gameObject.GetComponent<NetworkObject>());
                }
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    void SwitchCurrentOwnerOfObjectServerRpc(NetworkObjectReference myObject)
    {
        if (myObject.TryGet(out NetworkObject networkObject))
        {
            if (networkObject.IsOwnedByServer)
            {
                networkObject.ChangeOwnership(OwnerClientId);
            }
            else
            {
                networkObject.RemoveOwnership();
            }
        }
    }
}
