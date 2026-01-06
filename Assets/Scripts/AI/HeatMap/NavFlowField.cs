using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FastSweepingSolver
{
    // 网格单元格状态
    public enum CellType
    {
        Empty = 0,
        Obstacle = -1
    }

    // FSS求解器类
    public class FSSolver
    {
        public int width;
        public int height;
        private float[,] T;      // 到达时间场
        private float[,] F;      // 慢度场（F > 0）
        private bool[,] obstacle; // 障碍物标记

        public float[,] CostField => T;

        public FSSolver(int width, int height)
        {
            this.width = width;
            this.height = height;
            T = new float[height, width];
            F = new float[height, width];
            obstacle = new bool[height, width];

            // 初始化F为1（标准成本）
            for (int y = 0; y < height; y++)
                for (int x = 0; x < width; x++)
                    F[y, x] = 1.0f;
        }

        // 设置慢度场/成本
        public void SetCost(int x, int y, float cost)
        {
            if (cost <= 0)
            {
                obstacle[y, x] = true;
                F[y, x] = 0;
            }
            else
            {
                obstacle[y, x] = false;
                F[y, x] = cost;
            }
        }

        // 设置障碍物
        public void SetObstacle(int x, int y, bool isObstacle)
        {
            obstacle[y, x] = isObstacle;
            F[y, x] = isObstacle ? 0 : 1.0f;
        }

        public void ClearObstacle()
        {
            obstacle = new bool[height, width];
        }

        // 核心：求解局部Eikonal方程
        private float SolveLocal(int x, int y)
        {
            if (obstacle[y, x]) return float.MaxValue;

            float Fval = F[y, x];
            if (Fval <= 0) return float.MaxValue;

            // 获取四个邻居的值
            float left = (x > 0 && !obstacle[y, x - 1]) ? T[y, x - 1] : float.MaxValue;
            float right = (x < width - 1 && !obstacle[y, x + 1]) ? T[y, x + 1] : float.MaxValue;
            float up = (y > 0 && !obstacle[y - 1, x]) ? T[y - 1, x] : float.MaxValue;
            float down = (y < height - 1 && !obstacle[y + 1, x]) ? T[y + 1, x] : float.MaxValue;

            // 找到水平和垂直方向的最小值
            float hMin = Mathf.Min(left, right);
            float vMin = Mathf.Min(up, down);

            // 如果两个方向都不可达，返回极大值
            if (hMin == float.MaxValue && vMin == float.MaxValue)
                return float.MaxValue;

            // 如果只有一个方向可达
            if (hMin == float.MaxValue) return vMin + Fval;
            if (vMin == float.MaxValue) return hMin + Fval;

            // 两个方向都可达，求解二次方程
            // 排序使 a <= b
            float a = Mathf.Min(hMin, vMin);
            float b = Mathf.Max(hMin, vMin);

            // 检查 T 是否 <= a
            float discriminant = 2.0f * Fval * Fval - (b - a) * (b - a);

            if (discriminant < 0)
            {
                // 无实数解，取最小值+F
                return a + Fval;
            }

            float sqrtDisc = Mathf.Sqrt(discriminant);
            float candidate = (a + b + sqrtDisc) * 0.5f;

            // 检查 candidate 是否 >= b
            if (candidate >= b)
                return candidate;

            // 否则 candidate < b，需要检查是否 <= a
            return a + Fval;
        }

        // 单次扫描
        private float PerformSweep(int startY, int endY, int stepY,
                                    int startX, int endX, int stepX)
        {
            float maxChange = 0;

            for (int y = startY; stepY > 0 ? y < endY : y > endY; y += stepY)
            {
                for (int x = startX; stepX > 0 ? x < endX : x > endX; x += stepX)
                {
                    if (obstacle[y, x]) continue;

                    float oldValue = T[y, x];
                    float newValue = SolveLocal(x, y);

                    if (newValue < oldValue)
                    {
                        T[y, x] = newValue;
                        float change = oldValue - newValue;
                        if (change > maxChange) maxChange = change;
                    }
                }
            }

            return maxChange;
        }

        // 求解成本场
        public void Solve(int goalX, int goalY, int maxIterations = 8, float tolerance = 0.001f)
        {
            // 1. 初始化T场
            for (int y = 0; y < height; y++)
                for (int x = 0; x < width; x++)
                    T[y, x] = obstacle[y, x] ? float.MaxValue : float.MaxValue;

            // 设置目标点
            if (!obstacle[goalY, goalX])
                T[goalY, goalX] = 0;
            else
            {
                Debug.LogWarning("Goal is in obstacle! Finding nearest valid cell...");
                // 寻找最近的可通行点
                SetNearestValidGoal(ref goalX, ref goalY);
                T[goalY, goalX] = 0;
            }

            // 2. 迭代扫描
            for (int iter = 0; iter < maxIterations; iter++)
            {
                float maxChange = 0;

                // 扫描顺序1: 从上到下，从左到右
                maxChange = Mathf.Max(maxChange,
                    PerformSweep(0, height, 1, 0, width, 1));

                // 扫描顺序2: 从上到下，从右到左
                maxChange = Mathf.Max(maxChange,
                    PerformSweep(0, height, 1, width - 1, -1, -1));

                // 扫描顺序3: 从下到上，从左到右
                maxChange = Mathf.Max(maxChange,
                    PerformSweep(height - 1, -1, -1, 0, width, 1));

                // 扫描顺序4: 从下到上，从右到左
                maxChange = Mathf.Max(maxChange,
                    PerformSweep(height - 1, -1, -1, width - 1, -1, -1));

                // 检查收敛
                if (maxChange < tolerance)
                {
                    Debug.Log($"FSS converged after {iter + 1} iterations");
                    break;
                }
            }
        }

        // 寻找最近的可通行点作为目标
        private void SetNearestValidGoal(ref int goalX, ref int goalY)
        {
            // 简单的BFS寻找最近可通行点
            bool[,] visited = new bool[height, width];
            System.Collections.Generic.Queue<Vector2Int> queue =
                new System.Collections.Generic.Queue<Vector2Int>();

            queue.Enqueue(new Vector2Int(goalX, goalY));
            visited[goalY, goalX] = true;

            Vector2Int[] directions = new Vector2Int[]
            {
                new Vector2Int(1, 0), new Vector2Int(-1, 0),
                new Vector2Int(0, 1), new Vector2Int(0, -1)
            };

            while (queue.Count > 0)
            {
                Vector2Int current = queue.Dequeue();

                if (!obstacle[current.y, current.x])
                {
                    goalX = current.x;
                    goalY = current.y;
                    Debug.Log($"New goal set to ({goalX}, {goalY})");
                    return;
                }

                foreach (var dir in directions)
                {
                    int nx = current.x + dir.x;
                    int ny = current.y + dir.y;

                    if (nx >= 0 && nx < width && ny >= 0 && ny < height && !visited[ny, nx])
                    {
                        visited[ny, nx] = true;
                        queue.Enqueue(new Vector2Int(nx, ny));
                    }
                }
            }

            // 如果找不到，使用默认点
            goalX = width / 2;
            goalY = height / 2;
            obstacle[goalY, goalX] = false;
            F[goalY, goalX] = 1.0f;
            Debug.LogWarning("No valid goal found, using center");
        }

        public Vector2 GetGrad(int x, int y)
        {
            if (obstacle[y, x] || T[y, x] == float.MaxValue)
            {
                return Vector2.zero;
            }

            // 计算梯度
            float dx = 0, dy = 0;

            if (x > 0 && !obstacle[y, x - 1] && T[y, x - 1] != float.MaxValue)
                dx += T[y, x] - T[y, x - 1];
            if (x < width - 1 && !obstacle[y, x + 1] && T[y, x + 1] != float.MaxValue)
                dx += T[y, x] - T[y, x + 1];

            if (y > 0 && !obstacle[y - 1, x] && T[y - 1, x] != float.MaxValue)
                dy += T[y, x] - T[y - 1, x];
            if (y < height - 1 && !obstacle[y + 1, x] && T[y + 1, x] != float.MaxValue)
                dy += T[y, x] - T[y + 1, x];

            return new Vector2(dx, dy);
        }

        // 从成本场生成流场向量
        public Vector2[,] GenerateVectorField()
        {
            Vector2[,] vectorField = new Vector2[height, width];

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    // 负梯度方向是指向目标的方向
                    Vector2 grad = -GetGrad(x, y);

                    // 归一化
                    if (grad.sqrMagnitude > 0.0001f)
                        vectorField[y, x] = grad.normalized;
                    else
                        vectorField[y, x] = Vector2.zero;
                }
            }

            return vectorField;
        }

        // 可视化调试方法
        public void DebugDraw()
        {
            float maxT = 0;
            for (int y = 0; y < height; y++)
                for (int x = 0; x < width; x++)
                    if (T[y, x] < float.MaxValue && T[y, x] > maxT)
                        maxT = T[y, x];

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    if (obstacle[y, x])
                    {
                        Gizmos.color = Color.black;
                    }
                    else if (T[y, x] == float.MaxValue)
                    {
                        Gizmos.color = Color.gray;
                    }
                    else
                    {
                        // 成本越高颜色越红
                        float t = Mathf.Clamp01(T[y, x] / maxT);
                        Gizmos.color = Color.Lerp(Color.green, Color.red, t);
                    }

                    // 在Unity中绘制小方块
                    Gizmos.DrawCube(new Vector3(x, 0, y), Vector3.one * 0.9f);
                }
            }
        }
    }

    // 使用示例
    void ExampleUsage()
    {
        // 1. 创建求解器
        int width = 50;
        int height = 50;
        FSSolver solver = new(width, height);

        // 2. 设置地形成本
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                // 设置一些障碍物
                if (x == 20 && y > 10 && y < 40)
                    solver.SetObstacle(x, y, true);

                // 设置沼泽区域（高成本）
                if (Mathf.Pow(x - 35, 2) + Mathf.Pow(y - 25, 2) < 100)
                    solver.SetCost(x, y, 3.0f);
            }
        }

        // 3. 设置目标点并求解
        int goalX = 45;
        int goalY = 45;
        solver.Solve(goalX, goalY, maxIterations: 8);

        // 4. 获取成本场和流场
        float[,] costField = solver.CostField;
        Vector2[,] vectorField = solver.GenerateVectorField();

        // 5. 单位可以根据vectorField来移动
        // 单位位置 -> 找到对应网格 -> 获取向量 -> 移动
    }
}

