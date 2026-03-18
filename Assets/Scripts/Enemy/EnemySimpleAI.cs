using Game.Entites;
using UnityEngine;
using System.Collections.Generic;

public class EnemySimpleAI : EnemyAI
{
    protected EnemyBehaviorState behaviorState;
    private void Start()
    {
        behaviorState = new EnemyIdleState();
    }
    private void Update()
    {
        behaviorState.UpdateState(this.gameObject);
    }
    public EnemyBehaviorState GetBehaviorState()
    {
        return behaviorState;
    }
    public void SetBehaviorState(EnemyBehaviorState newState)
    {
        if (behaviorState != null)
        {
            behaviorState.ExitState(this.gameObject);
        }
        behaviorState = newState;
        behaviorState.EnterState(this.gameObject);
    }
}


public class EnemyAttackState : EnemyBehaviorState
{
    Vector3 movingForce;
    float updateTargetInterval = 1.5f;
    float updateTargetTime = 0f;
    public override void EnterState(GameObject enemy)
    {
    }
    public override void UpdateState(GameObject enemy)
    {
        updateTargetTime += Time.deltaTime;
        EnemySimpleAI enemyAI = enemy.GetComponent<EnemySimpleAI>();
        Enemy enemyData = enemy.GetComponent<Enemy>();
        if (enemyAI.target == null)
        {
            return;
        }
        
        Vector2 gradient = AIModule.Instance.HeatMapSet.NavFlowField.GetGradient(
            enemy.transform.position.x,
            enemy.transform.position.z
        );
        
        Vector3 targetDir = new Vector3(-gradient.x, 0, -gradient.y);
        if (targetDir.sqrMagnitude > 0.001f)
        {
            targetDir.Normalize();
        }
        
        if (updateTargetTime >= updateTargetInterval)
        {
            updateTargetTime = 0f;
            enemyData.AddMoveForce(targetDir, 1f);
        }
        
        LayerMask layerMask = LayerMask.GetMask("Player");
        Collider[] hitColliders = Physics.OverlapSphere(enemy.transform.position, enemyAI.attackRange, layerMask);
        if (hitColliders.Length > 0)
        {
            foreach (var hitCollider in hitColliders)
            {
                if( hitCollider.GetComponent<IDamageable>() is IDamageable damageable)
                {
                    damageable.TakeDamage(enemyData.attackValue);
                }
            }
            enemyAI.SetBehaviorState(new EnemyExplodeState());
        }
    }

}


public class EnemyExplodeState : EnemyBehaviorState
{
    public override void EnterState(GameObject enemy)
    {
        Enemy enemyData = enemy.GetComponent<Enemy>();
        enemyData.Die();
    }
}


