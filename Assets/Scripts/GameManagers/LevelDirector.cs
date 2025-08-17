using UnityEngine;
using Game.Entites;

public enum LevelDifficulty
{
    Easy,
    Normal,
    Hard
}

public class LevelDirector : MonoBehaviour
{
    public static LevelDirector instance;
    public LevelDifficulty levelDifficulty = LevelDifficulty.Easy;
    private float levelTime;
    private float attackTimeCounter = 0f;
    private float groupAttackTimeCounter = 0f;
    [SerializeField] private GameObject shuttle;
    [SerializeField] private float attackCooldown = 10f;
    [SerializeField] private float groupAttackCooldown = 80f; 
    private void Start()
    {
        if (instance != null)
        {
            return;
        }
        instance = this;
        InvokeRepeating(nameof(UpdateLevelTime), 1f, 1f);
    }
    private void Update()
    {
        levelTime += Time.deltaTime;
        attackTimeCounter += Time.deltaTime;
        groupAttackTimeCounter += Time.deltaTime;
        if (groupAttackTimeCounter >= groupAttackCooldown)
        {
            groupAttackTimeCounter = 0f;
            StartGroupAttack();
        }
        if (attackTimeCounter >= attackCooldown)
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
                }
            }
        }
    }
    private void StartGroupAttack()
    {
        
    }
}