public class NavFlowField : IHeatMap
{
    private Rect m_mapRect;
    private List<IObstacle> m_obstacles;
    private Vector2Int m_goal;

    FastSweepingSolver.FSSolver m_solver;

    public event OnHeatMapChange OnHeatMapChange;

    public void Initialize(Rect mapRect, int numXCells, int numYCells)
    {
        m_mapRect = mapRect;
        m_obstacles = new List<IObstacle>();
        m_solver = new FastSweepingSolver.FSSolver(numXCells, numYCells);
    }

    public void Update()
    {
        m_solver.ClearObstacle();
        foreach (IObstacle obstacle in m_obstacles)
        {
            for (int x = obstacle.ObstacleRect.xMin; x <= obstacle.ObstacleRect.xMax; x++)
            {
                for (int y = obstacle.ObstacleRect.yMin; y <= obstacle.ObstacleRect.yMax; y++)
                {
                    m_solver.SetObstacle(x, y, true);
                }
            }
        }
        m_solver.Solve(m_goal.x, m_goal.y, maxIterations: 8);

        OnHeatMapChange?.Invoke();
    }

    public float GetValue(float x, float y)
    {
        int width = m_solver.CostField.GetLength(0);
        int length = m_solver.CostField.GetLength(1);
        int cellX = Mathf.Clamp((int)((x - m_mapRect.xMin) / m_mapRect.width * m_solver.width), 0, width - 1);
        int cellY = Mathf.Clamp((int)((y - m_mapRect.yMin) / m_mapRect.height * m_solver.height), 0, length - 1);
        return m_solver.CostField[cellX, cellY];
    }

    public Vector2 GetGradient(float x, float y)
    {
        int width = m_solver.CostField.GetLength(0);
        int length = m_solver.CostField.GetLength(1);
        int cellX = Mathf.Clamp((int)((x - m_mapRect.xMin) / m_mapRect.width * m_solver.width), 0, width - 1);
        int cellY = Mathf.Clamp((int)((y - m_mapRect.yMin) / m_mapRect.height * m_solver.height), 0, length - 1);
        return m_solver.GetGrad(cellX, cellY);
    }

    public void AddObstacle(IObstacle obstacle)
    {
        m_obstacles.Add(obstacle);
    }

    public void RemoveObstacle(IObstacle obstacle)
    {
        m_obstacles.Remove(obstacle);
    }

    public void SetGoal(Vector2Int goal)
    {
        m_goal = goal;
    }

    public void Clear()
    {
        m_obstacles.Clear();
    }
}
