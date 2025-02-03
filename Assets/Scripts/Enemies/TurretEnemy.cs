using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class TurretEnemy : Enemy
{
    
    private float rotationSpeed;
    private float lookStraightTime;
    private float timeElapsed;

    private float timeElapseBetweenFire;
    private float delayFire;
    

    private Laser laser;
    
    [SerializeField] private Transform laserPrefab;
    
    public TurretEnemy(float hp) : base(hp)
    {
    }

    void Awake()
    {
        fieldOfView = gameObject.GetComponent<FieldOfView>();
        StartCoroutine(fieldOfView.FOVCoroutine());
        
        stateMachine = new StateMachine();

        agent.isStopped = true;
        agent.velocity = Vector3.zero;
        agent.updateRotation = false;
        lookStraightTime = 5.5f;
        timeElapsed = 0;

        rotationSpeed = 60;

    }
    
    void Start()
    {

        var patrolState = new EnemyPatrolState(this, animator);
        var attackState = new EnemyAttackState(this, animator);
        var investigateState = new EnemyInvestigateState(this, animator);
        
        At(patrolState, attackState, new FuncPredicate(()=>fieldOfView.Spotted));
        At(attackState, investigateState, new FuncPredicate(()=>!fieldOfView.Spotted));
        At(investigateState, patrolState, new FuncPredicate(()=>!fieldOfView.Spotted && !isInvestigating));
        At(investigateState, attackState, new FuncPredicate(()=>fieldOfView.Spotted));

        
        stateMachine.SetState(patrolState);
    }
    
    public override void Patrol()
    {
        timeElapsed = 0;
        timeElapseBetweenFire = 0;
        
        float rotationAmount = (rotationSpeed) * Time.deltaTime;
        this.transform.Rotate(Vector3.up, rotationAmount);
    }

    public override void Investigate()
    {
        if (!isInvestigating)
        {
            isInvestigating = true;
        }
        timeElapsed += Time.deltaTime;
        if (timeElapsed>=lookStraightTime)
        {
            timeElapsed = 0;
            isInvestigating = false;
        }
    }

    public override void Attack()
    {
        if (isInvestigating)
        {
            timeElapsed = 0;
            isInvestigating = false;
        }
        this.transform.LookAt(fieldOfView.Target.transform);

        timeElapseBetweenFire += Time.deltaTime;
        if (timeElapseBetweenFire >= delayFire)
        {
            timeElapseBetweenFire = 0;
            FireLaser();
        }
    }
    
    private void FireLaser()
    {
        Transform laserInstance = Instantiate(laserPrefab, this.transform.position, this.transform.rotation);
        
        Laser laserScript = laserInstance.GetComponent<Laser>();
        if (laserScript)
        {
            laserScript.Initialize(10, 0, 10f, 4f); // Set laser properties
        }
    }
    
}