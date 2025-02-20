using System.Buffers.Text;
using System.Collections;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
public class Agent : NetworkBehaviour, IInteractor
{
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
        if(!IsOwner)
        {
            playerCamera.gameObject.SetActive(false);
            return;
        }       
        playerRigidbody = GetComponent<Rigidbody>();
        Cursor.lockState = CursorLockMode.Locked;
        StartCoroutine("DelayedStart");
        
    
    }

    IEnumerator DelayedStart()
    {
        yield return new WaitForSeconds(0.5f);
        //TutorialManager.Instance.StartTutorial("player1");
    }

    void Update()
    {
        if(!IsOwner){
            return;
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

        if(Input.GetKeyDown(KeyCode.E)){
            RaycastHit hit;

            if(Physics.Raycast(playerCamera.position, playerCamera.forward, out hit, 3f) && hit.collider.gameObject.TryGetComponent<IInteractable>(out var myInteractable))
            {
                InteractWith(myInteractable);
            }
        }
        CheckAirborne();
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

    public bool canInteract(IInteractable interactable)
    {
        return true;
    }

    public void InteractWith(IInteractable interactable)
    {
        if(canInteract(interactable)){
            interactable.acceptInteraction(this);
        }
    }
}
