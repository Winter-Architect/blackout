using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class SpikeyEnemy : Enemy
{
    private Vector3 dest;
    private bool walkpointSet;
    private int range;
    private bool isReturningToCeiling;
    private Rigidbody rb;
    private PlayerNetwork player;
    private bool isAttacking = false;

    [SerializeField] private float speed = 10f;
    [SerializeField] private float attackForce = 15f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform ceilingPosition;
    [SerializeField] private float ceilingReturnDistance = 1f;

    private float timeElapsed;
    private float timeAmbushCheck;
    
    

    void Awake()
    {
        fieldOfView = GetComponent<FieldOfView>();
        StartCoroutine(fieldOfView.FOVCoroutine());

        stateMachine = new StateMachine();
        rb = GetComponent<Rigidbody>();
        agent = GetComponent<NavMeshAgent>();

        walkpointSet = false;
        range = 8;
        rb.useGravity = false;
        isReturningToCeiling = false;

        timeElapsed = 0;
        timeAmbushCheck = 1f;
    }

    void Start()
    {
        var ambushState = new EnemyAmbushState(this, animator);
        var runAwayState = new EnemyRunAwayState(this, animator);

        
        // State transitions
        At(ambushState, runAwayState, new FuncPredicate(() => isReturningToCeiling && agent.isOnNavMesh));
        At(runAwayState, ambushState, new FuncPredicate(() => !isReturningToCeiling && agent.isOnNavMesh));
            
        // Initialize in ambush state, suspended from ceiling
        MoveToInitialCeilingPosition();
        stateMachine.SetState(ambushState);
    }

    private void MoveToInitialCeilingPosition()
    {
        // Move to ceiling position at start
        if (ceilingPosition != null)
        {
            transform.position = ceilingPosition.position;
            if (agent != null && agent.isActiveAndEnabled)
            {
                agent.Warp(ceilingPosition.position);
            }
        }
    }

    public override void Patrol()
    {
        if (!walkpointSet)
        {
            SetNextDest();
        }
        if (walkpointSet)
        {
            agent.SetDestination(dest);
        }
        if (Vector3.Distance(transform.position, dest) < 2)
        {
            walkpointSet = false;
        }
    }

    public override void Attack()
    {
        if (agent.isOnNavMesh && !isAttacking)
        {
            agent.destination = sensorDetector.Target.transform.position;
        }
    }

    public override void Ambush()
    {
        agent.speed = 3f;
        if (!walkpointSet)
        {
            SetNextDest();
        }
        if (walkpointSet)
        {
            agent.SetDestination(dest);
        }
        if (Vector3.Distance(transform.position, dest) < 2)
        {
            walkpointSet = false;
        }
        
        if (fieldOfView.Spotted && !isAttacking && !isReturningToCeiling)
        {

            if (player == null)
            {
                player = PlayerNetwork.LocalPlayer;
            }
            if (timeElapsed >= timeAmbushCheck)
            {
                if (player != null)
                {
                    RaycastHit hit;
                    Vector3 directionToPlayer = (player.transform.position - transform.position).normalized;
                
                    if (Physics.Raycast(transform.position, directionToPlayer, out hit, Mathf.Infinity))
                    {
                        if (hit.collider.CompareTag("Player"))
                        {
                            SpikeAttack();
                        }
                    }

                    timeElapsed = 0;
                }
            }
            else
            {
                timeElapsed += Time.deltaTime;
            }
        }
        else
        {
            if (timeElapsed>0)
            {
                timeElapsed -= Time.deltaTime;
            }
            else
            {
                timeElapsed = 0;
            }
        }
    }

    public override void RunAway()
    {
        agent.speed = 6f;
        // Check if agent is valid
        if (!agent.isOnNavMesh || isAttacking) 
        {
            return;
        }

        // Set speed to maximum for returning to ceiling
        
        // Ensure we're heading to the ceiling
        agent.SetDestination(ceilingPosition.position);
        
        // Debug info to track progress
        float distanceToCeiling = Vector3.Distance(transform.position, ceilingPosition.position);
        
        // If we're close enough to the ceiling, consider it reached
        if (distanceToCeiling < ceilingReturnDistance)
        {
            // Snap to exact ceiling position
            transform.position = ceilingPosition.position;
            transform.rotation = ceilingPosition.rotation;
            
            isReturningToCeiling = false;
            
            Debug.Log("RunAway: Reached ceiling position");
        }
        else if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            // If the path is complete but we're not at the ceiling, try direct teleport
            Debug.LogWarning("RunAway: Path complete but not at ceiling, teleporting");
            transform.position = ceilingPosition.position;
            agent.Warp(ceilingPosition.position);
            isReturningToCeiling = false;
        }
        else
        {
            // Still on the way to ceiling
            Debug.Log($"RunAway: Distance to ceiling: {distanceToCeiling:F2}, Remaining: {agent.remainingDistance:F2}");
        }
    }

    private void SetNextDest()
    {
        float z = Random.Range(-range, range);
        float x = Random.Range(-range, range);

        Vector3 randomPoint = transform.position + new Vector3(x, 0, z);

        while (!CanReachDestination(randomPoint))
        {
             z = Random.Range(-range, range);
             x = Random.Range(-range, range);

            randomPoint = transform.position + new Vector3(x, 0, z);
        }

        dest = new Vector3(transform.position.x + x, transform.position.y, transform.position.z + z);

        if (Physics.Raycast(dest, Vector3.down, out RaycastHit hit, Mathf.Infinity, groundLayer))
        {
            walkpointSet = true;
        }
    }

    private void SpikeAttack()
    {
        
        if (!isAttacking && agent.isActiveAndEnabled && agent.isOnNavMesh)
        {
            isAttacking = true;
            
            // Disable NavMeshAgent and enable gravity for the jump
            agent.enabled = false;
            rb.useGravity = true;
            rb.linearVelocity = Vector3.zero;

            // Calculate direction to player
            if (player == null)
            {
                player = PlayerNetwork.LocalPlayer;
            }
            
            // Calculate the attack direction
            Vector3 attackDirection = (player.transform.position - transform.position).normalized;

            // Add upward arc (optional)
            //attackDirection.y = 0.5f;
            attackDirection = attackDirection.normalized;

            // Apply force with fixed direction but scalable speed
            rb.linearVelocity = attackDirection * attackForce;
            
            StartCoroutine(ReturnToCeilingAfterAttack());
        }
    }

    IEnumerator ReturnToCeilingAfterAttack()
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
            
            isReturningToCeiling = true;
        }
        else
        {
            rb.useGravity = false;
            rb.linearVelocity = Vector3.zero;
            transform.position = ceilingPosition.position;
            agent.enabled = true;
            agent.Warp(ceilingPosition.position);
            isAttacking = false;
            isReturningToCeiling = false;
        }
    }

    bool IsGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, 1f, groundLayer);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player") && isAttacking)
        {
            IDamageable damageable = collision.gameObject.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(20, 0);
            }
        }

        isAttacking = false;
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

}