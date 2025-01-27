using System;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using Random = Unity.Mathematics.Random;
using SystemInfo = UnityEngine.Device.SystemInfo;

[RequireComponent(typeof(NavMeshAgent))]
public class Enemy : NetworkBehaviour, IDamagable
{
    [SerializeField] protected NavMeshAgent agent;
    [SerializeField] protected Animator animator;
    
    protected bool lastPlayerPositionVisited;
    protected bool isInvestigating = false;
    
    protected float hp;
    
    private StateMachine stateMachine;

    public Enemy(float hp)
    {
        this.hp = hp;
    }
    
    void Awake() 
    {
        agent = GetComponent<NavMeshAgent>();

    }
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        stateMachine = new StateMachine();

        var patrolState = new EnemyPatrolState(this, animator, agent); 
        var huntDownState = new EnemyHuntDownState(this, animator, agent);
        var investigateState = new EnemyInvestigateState(this, animator, agent);
        
        At(patrolState, huntDownState, new FuncPredicate(()=>FieldOfView.Spotted));
        At(huntDownState, patrolState, new FuncPredicate(()=>!FieldOfView.Spotted && lastPlayerPositionVisited));
        At(huntDownState, investigateState, new FuncPredicate(()=>!FieldOfView.Spotted && (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)));
        At(investigateState, patrolState, new FuncPredicate(()=>!isInvestigating));
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
    }
    
    public void HuntDown()
    {
    }
    
    public void Investigate()
    {
    }
    
    public void TakeDamage(int dmg, int knockback)
    {
        this.hp -= dmg;
    }
}