using Unity.Netcode;
using UnityEngine;

public class PlayerController : NetworkBehaviour
{
    public float speed = 20;

    private ClientNetworkTransform _transform;

    private void Start()
    {
        _transform = GetComponent<ClientNetworkTransform>();
    }
    private void Update()
    {
        if (!IsOwner) return;
        var movement = new Vector3(Input.GetAxis("Horizontal"),0,  Input.GetAxis("Vertical"));
        _transform.transform.position += movement.normalized * speed * Time.deltaTime;
    }
}
