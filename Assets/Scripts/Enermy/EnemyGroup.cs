using Game.Entites;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
public class EnemyGroup
{
    public List<GameObject> enemyPrefab;
    public GameObject leader;
    public int maxMember {  get; private set;  }
    public Vector3 target;


    public EnemyGroup(List<GameObject> enemyPrefab, int maxMember)
    {
        this.enemyPrefab = enemyPrefab;
        this.maxMember = maxMember;
    }
    
    public void UpdateForce()
    {
        foreach(GameObject enemy in enemyPrefab)
        {
            Enemy enemyData = enemy.GetComponent<Enemy>();
            
        }
    }

}