using UnityEngine;

public class ControllableDrone : Controllable
{
    private float xRotation;
    private float yRotation;
    [SerializeField] private Transform pivotPoint;
    [SerializeField] private Transform bodyPivot;
    [SerializeField] private GameObject myCamera;
    private Rigidbody myRigidBody;
    [SerializeField] private float speed;
    [SerializeField] private float rotationSpeed;



    private void Awake()
    {
        myRigidBody = GetComponent<Rigidbody>();
    }
    public override void Control()
    {
        SetCamera(true);
        Move();
        ControlCamera();
    }

    public void Move()
    {
        float xInput = Input.GetAxisRaw("Horizontal");
        float yInput = Input.GetAxisRaw("Vertical");

        bodyPivot.Rotate(0, xInput * Time.fixedDeltaTime * rotationSpeed, 0);
        myRigidBody.linearVelocity = bodyPivot.forward*yInput*speed;
    }

    public void ControlCamera()
    {
        float mouseInputX = Input.GetAxisRaw("Mouse X");
        float mouseInputY = - Input.GetAxisRaw("Mouse Y");

        xRotation += mouseInputY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        yRotation += mouseInputX;
        

        Debug.Log(yRotation);

        Quaternion verticalRotation = Quaternion.Euler(xRotation, yRotation, 0f);

        pivotPoint.localRotation = verticalRotation;
    }

    public override void StopControlling()
    {
        SetCamera(false);
    }

    private void SetCamera(bool active)
    {
        myCamera.SetActive(active);
    }
}
