using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Serialization;

#nullable enable

public class Room : NetworkBehaviour
{
    public AudioClip musicClip;
    
    public int RoomID;
    public bool isTurningLeft;
    public bool isTurningRight;
    public bool isStairs;
    public float Weight;
    public AudioSource Audio; // Reference to AudioSource
    public List<Light> lights;
    public Sprite? Map;

    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;

        var placeHolders = gameObject.GetComponentsInChildren<NetObjectPlaceHolder>();
        foreach (var item in placeHolders)
        {
            item.Spawn();
            Debug.Log(item.name);
        }
    }

    public List<Controllable> GetControllablesWithin(Controllable[] controllables)
    {
        List<Controllable> res = new List<Controllable>();
        Bounds myBounds = GetComponent<Collider>().bounds;

        foreach (Controllable ctrl in controllables)
        {
            if (myBounds.Contains(ctrl.transform.position))
            {
                Debug.Log($"found {ctrl.gameObject.name}");
                res.Add(ctrl);
            }
            else
            {
                Debug.Log($"not found {ctrl.gameObject.name}");
            }
        }
        return res;
    }

    public bool ContainsPlayer(Agent Player)
    {
        try {
            Bounds myBounds = GetComponent<Collider>().bounds;
            if (Audio != null && !Audio.isPlaying)
            {
                Audio.Play();
            }
            
            return myBounds.Contains(Player.transform.position);
        } catch {
            return false;
        }
        
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "Player")
        {
            foreach (Light lit in lights)
            {
                lit.gameObject.SetActive(true);
            }
            AudioSource playerAudio = col.GetComponent<AudioSource>();
            if (playerAudio != null)
            {
                if (playerAudio.clip == musicClip && playerAudio.isPlaying)
                {
                    // Same music is already playing — do nothing.
                    return;
                }

                playerAudio.clip = musicClip;
                playerAudio.Play();
            }
        }
    }
    void OnTriggerExit(Collider col)
    {
        if(col.gameObject.tag == "Player")
        {
            var Player2 = FindFirstObjectByType<Support>();
            if (Player2 is not null)
            {
                Debug.LogWarning("Player2 is not null, rechecking for room");
                Player2.RecheckForRoom();
            }
            if (Audio != null && !Audio.isPlaying)
            {
                Audio.Stop();
                
            }
            foreach (Light lit in lights)
            {
                lit.gameObject.SetActive(false);
            }
        }
    }
}