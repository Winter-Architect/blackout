using UnityEngine;
using UnityEngine.AI;

public class EnemyRunAwayState : EnemyBaseState
{
    private NavMeshAgent agent;
    
    public EnemyRunAwayState(Enemy enemy, Animator animator) : base(enemy, animator)
    {
    }

    public override void OnEnter()
    {
        Debug.Log("Run Away State");
    }

    public override void Update()
    {
        this.enemy.RunAway();
    }
}