using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class FieldOfView : NetworkBehaviour
{
    
    private static Dictionary<GameObject, FieldOfView> instances = new Dictionary<GameObject, FieldOfView>();

    [SerializeField] public float radius;
    [SerializeField] [Range(0, 360)] public float angle;

    [SerializeField] private LayerMask playerMask;
    [SerializeField] private LayerMask obstacleMask;

    private GameObject _player;
    
    private GameObject _target;
    private bool spotted = false;

    public bool Spotted => spotted;
    public GameObject Target => _target;

    private void Awake()
    {
        instances[gameObject] = this;
    }
    
    /*
    private void OnDestroy()
    {
        instances.Remove(gameObject);
    }
    */
    
    public static FieldOfView GetInstance(GameObject obj)
    {
        return instances.ContainsKey(obj) ? instances[obj] : null;
    }
    


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (!IsClient) return;  
        Debug.Log($"[{gameObject.name}] FieldOfView Start() called.");
        _player = null;
        _target = null;
        StartCoroutine(FOVCoroutine());
    }
    
    public IEnumerator FOVCoroutine()
    {
        Debug.Log($"[{gameObject.name}] Starting FOVCoroutine...");
        WaitForSeconds searchDelay = new WaitForSeconds(0.33f);
        
        while (true)
        {
            yield return searchDelay;
            Debug.Log($"[{gameObject.name}] Calling FOVSearch...");
            FOVSearch();
            
        }
    }

    private void FOVSearch()
    {
        Debug.Log(gameObject + ", " + spotted);
        Collider[] objWithinRange = Physics.OverlapSphere(transform.position, radius, playerMask);
        if (objWithinRange.Length>0)
        {
            Transform playerTransform = objWithinRange[0].transform;
            Vector3 directionToPlayer =  (playerTransform.position - transform.position).normalized;
            
            if (Vector3.Angle(transform.forward, directionToPlayer) < angle / 2)
            {
                float distanceToTarget = Vector3.Distance(transform.position, playerTransform.position);
                if (!Physics.Raycast(transform.position, directionToPlayer, distanceToTarget, obstacleMask))
                {
                    spotted = true;
                    if (!_player)
                    {
                        _player = GameObject.FindGameObjectWithTag("Player");
                    }
                    if (!_target)
                    {
                        _target = _player;
                    }
                }
                else
                {
                    spotted = false;
                    _target = null;
                }
            }
            else
            {
                spotted = false;
                _target = null;
            }   
        }
        else if (spotted)
        {
            spotted = false;
            _target = null;
        }
    }
}