using System;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;


[RequireComponent(typeof(NavMeshAgent))]
public abstract class Enemy : NetworkBehaviour, IDamageable
{
    [SerializeField] protected NavMeshAgent agent;
    [SerializeField] protected Animator animator;
    
    protected FieldOfView fieldOfView;
    protected SensorDetector sensorDetector;
    
    protected bool isInvestigating = false;
    
    protected float hp;
    
    protected StateMachine stateMachine;
    
    void Awake() 
    {
        this.agent = GetComponent<NavMeshAgent>();
    }
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }
    
    void Update()
    {
        stateMachine.Update();
    }

    protected virtual void FixedUpdate()
    {
        stateMachine.FixedUpdate(); 
    }
    
    protected void At(IState from, IState to, IPredicate predicate)
    {
        stateMachine.AddTransition(from, to, predicate);
    }
    
    protected void Any(IState to, IPredicate predicate)
    {
        stateMachine.AddAnyTransition(to, predicate);
    }
    
    // Update is called once per frame

    
    public virtual void Patrol() // Lately to define in the child class (getting back previous patrol attributes)
    {
    }
    
    public virtual void HuntDown()
    {
    }
    
    public virtual void Investigate()
    {
    }

    public virtual void Attack()
    {
    }
    
    public virtual void Ambush()
    {
    }
    
    public virtual void RunAway()
    {
    }
    
    public void TakeDamage(float dmg, float knockback)
    {
        this.hp -= dmg;
    }
    
    protected void GoNavmesh()
    {
        if (!agent.isOnNavMesh)
        {
            NavMeshHit hit;
            if (NavMesh.SamplePosition(transform.position, out hit, 2.0f, NavMesh.AllAreas))
            {
                agent.Warp(hit.position);
            }
        }
    }

    public void GetDestroyed()
    {
        Destroy(gameObject);
    }
}