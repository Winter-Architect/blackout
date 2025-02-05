using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public class TurretEnemy : Enemy
{
    
    private float rotationSpeed;
    private float lookStraightTime;
    private float timeElapsed;

    private float timeElapseBetweenFire;
    private float delayFire;
    
    private LineRenderer laserLine;

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

        laserLine = GetComponent<LineRenderer>();

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
        Any(patrolState, new FuncPredicate(()=>PlayerNetwork.LocalPlayer is null));

        
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
            // FireLaser();
            timeElapseBetweenFire = 0;
            
        }
        FireLaserRaycast();

        /*
        if (PlayerNetwork.LocalPlayer is not null)
        {
            FireLaserRaycast();
        }
        */
        
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
            laserScript.Initialize(40, 0, 25f, 4f);
        }
    }

    [ServerRpc]
    public void FireLaserRaycastServerRpc()
    {
        FireLaserRaycast();
    }

    private void FireLaserRaycast()
    {
        laserLine.enabled = true;
        RaycastHit hit;

        laserLine.SetPosition(0, transform.position);
        
        float distanceToTarget = Vector3.Distance(transform.position, PlayerNetwork.LocalPlayer.transform.position);
    
        Transform thisTransform = gameObject.transform;
        Vector3 laserEndPoint = thisTransform.position + thisTransform.forward * distanceToTarget;
    
        if (Physics.Raycast(thisTransform.position, (PlayerNetwork.LocalPlayer.transform.position - transform.position).normalized, out hit))
        {
            if (hit.collider)
            {
                laserLine.SetPosition(1, hit.point);
            }

            IDamageable damageable = hit.collider.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(0.05f, 0);
            }
        }            
        else
        {
            laserLine.SetPosition(1, PlayerNetwork.LocalPlayer.transform.position);
        }
    }
}