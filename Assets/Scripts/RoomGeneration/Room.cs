using System.Collections.Generic;
using System.Runtime.Serialization;
using Unity.Netcode;
using UnityEngine;

public class Room : MonoBehaviour
{
    public int RoomID;
    public bool isTurningLeft;
    public bool isTurningRight;
    public bool isStairs;
    public float Weight;
    public AudioSource Audio; // Reference to AudioSource

    public List<Controllable> GetControllablesWithin(Controllable[] controllables)
    {
        List<Controllable> res = new List<Controllable>();
        Bounds myBounds = GetComponent<Collider>().bounds;

        foreach(Controllable ctrl in controllables)
        {
            if(myBounds.Contains(ctrl.transform.position)){
                Debug.Log($"found {ctrl.gameObject.name}");
                res.Add(ctrl);
            }
        }
        return res;
    }

    public bool ContainsPlayer(Agent Player)
    {
        Bounds myBounds = GetComponent<Collider>().bounds;
        if (Audio != null && !Audio.isPlaying)
        {
            Audio.Play();
        }
        return myBounds.Contains(Player.transform.position);
    }
        
    void OnTriggerExit(Collider col)
    {
        if(col.gameObject.tag == "Player")
        {
            var Player2 = FindFirstObjectByType<Support>();
            if(Player2 is not null)
            {
                Player2.RecheckForRoom();
            }
            if (Audio != null && !Audio.isPlaying)
            {
                Audio.Stop();
            }
        }
    }
}