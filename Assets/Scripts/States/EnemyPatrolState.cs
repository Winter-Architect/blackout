using UnityEngine;
using UnityEngine.AI;

public class EnemyPatrolState : EnemyBaseState
{
    private NavMeshAgent agent;
    
    public EnemyPatrolState(Enemy enemy, Animator animator) : base(enemy, animator)
    {
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