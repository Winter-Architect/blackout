
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
    
    
    public TestEnemy(float hp) : base(hp)
    {
    }
    
    
    void Awake()
    {
        fieldOfView = gameObject.GetComponent<FieldOfView>();
        StartCoroutine(fieldOfView.FOVCoroutine());
        
        _currentNodeIndex = 0;
        lastPlayerPositionArray = new []{transform.position};
        lastPlayerPositionVisited = true;
        
        
        lookAroundTime = 6.12f;
        timeElapsed = 0f;
        rotationSpeed = 30f;
        
        stateMachine = new StateMachine();
    }

    void Start()
    {
        _currentNode = _nodes[_currentNodeIndex];
        
        var patrolState = new EnemyPatrolState(this, animator); 
        var huntDownState = new EnemyHuntDownState(this, animator);
        var investigateState = new EnemyInvestigateState(this, animator);
        
        At(patrolState, huntDownState, new FuncPredicate(()=>fieldOfView.Spotted));
        At(huntDownState, patrolState, new FuncPredicate(()=>!fieldOfView.Spotted && lastPlayerPositionVisited));
        At(huntDownState, investigateState, new FuncPredicate(()=>!fieldOfView.Spotted && (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)));
        At(investigateState, patrolState, new FuncPredicate(()=>!isInvestigating));
        stateMachine.SetState(patrolState);
    }
    
    public override void Patrol()
    {
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
    }
    
    public override void HuntDown()
    {

        if (fieldOfView.Spotted)
        {
            if (lastPlayerPositionVisited)
            {
                lastPlayerPositionVisited = false;
            }
            lastPlayerPositionArray[0] = fieldOfView.Target.transform.position;
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

            if (timeElapsed >= lookAroundTime || fieldOfView.Spotted)
            {
                agent.updateRotation = true;
                agent.isStopped = false;
                lastPlayerPositionVisited = true;
                timeElapsed = 0f;
                isInvestigating = false;
            }
        }
    }
}