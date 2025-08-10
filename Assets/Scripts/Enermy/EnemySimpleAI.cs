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
    List<GridNode> path;
    public override void EnterState(GameObject enemy)
    {
        Debug.Log("Entering Attack State");
        EnemySimpleAI enemyAI = enemy.GetComponent<EnemySimpleAI>();
        Vector2Int startPos = GridManager.instance.GetRridXY(enemy.transform.position);
        Vector2Int targetPos = GridManager.instance.GetRridXY(enemyAI.target.transform.position);
        FindPath(startPos, targetPos);
        foreach (var node in path)
        {
            Grid grid = GridManager.instance.GetGridXY(node.x, node.y);
        }
    }
    public override void UpdateState(GameObject enemy)
    {
        EnemySimpleAI enemyAI = enemy.GetComponent<EnemySimpleAI>();
        Enemy enemyData = enemy.GetComponent<Enemy>();
        if (enemyAI.target == null)
        {
            return;
        }
        if (path == null || path.Count == 0)
        {
            Debug.Log("Path is empty, transitioning to idle state.");
            enemyAI.SetBehaviorState(new EnemyIdleState());
            return;
        }
        Vector2Int currentPos = GridManager.instance.GetRridXY(enemy.transform.position);
        if (currentPos == new Vector2Int(path[0].x, path[0].y))
        {
            path.Remove(path[0]);
        }
        Vector3 targetDir = GridManager.instance.GetGridXY(path[0].x, path[0].y).pos - enemy.transform.position;
        targetDir.y = 0;
        targetDir.Normalize();
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
    void FindPath(Vector2Int start, Vector2Int target)
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
        path = pathfinder.FindPath(gridNodes, GridManager.instance.width, GridManager.instance.length, start, target);
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


