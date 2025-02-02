using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class SensorDetector : NetworkBehaviour
{
    private static Dictionary<GameObject, SensorDetector> instances = new Dictionary<GameObject, SensorDetector>();
    
    
    private GameObject _target;
    private bool detected = false;

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
        
        Debug.Log("Initial state of isWalking: " + isWalking); // Debug the initial state

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
        }
    }
}