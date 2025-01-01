using UnityEngine;
using UnityEngine.AI;

public class AIMovement : MonoBehaviour
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
        _entity.SetDestination(_player.transform.position);
    }

    // Update is called once per frame
    void Update()
    {
       _entity.destination = _player.transform.position;
           if (!_entity.pathPending && _entity.remainingDistance <= _entity.stoppingDistance)
    {
        Debug.Log("Destination reached or cannot move.");
    }
    }
}
