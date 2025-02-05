
using UnityEngine;

public class RottenSlime : Enemy
{
    private Vector3 dest;
    private bool walkpointSet;
    private int range;

    [SerializeField] private LayerMask groundLayer;
    
    void Awake()
    {
        fieldOfView = gameObject.GetComponent<FieldOfView>();
        StartCoroutine(fieldOfView.FOVCoroutine());
        
        sensorDetector = gameObject.GetComponent<SensorDetector>();
        StartCoroutine(sensorDetector.SensorDetectorCoroutine());
        
        stateMachine = new StateMachine();

        walkpointSet = false;
        range = 8;

    }

    void Start()
    {
        var patrolState = new EnemyPatrolState(this, animator); 
        var huntDownState = new EnemyHuntDownState(this, animator);
        var ambushState = new EnemyAmbushState(this, animator);
        var attackState = new EnemyAttackState(this, animator);
        
        At(ambushState, attackState, new FuncPredicate(()=>fieldOfView.Spotted && sensorDetector.Detected));
        At(attackState, patrolState, new FuncPredicate(()=>!sensorDetector.Detected));
        At(patrolState, attackState, new FuncPredicate(()=>fieldOfView.Spotted && sensorDetector.Detected));
        
        stateMachine.SetState(patrolState);
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
        throw new System.NotImplementedException();
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
}
