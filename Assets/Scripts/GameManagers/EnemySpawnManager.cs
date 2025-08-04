using Unity.Burst.CompilerServices;
using UnityEngine;


namespace Game.Entites
{
    class EnemySpawnManager : MonoBehaviour
    {
        public static EnemySpawnManager instance;
        public GameObject enemyPrefab;
        [SerializeField] private int startSpawnCount = 20;
        [SerializeField] private int _maxSpawnCount = 40;
        public int maxSpawnCount
        {
            get { return _maxSpawnCount; } private set {; }
        }
        public int currentSpawnCount { get; private set; } = 0;
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
            InvokeRepeating(nameof(RandomSpawnEnemy), 0f, 3f);
            StartSpawn();
        }
        void StartSpawn()
        {
            for (int i = 0; i < startSpawnCount; i++)
            {
                RandomSpawnEnemy();
            }
        }
        void RandomPositionWaveSpawn()
        {
            currentSpawnCount = GameObject.FindGameObjectsWithTag("Enemy").Length;
            if (currentSpawnCount >= maxSpawnCount)
            {
                return;
            }
            Vector3 cameraPos = Camera.main.transform.position;
            Vector3 spawnPosition = new Vector3(
                Random.Range(cameraPos.x, cameraPos.x + 90),
                cameraPos.y,
                Random.Range(cameraPos.z, cameraPos.z + 90)
            );
            spawnPosition.x += GridManager.instance.length / 2;
            spawnPosition.z += GridManager.instance.width / 2;
            int x = Mathf.RoundToInt(spawnPosition.x);
            int z = Mathf.RoundToInt(spawnPosition.z);
            spawnPosition = GridManager.instance.GetGridXY(x, z).pos;
            spawnPosition.y += 1f;
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
            currentSpawnCount++;
        }
        void SpawnEnemy(GameObject enemy, Grid grid)
        {
            GameObject enemiesparent = GameObject.Find("Enemies");
            if (enemiesparent == null)
            {
                enemiesparent = new GameObject("Enemies");
            }
            Vector3 spawnPosition = grid.pos + new Vector3(0, 1f, 0);
            GameObject enemyInstance = Instantiate(enemy, spawnPosition, Quaternion.identity);
            enemyInstance.transform.SetParent(enemiesparent.transform);
        }
        void RandomSpawnEnemy()
        {
            currentSpawnCount = GameObject.FindGameObjectsWithTag("Enemy").Length;
            if (currentSpawnCount >= maxSpawnCount)
            {
                return;
            }
            int x = Random.Range(0, GridManager.instance.length);
            int z = Random.Range(0, GridManager.instance.width);
            Grid grid = GridManager.instance.GetGridXY(x, z);
            if (grid == null )
            {
                return; // Skip if the grid is null or occupied
            }
            SpawnEnemy(enemyPrefab, grid);
        }

    }
}
