using System;
using System.Collections.Generic;
using System.Linq;
using Blackout.Inventory;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class Support : NetworkBehaviour
{
    private LinkedList<Controllable> _controllables = new LinkedList<Controllable>();

    private Controllable[] foundControllables;
    private Room[] foundRooms;

    public Room currentRoom;

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
    private bool alreadystartedtuto = false;
    
    
    [SerializeField] private UIDocument loadingUIGameObject;
    private UIDocument loadingUI;
     private Label LoadingText;

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
        if (currentRoom == null)
        {
            Debug.Log("No room found for the player.");
            return;
        }
        Controllables = new LinkedList<Controllable>(currentRoom.GetControllablesWithin(foundControllables));
        current = Controllables.First;
        UnityEngine.Cursor.lockState = CursorLockMode.Locked;

        if (current != null)
        {
            SwitchCurrentOwnerOfObjectServerRpc(current.Value.gameObject.GetComponent<NetworkObject>());
        }

        cursorState = CursorLockMode.Locked;




    }

    public void UpdateFoundRooms()
    {
        foundRooms = FindObjectsByType<Room>(FindObjectsSortMode.None);
    }

    private System.Collections.IEnumerator ChangeTextAfterDelay(string text)
    {
        yield return new WaitForSeconds(3f); // 3 secondes
        LoadingText.text = text;
    }


    public void RecheckForRoom()
    {
        if (!IsOwner)
        {
            return;
        }

        // Debug.LogWarning("Rechecking for room");

        supportHUD.SetActive(IsOwner);

        foundRooms = FindObjectsByType<Room>(FindObjectsSortMode.None);


        // Debug.LogWarning($"Found {foundRooms.Length} rooms");

        foreach (var room in foundRooms)
        {
            if (room.ContainsPlayer(player1))
            {
                
                currentRoom = room;
                break;
            }
        }

      
        if (currentRoom == null)
        {
            // Debug.Log("No room found for the player.");
            return;
        }
        Controllables = new LinkedList<Controllable>(currentRoom.GetControllablesWithin(foundControllables));
        // Debug.LogWarning(foundControllables.Length + " controllables found");
        // Debug.LogWarning("controllable count " + Controllables?.Count);
        if (Controllables.Count > 0) current = Controllables.First;
        else {
            // Debug.LogWarning("No controllables found in the room");
            current = null;}
        if (current == null) return;
        current.Value.StopControlling();
        // Debug.Log(current.Value.gameObject.name);
        SwitchCurrentOwnerOfObjectServerRpc(current.Value.gameObject.GetComponent<NetworkObject>());

    }
    private void Update()
    {
        
        if (!IsOwner || NetworkManager.Singleton == null || !NetworkManager.Singleton.IsListening)
            return;

        if (player1 is null)
            return;

        var keyPads = GameObject.FindGameObjectsWithTag("KeyPad");
        foreach (var keyPad in keyPads)
        {
            var uiDocument = keyPad.GetComponent<UIDocument>();
            if (uiDocument != null)
            {
                Destroy(uiDocument);
            }
        }

        if (currentRoom == null)
        {
            RecheckForRoom();
            if (loadingUI == null)
            {
                loadingUI = Instantiate(loadingUIGameObject);
            }
            loadingUI.enabled = true;
            if (LoadingText == null) LoadingText = loadingUI.rootVisualElement.Q<Label>("Text");
            StartCoroutine(ChangeTextAfterDelay("Waiting for the agent to entre the facility..."));
            StartCoroutine(ChangeTextAfterDelay("Hacking in progress..."));
        }
        else
        {
            if (loadingUI != null)
                Destroy(loadingUI);
            if (!alreadystartedtuto)
                TutorialManager.Instance.StartTutorial("player2");
            alreadystartedtuto = true;
        }

        foundControllables = FindObjectsByType<Controllable>(FindObjectsSortMode.None);
        if ((player1.isDead.Value || player1.Health <= 0) && !isGameOverScreenActive)
        {
            PlayerPrefs.SetInt("CurrentRoomID", currentRoom.RoomID);
            supportHUD?.SetActive(false);
            var gameOverScreenInstance = Instantiate(GameOverScreenPrefab);
            isGameOverScreenActive = true;
            cursorState = CursorLockMode.None;
            UnityEngine.Cursor.lockState = CursorLockMode.None;
            UnityEngine.Cursor.visible = true;
            GameOverScreen = gameOverScreenInstance.GetComponent<UIDocument>();
            GameOverScreen.sortingOrder = 99999;
        }

        if (player1.isGameWon.Value && !isGameOverScreenActive)
        {
            PlayerPrefs.SetInt("CurrentRoomID", currentRoom.RoomID);
            supportHUD?.SetActive(false);
            var instantiatedGameOverScreen = Instantiate(GameOverScreenPrefab);
            isGameOverScreenActive = true;
            cursorState = CursorLockMode.None;
            UnityEngine.Cursor.lockState = CursorLockMode.None;
            UnityEngine.Cursor.visible = true;
            GameOverScreen = instantiatedGameOverScreen.GetComponent<UIDocument>();
           // GameOverScreen.rootVisualElement.Q<Label>("Score").text = "";
            GameOverScreen.rootVisualElement.Q<Label>("Text").text = "Mission Complete!";
            GameOverScreen.sortingOrder = 99999;
        }

        if (current != null && current.Value != null && current.Value.gameObject != null)
        {
            var netObj = current.Value.gameObject.GetComponent<NetworkObject>();
            if (netObj != null && netObj.gameObject != null)
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

        if (supportHUDscript != null && supportHUDscript.isTermOpen)
        {
            cursorState = CursorLockMode.None;
        }
        else if (player1.isDead.Value || player1.Health <= 0 || player1.isGameWon.Value)

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
        enabled = false;
    }
}
