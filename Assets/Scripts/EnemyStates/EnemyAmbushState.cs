using UnityEngine;

public class EnemyAmbushState : EnemyBaseState
{
    public EnemyAmbushState(Enemy enemy, Animator animator) : base(enemy, animator)
    {
    }

    public override void OnEnter()
    {
        Debug.Log("Ambush State");
    }

    public override void Update()
    {
        enemy.Ambush();
    }
    
}
