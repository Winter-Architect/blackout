using System;
using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class FieldOfView : NetworkBehaviour
{
    
    [SerializeField] public float radius;
    [SerializeField] [Range(0, 360)] public float angle;

    [SerializeField] private LayerMask playerMask;
    [SerializeField] private LayerMask obstacleMask;

    private GameObject _player;
    public static GameObject Target => _target;
    public static bool Spotted => spotted;

    private static GameObject _target;
    private static bool spotted = false;
    


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _player = null;
        _target = null;
        StartCoroutine(FOVCoroutine());
    }
    
    private IEnumerator FOVCoroutine()
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