using UnityEngine;

public class EnemyAttackState : EnemyBaseState
{
    public EnemyAttackState(Enemy enemy, Animator animator) : base(enemy, animator)
    {
    }

    public override void OnEnter()
    {
        Debug.Log("Attack State");
    }

    public override void Update()
    {
        enemy.Attack();
    }
}
