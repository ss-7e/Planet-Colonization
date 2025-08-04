using Game.Entites;
using NPBehave;
using UnityEngine;

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
        Debug.Log($"Transitioning from {behaviorState.GetType().Name} to {newState.GetType().Name}");
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
    public override void EnterState(GameObject enemy)
    {
        Debug.Log("Entering Attack State");
        // Logic for entering attack state
    }
    public override void UpdateState(GameObject enemy)
    {
        EnemySimpleAI enemyAI = enemy.GetComponent<EnemySimpleAI>();
        Enemy enemyData = enemy.GetComponent<Enemy>();
        if (enemyAI.target == null)
        {
            return;
        }
        Vector3 targetDir = (enemyAI.target.transform.position - enemy.transform.position).normalized;
        targetDir.y = 0;
        Vector2Int startPos = new Vector2Int(Mathf.RoundToInt(enemy.transform.position.x), Mathf.RoundToInt(enemy.transform.position.z));
        Vector2Int endPos = new Vector2Int(Mathf.RoundToInt(enemyAI.target.transform.position.x), Mathf.RoundToInt(enemyAI.target.transform.position.z));

        enemy.transform.rotation = Quaternion.LookRotation(targetDir);
        enemy.transform.position += targetDir * Time.deltaTime * enemyAI.moveSpeed;
        LayerMask layerMask = LayerMask.GetMask("Player");
        Collider[] hitColliders = Physics.OverlapSphere(enemy.transform.position, enemyAI.attackRange, layerMask);
        if (hitColliders.Length > 0)
        {
            foreach (var hitCollider in hitColliders)
            {
                if( hitCollider.GetComponent<IDamageable>() is IDamageable damageable)
                {
                    damageable.TakeDamage(enemyData.attackValue);
                    Debug.Log("Enemy attacked player for " + enemyData.attackValue + " damage.");
                }
            }
            Debug.Log("Enemy has attacked the player, transitioning to explode state.");
            enemyAI.SetBehaviorState(new EnemyExplodeState());
        }
    }
    public void FindPath()
    {
        Grid[] grids = GridManager.instance.grid;
        GridNode[] gridNodes = new GridNode[grids.Length];
        for (int i = 0; i < grids.Length; i++)
        {
            gridNodes[i] = new GridNode
            {
                x = i % GridManager.instance.width,
                y = i / GridManager.instance.width,
                isObstacle = grids[i].isObstacle
            };
        }
        Pathfinder pathfinder = new Pathfinder();
    }
    public override void ExitState(GameObject enemy)
    {
        Debug.Log("Exiting Attack State");
        // Logic for exiting attack state
    }
}

public class EnemyExplodeState : EnemyBehaviorState
{
    public override void EnterState(GameObject enemy)
    {
        Debug.Log("Entering Explode State");
        Enemy enemyData = enemy.GetComponent<Enemy>();
        enemyData.Die();
    }
}


