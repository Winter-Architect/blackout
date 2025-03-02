using UnityEngine;
using UnityEngine.AI;

public class EnemyHuntDownState : EnemyBaseState
{
    private NavMeshAgent agent;

    public EnemyHuntDownState(Enemy enemy, Animator animator) : base(enemy, animator)
    {
    }

    public override void OnEnter()
    {
        Debug.Log("Hunt Down state");
    }

    public override void Update()
    {
        this.enemy.HuntDown();
    }
}