
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

    void Awake()
    {
        _currentNodeIndex = 0;
        lastPlayerPositionArray = new []{transform.position};
        lastPlayerPositionVisited = true;
        
        
        lookAroundTime = 5.5f;
        timeElapsed = 0f;
    }

    void Start()
    {
        _currentNode = _nodes[_currentNodeIndex];
    }
    
    public void Patrol()
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
    
    public void HuntDown()
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
    
    
    public void Investigate()
    {
        isInvestigating = true;
        agent.isStopped = true;
        agent.velocity = Vector3.zero;
        agent.updateRotation = false;

        if (agent.isStopped)
        {
            if (timeElapsed<=1.9f)
            {
                float rotationAmount = rotationSpeed * Time.deltaTime;
                transform.Rotate(Vector3.up, -rotationAmount);
                timeElapsed += Time.deltaTime;
            }
            else
            {
                float rotationAmount = rotationSpeed * Time.deltaTime;
                transform.Rotate(Vector3.up, rotationAmount);
                timeElapsed += Time.deltaTime;
            }
            
            if (timeElapsed >= lookAroundTime || FieldOfView.Spotted)
            {
                agent.updateRotation = true;
                agent.isStopped = false;
                lastPlayerPositionVisited = true;
                agent.updateRotation = true;
                timeElapsed = 0f;
                isInvestigating = false;
            }
        }
    }
}