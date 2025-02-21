
using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class RottenSlime : Enemy
{
    private Vector3 dest;
    private bool walkpointSet;
    private int range;
    private bool hasAmbushed;
    private Rigidbody rb;
    private PlayerNetwork player;
    
    //[SerializeField] private float jumpForce = 15f;
    //[SerializeField] private float forwardForce = 5f;
    [SerializeField] private LayerMask groundLayer;
    
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
        range = 10;
        rb.useGravity = false;

    }

    void Start()
    {
        var patrolState = new EnemyPatrolState(this, animator); 
        var ambushState = new EnemyAmbushState(this, animator);
        var attackState = new EnemyAttackState(this, animator);
        
        At(ambushState, attackState, new FuncPredicate(()=>hasAmbushed && agent.enabled && !rb.useGravity));
        At(attackState, patrolState, new FuncPredicate(()=>!sensorDetector.Detected && agent.enabled && !rb.useGravity));
        At(patrolState, attackState, new FuncPredicate(()=>fieldOfView.Spotted && sensorDetector.Detected && agent.enabled && !rb.useGravity));
        
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
        agent.destination = player.transform.position;
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
            agent.enabled = false;
            rb.useGravity = true;
            rb.linearVelocity = Vector3.zero;
            rb.AddForce(Vector3.down * 3f, ForceMode.VelocityChange);
            hasAmbushed = true;
        }
    }

    IEnumerator ReEnableAgent()
    {
        yield return new WaitForSeconds(0.5f);
        agent.enabled = true;
        rb.useGravity = false;
        rb.linearVelocity = Vector3.zero;
    }

    void OnCollisionStay(Collision collision)
    {
        if (!agent.enabled)
        {
            StartCoroutine(ReEnableAgent());
        }

        if (collision.gameObject.CompareTag("Player"))
        {
            // Damage the player
            IDamageable player = collision.gameObject.GetComponent<IDamageable>();
            if (player != null)
            {
                player.TakeDamage(20, 0);
            }
        }
    }
}
