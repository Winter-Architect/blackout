using UnityEngine;
using Unity.Netcode;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;


public class AIMovement : NetworkBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private NavMeshAgent _entity;

    [SerializeField] private Transform[] _nodes;

    private Transform _currentNode; 
    private int _currentNodeIndex;
    
    private FieldOfView fieldOfView;
    
    private Vector3[] lastPlayerPositionArray;
    private bool lastPlayerPositionVisited;

    void Awake()
    {
        _entity = GetComponent<NavMeshAgent>();
        _currentNodeIndex = 0;
        lastPlayerPositionArray = new []{transform.position};
        lastPlayerPositionVisited = true;
    }
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _currentNode = _nodes[_currentNodeIndex];
    }

    // Update is called once per frame
    void Update()
    {
        Move();
    }

    void Move()
    {
        if (FieldOfView.Spotted)
        {
            if (lastPlayerPositionVisited)
            {
                lastPlayerPositionVisited = false;
            }
            lastPlayerPositionArray[0] = FieldOfView.Target.transform.position;
            _entity.destination = lastPlayerPositionArray[0];
        }
        else
        {
            if (!lastPlayerPositionVisited)
            {
                _entity.destination = lastPlayerPositionArray[0];
                if (!_entity.hasPath || _entity.velocity.sqrMagnitude == 0f)
                {
                    lastPlayerPositionVisited = true;
                }
            }
            else
            {
                if (!_entity.hasPath || _entity.velocity.sqrMagnitude == 0f)
                {
                    _currentNodeIndex = (_currentNodeIndex + 1) % _nodes.Length;
                    _currentNode = _nodes[_currentNodeIndex];
                }
                _entity.destination = _currentNode.transform.position;
            }
            
        }
    }
}
