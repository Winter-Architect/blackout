using Unity.Netcode;
using UnityEngine;

public class NetObjectPlaceHolder : MonoBehaviour
{
    public GameObject MyPrefab;
    public void Spawn()
    {
        var Obj = Instantiate(MyPrefab, transform.position, transform.rotation);
        var Net = Obj.GetComponent<NetworkObject>();
        Net.Spawn();
    }

}
