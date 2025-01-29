using UnityEngine;
using UnityEngine.AI;

public class TurretEnemy : Enemy
{
    
    private float rotationSpeed;
    private float lookStraightTime;
    private float timeElapsed;

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

        
        stateMachine.SetState(patrolState);
    }

    // ERROR: One FieldOfView for all items
    
    public override void Patrol()
    {
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
            isInvestigating = false;
        }
    }

    public override void Attack()
    {
        base.Attack();
    }
}