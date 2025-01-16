using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UIElements;

public class ControllableCamera : NetworkBehaviour
{

    // [SerializeField] private float mouseSensitivityX = 1f;
    // [SerializeField] private float mouseSensitivityY = 1f;

    private float MAX_DISTANCE = 300000f;

    private float xRotation;
    private float yRotation;

    public GameObject toControl {get; set;}
    [SerializeField] private Transform pivotPoint;
    [SerializeField] public Transform myCameraTransform;

    private Outline AddOutlineToObjectOrGetOutline(GameObject objectToOutline)
    {
        if(objectToOutline.TryGetComponent<Outline>(out Outline outline)){
            return outline;
        }
        else{
            var line = objectToOutline.AddComponent<Outline>();
            return line;
        }
        
    }

    public void Control()
    {
        MoveCamera();
        Look();
        GetControlledObjectOutline();
        
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

    public void Look()
    {


        RaycastHit hit;

        if(Physics.Raycast(myCameraTransform.position, myCameraTransform.forward, out hit, MAX_DISTANCE) && hit.collider.gameObject.CompareTag("Controllables"))
        {
            if(Input.GetMouseButtonDown(1))
            {
                toControl = hit.collider.gameObject;
            }
            
            
        }
        
    }

    public void GetControlledObjectOutline()
    {
        var outline = AddOutlineToObjectOrGetOutline(gameObject);
        outline.OutlineMode = Outline.Mode.OutlineAll;
        outline.OutlineColor = Color.yellow;
        outline.OutlineWidth = 5f;
    }

}
