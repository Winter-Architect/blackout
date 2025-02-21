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

    [SerializeField] private float speed;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform ceilingPosition;

    void Awake()
    {
        fieldOfView = GetComponent<FieldOfView>();
        StartCoroutine(fieldOfView.FOVCoroutine());

        sensorDetector = GetComponent<SensorDetector>();
        StartCoroutine(sensorDetector.SensorDetectorCoroutine());

        stateMachine = new StateMachine();
        rb = GetComponent<Rigidbody>();

        walkpointSet = false;
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

        At(ambushState, runAwayState, new FuncPredicate(() => isReturningToCeiling && agent.isOnNavMesh));
        At(runAwayState, ambushState, new FuncPredicate(() => !isReturningToCeiling && agent.isOnNavMesh));
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
        agent.destination = sensorDetector.Target.transform.position;
    }

    public override void Ambush()
    {
        if (sensorDetector.Detected && fieldOfView.Spotted)
        {
            if (player is null)
            {
                player = PlayerNetwork.LocalPlayer;
            }
            RaycastHit hit;
            if (Physics.Raycast(transform.position, (player.transform.position - transform.position).normalized, out hit, Mathf.Infinity))
            {
                if (hit.collider.CompareTag("Player"))
                {
                    SpikeAttack();
                }
            }
        }
    }

    public override void RunAway()
    {
        if (!agent.isOnNavMesh) return;

        agent.SetDestination(ceilingPosition.position);

        if (!agent.hasPath || agent.velocity.sqrMagnitude < 0.1f)
        {
            isReturningToCeiling = false;
        }
    }

    private void SetNextDest()
    {
        float z = Random.Range(-range, range);
        float x = Random.Range(-range, range);

        dest = new Vector3(transform.position.x + x, transform.position.y, transform.position.z + z);

        if (Physics.Raycast(dest, Vector3.down, out RaycastHit hit, Mathf.Infinity, groundLayer))
        {
            walkpointSet = true;
        }
    }

    private void SpikeAttack()
    {
        if (agent.isOnNavMesh)
        {
            agent.enabled = false;
            rb.useGravity = true;
            rb.velocity = Vector3.zero;

            Vector3 attackDirection = (player.transform.position - transform.position).normalized;
            rb.AddForce(attackDirection * 10f, ForceMode.VelocityChange);

            StartCoroutine(WaitThenReturn());
        }
    }

    IEnumerator WaitThenReturn()
    {
        yield return new WaitForSeconds(0.5f); // Attendre avant de vérifier

        yield return StartCoroutine(WaitUntilGrounded()); // Attendre qu'il touche le sol

        // Vérifier si on est sur un NavMesh
        NavMeshHit hit;
        if (NavMesh.SamplePosition(transform.position, out hit, 2f, NavMesh.AllAreas))
        {
            transform.position = hit.position;
        }

        agent.enabled = true;
        rb.useGravity = false;
        rb.velocity = Vector3.zero;

        ReturnToCeiling();
    }

    IEnumerator WaitUntilGrounded()
    {
        while (!IsGrounded())
        {
            yield return null;
        }
    }

    bool IsGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 1f, groundLayer);
    }

    void OnCollisionStay(Collision collision)
    {
        if (!agent.enabled)
        {
            StartCoroutine(WaitThenReturn());
        }
        if (collision.gameObject.CompareTag("Player"))
        {
            IDamageable player = collision.gameObject.GetComponent<IDamageable>();
            if (player != null)
            {
                player.TakeDamage(20, 0);
            }
        }
    }

    void ReturnToCeiling()
    {
        if (agent.isOnNavMesh)
        {
            agent.SetDestination(ceilingPosition.position);
            isReturningToCeiling = true;
        }
        else
        {
            Debug.Log("L'agent n'est pas sur le NavMesh, tentative de repositionnement...");
            StartCoroutine(WaitThenReturn());
        }
    }
}
