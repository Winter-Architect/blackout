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

    [SerializeField] private Transform[] _nodes;

    private Transform _currentNode; 
    private int _currentNodeIndex;
    

    void Awake()
    {
        _entity = GetComponent<NavMeshAgent>();
        _currentNodeIndex = 0;

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
        if (_player)
            _entity.destination = _player.transform.position;
        else
        {
            Debug.Log(_entity.transform.position + ", " + _currentNode.transform.position);
            if (!_entity.hasPath || _entity.velocity.sqrMagnitude == 0f)
            {
                _currentNodeIndex = (_currentNodeIndex+1)%_nodes.Length;
                _currentNode = _nodes[_currentNodeIndex];
            }
            _entity.destination = _currentNode.transform.position;
        }
    }

    private void OnTriggerEnter(Collider objectCollider)
    {
        if (!_player)
        {
            Debug.Log("Has entered" + ", " + GameObject.FindGameObjectWithTag("Player") + ", " + objectCollider.gameObject);

            if (GameObject.FindGameObjectWithTag("Player") == objectCollider.gameObject)
            {
                _player = GameObject.FindGameObjectWithTag("Player");
            }
        }
    }

    private void OnTriggerExit(Collider objectCollider)
    {
        if (objectCollider.gameObject == _player) 
        {
            _player = null;
        }
    }
}
