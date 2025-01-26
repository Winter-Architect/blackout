using System;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using Random = Unity.Mathematics.Random;
using SystemInfo = UnityEngine.Device.SystemInfo;

[RequireComponent(typeof(NavMeshAgent))]
public class Enemy : NetworkBehaviour
{
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Animator animator;
    
    //////// For the pratrol /////////
    [SerializeField] private Transform[] _nodes;
    private Transform _currentNode; 
    private int _currentNodeIndex;
    
    private Vector3[] lastPlayerPositionArray;
    private bool lastPlayerPositionVisited;
    private FieldOfView fieldOfView;
    //////////////////////////////////
    
    Random random;
    private bool isInvestigating = false;

    private float rotationSpeed;
    private float lookAroundTime;
    private float timeElapsed;
    
    private StateMachine stateMachine;
    
    
    void Awake() // Patrol
    {
        agent = GetComponent<NavMeshAgent>();
        _currentNodeIndex = 0;
        lastPlayerPositionArray = new []{transform.position};
        lastPlayerPositionVisited = true;

        random = new Random(5);
    }
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rotationSpeed = 25f; 
        lookAroundTime = 5f;
        timeElapsed = 0f;
        
        stateMachine = new StateMachine();
        
        _currentNode = _nodes[_currentNodeIndex]; // Patrol

        var patrolState = new EnemyPatrolState(this, animator, agent); 
        var huntDownState = new EnemyHuntDownState(this, animator, agent);
        
        At(patrolState, huntDownState, new FuncPredicate(()=>FieldOfView.Spotted));
        At(huntDownState, patrolState, new FuncPredicate(()=>!FieldOfView.Spotted && lastPlayerPositionVisited));
        stateMachine.SetState(patrolState);
    }

    void At(IState from, IState to, IPredicate predicate)
    {
        stateMachine.AddTransition(from, to, predicate);
    }
    
    void Any(IState to, IPredicate predicate)
    {
        stateMachine.AddAnyTransition(to, predicate);
    }
    
    
    // Update is called once per frame
    void Update()
    {
        stateMachine.Update();
    }

    private void FixedUpdate()
    {
        stateMachine.FixedUpdate(); 
    }

    public void Patrol() // Lately to define in the child class (getting back previous patrol attributes)
    {
        if (agent.updateRotation == false)
        {
            agent.updateRotation = true;
        }
        if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
        {
            _currentNodeIndex = (_currentNodeIndex + 1) % _nodes.Length;
            _currentNode = _nodes[_currentNodeIndex];
        }
        agent.destination = _currentNode.transform.position;

    }

    public void HuntDown()
    {

        if (FieldOfView.Spotted)
        {
            if (lastPlayerPositionVisited)
            {
                lastPlayerPositionVisited = false;
            }
            lastPlayerPositionArray[0] = FieldOfView.Target.transform.position;
            agent.destination = lastPlayerPositionArray[0];
        }
        else if (!lastPlayerPositionVisited)
        {
            agent.destination = lastPlayerPositionArray[0];
            if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
            {
                Investigate();
            }
        }
    }


    public void Investigate()
    {
        agent.isStopped = true;
        agent.velocity = Vector3.zero;
        agent.updateRotation = false;

        if (agent.isStopped)
        {
            float rotationAmount = rotationSpeed * Time.deltaTime;
            transform.Rotate(Vector3.up, rotationAmount);
            timeElapsed += Time.deltaTime;
            
            if (timeElapsed >= lookAroundTime || FieldOfView.Spotted)
            {
                Debug.Log("t a chier");
                agent.updateRotation = true;
                agent.isStopped = false;
                lastPlayerPositionVisited = true;
                agent.updateRotation = true;
                timeElapsed = 0f;
            }
        }
    }


}