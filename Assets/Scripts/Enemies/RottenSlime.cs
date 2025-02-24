using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class RottenSlime : Enemy
{
    private Vector3 dest;
    private bool walkpointSet;
    private int range;
    private bool hasAmbushed;
    private bool isAttacking;
    private Rigidbody rb;
    private PlayerNetwork player;

    [SerializeField] private float jumpForce = 30f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float groundCheckDistance = 1f;
    [SerializeField] private float navMeshSearchRadius = 3f;

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
        range = 10;
        rb.useGravity = false;
    }

    void Start()
    {
        var patrolState = new EnemyPatrolState(this, animator);
        var ambushState = new EnemyAmbushState(this, animator);
        var attackState = new EnemyAttackState(this, animator);

        At(ambushState, attackState, new FuncPredicate(() => hasAmbushed && !isAttacking));
        At(attackState, patrolState, new FuncPredicate(() => !sensorDetector.Detected && agent.enabled));
        At(patrolState, attackState, new FuncPredicate(() => fieldOfView.Spotted && sensorDetector.Detected));
        
        stateMachine.SetState(ambushState);
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
        if (player != null && agent.isOnNavMesh && !isAttacking)
        {
            agent.destination = player.transform.position;
        }
    }

    public override void Ambush()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, Mathf.Infinity))
        {
            if (hit.collider.CompareTag("Player"))
            {
                player = PlayerNetwork.LocalPlayer;
                JumpAttack();
                
            }
        }
    }

    private void SetNextDest()
    {
        float z = Random.Range(-range, range);
        float x = Random.Range(-range, range);

        dest = new Vector3(transform.position.x + x, transform.position.y, transform.position.z + z);
        
        if (Physics.Raycast(dest, Vector3.down, groundLayer))
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
            Vector3 attackDirection = (player.transform.position - transform.position).normalized;
            attackDirection.y = 0.5f;
            rb.AddForce(attackDirection * jumpForce, ForceMode.Impulse);
            hasAmbushed = true;
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
    }
}
