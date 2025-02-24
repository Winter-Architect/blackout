﻿using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class SensorDetector : NetworkBehaviour
{
    private static Dictionary<GameObject, SensorDetector> instances = new Dictionary<GameObject, SensorDetector>();
    
    
    private GameObject _target;
    private bool detected = false;
    private float detectionRange;

    public bool Detected
    {
        get => detected;
        set => detected = value;
    }
    public GameObject Target => _target;

    [SerializeField] public float range;

    public bool IsRunning => isRunning; // player.isRunning
    public bool IsWalking => isWalking; // player.isWalking
    public bool IsSneaking => isSneaking; // player.isSneaking

    private bool isRunning; // player.isRunning
    private bool isWalking; // player.isWalking
    private bool isSneaking; // player.isSneaking
    
    
    [SerializeField] private LayerMask playerMask;

    private void Awake()
    {
        instances[gameObject] = this;
        isWalking = true;
    }


    public SensorDetector()
    {
        if (!IsClient) return;

        isRunning = false;
        isSneaking = false;
        isWalking = true;
        
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
        }
        
        detectionRange = range;
        if (isSneaking)
            detectionRange /= 2;
        else if (isRunning)
            detectionRange *= 1.5f;
        
        Collider[] objWithinRange = Physics.OverlapSphere(transform.position, detectionRange, playerMask);
        if (objWithinRange.Length>0)
        {
            detected = true;
        }
        else
        {
            detected = false;
        }
        
        /*
        if (!_target)
        {
            _target = GameObject.FindGameObjectWithTag("Player");
        }
        else
        {
            if (isWalking)
            {
                if (Vector3.Distance(transform.position, _target.transform.position) < range)
                {
                    detected = true;
                }
                else
                {
                    detected = false;
                }
            }
            else if (isSneaking)
            {
                if (Vector3.Distance(transform.position, _target.transform.position) < range/2)
                {
                    detected = true;
                }
                else
                {
                    detected = false;
                }
            }
            else if (isRunning)
            {
                if (Vector3.Distance(transform.position, _target.transform.position) < range*1.5)
                {
                    detected = true;
                }
                else
                {
                    detected = false;
                }
            }
        }*/
    }
}