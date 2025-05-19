using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Serialization;

public class Room : MonoBehaviour
{
    public AudioClip musicClip;
    
    public int RoomID;
    public bool isTurningLeft;
    public bool isTurningRight;
    public bool isStairs;
    public float Weight;
    public AudioSource Audio; // Reference to AudioSource
    public List<Light> lights;
    
    /*
    private void Start()
    {
        foreach (Light lit in lights)
        {
            lit.gameObject.SetActive(false);
        }
    }
    */
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

    public List<Enemy> GetEnemiesWithin(Enemy[] enemies)
    {
        List<Enemy> res = new List<Enemy>();
        Bounds myBounds = GetComponent<Collider>().bounds;

        foreach(Enemy enemy in enemies)
        {
            if(myBounds.Contains(enemy.transform.position)){
                Debug.Log($"found {enemy.gameObject.name}");
                res.Add(enemy);
            }
        }
        return res;
    }

    public List<Transform> GetSpawnPoints(string prefix)
    {
        List<Transform> spawnPoints = new List<Transform>();

        foreach (Transform child in GetComponentsInChildren<Transform>(true))
        {
            if (child.name.StartsWith(prefix))
            {
                spawnPoints.Add(child);
            }
        }

        return spawnPoints;
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
            if(Player2 is not null)
            {
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