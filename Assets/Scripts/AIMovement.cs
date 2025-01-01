using UnityEngine;
using UnityEngine.AI;

public class AIMovement : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private NavMeshAgent _entity;
    [SerializeField]
    private GameObject _player;
    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _entity = GetComponent<NavMeshAgent>();
        if (_entity is null){
            Debug.LogError("entity is null");
        }
        _entity.SetDestination(_player.transform.position);
    }

    // Update is called once per frame
    void Update()
    {
       
    }
}
