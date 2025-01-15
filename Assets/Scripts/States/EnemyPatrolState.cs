using UnityEngine;
using UnityEngine.AI;

public class EnemyPatrolState : EnemyBaseState
{
    private NavMeshAgent agent;
    
    public EnemyPatrolState(Enemy enemy, Animator animator, NavMeshAgent agent) : base(enemy, animator)
    {
        this.agent = agent;
    }

    public override void OnEnter()
    {
        Debug.Log("Patrol State");
    }

    public override void Update()
    {
        this.enemy.Patrol();
    }
}