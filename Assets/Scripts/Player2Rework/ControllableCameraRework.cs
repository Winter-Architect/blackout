using UnityEngine;

public class ControllableCameraRework : Controllable
{

    private float xRotation;
    private float yRotation;
    [SerializeField] private Transform pivotPoint;
    [SerializeField] private GameObject myCamera;
    public override void Control()
    {
        SetCamera(true);
        MoveCamera();
    }

    public override void StopControlling()
    {
        SetCamera(false);
    }
    private void MoveCamera()
    {

        float mouseInputX = Input.GetAxisRaw("Mouse X");
        float mouseInputY = - Input.GetAxisRaw("Mouse Y");

        xRotation += mouseInputY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        yRotation += mouseInputX;
        yRotation = Mathf.Clamp(yRotation, -90f, 90f);


        Quaternion verticalRotation = Quaternion.Euler(xRotation, yRotation, 0f);

        pivotPoint.localRotation = verticalRotation;
    }
    private void SetCamera(bool active)
    {
        myCamera.SetActive(active);
    }
}
