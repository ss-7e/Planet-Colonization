using Unity.Burst.CompilerServices;
using UnityEngine;



namespace Game.Entites
{
    class EnemySpawnManager : MonoBehaviour
    {
        public static EnemySpawnManager instance;
        public GameObject enemyPrefab; // Assign in the inspector or dynamically load
        void Start()
        {
            if (enemyPrefab == null)
            {
                Debug.LogError("Enemy prefab is not assigned in the inspector.");
            }
            if (instance != null)
            {
                Debug.LogError("More than one EnermySpawnManager in scene!");
                return;
            }
            instance = this;
            
            InvokeRepeating("RandomPositionWaveSpawn", 0f, 2f); // Spawn every 2 seconds
        }
        void RandomPositionWaveSpawn()
        {
            Vector3 cameraPos = Camera.main.transform.position;
            Vector3 spawnPosition = new Vector3(
                Random.Range(cameraPos.x, cameraPos.x + 20),
                cameraPos.y,
                Random.Range(cameraPos.z, cameraPos.z + 20)
            );
            spawnPosition.x += GridManager.instance.length / 2;
            spawnPosition.z += GridManager.instance.width / 2;
            int x = Mathf.RoundToInt(spawnPosition.x);
            int z = Mathf.RoundToInt(spawnPosition.z);
            spawnPosition = GridManager.instance.grid[x, z].pos;
            spawnPosition.y += 3f;
            SpawnEnemy(enemyPrefab, spawnPosition);
        }

        void SpawnEnemy(GameObject enemy, Vector3 pos)
        {
            if (enemy == null)
            {
                Debug.LogError("Enemy prefab is not assigned.");
                return;
            }
            GameObject enemiesparent = GameObject.Find("Enemies");
            if (enemiesparent == null)
            {
                enemiesparent = new GameObject("Enemies");
            }
            GameObject enemyInstance = Instantiate(enemy, pos, Quaternion.identity);
            enemyInstance.transform.SetParent(enemiesparent.transform);
        }
        void SpawnEnemy(GameObject enemy, Grid grid)
        {
            GameObject enemiesparent = GameObject.Find("Enemies");
            if (enemiesparent == null)
            {
                enemiesparent = new GameObject("Enemies");
            }
            GameObject enemyInstance = Instantiate(enemy, grid.pos, Quaternion.identity);
            enemyInstance.transform.SetParent(enemiesparent.transform);
        }

    }
}
