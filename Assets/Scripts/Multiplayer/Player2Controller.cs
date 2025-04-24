using Unity.Netcode;
using UnityEngine;
using UnityEngine.UIElements;

public class Player2Controller : NetworkBehaviour
{
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
        //Move Camera
        myCamera.transform.Rotate(new Vector3(0, Input.GetAxis("Mouse X"), 0));
        myCamera.transform.localRotation = Quaternion.Euler(0, myCamera.transform.rotation.eulerAngles.y, Input.GetAxis("Mouse Y"));
    }
}
