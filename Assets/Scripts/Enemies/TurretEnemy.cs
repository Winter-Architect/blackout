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


    [SerializeField] private bool isRaycastLaser;
    [SerializeField] private Transform laserPrefab;
    

    void Awake()
    {
        fieldOfView = gameObject.GetComponent<FieldOfView>();
        StartCoroutine(fieldOfView.FOVCoroutine());
        
        stateMachine = new StateMachine();
        
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
        if (!IsServer)
        {
            return;
        }
        if (isRaycastLaser)
        {
            if (!fieldOfView.Target) return;
            this.transform.LookAt(fieldOfView.Target.transform);
            FireLaserRaycast();
        }
        else
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
                FireLaser();
                timeElapseBetweenFire = 0;
            }
        }
    }
    
    private void FireLaser()
    {
        if (IsServer)
        {
            Transform laserInstance = Instantiate(laserPrefab, transform.position, transform.rotation);

            NetworkObject laserNetworkObject = laserInstance.GetComponent<NetworkObject>();
            if (laserNetworkObject is not null)
            {
                laserNetworkObject.Spawn();
            }

            Laser laserScript = laserInstance.GetComponent<Laser>();
            if (laserScript is not null)
            {
                laserScript.Initialize(40, 0, 25f, 4f);
            }

            FireLaserClientRpc(transform.position, transform.rotation);
        }
        else
        {
            FireLaserServerRpc();
        }
    }
    
    private void FireLaserRaycast()
    {
        laserLine.enabled = true;
        RaycastHit hit;

        laserLine.SetPosition(0, transform.position);

        float distanceToTarget = Vector3.Distance(transform.position, fieldOfView.Target.transform.position);
        
        
        Vector3 laserEndPoint = transform.position + transform.forward * distanceToTarget;

        if (Physics.Raycast(transform.position, (fieldOfView.Target.transform.position - transform.position).normalized, out hit))
        {
            if (hit.collider)
            {
                laserLine.SetPosition(1, fieldOfView.Target.transform.position);
                IDamageable damageable = hit.collider.GetComponent<IDamageable>();
                if (damageable != null)
                {
                    damageable.TakeDamage(0.05f, 0);
                }
            }
        }
        else
        {
            laserLine.SetPosition(1, laserEndPoint);
        }

        UpdateLaserPositionClientRpc(laserLine.GetPosition(0), laserLine.GetPosition(1));
    }

    [ServerRpc]
    private void FireLaserServerRpc()
    {
        FireLaser();
    }
    
    [ClientRpc]
    private void FireLaserClientRpc(Vector3 position, Quaternion rotation)
    {
        Transform laserInstance = Instantiate(laserPrefab, position, rotation);

        Laser laserScript = laserInstance.GetComponent<Laser>();
        if (laserScript is not null)
        {
            laserScript.Initialize(40, 0, 25f, 4f);
        }
    }
    
    [ClientRpc]
    private void UpdateLaserPositionClientRpc(Vector3 start, Vector3 end)
    {
        laserLine.enabled = true;
        laserLine.SetPosition(0, start);
        laserLine.SetPosition(1, end);
    }

    
}