
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ZombZomb : Enemy
{
    //////// For the pratrol /////////
    [SerializeField] private Transform[] _nodes;
    private Transform _currentNode; 
    private int _currentNodeIndex;
    protected bool lastPlayerPositionVisited;
    private Vector3[] lastPlayerPositionArray;
    //////////////////////////////////
    
    private float rotationSpeed;
    private float lookAroundTime;
    private float timeElapsed;
    private GameObject target;
    [SerializeField] private float timeElapsedHearing;

    private bool isHeard;
    
    private bool _isWaitingForNextNode;

    void Awake()
    {
        fieldOfView = gameObject.GetComponent<FieldOfView>();
        StartCoroutine(fieldOfView.FOVCoroutine());
        
        sensorDetector = gameObject.GetComponent<SensorDetector>();
        StartCoroutine(sensorDetector.SensorDetectorCoroutine());
        
        
        _currentNodeIndex = 0;
        lastPlayerPositionArray = new []{transform.position};
        lastPlayerPositionVisited = true;
        
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.constraints = RigidbodyConstraints.FreezeAll;
        
        lookAroundTime = 6.12f;
        timeElapsed = 0f;
        timeElapsedHearing = 0f;
        rotationSpeed = 30f;

        isHeard = false;
        _isWaitingForNextNode = false;
            
        stateMachine = new StateMachine();
    }

    void Start()
    {
        target = null;
        
        _currentNode = _nodes[_currentNodeIndex];
        var patrolState = new EnemyPatrolState(this, animator); 
        var huntDownState = new EnemyHuntDownState(this, animator);
        var investigateState = new EnemyInvestigateState(this, animator);
        
        At(patrolState, huntDownState, new FuncPredicate(()=>fieldOfView.Spotted));
        At(huntDownState, patrolState, new FuncPredicate(()=>!fieldOfView.Spotted && lastPlayerPositionVisited));
        At(huntDownState, investigateState, new FuncPredicate(()=>!fieldOfView.Spotted && !isHeard && (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)));
        At(investigateState, patrolState, new FuncPredicate(()=>!isInvestigating));
        At(patrolState, huntDownState, new FuncPredicate(() => isHeard));
        At(investigateState, huntDownState, new FuncPredicate(()=>fieldOfView.Spotted && isHeard));
        
        stateMachine.SetState(patrolState);
    }
    
    
    public override void Patrol()
    {
        if (!target)
        {
            if (fieldOfView.Target)
            {
                target = fieldOfView.Target;
            }

            if (sensorDetector.Target)
            {
                target = sensorDetector.Target;
            }
        }
        
        if (isInvestigating)
        {
            SetEndInvestigateDatas();
        }
        
        if (agent.updateRotation == false)
        {
            agent.updateRotation = true;
        }
        if (!agent.pathPending && agent.remainingDistance < 0.3f && !_isWaitingForNextNode)
        {
            StartCoroutine(NewDest());
        }

        GoNavmesh();
        Listen();
    }



    public override void HuntDown()
    {
        if (!target)
        {
            return;
        }
        if (isInvestigating)
        {
            SetEndInvestigateDatas();
        }

        if (isHeard)
            timeElapsedHearing = 0;
        
        if (fieldOfView.Spotted || isHeard)
        {
            if (lastPlayerPositionVisited)
            {
                lastPlayerPositionVisited = false;
            }
            lastPlayerPositionArray[0] = target.transform.position;
            agent.destination = lastPlayerPositionArray[0];
        }
        else if (!lastPlayerPositionVisited)
        {
            NavMeshHit hit;
            if (NavMesh.SamplePosition(lastPlayerPositionArray[0], out hit, 2.0f, NavMesh.AllAreas))
            {
                lastPlayerPositionArray[0] = hit.position;
            }
            agent.destination = lastPlayerPositionArray[0];
            
            if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
            {
                lastPlayerPositionVisited = true;
            }
        }
        GoNavmesh();
        Listen();
    }


    public override void Investigate()
    {
        isInvestigating = true;
        agent.isStopped = true;
        agent.velocity = Vector3.zero;
        agent.updateRotation = false;
        
        if (agent.isStopped)
        {
            if (timeElapsed<=2.6f)
            {
                float rotationAmount = rotationSpeed * Time.deltaTime;
                this.transform.Rotate(Vector3.up, -rotationAmount);
                timeElapsed += Time.deltaTime;
            }
            else if (timeElapsed<=3.6f)
            {
                float rotationAmount = (rotationSpeed+35) * Time.deltaTime;
                this.transform.Rotate(Vector3.up, rotationAmount);
                timeElapsed += Time.deltaTime;
            }
            else
            {
                float rotationAmount = rotationSpeed * Time.deltaTime;
                this.transform.Rotate(Vector3.up, rotationAmount);
                timeElapsed += Time.deltaTime;
            }

            if (timeElapsed >= lookAroundTime)
            {
                SetEndInvestigateDatas();
            }
        }
        GoNavmesh();
        Listen();
    }

    public void Listen()
    {
        if (sensorDetector.Detected)
        {
            if (!target)
            {
                target = sensorDetector.Target;
            }
            if (timeElapsedHearing<2f)
            {
                timeElapsedHearing += Time.deltaTime;
            }
            else
            {
                isHeard = true;
            }

        }
        else
        {
            if (isHeard)
            {
                isHeard = false;
            }
            if (timeElapsedHearing>0)
            {
                timeElapsedHearing -= Time.deltaTime/2.35f; 
            }
            else
            {
                timeElapsedHearing = 0;
            }
        }
    }

    public void SetEndInvestigateDatas()
    {
        agent.updateRotation = true;
        agent.isStopped = false;
        lastPlayerPositionVisited = true;
        timeElapsed = 0f;
        isInvestigating = false;
    }
    
    public void InitializePath(List<Transform> nodes)
    {
        _nodes = new Transform[nodes.Count];
        int i = 0;
        foreach (var node in nodes)
        {
            _nodes[i] = node;
            i++;
        }
    }
    
    IEnumerator NewDest()
    {
        _isWaitingForNextNode = true;
        yield return new WaitForSeconds(1f);
    
        _currentNodeIndex = (_currentNodeIndex + 1) % _nodes.Length;
        _currentNode = _nodes[_currentNodeIndex];
        agent.destination = _currentNode.position;

        _isWaitingForNextNode = false;
    }
    
}