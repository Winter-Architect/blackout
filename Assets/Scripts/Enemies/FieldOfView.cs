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
    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (!IsClient) return;  
        _target = null;
        StartCoroutine(FOVCoroutine());
    }
    
    public IEnumerator FOVCoroutine()
    {
        WaitForSeconds searchDelay = new WaitForSeconds(0.33f);
        
        while (true)
        {
            yield return searchDelay;
            FOVSearch();
            
        }
    }

    private void FOVSearch()
    {
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
                    if (!_target)
                    {
                        _target = GameObject.FindGameObjectWithTag("Player");
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