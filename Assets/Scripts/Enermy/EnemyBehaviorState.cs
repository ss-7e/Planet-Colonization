using UnityEngine;

public class EnemyBehaviorState
{
    public virtual void EnterState(GameObject enemy) { }
    public virtual void UpdateState(GameObject enemy) { }
    public virtual void ExitState(GameObject enemy) { }

}

public class EnemyIdleState : EnemyBehaviorState
{
    public override void EnterState(GameObject enemy)
    {
        // Logic for entering idle state
        Debug.Log("Entering Idle State");
    }
    public override void UpdateState(GameObject enemy)
    {
        
    }
}
