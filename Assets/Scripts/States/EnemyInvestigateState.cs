using UnityEngine;
using UnityEngine.AI;


public class EnemyInvestigateState : EnemyBaseState
{
    
    public EnemyInvestigateState(Enemy enemy, Animator animator) : base(enemy, animator)
    {
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