using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Enemy : NetworkBehaviour
{
    [SerializeField] private NavMeshAgent agent;
    private StateMachine stateMachine;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        stateMachine = new StateMachine();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
