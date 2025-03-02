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
    private GameObject player;
    private bool isAttacking = false;

    [SerializeField] private float attackForce = 10f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform ceilingPosition;
    [SerializeField] private float ceilingReturnDistance = 1f;

    private float timeElapsed;
    private float timeAmbushCheck;

    private bool ambush = false;
    
    

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
        var patrolState = new EnemyPatrolState(this, animator);

        At(ambushState, runAwayState, new FuncPredicate(() => isReturningToCeiling && agent.isOnNavMesh));
        At(patrolState, ambushState, new FuncPredicate(() => ambush));
        At(runAwayState, patrolState, new FuncPredicate(() => !isReturningToCeiling));
        stateMachine.SetState(patrolState);
        
        MoveToInitialCeilingPosition();
    }
    
    private void MoveToInitialCeilingPosition()
    {
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

    public override void Attack()
    {
        if (agent.isOnNavMesh && !isAttacking)
        {
            agent.destination = sensorDetector.Target.transform.position;
        }
    }

    public override void Ambush()
    {
        if (player != null)
        {
            
            RaycastHit hit;
            Vector3 directionToPlayer = (player.transform.position - transform.position).normalized;
        
            if (Physics.Raycast(transform.position, directionToPlayer, out hit, Mathf.Infinity))
            {
                if (hit.collider.CompareTag("Player") || hit.collider.name == "Plane")
                {
                    SpikeAttack();
                }
                SpikeAttack();
            }

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
        agent.speed = 4f;

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
        
        isAttacking = true;
        agent.enabled = false;
        rb.useGravity = true;
        rb.linearVelocity = Vector3.zero;
        
        if (player == null)
        {
            player = fieldOfView.Target;
        }
        
        Vector3 attackDirection = (player.transform.position - transform.position).normalized;
        
        attackDirection = attackDirection.normalized;

        rb.linearVelocity = attackDirection * attackForce;
        
        StartCoroutine(ReturnToCeilingAfterAttack());
    }

    IEnumerator ReturnToCeilingAfterAttack()
    {
        rb.isKinematic = false;
        rb.useGravity = false;
        
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
        
        yield return new WaitForSeconds(0.3f);
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
        rb.isKinematic = true;
        if (collision.gameObject.CompareTag("Player") && isAttacking)
        {
            IDamageable damageable = collision.gameObject.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(20, 0);
            }
        }
        isAttacking = false;
        rb.isKinematic = false;
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