using System;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

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
        stateMachine = new StateMachine();
        
        _currentNode = _nodes[_currentNodeIndex]; // Patrol

        var patrolState = new EnemyPatrolState(this, animator, agent); 
        var huntDownState = new EnemyHuntDownState(this, animator, agent);
        
        Any(patrolState, new FuncPredicate(()=>true));
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
                    lastPlayerPositionVisited = true;
                }
            }
    }

    public void Investigate()
    {
        
        Vector3 dir = transform.forward();
        dir = Quaternion.AngleAxis(90, Vector3.up) * dir;
        List<Vector3> lst = new List<Vector3>();
    
        for (int i = 0; i < 5; i++)
        {
            lst.Add(transform.position + dir)
            dir = Quaternion.AngleAxis(-30, Vector3.up) * dir;
        }


    }
}