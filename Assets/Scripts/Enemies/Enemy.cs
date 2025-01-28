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
    
    protected bool lastPlayerPositionVisited;
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
    
    public void TakeDamage(int dmg, int knockback)
    {
        this.hp -= dmg;
    }
}