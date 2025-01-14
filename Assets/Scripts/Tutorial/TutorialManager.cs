using Unity.Netcode;
using UnityEngine;

public class TutorialManager : NetworkBehaviour
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


    private bool finishedMoving = false;
    private bool finishedJumping = false;
    private string player;

    private bool finishedSwappingControls = false;

    private int messageCounter  = 0;


    void Awake()
    {
        if(Instance is null){
            Instance = this;
        }
        else{
            Destroy(gameObject);
        }
    }

    void Update()
    {
        if(startedMovingTutorial)
        {
            if(Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.Q)){
                pressedA = true;
            }
            if(Input.GetKeyDown(KeyCode.S)){
                pressedS = true;
            }
            if(Input.GetKeyDown(KeyCode.D)){
                pressedD = true;
            }
            if(Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.W)){
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

        if(startedSwitchTutorial){
            if(Input.GetKeyDown(KeyCode.Mouse1)){
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
                        if(finishedJumping){
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
        if(player == "player1")
        {
            GameObject myDialogUI = Instantiate(dialogUI);
            DialogBox dialogBox = myDialogUI.GetComponent<DialogBox>();
            dialogBox.EnqueueMessage("Welcome to WinterArchitect");
            dialogBox.EnqueueMessage("Use W, A, S, D or Arrow Keys to move around");
            dialogBox.EnqueueMessage("Press SpaceBar to Jump");
            dialogBox.DisplayNextText();
            this.myDialogUI = myDialogUI;


        }
        else if(player == "player2")
        {
            GameObject myDialogUI = Instantiate(dialogUI);
            DialogBox dialogBox = myDialogUI.GetComponent<DialogBox>();
            dialogBox.EnqueueMessage("Welcome to WinterArchitect");
            dialogBox.EnqueueMessage("You have successfully inflitrated the electronics system of the Site");
            dialogBox.EnqueueMessage("Look at another camera and RightClick to control it and get a different perspective");
            dialogBox.DisplayNextText();
            this.myDialogUI = myDialogUI;
        }

    }





}
