
using System;
using UnityEngine;
using UnityEngine.AI;

public class TestEnemy : Enemy
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
    [SerializeField] private float timeElapsedHearing;

    private bool isHeard;
    
    public TestEnemy(float hp) : base(hp)
    {
    }
    
    
    void Awake()
    {
        fieldOfView = gameObject.GetComponent<FieldOfView>();
        StartCoroutine(fieldOfView.FOVCoroutine());
        
        sensorDetector = gameObject.GetComponent<SensorDetector>();
        StartCoroutine(sensorDetector.SensorDetectorCoroutine());
        
        
        _currentNodeIndex = 0;
        lastPlayerPositionArray = new []{transform.position};
        lastPlayerPositionVisited = true;
        
        
        lookAroundTime = 6.12f;
        timeElapsed = 0f;
        timeElapsedHearing = 0f;
        rotationSpeed = 30f;

        isHeard = false;
        
        stateMachine = new StateMachine();
        
    }

    void Start()
    {
        _currentNode = _nodes[_currentNodeIndex];
        var patrolState = new EnemyPatrolState(this, animator); 
        var huntDownState = new EnemyHuntDownState(this, animator);
        var investigateState = new EnemyInvestigateState(this, animator);
        
        At(patrolState, huntDownState, new FuncPredicate(()=>fieldOfView.Spotted || isHeard));
        At(huntDownState, patrolState, new FuncPredicate(()=>!fieldOfView.Spotted && lastPlayerPositionVisited));
        At(huntDownState, investigateState, new FuncPredicate(()=>!fieldOfView.Spotted && !isHeard && (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)));
        At(investigateState, patrolState, new FuncPredicate(()=>!isInvestigating));
        At(investigateState, huntDownState, new FuncPredicate(()=>fieldOfView.Spotted || isHeard));
        
        stateMachine.SetState(patrolState);
    }
    
    
    public override void Patrol()
    {
        if (isInvestigating)
        {
            SetEndInvestigateDatas();
        }
        
        if (agent.updateRotation == false)
        {
            agent.updateRotation = true;
        }
        if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
        {
            _currentNodeIndex = (_currentNodeIndex + 1) % _nodes.Length;
            _currentNode = _nodes[_currentNodeIndex];
        }
        agent.destination = _currentNode.transform.position;
        Listen();
    }
    
    public override void HuntDown()
    {
        if (isInvestigating)
        {
            SetEndInvestigateDatas();
        }

        if (isHeard)
        {
            lastPlayerPositionArray[0] = PlayerNetwork.LocalPlayer.transform.position;
            lastPlayerPositionVisited = false;
            timeElapsedHearing = 0;
        }
        
        if (fieldOfView.Spotted || isHeard)
        {
            if (lastPlayerPositionVisited)
            {
                lastPlayerPositionVisited = false;
            }
            lastPlayerPositionArray[0] = PlayerNetwork.LocalPlayer.transform.position;
            agent.destination = lastPlayerPositionArray[0];
        }
        else if (!lastPlayerPositionVisited)
        {
            agent.destination = lastPlayerPositionArray[0];
            if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
            {
                lastPlayerPositionVisited = true;
            }
        }
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
        Listen();
    }

    public void Listen()
    {
        if (sensorDetector.Detected)
        {
            if (PlayerNetwork.LocalPlayer.rb.linearVelocity.sqrMagnitude == 0f)
            {
                if (timeElapsedHearing>0)
                {
                    timeElapsedHearing -= Time.deltaTime/3;
                }
                else
                {
                    timeElapsedHearing = 0;
                }
            }
            else
            {
                if (timeElapsedHearing<1.5f)
                {
                    timeElapsedHearing += Time.deltaTime;
                }
            }
            if (timeElapsedHearing>=1.5f)
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
}