﻿
﻿using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class RottenSlime : Enemy
{
    private Vector3 dest;
    private bool walkpointSet;
    private float rangeX;
    private float rangeZ;
    private bool hasAmbushed;
    private bool isAttacking;
    private Rigidbody rb;
    private GameObject player;

    [SerializeField] private float jumpForce = 30f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float groundCheckDistance = 1f;
    private float lastDamageTime = -Mathf.Infinity;
    private float damageCooldown = 0.33f;

    void Awake()
    {
        fieldOfView = GetComponent<FieldOfView>();
        StartCoroutine(fieldOfView.FOVCoroutine());
        
        sensorDetector = GetComponent<SensorDetector>();
        StartCoroutine(sensorDetector.SensorDetectorCoroutine());
        
        stateMachine = new StateMachine();
        rb = GetComponent<Rigidbody>();
        agent = GetComponent<NavMeshAgent>();
        
        walkpointSet = false;
        hasAmbushed = false;
        isAttacking = false;
        rangeX = 1f;
        rangeZ = 6f;
        rb.useGravity = false;
        
        rb.isKinematic = true;
        rb.constraints = RigidbodyConstraints.FreezeAll;
    }

    void Start()
    {
        
        var patrolState = new EnemyPatrolState(this, animator);
        var ambushState = new EnemyAmbushState(this, animator);
        var attackState = new EnemyAttackState(this, animator);

        At(ambushState, attackState, new FuncPredicate(() => hasAmbushed && !isAttacking));
        At(attackState, patrolState, new FuncPredicate(() => !fieldOfView.Spotted && agent.enabled));
        At(patrolState, attackState, new FuncPredicate(() => fieldOfView.Spotted));
        
        stateMachine.SetState(patrolState);
    }

    public override void Patrol()
    {
        if (!agent.isOnNavMesh)
        {
            Debug.Log("not on navmesh");
            GoNavmesh();
            if (!agent.isOnNavMesh)
                StartCoroutine(ReturnToNavMesh());
        }
        else
        {
            if (!walkpointSet)
            {
                Debug.Log("-------- 0 --------");
                SetNextDest();
            }
            if (walkpointSet)
            {
                Debug.Log("-------- 1 --------");

                agent.SetDestination(dest);
            }
            if (Vector3.Distance(transform.position, dest) < 1 || !agent.hasPath)
            {
                Debug.Log("-------- 2 --------");
                walkpointSet = false;
            }
        }
    }

    public override void Attack()
    {
        if (player != null && agent.isOnNavMesh)
        {
            walkpointSet = false;
            agent.SetDestination(player.transform.position);
        }
        else
        {
            player = fieldOfView.Target;
        }
    }

    public override void Ambush()
    {
        if (player != null)
        {
            RaycastHit playerGroundHit;
            float playerGroundCheckDistance = 1.0f;
        
            if (Physics.Raycast(player.transform.position, Vector3.down, out playerGroundHit, playerGroundCheckDistance, groundLayer))
            {
                RaycastHit hit;
                float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
                Vector3 directionToPlayer = (player.transform.position - transform.position).normalized;
            
                if (Physics.Raycast(transform.position, directionToPlayer, out hit, Mathf.Infinity))
                {
                    if (hit.collider.CompareTag("Player"))
                    {
                        if (distanceToPlayer<11f)
                        {
                            JumpAttack();
                        }
                    }
                }
            }
        }
        else
        {
            if (fieldOfView.Spotted && fieldOfView.Target)
            {
                player = fieldOfView.Target;
            }
        }
    }

    private void SetNextDest()
    {
        float z = Random.Range(-rangeZ, rangeZ);
        float x = Random.Range(-rangeX, rangeX);

        Vector3 randomPoint = transform.position + new Vector3(x, 0, z);

        while (!CanReachDestination(randomPoint))
        {
            z = Random.Range(-rangeZ, rangeZ);
            x = Random.Range(-rangeX, rangeX);

            randomPoint = transform.position + new Vector3(x, 0, z);
        }

        dest = new Vector3(transform.position.x + x, transform.position.y, transform.position.z + z);
        walkpointSet = true;
        if (Physics.Raycast(dest, Vector3.down, out RaycastHit hit, Mathf.Infinity, groundLayer))
        {
            walkpointSet = true;
        }
    }

    private void JumpAttack()
    {
        if (agent.isOnNavMesh)
        {
            isAttacking = true;
            agent.enabled = false;
            rb.useGravity = true;
            rb.linearVelocity = Vector3.zero;
            Vector3 attackDirection = Vector3.down;
            attackDirection.y = 0.5f;
            rb.AddForce(attackDirection * jumpForce, ForceMode.Impulse);
            hasAmbushed = true;
            agent.agentTypeID = SwitchAgentToClimbableID();

            StartCoroutine(ReturnToNavMesh());
        }
    }

    IEnumerator ReturnToNavMesh()
    {
        yield return new WaitForSeconds(0.3f);
        
        float groundCheckTimeout = 3f; 
        float elapsed = 0f;
        
        while (!IsGrounded() && elapsed < groundCheckTimeout)
        {
            elapsed += Time.deltaTime;
            yield return null;
        }
        if (elapsed >= groundCheckTimeout)
        {
            rb.linearVelocity = Vector3.zero;
        }
        
        yield return new WaitForSeconds(0.5f);
        
        NavMeshHit hit;
        if (NavMesh.SamplePosition(transform.position, out hit, 3f, NavMesh.AllAreas))
        {
            transform.position = hit.position;
            
            agent.enabled = true;
            rb.useGravity = false;
            rb.linearVelocity = Vector3.zero;
            isAttacking = false;
        }
        else
        {
            rb.useGravity = false;
            rb.linearVelocity = Vector3.zero;
            transform.position = dest;
            agent.enabled = true;
            agent.Warp(dest);
            isAttacking = true;
        }
        
    }

    bool IsGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, groundCheckDistance, groundLayer);
    }
    
    
    bool CanReachDestination(Vector3 destination)
    {
        NavMeshPath path = new NavMeshPath();

        bool hasPath = NavMesh.CalculatePath(transform.position, destination, NavMesh.AllAreas, path);
        
        if (hasPath && path.status == NavMeshPathStatus.PathComplete)
        {
            return true;
        }
        
        return false;
    }
    
    int SwitchAgentToClimbableID()
    {
        int count = NavMesh.GetSettingsCount();
        
        for (int i = 0; i < count; i++)
        {
            NavMeshBuildSettings settings = NavMesh.GetSettingsByIndex(i);
            string currentName = NavMesh.GetSettingsNameFromID(settings.agentTypeID);

            if (currentName == "Climbable")
            {
                return settings.agentTypeID;
            }
        }

        return -1;
    }
    
    void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (Time.time - lastDamageTime >= damageCooldown)
            {
                IDamageable damageable = collision.gameObject.GetComponent<IDamageable>();
                if (damageable != null)
                {
                    damageable.TakeDamage(5, 0);
                    lastDamageTime = Time.time;
                }
            }
        }
    }
    
    public void Initialized()
    {
        if (agent != null && agent.isOnNavMesh)
            agent.Warp(transform.position);
    }
}
