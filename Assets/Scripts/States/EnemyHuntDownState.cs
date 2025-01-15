using UnityEditor.ProjectWindowCallback;
using UnityEngine;
using UnityEngine.AI;

public class EnemyHuntDownState : EnemyBaseState
{
        private NavMeshAgent agent;

        public EnemyHuntDownState(Enemy enemy, Animator animator, NavMeshAgent agent) : base(enemy, animator)
        {
                
        }
}