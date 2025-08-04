using UnityEngine;
using Game.Entites;
public class LevelDirector : MonoBehaviour
{
    private float levelTime;
    private float attackTimeCounter = 0f;
    [SerializeField] private GameObject shuttle;
    [SerializeField] private float attackWave = 300f;
    private void Start()
    {
        InvokeRepeating(nameof(UpdateLevelTime), 1f, 1f);
    }
    private void Update()
    {
        levelTime += Time.deltaTime;
        attackTimeCounter += Time.deltaTime;
        if (attackTimeCounter >= attackWave)
        {
            attackTimeCounter = 0f;
            StartAttack();
        }
    }
    private void UpdateLevelTime()
    {
        UIManager.instance.SetLevelTime(levelTime);
    }

    private void StartAttack()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        if (enemies.Length > 0)
        {
            foreach (GameObject enemy in enemies)
            {
                EnemySimpleAI enemyAI = enemy.GetComponent<EnemySimpleAI>();
                if (enemyAI.GetBehaviorState() is EnemyIdleState)
                {
                    enemyAI.target = shuttle.transform;
                    enemyAI.SetBehaviorState(new EnemyAttackState());
                    Debug.Log($"Enemy {enemy.name} is now attacking.");
                }
            }
        }
        else
        {
            Debug.Log("No enemies found to attack.");
        }
    }
}