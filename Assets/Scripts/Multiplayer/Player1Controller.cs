using Unity.Netcode;
using UnityEngine;

public class Player1Controller : NetworkBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private Camera myCamera;

    void Start()
    {
        if(!IsOwner){
            myCamera.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        if(!IsOwner){
            return;
        }
        //RotateCamera
        myCamera.transform.Rotate(new Vector3(0, Input.GetAxis("Mouse X"), 0));

        //Move
        Vector3 myForwardVector = myCamera.transform.forward; 
        Vector3 myRightVector = myCamera.transform.right;
        Vector3 myDirectionVector = ((myForwardVector * Input.GetAxisRaw("Vertical")) + (myRightVector * Input.GetAxisRaw("Horizontal"))).normalized;
        transform.position += myDirectionVector * speed * Time.deltaTime;

    }
}
