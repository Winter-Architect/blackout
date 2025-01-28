
using UnityEngine;
using UnityEngine.AI;

public class TestEnemy : Enemy
{
    //////// For the pratrol /////////
    [SerializeField] private Transform[] _nodes;
    private Transform _currentNode; 
    private int _currentNodeIndex;
    
    private Vector3[] lastPlayerPositionArray;
    private FieldOfView fieldOfView;
    //////////////////////////////////
    
    private float rotationSpeed;
    private float lookAroundTime;
    private float timeElapsed;
    
    
    public TestEnemy(float hp) : base(hp)
    {
    }

    protected void At(IState from, IState to, IPredicate predicate)
    {
        stateMachine.AddTransition(from, to, predicate);
    }
    
    protected void Any(IState to, IPredicate predicate)
    {
        stateMachine.AddAnyTransition(to, predicate);
    }
    
    void Awake()
    {
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

        var patrolState = new EnemyPatrolState(this, animator, agent); 
        var huntDownState = new EnemyHuntDownState(this, animator, agent);
        var investigateState = new EnemyInvestigateState(this, animator, agent);
        
        At(patrolState, huntDownState, new FuncPredicate(()=>FieldOfView.Spotted));
        At(huntDownState, patrolState, new FuncPredicate(()=>!FieldOfView.Spotted && lastPlayerPositionVisited));
        At(huntDownState, investigateState, new FuncPredicate(()=>!FieldOfView.Spotted && (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)));
        At(investigateState, patrolState, new FuncPredicate(()=>!isInvestigating));
        stateMachine.SetState(patrolState);
    }
    
    void Update()
    {
        stateMachine.Update();
    }

    protected virtual void FixedUpdate()
    {
        stateMachine.FixedUpdate(); 
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

        if (FieldOfView.Spotted)
        {
            if (lastPlayerPositionVisited)
            {
                lastPlayerPositionVisited = false;
            }
            lastPlayerPositionArray[0] = FieldOfView.Target.transform.position;
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
                Debug.Log("Here");
                float rotationAmount = rotationSpeed * Time.deltaTime;
                this.transform.Rotate(Vector3.up, -rotationAmount);
                timeElapsed += Time.deltaTime;
            }
            else if (timeElapsed<=3.6f)
            {
                Debug.Log("Here");
                float rotationAmount = (rotationSpeed+35) * Time.deltaTime;
                this.transform.Rotate(Vector3.up, rotationAmount);
                timeElapsed += Time.deltaTime;
            }
            else
            {
                Debug.Log("There");
                float rotationAmount = rotationSpeed * Time.deltaTime;
                this.transform.Rotate(Vector3.up, rotationAmount);
                timeElapsed += Time.deltaTime;
            }

            if (timeElapsed >= lookAroundTime || FieldOfView.Spotted)
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