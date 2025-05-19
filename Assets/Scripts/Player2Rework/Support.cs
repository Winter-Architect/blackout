using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

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
    private GameObject supportHUD;
    private CameraHUD supportHUDscript;
    
    private CursorLockMode cursorState;

    public GameObject GameOverScreenPrefab;
    private UIDocument GameOverScreen;
    public bool isGameOverScreenActive = false;
    public override void OnNetworkSpawn()
    {
        supportHUD = GameObject.Find("SupportHUD");
        if (supportHUD != null)
        {
            UIDocument ui = supportHUD.GetComponent<UIDocument>();
            supportHUDscript = supportHUD.gameObject.GetComponent<CameraHUD>();
            ui.enabled = IsOwner;
        }

        if (!IsOwner) return;

        //var foundControllables = FindObjectsByType<Controllable>(FindObjectsSortMode.None);
        //Controllables = new LinkedList<Controllable>(foundControllables);
        //current = Controllables.First;

        Destroy(GameObject.FindGameObjectWithTag("Inventory"));

        foundControllables = FindObjectsByType<Controllable>(FindObjectsSortMode.None);

        foundRooms = FindObjectsByType<Room>(FindObjectsSortMode.None);

        player1 = FindFirstObjectByType<Agent>();

        foreach (var room in foundRooms)
        {
            if (room.ContainsPlayer(player1))
            {
                currentRoom = room;
                break;
            }
        }

        Controllables = new LinkedList<Controllable>(currentRoom.GetControllablesWithin(foundControllables));
        current = Controllables.First;
        UnityEngine.Cursor.lockState = CursorLockMode.Locked;

        if (current != null)
        {
            SwitchCurrentOwnerOfObjectServerRpc(current.Value.gameObject.GetComponent<NetworkObject>());
        }

        cursorState = CursorLockMode.Locked;

        TutorialManager.Instance.StartTutorial("player2");


    }

    public void RecheckForRoom(){
        if(!IsOwner){
            return;
        }

        
        supportHUD.SetActive(IsOwner);
        foreach(var room in foundRooms)
        {
            if(room.ContainsPlayer(player1))
            {
                currentRoom = room;
                break;
            }
        }

        if (current == null) return;
        current.Value.StopControlling();
        Controllables = new LinkedList<Controllable>(currentRoom.GetControllablesWithin(foundControllables));
        current = Controllables.First;
        if (current == null) return;
        Debug.Log(current.Value.gameObject.name);
        SwitchCurrentOwnerOfObjectServerRpc(current.Value.gameObject.GetComponent<NetworkObject>());

    }
    private void Update()
    {
        // Si le NetworkManager n'est plus actif, on ne fait rien
        if (!IsOwner || NetworkManager.Singleton == null || !NetworkManager.Singleton.IsListening)
            return;

        if (player1 is null)
            return;

        if ((player1.isDead || player1.Health <= 0) && !isGameOverScreenActive)
        {
            supportHUD?.SetActive(false);
            Instantiate(GameOverScreenPrefab);
            isGameOverScreenActive = true;
            cursorState = CursorLockMode.None;
            GameOverScreen = GameOverScreenPrefab.GetComponent<UIDocument>();
            GameOverScreen.sortingOrder = 99999;
        }

        // Vérification de la validité de current et de son NetworkObject
        if (current != null && current.Value != null && current.Value.gameObject != null)
        {
            var netObj = current.Value.gameObject.GetComponent<NetworkObject>();
            if (netObj != null && netObj != null && netObj.gameObject != null)
            {
                SwitchCurrent();
                if (netObj.IsOwnedByServer)
                {
                    SwitchCurrentOwnerOfObjectServerRpc(netObj);
                }
                current.Value.Control();
            }
            else
            {
                current = null;
            }
        }
        else
        {
            if (current != null)
            {
                current = null;
            }
        }

        // Gestion du curseur
        if (supportHUDscript != null && supportHUDscript.isTermOpen)
        {
            cursorState = CursorLockMode.None;
        }
        else if (player1.isDead || player1.Health <= 0)
        {
            cursorState = CursorLockMode.None;
        }
        else
        {
            cursorState = CursorLockMode.Locked;
        }
        UnityEngine.Cursor.lockState = cursorState;
        UnityEngine.Cursor.visible = cursorState == CursorLockMode.None;
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

    private void OnEnable()
    {
        SceneManager.sceneUnloaded += OnSceneUnloaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneUnloaded -= OnSceneUnloaded;
    }

    private void OnSceneUnloaded(Scene scene)
    {
        // Désactive ce script pour éviter toute exécution après destruction des objets
        enabled = false;
    }
}
