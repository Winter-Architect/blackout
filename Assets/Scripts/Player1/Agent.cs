using System;
using System.Buffers.Text;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerInventory : MonoBehaviour
{
     // the current room's key
}

public class Agent : NetworkBehaviour, IInteractor
{

    private AgentInteractionHandler handler = new AgentInteractionHandler();

    private SphereCollider myCheckTrigger;
    [SerializeField] private float interactionRange;

    LinkedList<BaseInteractable> interactablesInRange = new LinkedList<BaseInteractable>();
    LinkedListNode<BaseInteractable> currentSelectedInteractable;
    [SerializeField] private float speed;
    [SerializeField] private float jumpForce = 20f;

    [SerializeField] private float currentSpeed = 0.5f;
    [SerializeField] private Transform groundCheck;

    private const float BASE_SPEED = 16f;
    private const float ACCELERATION = 1.5f;

    [SerializeField] private bool isAirborne = true;
    [SerializeField] private Transform playerCameraPivotTransform;
    [SerializeField] private Transform playerCamera;

    [SerializeField] private Transform playerBody;
    [SerializeField] private float xMouseSensitivity = 2f;
    [SerializeField] private float yMouseSensitivity = 2f;
    
    [SerializeField] private Animator animator;
    private Rigidbody playerRigidbody;
    private float xInput;
    private float yInput;
    private float xMouseInput;
    private float yMouseInput;

    private bool shiftPressed;

    private bool isMoving;

    private float xRotation = 0f;
    private float yRotation = 0f;
    
    //inventory's values
    
    public bool hasKey = false;
    
    public override void OnNetworkSpawn()
    {
        myCheckTrigger = gameObject.AddComponent<SphereCollider>();
        myCheckTrigger.isTrigger = true;
        myCheckTrigger.radius = interactionRange;

        if(!IsOwner)
        {
            playerCamera.gameObject.SetActive(false);
            return;
        }      
                                                                                                                                                                                                                                                                                        playerRigidbody = GetComponent<Rigidbody>();
        Cursor.lockState = CursorLockMode.Locked; 
        
    
    }

    void Update()
    {

        SwitchCurrentInteractable();
        if(!IsOwner){
            return;
        }
        if(currentSelectedInteractable is not null)
        {
            Debug.Log(currentSelectedInteractable.Value.gameObject.name);

        }
        //Get Input
        shiftPressed = Input.GetKey(KeyCode.LeftShift);
        xInput = Input.GetAxisRaw("Horizontal");
        yInput = Input.GetAxisRaw("Vertical");
        xMouseInput = Input.GetAxis("Mouse X");
        yMouseInput = -Input.GetAxis("Mouse Y");

        if(Input.GetKeyDown(KeyCode.Space)){
            Jump();
        }

        if(currentSelectedInteractable is not null)
        {
            if(currentSelectedInteractable.Value.gameObject.TryGetComponent<Outline>(out var outline))
            {
                outline.OutlineColor = Color.red;
            }            

            
            if(Input.GetKey(KeyCode.E))
            {
                InteractWith(currentSelectedInteractable.Value);
            }
        }


        CheckAirborne();

    }

    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.TryGetComponent<IInteractable>(out IInteractable myInteractable))
        {
            if(!interactablesInRange.Contains((BaseInteractable)myInteractable))
            {
                interactablesInRange.AddLast((BaseInteractable)myInteractable);
                currentSelectedInteractable = interactablesInRange.First;
            }
        }   
    }
    void OnTriggerExit(Collider other)
    {
        if(other.gameObject.TryGetComponent<IInteractable>(out IInteractable myInteractable))
        {
            if(interactablesInRange.Contains((BaseInteractable)myInteractable))
            {
                interactablesInRange.Remove((BaseInteractable)myInteractable);
            }
            if(interactablesInRange.Count == 0)
            {
                currentSelectedInteractable = null;
            }
        }   
    }

    private void SwitchCurrentInteractable()

    {
        if(Input.GetKeyDown(KeyCode.O))
        {
            currentSelectedInteractable.Value.gameObject.GetComponent<Outline>().OutlineColor = Color.white;
            currentSelectedInteractable = currentSelectedInteractable.Next ?? currentSelectedInteractable.List.First;
        }
        else if(Input.GetKeyDown(KeyCode.P))
        {
            currentSelectedInteractable.Value.gameObject.GetComponent<Outline>().OutlineColor = Color.white;
            currentSelectedInteractable = currentSelectedInteractable.Previous ?? currentSelectedInteractable.List.Last;
        }
    }

    void FixedUpdate()
    {
        if(!IsOwner){
            return;
        }
        isMoving = xInput != 0 || yInput != 0;
        Animate();
        ControlCamera();
        if(xInput != 0 || yInput != 0)
        {
            Move();
        }
        else
        {
            currentSpeed = 0.5f;
        }
    
        
    }

    void CheckAirborne()
    {
        RaycastHit hit;
        if(Physics.Raycast(groundCheck.position, Vector3.down, out hit, 0.07f * transform.localScale.y)){
            isAirborne = false;
        }
        else{
            isAirborne = true;
        }
    }

    void ControlCamera()
    {

        xRotation += yMouseInput * yMouseSensitivity;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        yRotation += xMouseInput * xMouseSensitivity;

        Quaternion verticalRotation = Quaternion.Euler(xRotation, yRotation, 0f);

        playerCameraPivotTransform.localRotation = verticalRotation;

    
    }


    void Move()
    {
        Vector3 myForward = new Vector3(playerCamera.forward.x, 0, playerCamera.forward.z).normalized;
        Vector3 myRight  = new Vector3(playerCamera.right.x, 0, playerCamera.right.z).normalized;

        if(shiftPressed){
            speed = BASE_SPEED * 1.5f;
        }
        else{
            speed = BASE_SPEED;
        }

        currentSpeed = Mathf.MoveTowards(currentSpeed, speed, ACCELERATION * Time.deltaTime);
       

        Vector3 myMovement = (myForward * yInput + myRight * xInput).normalized * currentSpeed;
        Vector3 myVelocity = new Vector3(myMovement.x, playerRigidbody.linearVelocity.y, myMovement.z);
        playerBody.LookAt(transform.position + myMovement);
        playerRigidbody.linearVelocity = myVelocity;
        
    }

    void Animate()
    {
        animator.SetBool("isMoving", isMoving);
        animator.SetBool("isRunning", shiftPressed);
    }

    void Jump()
    {
        if(!isAirborne)
        {
            playerRigidbody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            animator.SetTrigger("Jump");
            isAirborne = true;
        }
    }

    public bool CanInteract(IInteractable interactable)
    {
        return true;
    }

    public void InteractWith(IInteractable interactable)
    {
        if(CanInteract(interactable)){
            interactable.AcceptInteraction(handler);
        }
    }

    public class AgentInteractionHandler : IInteractionHandler
    {
        //Definir interactions de base avec differents types dInteractables, genre clic de bouton = animation
        public void InteractWith(BaseInteractable interactable)
        {
            Debug.Log("Clicked");
        }

        public void InteractWith(InteractableButton button)
        {
            Debug.Log("Clicked 22");
        }
    }
}
