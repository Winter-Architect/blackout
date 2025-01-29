using System;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using Random = Unity.Mathematics.Random;
using SystemInfo = UnityEngine.Device.SystemInfo;

[RequireComponent(typeof(NavMeshAgent))]
public abstract class Enemy : NetworkBehaviour, IDamagable
{
    [SerializeField] protected NavMeshAgent agent;
    [SerializeField] protected Animator animator;
    
    protected FieldOfView fieldOfView;
    protected SensorDetector sensorDetector;
    
    protected bool isInvestigating = false;
    
    protected float hp;
    
    protected StateMachine stateMachine;

    protected Enemy(float hp)
    {
        this.hp = hp;
    }
    
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
    
    public void TakeDamage(int dmg, int knockback)
    {
        this.hp -= dmg;
    }
}