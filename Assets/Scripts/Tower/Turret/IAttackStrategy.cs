using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public interface IAttackStrategy
{
    Transform SelectTarget(List<Transform> enemies, Transform turretTransform);
}

public class ClosestEnemyStrategy : IAttackStrategy
{
    public Transform SelectTarget(List<Transform> enemies, Transform turretTransform)
    {
        Transform closest = null;
        float minDistance = Mathf.Infinity;

        foreach (var enemy in enemies)
        {
            float dist = Vector3.Distance(turretTransform.position, enemy.position);
            if (dist < minDistance)
            {
                minDistance = dist;
                closest = enemy;
            }
        }

        return closest;
    }
}
//public class StrongestEnemyStrategy : IAttackStrategy
//{
//    public Transform SelectTarget(List<Transform> enemies, Transform turretTransform)
//    {
//        Transform strongest = null;
//        float maxHealth = -1f;

//        foreach (var enemy in enemies)
//        {
//            Entites enemyScript = enemy.GetComponent<Entites>();
//            if (enemyScript != null && enemyScript.health > maxHealth)
//            {
//                maxHealth = enemyScript.health;
//                strongest = enemy;
//            }
//        }

//        return strongest;
//    }
//}
