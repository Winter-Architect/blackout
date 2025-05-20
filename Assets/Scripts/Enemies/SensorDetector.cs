﻿using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class SensorDetector : NetworkBehaviour
{
    private static Dictionary<GameObject, SensorDetector> instances = new Dictionary<GameObject, SensorDetector>();
    
    
    private GameObject _target;
    private Agent agent;
    private bool detected = false;
    private float detectionRange;
    private const float BASE_SPEED = 3f;

    public bool Detected
    {
        get => detected;
        set => detected = value;
    }
    public GameObject Target => _target;

    [SerializeField] public float range;

    public bool IsRunning => agent.currentSpeed > BASE_SPEED; // player.isRunning
    public bool IsWalking => Mathf.Approximately(agent.currentSpeed, BASE_SPEED); // player.isWalking
    public bool IsSneaking => agent.currentSpeed < BASE_SPEED; // player.isSneaking
    
    
    [SerializeField] private LayerMask playerMask;

    private void Awake()
    {
        instances[gameObject] = this;
    }


    public SensorDetector()
    {
        if (!IsClient) return;
        _target = null;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(SensorDetectorCoroutine());
    }
    
    public IEnumerator SensorDetectorCoroutine()
    {
        WaitForSeconds searchDelay = new WaitForSeconds(0.33f);
        
        while (true)
        {
            yield return searchDelay;
            SensorDetectorSearch();
            
        }
    }

    public void SensorDetectorSearch()
    {
        if (!_target)
        {
            _target = GameObject.FindGameObjectWithTag("Player");
            if (_target is null) return;
            agent = _target.GetComponent<Agent>();
            if (agent is null) return;
        }
        
        detectionRange = range;
        if (agent)
        {
            if (agent.currentSpeed < BASE_SPEED)
                detectionRange /= 2;
            else if (agent.currentSpeed > BASE_SPEED)
                detectionRange *= 1.5f;
        }
        
        /*
        Collider[] objWithinRange = Physics.OverlapSphere(transform.position, detectionRange, playerMask);
        if (objWithinRange.Length>0)
        {
            detected = true;
        }
        else
        {
            detected = false;
        }*/
        
        detected = Vector3.Distance(transform.position, _target.transform.position) < detectionRange;
        
    }
}