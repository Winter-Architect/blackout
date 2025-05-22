using System.Collections;
using UnityEditor.UI;
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
    private GameObject player;
    private bool isAttacking = false;

    [SerializeField] private float attackForce = 30f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform ceilingPosition;
    [SerializeField] private float ceilingReturnDistance = 1f;
    private float lastAttackTime = -Mathf.Infinity;
    private float attackCooldown = 4.5f;
    
    private float timeElapsed;
    private float timeAmbushCheck;

    private bool ambush = false;
    private bool home = false;
    private Vector3 lastPositionCheck;
    private float lastPositionCheckTime;
    private float movementCheckInterval = 1f;
    private float minimumMovementThreshold = 0.2f;
    private bool isStuck = false;

    
    
    void Awake()
    {
        fieldOfView = GetComponent<FieldOfView>();
        StartCoroutine(fieldOfView.FOVCoroutine());

        stateMachine = new StateMachine();
        rb = GetComponent<Rigidbody>();
        agent = GetComponent<NavMeshAgent>();

        walkpointSet = false;
        range = 2;
        rb.useGravity = false;
        isReturningToCeiling = false;

        timeElapsed = 0;
        timeAmbushCheck = 1.5f;
        agent.speed = 1f;
        
        lastPositionCheck = transform.position;
        lastPositionCheckTime = Time.time;
    }

    void Start()
    {
        var ambushState = new EnemyAmbushState(this, animator);
        var runAwayState = new EnemyRunAwayState(this, animator);
        var patrolState = new EnemyPatrolState(this, animator);

        At(ambushState, runAwayState, new FuncPredicate(() => isReturningToCeiling && agent.isOnNavMesh));
        At(patrolState, ambushState, new FuncPredicate(() => ambush));
        At(runAwayState, patrolState, new FuncPredicate(() => !isReturningToCeiling));
        stateMachine.SetState(patrolState);
        
        MoveToInitialCeilingPosition();
    }
    
    public void Initialized(Transform ceilingPos)
    {
        ceilingPosition = ceilingPos;
        MoveToInitialCeilingPosition();
    }
    

    public override void Patrol()
    {
        GoNavmesh();
        Debug.Log("PATROL");
        CheckMovement();
        
        if (!home)
        {
            Debug.Log("------ -1 ------");
            MoveToInitialCeilingPosition();
        }
        else if (!walkpointSet)
        {
            Debug.Log("------ 0 ------");
            SetNextDest();
        }
        else if (walkpointSet)
        {
            Debug.Log("------ 1 ------ Debug.Log(\"Is on Navmesh:\")" + agent.isOnNavMesh + agent.hasPath);
            agent.enabled = true;
            agent.SetDestination(dest);
        }
        else if (Vector3.Distance(transform.position, dest) < 1)
            walkpointSet = false;

        if (fieldOfView.Spotted)
        {
            if (player == null)
            {
                player = fieldOfView.Target;
            }
            if (timeElapsed >= timeAmbushCheck)
            {
                ambush = true;
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
    
    public override void Ambush()
    {
        if (player != null)
        {
            Debug.Log("AMBUSH");
            SpikeAttack();
            timeElapsed = 0;
            ambush = false;
        }
        else
        {
            player = fieldOfView.Target;
        }
    }

    public override void RunAway()
    {
        CheckMovement();
        GoNavmesh();
        if (Vector3.Distance(transform.position, dest) < 1 || isStuck || agent.isStopped)
            walkpointSet = false;
        Debug.Log("RUN AWAY");

        agent.speed = 4f;
        timeElapsed = 0;
        timeAmbushCheck = 1.5f;
        
        if (isAttacking)
        {
            return;
        }
        agent.SetDestination(ceilingPosition.position);
            
        float distanceToCeiling = Vector3.Distance(transform.position, ceilingPosition.position);
        
        if (distanceToCeiling < ceilingReturnDistance)
        {
            transform.position = ceilingPosition.position;
            transform.rotation = ceilingPosition.rotation;
            
            isReturningToCeiling = false;
        }
        else if (!agent.pathPending && agent.remainingDistance < 0.2f)
        {
            transform.position = ceilingPosition.position;
            agent.Warp(ceilingPosition.position);
            isReturningToCeiling = false;
        }
        
    }
    
    private void SpikeAttack()
    {
        isAttacking = true;
        agent.enabled = false;
        rb.useGravity = true;
        rb.linearVelocity = Vector3.zero;
        
        if (player == null)
            player = fieldOfView.Target;

        Vector3 start = transform.position;
        Vector3 end = player.transform.position;

        float gravity = Physics.gravity.y; // Should be negative
        float heightOffset = 2f;           // How high the jump goes
        float heightDifference = end.y - start.y;

        // Horizontal distance and direction
        Vector3 displacementXZ = new Vector3(end.x - start.x, 0, end.z - start.z);
        float distance = displacementXZ.magnitude;

        float initialHeight = Mathf.Max(heightOffset, heightDifference + 1f); // Ensure clearance

        // Compute time using height = 0.5 * g * t^2 → t = sqrt(2h / -g)
        float timeToApex = Mathf.Sqrt(2 * initialHeight / -gravity);
        float totalTime = timeToApex + Mathf.Sqrt(2 * (initialHeight - heightDifference) / -gravity);

        // Calculate velocities
        Vector3 velocityY = Vector3.up * Mathf.Sqrt(-2 * gravity * initialHeight);
        Vector3 velocityXZ = displacementXZ / totalTime;

        // Apply velocity
        rb.linearVelocity = velocityXZ + velocityY;

    }

    IEnumerator ReturnToCeilingAfterAttack()
    {
        yield return new WaitForSeconds(2f);
        
        yield return new WaitForSeconds(0.3f);
        
        rb.isKinematic = false;
        rb.useGravity = false;
        
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
        
        yield return new WaitForSeconds(0.3f);
        NavMeshHit hit;
        if (NavMesh.SamplePosition(transform.position, out hit, 10f, NavMesh.AllAreas))
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

        if (agent.isOnNavMesh)
        {
            isReturningToCeiling = true;
        }
        else
        {
            isReturningToCeiling = true;
            StartCoroutine(ReturnToCeilingAfterAttack());
        }
    }

    bool IsGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, 1f, groundLayer);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (Time.time - lastAttackTime < attackCooldown) return;

        rb.linearVelocity = Vector3.zero;
        rb.isKinematic = false;

        if (collision.gameObject.CompareTag("Player") && isAttacking)
        {
            IDamageable damageable = collision.gameObject.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(10, 0);
                lastAttackTime = Time.time;
            }
        }

        rb.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation;

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

    private void CheckMovement()
    {
        if (Time.time - lastPositionCheckTime >= movementCheckInterval)
        {
            float distanceMoved = Vector3.Distance(transform.position, lastPositionCheck);

            if (distanceMoved < minimumMovementThreshold)
            {
                isStuck = true;
            }

            lastPositionCheck = transform.position;
            lastPositionCheckTime = Time.time;
        }
    }
    
    private void MoveToInitialCeilingPosition()
    {
        if (ceilingPosition != null)
        {
            transform.position = ceilingPosition.position;
            if (agent != null && agent.isActiveAndEnabled)
            {
                NavMeshHit navMeshHit;
                if (NavMesh.SamplePosition(transform.position, out navMeshHit, 5f, NavMesh.AllAreas))
                {
                    agent.Warp(navMeshHit.position);
                    isReturningToCeiling = false;
                    home = true;
                }

                home = false;
            }
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

        Debug.Log("Is on Navmesh:" + agent.isOnNavMesh);
        dest = new Vector3(transform.position.x + x, transform.position.y, transform.position.z + z);
        NavMeshHit navMeshHit;
        if (NavMesh.SamplePosition(dest, out navMeshHit, 3.0f, NavMesh.AllAreas))
        {
            Debug.Log("Is inside");

            dest = navMeshHit.position; 
            walkpointSet = true;

        }
        if (Physics.Raycast(dest, Vector3.down, out RaycastHit hit, Mathf.Infinity, groundLayer))
        {
            Debug.Log("Is walkpointSet true:" + walkpointSet);
            walkpointSet = true;
            isStuck = false;
        }
    }

    
    protected new virtual void GoNavmesh()
    {
        NavMeshHit hit;
        if (NavMesh.SamplePosition(transform.position, out hit, 10.0f, NavMesh.AllAreas))
        {
            agent.Warp(hit.position); 
        }
    }
}