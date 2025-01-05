using Unity.VisualScripting;
using Unity.Netcode;
using UnityEngine;


public class PlayerNetwork : NetworkBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    // Update is called once per frame
    private void Update()
    {
        if (!IsOwner) return;

         Vector3 moveDir = new Vector3(0, 0, 0);

         if (Input.GetKey(KeyCode.W)) moveDir.z = +1f;
         if (Input.GetKey(KeyCode.S)) moveDir.z = -1f;
         if (Input.GetKey(KeyCode.A)) moveDir.x = -1f;
         if (Input.GetKey(KeyCode.D)) moveDir.x = +1f;

        float moveSpeed = 8f;
        transform.position += moveDir * moveSpeed * Time.deltaTime;
    }
}
