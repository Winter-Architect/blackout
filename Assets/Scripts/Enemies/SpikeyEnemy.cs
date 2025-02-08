

using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class SpikeyEnemy : Enemy
{
    private Vector3 dest;
    private bool walkpointSet;
    private int range;
    private bool hasAmbushed;
    private bool isReturningToCeiling;
    private Rigidbody rb;
    private PlayerNetwork player;
    
    [SerializeField] private float jumpForce = 15f;
    [SerializeField] private float forwardForce = 5f;
    [SerializeField] private float speed = 20f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Vector3 ceilingPosition;
    
    void Awake()
    {
        fieldOfView = gameObject.GetComponent<FieldOfView>();
        StartCoroutine(fieldOfView.FOVCoroutine());
        
        sensorDetector = gameObject.GetComponent<SensorDetector>();
        StartCoroutine(sensorDetector.SensorDetectorCoroutine());
        
        stateMachine = new StateMachine();

        rb = GetComponent<Rigidbody>();
        
        walkpointSet = false;
        hasAmbushed = false;
        range = 8;
        rb.useGravity = false;
        isReturningToCeiling = false;

    }

    void Start()
    {
        var patrolState = new EnemyPatrolState(this, animator); 
        var huntDownState = new EnemyHuntDownState(this, animator);
        var ambushState = new EnemyAmbushState(this, animator);
        var runAwayState = new EnemyRunAwayState(this, animator);
        var attackState = new EnemyAttackState(this, animator);
        
        At(ambushState, runAwayState, new FuncPredicate(()=>isReturningToCeiling));
        At(runAwayState, ambushState, new FuncPredicate(()=>!isReturningToCeiling));
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
        if (Vector3.Distance(transform.position, dest)<2)
        {
            walkpointSet = false;
        }
    }
    

    public override void Attack()
    {
        agent.destination = sensorDetector.Target.transform.position;
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
                hasAmbushed = true;
            }
        }
    }

    public override void RunAway()
    {
        transform.position = Vector3.Lerp(transform.position, ceilingPosition, speed * Time.deltaTime);

        if (Vector3.Distance(transform.position, ceilingPosition) < 0.1f)
        {
            isReturningToCeiling = false; // Stop moving when close enough
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
        rb.useGravity = true;
        Vector3 attackDirection = (player.transform.position - transform.position).normalized;
    
        // Instead of setting the NavMeshAgent destination, use velocity
        rb.linearVelocity = Vector3.down * jumpForce + attackDirection * forwardForce; 
    }
    
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // Damage the player
            IDamageable player = collision.gameObject.GetComponent<IDamageable>();
            if (player != null)
            {
                player.TakeDamage(20, 0);
            }
        }
        else
        {
            // If it hits the ground instead of the player, return to the ceiling
            Invoke(nameof(ReturnToCeiling), 2f); // Wait 2 seconds before retreating
        }
    }

    void ReturnToCeiling()
    {
        isReturningToCeiling = true;
    }
}
