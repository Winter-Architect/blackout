using UnityEngine;
using Unity.Netcode;
using UnityEngine.AI;
using System.Collections;

public class AIMovement : NetworkBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private NavMeshAgent _entity;
    [SerializeField]
    private GameObject _player;

    

    void Awake()
    {
        _entity = GetComponent<NavMeshAgent>();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (_player)
        {
            Move();
        }
        else
        {
            _player = GameObject.FindGameObjectWithTag("Player");
        }
    }




    void Move()
    {
        _entity.destination = _player.transform.position;
    }


}
