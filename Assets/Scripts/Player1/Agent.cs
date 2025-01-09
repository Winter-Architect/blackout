using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class Agent : NetworkBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private Transform playerCameraPivotTransform;
    [SerializeField] private Transform playerCamera;

    [SerializeField] private Transform playerBody;
    [SerializeField] private float xMouseSensitivity = 2f;
    [SerializeField] private float yMouseSensitivity = 2f;
    private Rigidbody playerRigidbody;
    private float xInput;
    private float yInput;
    private float xMouseInput;
    private float yMouseInput;


    private float xRotation = 0f;
    private float yRotation = 0f;

    public override void OnNetworkSpawn()
    {
        if(!IsOwner)
        {
            return;
        }       
        playerRigidbody = GetComponent<Rigidbody>();
        Cursor.lockState = CursorLockMode.Locked;
    
    }

    void Update()
    {
        if(!IsOwner){
            return;
        }
        //Get Input
        xInput = Input.GetAxisRaw("Horizontal");
        yInput = Input.GetAxisRaw("Vertical");
        xMouseInput = Input.GetAxis("Mouse X");
        yMouseInput = -Input.GetAxis("Mouse Y");
    }

    void FixedUpdate()
    {
        if(!IsOwner){
            return;
        }
        ControlCamera();
        Move();
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

        Vector3 myMovement = (myForward * yInput + myRight * xInput).normalized * speed;
        Vector3 myVelocity = new Vector3(myMovement.x, playerRigidbody.linearVelocity.y, myMovement.z);
        playerBody.LookAt(transform.position + myMovement);
        playerRigidbody.linearVelocity = myVelocity;

    }



}
