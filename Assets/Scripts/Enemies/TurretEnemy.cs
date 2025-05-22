using System.Collections;
using System.Collections.Generic;
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
    
    private LineRenderer laserLine;

    private Laser laser;


    public bool isRaycastLaser;
    [SerializeField] private Transform laserPrefab;
    public bool isIdle;
    private Quaternion targetRotation;
    private float currentAngle;
    private float currentRotation;
    private bool rotatingRight;
    
    private float baseRotationY;  // new: to store the initial Y rotation
    private float minRotation;
    private float maxRotation;
    
    void Awake()
    {
        fieldOfView = gameObject.GetComponent<FieldOfView>();
        StartCoroutine(fieldOfView.FOVCoroutine());
        
        stateMachine = new StateMachine();
        
        lookStraightTime = 5.5f;
        timeElapsed = 0;

        delayFire = 1.5f;
        rotationSpeed = 40;
        
        baseRotationY = transform.rotation.eulerAngles.y;

        currentAngle = baseRotationY;
        rotatingRight = true;
        
        laserLine = GetComponent<LineRenderer>();
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeAll;
        isIdle = true;
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
        
        float rotationAmount = rotationSpeed * Time.deltaTime;

        if (isIdle) minRotation = baseRotationY - 45f;
        else
            minRotation = baseRotationY - 90f;
        
        if (isIdle) maxRotation = baseRotationY + 45f;
        else
            maxRotation = baseRotationY + 90f;

        if (rotatingRight)
        {
            currentAngle += rotationAmount;
            if (currentAngle >= maxRotation)
            {
                currentAngle = maxRotation;
                rotatingRight = false;
            }
        }
        else
        {
            currentAngle -= rotationAmount;
            if (currentAngle <= minRotation)
            {
                currentAngle = minRotation;
                rotatingRight = true;
            }
        }

        transform.rotation = Quaternion.Euler(0f, currentAngle, 0f);
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
        GoNavmesh();
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
                    damageable.TakeDamage(0.07f, 0);
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
            laserScript.Initialize(12.5f, 0, 25f, 4f);
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