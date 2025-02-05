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
    
    [SerializeField] private LineRenderer laserLine;
    [SerializeField] private LayerMask hitLayers;
    [SerializeField] private LayerMask obstacleMask;
    
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

        delayFire = 1.5f;
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
        laserLine.enabled = false;
        
        float rotationAmount = (rotationSpeed) * Time.deltaTime;
        this.transform.Rotate(Vector3.up, rotationAmount);
    }

    public override void Investigate()
    {
        if (!isInvestigating)
        {
            isInvestigating = true;
        }
        
        laserLine.enabled = false;
        
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
            FireLaserRaycastServerRpc();
            timeElapseBetweenFire = 0;
            
        }
    }
    
    private void FireLaser()
    {
        Transform laserInstance = Instantiate(laserPrefab, this.transform.position, this.transform.rotation);

        NetworkObject laserNetworkObject = laserInstance.GetComponent<NetworkObject>();
        if (laserNetworkObject is not null && IsServer)
        {
            laserNetworkObject.Spawn();
        }

        Laser laserScript = laserInstance.GetComponent<Laser>();
        if (laserScript is not null)
        {
            laserScript.Initialize(10, 0, 25f, 2f);
        }
    }

    [ServerRpc]
    public void FireLaserRaycastServerRpc()
    {
        FireLaserRaycast();
    }

    private void FireLaserRaycast()
    {
        RaycastHit hit;
        
        float distanceToTarget =  Vector3.Distance(transform.position, PlayerNetwork.LocalPlayer.transform.position);
        
        Transform thisTransform = gameObject.transform;
        Vector3 laserEndPoint = thisTransform.position + thisTransform.forward * distanceToTarget;
        
        if (Physics.Raycast(thisTransform.position, thisTransform.forward, out hit, distanceToTarget, hitLayers))
        {
            laserEndPoint = hit.point;
            
            IDamageable damageable = hit.collider.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(10, 0);
            }
        }
        
        laserLine.enabled = true;
        laserLine.SetPosition(0, thisTransform.position);
        laserLine.SetPosition(1, laserEndPoint);
    }
}