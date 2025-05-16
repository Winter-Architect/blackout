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
    private bool startedSwitchTutorial = false;
    private bool pressedRightClick = false;
    private bool startedPickingObject = false;
    private bool startedEquip = false;
    private bool startedOpeningDoorBox = false;
    private bool startedOpeningDoor = false;


    private bool finishedMoving = false;
    private bool finishedJumping = false;
    private bool finishedPickingObject = false;
    private bool finishedEquip = false;
    private bool finisedOpeningdoorBox = false;
    private bool finisedOpeningdoor = false;
    private string player;

    private bool finishedSwappingControls = false;

    private int messageCounter  = 0;
    private InventoryController inventoryController;
    private Agent agent;
    [SerializeField] private Drawer SwitchesBoxDoor;
    [SerializeField] private switchesManager switchesManager;


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

        if(startedJumpingTutorial){
            if(Input.GetKeyDown(KeyCode.Space)){
                pressedSpaceBar = true;
            }
        }

        finishedJumping = pressedSpaceBar;

        if (startedPickingObject)
            if (inventoryController != null && inventoryController.inventory.Count > 0) finishedPickingObject = true;
        

        if (startedEquip)
            if (agent != null && agent.isItemEquipped) finishedEquip = true;

        if (startedOpeningDoorBox)
            if (SwitchesBoxDoor.open) finisedOpeningdoorBox = true;

        if (startedOpeningDoor)
            if (switchesManager.DoorCondition) finisedOpeningdoor = true;

        // SUPPORT

            if (startedSwitchTutorial)
            {
                if (Input.GetKeyDown(KeyCode.Mouse1))
                {
                    pressedRightClick = true;
                }
            }

        finishedSwappingControls = pressedRightClick;


        if(Input.GetKeyDown(KeyCode.Mouse0) && myDialogUI)
        {
            DialogBox dialogBox = myDialogUI.GetComponent<DialogBox>();
            if(player == "player1")
            {
                switch(messageCounter)
                {
                    case 0 :
                    {
                        dialogBox.DisplayNextText();
                        messageCounter ++;
                        startedMovingTutorial = true;

                        break;
                    }
                    case 1 :
                    {
                        if(finishedMoving){
                            dialogBox.DisplayNextText();
                            messageCounter++;
                            startedJumpingTutorial = true;
                        }

                        
                        break;
                    }
                    case 2 :
                    {
                            if (finishedJumping)
                            {
                                dialogBox.DisplayNextText();
                                messageCounter++;
                                startedPickingObject = true;
                        }
                        break;
                    }
                    case 3 :
                        {
                            if (finishedPickingObject)
                            {
                                dialogBox.DisplayNextText();
                                messageCounter++;
                                startedEquip = true;
                            }
                            break;
                    }
                    case 4:
                        {
                            if (finishedEquip)
                            {
                                dialogBox.DisplayNextText();
                                messageCounter++;
                                startedOpeningDoorBox = true;
                            }
                            break;
                    }
                    case 5 :
                        {
                            dialogBox.DisplayNextText();
                            messageCounter++;
                            break;
                    }
                    case 6:
                        {
                            if (finisedOpeningdoorBox)
                            {
                                dialogBox.DisplayNextText();
                                messageCounter++;
                                startedOpeningDoor = true;
                            }
                            break;
                    }
                    case 7 :
                    {
                            if (finisedOpeningdoor)
                            {
                                dialogBox.DisplayNextText();
                                messageCounter++;
                                // startedOpeningDoor = true;
                                PlayerPrefs.SetInt("TutorialDone_player1", 1);
                            }
                            break;
                    }
                    default:
                    {
                        dialogBox.DisplayNextText();
                        break;
                    }
                }
            }
            if(player == "player2")
            {
                switch(messageCounter)
                {
                    case 0 :
                    {
                        dialogBox.DisplayNextText();
                        messageCounter ++;

                        break;
                    }
                    case 1 :
                    {
                        
                        dialogBox.DisplayNextText();
                        startedSwitchTutorial = true;
                        messageCounter++;
                        
                        break;
                    }
                    case 2 :
                    {
                        if(finishedSwappingControls){
                            dialogBox.DisplayNextText();
                            messageCounter++;
                        }
                        break;
                    }
                    default:
                    {
                        dialogBox.DisplayNextText();
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
            dialogBox.EnqueueMessage("Use W, A, S, D or Arrow Keys to move around", "> move\n> click to continue");
            dialogBox.EnqueueMessage("Press SpaceBar to jump", "> jump\n> click to continue");
            dialogBox.EnqueueMessage("Try to pick up an object by going near it and pressing \"E\"", "> pick up an object\n> click to continue");
            dialogBox.EnqueueMessage("Try to equip it with \"1\" and activate it with a left click!", "> Equip an object\n> Activate it\n> click to continue");
            dialogBox.EnqueueMessage("Now let's try to open that door!", "> click to continue");
            dialogBox.EnqueueMessage("See that gray box on the left of the door? Open it by pressing \"E\"!", "> Open the box!\n> Click to continue!");
            dialogBox.EnqueueMessage("Now activate the top left switch! To navigate through the different switches use \"O\" or \"P\"! When you are selecting the correct switch, press \"E\"", "> Activate the top left switch!\n> Click to continue!");
            dialogBox.EnqueueMessage("Great! You are now ready to face death! Good luck out there! And try not to die");
            dialogBox.DisplayNextText();
            this.myDialogUI = myDialogUI;
        }
        else if (player == "player2")
        {
            GameObject myDialogUI = Instantiate(dialogUI);
            DialogBox dialogBox = myDialogUI.GetComponent<DialogBox>();
            dialogBox.EnqueueMessage("Welcome to Blackout", "");
            dialogBox.EnqueueMessage("You have successfully inflitrated the electronics system of the Site", "");
            dialogBox.EnqueueMessage("Look at another camera and RightClick to control it and get a different perspective", "switch");
            dialogBox.DisplayNextText();
            this.myDialogUI = myDialogUI;
        }
    }





}
