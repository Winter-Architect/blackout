using UnityEngine;
using UnityEngine.AI;


public class EnemyInvestigateState : EnemyBaseState
{
    private NavMeshAgent agent;
    
    public EnemyInvestigateState(Enemy enemy, Animator animator, NavMeshAgent agent) : base(enemy, animator)
    {
        this.agent = agent;
    }
    
    public override void OnEnter()
    {
        Debug.Log("Investigate State");
    }

    public override void Update()
    {
        this.enemy.Investigate();
    }
    
}