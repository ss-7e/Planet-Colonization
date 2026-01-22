using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class ObjectPoolTest : MonoBehaviour
{
            
    [Header("生成设置")]
    public GameObject projectilePrefab;
    public bool useObjectPool = true;
    public int maxConcurrentProjectiles = 500;

    [Header("随机生成参数")]
    public float minSpawnInterval = 0.01f;
    public float maxSpawnInterval = 0.1f;
    public float minLifeTime = 1f;
    public float maxLifeTime = 3f;

    [Header("测试控制")]
    public bool isTesting = false;
    public KeyCode startStopKey = KeyCode.Space;
    public KeyCode toggleModeKey = KeyCode.P;
    public KeyCode clearKey = KeyCode.R;

    [Header("性能监控")]
    public bool showStats = true;
    public int sampleCount = 100;

    // 统计变量
    private Queue<float> spawnTimeSamples = new Queue<float>();
    private Queue<float> destroyTimeSamples = new Queue<float>();
    private Queue<float> memorySamples = new Queue<float>();
    private Queue<float> fpsSamples = new Queue<float>();
    private long totalSpawned = 0;
    private long totalDestroyed = 0;
    private float testStartTime = 0f;
    private float totalTestTime = 0f;
    private Stopwatch stopwatch = new Stopwatch();

    // 性能报告
    private class PerformanceReport
    {
        public bool useObjectPool;
        public float avgSpawnTime;
        public float avgDestroyTime;
        public float avgMemory;
        public float avgFPS;
        public float minFPS;
        public float maxFPS;
        public int maxActiveCount;
        public long totalSpawned;
        public long totalDestroyed;
        public float totalTestTime;
    }

    private PerformanceReport lastReport;

    // 对象池
    private Queue<GameObject> objectPool = new Queue<GameObject>();
    private List<ProjectileData> activeProjectiles = new List<ProjectileData>();
    private Transform poolContainer;

    // 定时器
    private float spawnTimer = 0f;
    private float nextSpawnTime = 0f;
    private float statsUpdateTimer = 0f;
    private const float STATS_UPDATE_INTERVAL = 5f;

    private struct ProjectileData
    {
        public GameObject gameObject;
        public float destroyTime;
        public Vector3 direction;
    }

    void Start()
    {
        poolContainer = new GameObject("ObjectPoolContainer").transform;
        poolContainer.SetParent(transform);
        poolContainer.gameObject.SetActive(false);
        stopwatch.Start();

        // 初始化随机生成时间
        nextSpawnTime = Random.Range(minSpawnInterval, maxSpawnInterval);

        UnityEngine.Debug.Log($"=== 炮弹性能测试就绪 ===");
        UnityEngine.Debug.Log($"控制按键:");
        UnityEngine.Debug.Log($"  {startStopKey}: 开始/停止测试");
        UnityEngine.Debug.Log($"  {toggleModeKey}: 切换对象池/常规模式");
        UnityEngine.Debug.Log($"  {clearKey}: 清理所有炮弹");
        UnityEngine.Debug.Log($"当前模式: {(useObjectPool ? "对象池" : "常规")}");
    }

    void Update()
    {
        // 处理输入
        HandleInput();

        // 更新FPS采样
        UpdateFPSSample();

        if (isTesting)
        {
            // 更新生成计时器
            spawnTimer += Time.deltaTime;

            // 检查是否需要生成新炮弹
            if (spawnTimer >= nextSpawnTime && activeProjectiles.Count < maxConcurrentProjectiles)
            {
                for(int i = 0; i < 50 && i < spawnTimer/nextSpawnTime; i++)  // 每帧最多生成10个以防止卡顿
                    SpawnProjectile();
                spawnTimer = 0f;
                nextSpawnTime = Random.Range(minSpawnInterval, maxSpawnInterval);
            }

            // 更新所有活跃炮弹并检查生命周期
            UpdateProjectiles();

            // 更新测试时间
            totalTestTime = Time.time - testStartTime;
        }

        // 更新统计数据（无论是否测试中）
        UpdateStats();

        // 显示统计数据
        if (showStats)
        {
            DisplayStats();
        }
    }

    void HandleInput()
    {
        // 开始/停止测试
        if (Input.GetKeyDown(startStopKey))
        {
            ToggleTesting();
        }

        // 切换模式
        if (Input.GetKeyDown(toggleModeKey))
        {
            ToggleObjectPoolMode();
        }

        // 清理所有炮弹
        if (Input.GetKeyDown(clearKey))
        {
            ClearAllProjectiles();
        }

        // 调整最大数量（仅在测试中）
        if (isTesting)
        {
            if (Input.GetKeyDown(KeyCode.Equals) || Input.GetKeyDown(KeyCode.Plus))
            {
                maxConcurrentProjectiles += 50;
                UnityEngine.Debug.Log($"最大数量增加至: {maxConcurrentProjectiles}");
            }
            if (Input.GetKeyDown(KeyCode.Minus))
            {
                maxConcurrentProjectiles = Mathf.Max(50, maxConcurrentProjectiles - 50);
                UnityEngine.Debug.Log($"最大数量减少至: {maxConcurrentProjectiles}");
            }
        }
    }

    void ToggleTesting()
    {
        isTesting = !isTesting;

        if (isTesting)
        {
            // 开始测试
            testStartTime = Time.time;
            totalSpawned = 0;
            totalDestroyed = 0;

            // 激活对象池容器
            if (poolContainer != null)
                poolContainer.gameObject.SetActive(true);

            // 清空采样数据
            spawnTimeSamples.Clear();
            destroyTimeSamples.Clear();
            memorySamples.Clear();
            fpsSamples.Clear();

            UnityEngine.Debug.Log($"=== 测试开始 ===");
            UnityEngine.Debug.Log($"模式: {(useObjectPool ? "对象池" : "常规")}");
            UnityEngine.Debug.Log($"最大数量: {maxConcurrentProjectiles}");
            UnityEngine.Debug.Log($"生成间隔: {minSpawnInterval:F2}-{maxSpawnInterval:F2}秒");
            UnityEngine.Debug.Log($"生命周期: {minLifeTime:F1}-{maxLifeTime:F1}秒");
        }
        else
        {
            // 停止测试
            UnityEngine.Debug.Log($"=== 测试停止 ===");

            // 生成最终性能报告
            GenerateFinalReport();

            // 清理所有炮弹
            ClearAllProjectiles();

            // 停用对象池容器以节省性能
            if (poolContainer != null)
                poolContainer.gameObject.SetActive(false);
        }
    }

    void ToggleObjectPoolMode()
    {
        // 如果正在测试，先停止
        bool wasTesting = isTesting;
        if (isTesting)
        {
            isTesting = false;
            ClearAllProjectiles();
        }

        // 切换模式
        useObjectPool = !useObjectPool;

        // 清空对象池
        if (objectPool != null)
        {
            while (objectPool.Count > 0)
            {
                GameObject obj = objectPool.Dequeue();
                if (obj != null) Destroy(obj);
            }
        }

        UnityEngine.Debug.Log($"切换为 {(useObjectPool ? "对象池模式" : "常规模式")}");

        // 如果之前正在测试，重新开始
        if (wasTesting)
        {
            StartCoroutine(DelayedRestartTest());
        }
    }

    System.Collections.IEnumerator DelayedRestartTest()
    {
        yield return new WaitForSeconds(0.5f);
        isTesting = true;
        testStartTime = Time.time;

        if (poolContainer != null)
            poolContainer.gameObject.SetActive(true);

        UnityEngine.Debug.Log($"测试以新模式重新开始");
    }

    void UpdateFPSSample()
    {
        float currentFPS = 1f / Time.deltaTime;
        fpsSamples.Enqueue(currentFPS);
        if (fpsSamples.Count > sampleCount * 2) fpsSamples.Dequeue();
    }

    void SpawnProjectile()
    {
        long startTicks = stopwatch.ElapsedTicks;

        // 随机位置和方向
        Vector3 spawnPos = transform.position + Random.insideUnitSphere * 5f;
        Vector3 randomDir = Random.onUnitSphere;
        Quaternion spawnRot = Quaternion.LookRotation(randomDir);

        GameObject projectile;

        if (useObjectPool)
        {
            // 对象池模式
            if (objectPool.Count > 0)
            {
                projectile = objectPool.Dequeue();
                projectile.transform.position = spawnPos;
                projectile.transform.rotation = spawnRot;
                projectile.SetActive(true);
            }
            else
            {
                projectile = Instantiate(projectilePrefab, spawnPos, spawnRot, poolContainer);
                projectile.SetActive(true);
            }
        }
        else
        {
            // 常规模式
            projectile = Instantiate(projectilePrefab, spawnPos, spawnRot, transform);
        }

        // 设置随机生命周期
        float lifeTime = Random.Range(minLifeTime, maxLifeTime);

        // 获取或添加简单移动组件
        var moveScript = projectile.GetComponent<SimpleMove>();
        if (moveScript == null)
        {
            moveScript = projectile.AddComponent<SimpleMove>();
        }
        moveScript.speed = Random.Range(5f, 15f);

        // 添加到活跃列表
        activeProjectiles.Add(new ProjectileData
        {
            gameObject = projectile,
            destroyTime = Time.time + lifeTime,
            direction = randomDir
        });

        totalSpawned++;

        // 记录生成时间（毫秒）
        float spawnTime = (stopwatch.ElapsedTicks - startTicks) / 10000f;
        spawnTimeSamples.Enqueue(spawnTime);
        if (spawnTimeSamples.Count > sampleCount) spawnTimeSamples.Dequeue();
    }

    void UpdateProjectiles()
    {
        long startTicks = stopwatch.ElapsedTicks;
        int destroyedCount = 0;

        // 检查并销毁过期的炮弹
        for (int i = activeProjectiles.Count - 1; i >= 0; i--)
        {
            if (Time.time >= activeProjectiles[i].destroyTime)
            {
                DestroyProjectile(activeProjectiles[i].gameObject, i);
                destroyedCount++;
            }
        }

        // 记录销毁时间
        if (destroyedCount > 0)
        {
            float destroyTime = (stopwatch.ElapsedTicks - startTicks) / 10000f;
            destroyTimeSamples.Enqueue(destroyTime);
            if (destroyTimeSamples.Count > sampleCount) destroyTimeSamples.Dequeue();
        }
    }

    void DestroyProjectile(GameObject projectile, int index)
    {
        if (useObjectPool)
        {
            // 对象池模式：回收对象
            projectile.SetActive(false);
            objectPool.Enqueue(projectile);
        }
        else
        {
            // 常规模式：销毁对象
            Destroy(projectile);
        }

        activeProjectiles.RemoveAt(index);
        totalDestroyed++;
    }

    void ClearAllProjectiles()
    {
        foreach (var projData in activeProjectiles)
        {
            if (projData.gameObject != null)
            {
                if (useObjectPool)
                {
                    projData.gameObject.SetActive(false);
                    objectPool.Enqueue(projData.gameObject);
                }
                else
                {
                    Destroy(projData.gameObject);
                }
            }
        }

        activeProjectiles.Clear();

        // 强制GC清理（仅用于观察对比）
        if (!useObjectPool)
        {
            System.GC.Collect();
        }

        UnityEngine.Debug.Log($"已清理所有炮弹 (共{activeProjectiles.Count}个活跃)");
    }

    void UpdateStats()
    {
        // 记录内存使用
        float memoryMB = System.GC.GetTotalMemory(false) / 1024f / 1024f;
        memorySamples.Enqueue(memoryMB);
        if (memorySamples.Count > sampleCount) memorySamples.Dequeue();

        // 定期输出性能报告
        statsUpdateTimer += Time.deltaTime;
        if (isTesting && statsUpdateTimer >= STATS_UPDATE_INTERVAL)
        {
            OutputPeriodicReport();
            statsUpdateTimer = 0f;
        }
    }

    void OutputPeriodicReport()
    {
        float avgSpawnTime = GetAverage(spawnTimeSamples);
        float avgDestroyTime = GetAverage(destroyTimeSamples);
        float avgMemory = GetAverage(memorySamples);
        float avgFPS = GetAverage(fpsSamples);

        string report = $"\n=== 实时性能报告 ===";
        report += $"\n测试时长: {totalTestTime:F1}秒";
        report += $"\n模式: {(useObjectPool ? "对象池" : "常规")}";
        report += $"\n活跃炮弹: {activeProjectiles.Count}/{maxConcurrentProjectiles}";
        report += $"\n生成/销毁: {totalSpawned}/{totalDestroyed}";
        report += $"\n平均FPS: {avgFPS:F1}";
        report += $"\n平均生成时间: {avgSpawnTime:F3}ms";
        report += $"\n平均销毁时间: {avgDestroyTime:F3}ms";
        report += $"\n平均内存: {avgMemory:F1}MB";
        report += $"\n=====================";

        UnityEngine.Debug.Log(report);
    }

    void GenerateFinalReport()
    {
        if (totalTestTime < 1f) return; // 测试时间太短不生成报告

        lastReport = new PerformanceReport
        {
            useObjectPool = useObjectPool,
            avgSpawnTime = GetAverage(spawnTimeSamples),
            avgDestroyTime = GetAverage(destroyTimeSamples),
            avgMemory = GetAverage(memorySamples),
            avgFPS = GetAverage(fpsSamples),
            minFPS = GetMin(fpsSamples),
            maxFPS = GetMax(fpsSamples),
            maxActiveCount = GetMaxActiveCount(),
            totalSpawned = totalSpawned,
            totalDestroyed = totalDestroyed,
            totalTestTime = totalTestTime
        };

        string finalReport = $"\n══════════════════════════════════════════";
        finalReport += $"\n           最终性能测试报告";
        finalReport += $"\n══════════════════════════════════════════";
        finalReport += $"\n测试模式: {(lastReport.useObjectPool ? "对象池" : "常规")}";
        finalReport += $"\n测试时长: {lastReport.totalTestTime:F1}秒";
        finalReport += $"\n总生成数量: {lastReport.totalSpawned}";
        finalReport += $"\n总销毁数量: {lastReport.totalDestroyed}";
        finalReport += $"\n最大同时活跃: {lastReport.maxActiveCount}";
        finalReport += $"\n──────────────────────────────────────────";
        finalReport += $"\n平均FPS: {lastReport.avgFPS:F1}";
        finalReport += $"\n最低FPS: {lastReport.minFPS:F1}";
        finalReport += $"\n最高FPS: {lastReport.maxFPS:F1}";
        finalReport += $"\n平均生成时间: {lastReport.avgSpawnTime:F3}ms";
        finalReport += $"\n平均销毁时间: {lastReport.avgDestroyTime:F3}ms";
        finalReport += $"\n平均内存使用: {lastReport.avgMemory:F1}MB";
        finalReport += $"\n══════════════════════════════════════════\n";

        UnityEngine.Debug.Log(finalReport);
    }

    float GetAverage(Queue<float> queue)
    {
        if (queue.Count == 0) return 0f;
        float sum = 0f;
        foreach (float value in queue) sum += value;
        return sum / queue.Count;
    }

    float GetMin(Queue<float> queue)
    {
        if (queue.Count == 0) return 0f;
        float min = float.MaxValue;
        foreach (float value in queue) if (value < min) min = value;
        return min;
    }

    float GetMax(Queue<float> queue)
    {
        if (queue.Count == 0) return 0f;
        float max = 0f;
        foreach (float value in queue) if (value > max) max = value;
        return max;
    }

    int GetMaxActiveCount()
    {
        // 记录的最大活跃数
        return Mathf.Max(activeProjectiles.Count, maxConcurrentProjectiles);
    }

    void DisplayStats()
    {
        float avgSpawnTime = GetAverage(spawnTimeSamples);
        float avgDestroyTime = GetAverage(destroyTimeSamples);
        float avgMemory = GetAverage(memorySamples);
        float avgFPS = GetAverage(fpsSamples);

        string stats = $"=== 炮弹性能测试 ===\n";
        stats += $"状态: {(isTesting ? "测试中" : "就绪")} ({startStopKey}键切换)\n";
        stats += $"模式: {(useObjectPool ? "对象池" : "常规")} ({toggleModeKey}键切换)\n";
        stats += $"活跃炮弹: {activeProjectiles.Count}/{maxConcurrentProjectiles}\n";

        if (isTesting)
        {
            stats += $"测试时间: {totalTestTime:F1}秒\n";
            stats += $"生成/销毁: {totalSpawned}/{totalDestroyed}\n";
            stats += $"平均FPS: {avgFPS:F1}\n";
            stats += $"平均生成: {avgSpawnTime:F3}ms\n";
            stats += $"平均销毁: {avgDestroyTime:F3}ms\n";
            stats += $"平均内存: {avgMemory:F1}MB\n";
        }
        else
        {
            stats += $"就绪 - 按 {startStopKey} 开始测试\n";
        }

        stats += $"\n参数设置:\n";
        stats += $"生成间隔: {minSpawnInterval:F2}-{maxSpawnInterval:F2}秒\n";
        stats += $"生命周期: {minLifeTime:F1}-{maxLifeTime:F1}秒\n";

        if (lastReport != null)
        {
            stats += $"\n上次测试结果:\n";
            stats += $"平均FPS: {lastReport.avgFPS:F1} ({(lastReport.useObjectPool ? "对象池" : "常规")})\n";
        }

        GUI.Label(new Rect(10, 10, 400, 400), stats);
    }

    void OnGUI()
    {
        if (!showStats) return;

        GUILayout.BeginArea(new Rect(10, 10, 400, 500));

        // 标题
        GUILayout.Label($"=== 炮弹性能测试 ===", GUI.skin.box);

        // 状态显示
        GUILayout.Space(10);
        GUILayout.Label($"测试状态: {(isTesting ? "▶ 运行中" : "⏸ 已停止")}");
        GUILayout.Label($"当前模式: {(useObjectPool ? "对象池" : "常规")}");
        GUILayout.Label($"活跃炮弹: {activeProjectiles.Count}/{maxConcurrentProjectiles}");

        if (isTesting)
        {
            GUILayout.Label($"测试时间: {totalTestTime:F1}秒");
            GUILayout.Label($"总生成: {totalSpawned} | 总销毁: {totalDestroyed}");

            float currentFPS = 1f / Time.deltaTime;
            GUILayout.Label($"当前FPS: {currentFPS:F1}");
        }

        GUILayout.Space(20);

        // 控制按钮
        if (GUILayout.Button(isTesting ? $"停止测试 ({startStopKey})" : $"开始测试 ({startStopKey})",
            GUILayout.Height(40)))
        {
            ToggleTesting();
        }

        if (GUILayout.Button($"切换模式 ({toggleModeKey})", GUILayout.Height(30)))
        {
            ToggleObjectPoolMode();
        }

        if (GUILayout.Button($"清理炮弹 ({clearKey})", GUILayout.Height(30)))
        {
            ClearAllProjectiles();
        }

        GUILayout.Space(20);

        // 参数调整（仅在停止测试时可调整）
        GUI.enabled = !isTesting;
        GUILayout.Label($"=== 参数设置 ===");

        GUILayout.Label($"最大同时存在数量: {maxConcurrentProjectiles}");
        maxConcurrentProjectiles = (int)GUILayout.HorizontalSlider(maxConcurrentProjectiles, 50, 2000);

        GUILayout.Space(10);
        GUILayout.Label($"生成间隔: {minSpawnInterval:F2}-{maxSpawnInterval:F2}秒");
        GUILayout.BeginHorizontal();
        minSpawnInterval = GUILayout.HorizontalSlider(minSpawnInterval, 0.01f, 0.5f, GUILayout.Width(180));
        maxSpawnInterval = GUILayout.HorizontalSlider(maxSpawnInterval, 0.05f, 1f, GUILayout.Width(180));
        GUILayout.EndHorizontal();

        GUILayout.Space(10);
        GUILayout.Label($"生命周期: {minLifeTime:F1}-{maxLifeTime:F1}秒");
        GUILayout.BeginHorizontal();
        minLifeTime = GUILayout.HorizontalSlider(minLifeTime, 0.5f, 5f, GUILayout.Width(180));
        maxLifeTime = GUILayout.HorizontalSlider(maxLifeTime, 1f, 10f, GUILayout.Width(180));
        GUILayout.EndHorizontal();

        GUI.enabled = true;

        GUILayout.EndArea();
    }

    void OnDestroy()
    {
        if (isTesting)
        {
            GenerateFinalReport();
        }
        ClearAllProjectiles();
        stopwatch.Stop();
    }
}

// 简单移动组件
public class SimpleMove : MonoBehaviour
{
    public float speed = 10f;

    void Update()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }
}