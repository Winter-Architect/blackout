using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

public class ControllablesSpawner : NetworkBehaviour
{

    public GameObject controllableCamera;
    public List<Transform> placesToSpawn = new List<Transform>();

    public override void OnNetworkSpawn()
    {
        if(IsServer)
        {
            var myItems = GameObject.FindGameObjectsWithTag("Controllables");
            foreach(var Item in myItems)
            {
                placesToSpawn.Add(Item.transform);
            }
            SpawnControllables();
        }
        
        
    }


    async void SpawnControllables()
    {

        foreach(var place in placesToSpawn){
            var camera = Instantiate(controllableCamera, place.position, place.rotation);
            camera.GetComponent<NetworkObject>().Spawn();
        }
        

        await Task.Yield();

        
    }

}
