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
    List<GridNode> path;
    Vector3 movingForce;
    float updateTargetInterval = 1.5f;
    float updateTargetTime = 0f;
    public override void EnterState(GameObject enemy)
    {
        EnemySimpleAI enemyAI = enemy.GetComponent<EnemySimpleAI>();
        Vector2Int startPos = GridManager.instance.GetGridXY(enemy.transform.position);
        Vector2Int targetPos = GridManager.instance.GetGridXY(enemyAI.target.transform.position);
        FindPath(startPos, targetPos);
        foreach (var node in path)
        {
            Grid grid = GridManager.instance.GetGridXY(node.x, node.y);
        }
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
        if (path == null || path.Count == 0)
        {
            enemyAI.SetBehaviorState(new EnemyIdleState());
            return;
        }
        Vector2Int currentPos = GridManager.instance.GetGridXY(enemy.transform.position);
        if (currentPos == new Vector2Int(path[0].x, path[0].y))
        {
            path.Remove(path[0]);
        }
        Vector3 targetDir = GridManager.instance.GetGridXY(path[0].x, path[0].y).pos - enemy.transform.position;
        targetDir.y = 0;
        targetDir.Normalize();
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
    public void FindPath(Vector2Int start, Vector2Int target)
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

}


public class EnemyExplodeState : EnemyBehaviorState
{
    public override void EnterState(GameObject enemy)
    {
        Enemy enemyData = enemy.GetComponent<Enemy>();
        enemyData.Die();
    }
}


