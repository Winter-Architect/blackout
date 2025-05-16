using Blackout.Inventory;
using Unity.Netcode;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    [SerializeField] private GameObject dialogUI;

    private GameObject myDialogUI;

    public static TutorialManager Instance;

    private bool pressedW = false;
    private bool pressedS = false;
    private bool pressedA = false;
    private bool pressedD = false;
    private bool pressedSpaceBar = false;
    private bool startedMovingTutorial = false;
    private bool startedJumpingTutorial = false;
    private bool startedSprintTutorial = false;
    private bool startedSwitchTutorial = false;
    private bool pressedRightClick = false;
    private bool pressedLeftClick = false;
    private bool startedPickingObject = false;
    private bool startedEquip = false;
    private bool startedOpeningDoorBox = false;
    private bool startedOpeningDoor = false;
    private bool pressedSprint = false;

    private bool finishedMoving = false;
    private bool finishedJumping = false;
    private bool finishedPickingObject = false;
    private bool finishedEquip = false;
    private bool finisedOpeningdoorBox = false;
    private bool finisedOpeningdoor = false;
    private bool finishedSprint = false;
    private string player;

    private bool finishedSwappingControls = false;

    private int messageCounter = 0;
    private InventoryController inventoryController;
    private Agent agent;
    [SerializeField] private Drawer SwitchesBoxDoor;
    [SerializeField] private Door firstDoorToOpen;
    [SerializeField] private Terminal terminal;

    private bool waitingToDestroy = false;
    private float destroyTimer = 0f;

    void Awake()
    {
        if (Instance is null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        inventoryController = FindFirstObjectByType<InventoryController>();
        agent = FindFirstObjectByType<Agent>();
    }

    void Update()
    {
        if (agent == null) agent = FindFirstObjectByType<Agent>();
        if (startedMovingTutorial)
        {
            if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.Q))
            {
                pressedA = true;
            }
            if (Input.GetKeyDown(KeyCode.S))
            {
                pressedS = true;
            }
            if (Input.GetKeyDown(KeyCode.D))
            {
                pressedD = true;
            }
            if (Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.W))
            {
                pressedW = true;
            }
        }

        finishedMoving = pressedA && pressedD && pressedS && pressedW;

        if (startedJumpingTutorial)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                pressedSpaceBar = true;
            }
        }

        finishedJumping = pressedSpaceBar;

        if (startedSprintTutorial)
        {
            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                pressedSprint = true;
            }
        }

        finishedSprint = pressedSprint;

        if (startedPickingObject)
            if (inventoryController != null && inventoryController.inventory.Count > 0) finishedPickingObject = true;

        if (startedEquip)
        {
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                pressedLeftClick = true;
            }
            if (agent != null && agent.isItemEquipped) finishedEquip = true;
        }

        if (startedOpeningDoorBox)
            if (SwitchesBoxDoor.open) finisedOpeningdoorBox = true;

        if (startedOpeningDoor)
            finisedOpeningdoor = firstDoorToOpen.Condition;

        // SUPPORT

        if (startedSwitchTutorial)
        {
            if (Input.GetKeyDown(KeyCode.Mouse1))
            {
                pressedRightClick = true;
            }
        }

        finishedSwappingControls = pressedRightClick;

        // AUTOMATISATION DE LA PROGRESSION DU TUTORIEL
        if (myDialogUI)
        {
            DialogBox dialogBox = myDialogUI.GetComponent<DialogBox>();
            if (player == "player1")
            {
                switch (messageCounter)
                {
                    case 0: // Attente du clic pour commencer
                    {
                        if (Input.GetKeyDown(KeyCode.Mouse0))
                        {
                            dialogBox.DisplayNextText();
                            messageCounter++;
                            startedMovingTutorial = true;
                        }
                        break;
                    }
                    case 1:
                    {
                        if (finishedMoving)
                        {
                            dialogBox.DisplayNextText();
                            messageCounter++;
                            startedJumpingTutorial = true;
                        }
                        break;
                    }
                    case 2: // Après le jump, on attend le sprint
                    {
                        if (finishedJumping)
                        {
                            dialogBox.DisplayNextText();
                            messageCounter++;
                            startedSprintTutorial = true;
                        }
                        break;
                    }
                    case 3: // Sprint
                    {
                        if (finishedSprint)
                        {
                            dialogBox.DisplayNextText();
                            messageCounter++;
                            startedPickingObject = true;
                        }
                        break;
                    }
                    case 4:
                    {
                        if (finishedPickingObject)
                        {
                            dialogBox.DisplayNextText();
                            messageCounter++;
                            startedEquip = true;
                        }
                        break;
                    }
                    case 5:
                    {
                        if (finishedEquip && pressedLeftClick)
                        {
                            dialogBox.DisplayNextText();
                            messageCounter++;
                            startedOpeningDoorBox = true;
                        }
                        break;
                    }
                    case 6:
                    {
                        dialogBox.DisplayNextText();
                        messageCounter++;
                        break;
                    }
                    case 7:
                    {
                        if (finisedOpeningdoorBox)
                        {
                            dialogBox.DisplayNextText();
                            messageCounter++;
                            startedOpeningDoor = true;
                        }
                        break;
                    }
                    case 8:
                    {
                        if (finisedOpeningdoor)
                        {
                            dialogBox.DisplayNextText();
                            messageCounter++;
                            waitingToDestroy = true;
                            destroyTimer = 0f;
                        }
                        break;
                    }
                    case 9: // Attente 5 secondes puis suppression
                    {
                        PlayerPrefs.SetInt("TutorialDone_player1", 1);
                        if (waitingToDestroy)
                            {
                                destroyTimer += Time.deltaTime;
                                if (destroyTimer >= 5f)
                                {
                                    Destroy(myDialogUI);
                                    myDialogUI = null;
                                    waitingToDestroy = false;
                                }
                            }
                        break;
                    }
                    default:
                    {
                        break;
                    }
                }
            }
            if (player == "player2")
            {
                switch (messageCounter)
                {
                    case 0: // Attente du clic pour commencer
                    {
                        if (Input.GetKeyDown(KeyCode.Mouse0))
                        {
                            dialogBox.DisplayNextText();
                            messageCounter++;
                        }
                        break;
                    }
                    case 1: // Attente du clic pour continuer
                    {
                        if (Input.GetKeyDown(KeyCode.Mouse0))
                        {
                            dialogBox.DisplayNextText();
                            messageCounter++;
                        }
                        break;
                    }
                    case 2: // Changement de caméra (clic droit)
                    {
                        if (Input.GetKeyDown(KeyCode.Mouse1))
                        {
                            dialogBox.DisplayNextText();
                            messageCounter++;
                        }
                        break;
                    }
                    case 3: // Ouvrir le terminal (T)
                    {
                        if (Input.GetKeyDown(KeyCode.T))
                        {
                            dialogBox.DisplayNextText();
                            messageCounter++;
                        }
                        break;
                    }
                    case 4: // Envoyer la commande "help"
                    {
                        // À adapter selon ton système de terminal, exemple :
                        if (terminal.messageHistory.Contains("help"))
                        {
                            dialogBox.DisplayNextText();
                            messageCounter++;
                        }
                        break;
                    }
                    case 5: // Ouvrir la map (bouton ou "map")
                    {
                        // À adapter selon ton système de map, exemple :
                        if (terminal.isMapOpen)
                        {
                            dialogBox.DisplayNextText();
                            messageCounter++;
                        }
                        break;
                    }
                    case 6: // Fermer le terminal
                    {
                        // À adapter selon ton système, ici on suppose terminal.isOpen == false quand il est fermé
                        if (!terminal.isOpen)
                        {
                            dialogBox.DisplayNextText();
                            waitingToDestroy = true;
                            messageCounter++;
                        }
                        break;
                    }
                    case 7: // Dernier message, clic pour fermer le tuto
                    {
                        PlayerPrefs.SetInt("TutorialDone_player2", 1);
                        if (waitingToDestroy)
                        {
                            destroyTimer += Time.deltaTime;
                            if (destroyTimer >= 5f)
                            {
                                Destroy(myDialogUI);
                                myDialogUI = null;
                                waitingToDestroy = false;
                            }
                        }
                        break;
                    }
                    default:
                    {
                        break;
                    }
                }
            }
        }
    }

    public void FinshSwappingControls()
    {
        finishedSwappingControls = true;
    }
    public void FinishMoving()
    {
        finishedMoving = true;
    }
    public void FinishJumping()
    {
        finishedJumping = true;
    }

    public void StartTutorial(string player)
    {
        this.player = player;
        if (PlayerPrefs.GetInt($"TutorialDone_{player}", 0) == 1) return;
        if (player == "player1")
        {
            GameObject myDialogUI = Instantiate(dialogUI);
            DialogBox dialogBox = myDialogUI.GetComponent<DialogBox>();
            dialogBox.EnqueueMessage("Welcome to Blackout", "> click to continue"); // Pas d'action
            dialogBox.EnqueueMessage("Use Z, Q, S, D or Arrow Keys to move around", "> move");
            dialogBox.EnqueueMessage("Press SpaceBar to jump", "> jump");
            dialogBox.EnqueueMessage("Hold LeftShift to sprint", "> sprint"); // <-- Ajouté ici
            dialogBox.EnqueueMessage("Try to pick up an object by going near it and pressing \"E\"", "> pick up an object");
            dialogBox.EnqueueMessage("Try to equip it with \"1\" and activate it with a left click!", "> Equip an object\n> Activate it");
            dialogBox.EnqueueMessage("Now let's try to open that door!", "> click to continue");
            dialogBox.EnqueueMessage("See that gray box on the left of the door? Open it by pressing \"E\"!", "> Open the box!");
            dialogBox.EnqueueMessage("Now activate the top left switch! To navigate through the different switches use \"O\" or \"P\"! When you are selecting the correct switch, press \"E\"", "> Activate the top left switch!");
            dialogBox.EnqueueMessage("Great! You are now ready to face death! Good luck out there! And try not to die");
            dialogBox.DisplayNextText();
            this.myDialogUI = myDialogUI;
        }
        else if (player == "player2")
        {
            GameObject myDialogUI = Instantiate(dialogUI);
            DialogBox dialogBox = myDialogUI.GetComponent<DialogBox>();
            dialogBox.EnqueueMessage("Welcome to Blackout", "> Click to continue!");
            dialogBox.EnqueueMessage("You have successfully inflitrated the electronics system of the Site", "> Click to continue!");
            dialogBox.EnqueueMessage("Control other Controllables with RightClick!", "> switch caméra");
            dialogBox.EnqueueMessage("You've got access to the terminal of the facility! Press \"T\" to open it!", "> Open the terminal");
            dialogBox.EnqueueMessage("You have access to some commands! Send the command \"help\" to see all of them.", "> list all of the commands");
            dialogBox.EnqueueMessage("In some room, you have access to the map of the room with some details! Open the map with the button or by tapping (\"map\")", "> open the map");
            dialogBox.EnqueueMessage("You can now close the terminal by pressing the button or write \"exit\"", "> Close the terminal");
            dialogBox.EnqueueMessage("You have prooven your value! You are now ready to help our agent to complete this mission!");
            dialogBox.DisplayNextText();
            this.myDialogUI = myDialogUI;
        }
    }
}
