using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// FSS求解器类
public class FastSweepingSolver
{
    private int _width;
    private int _height;
    private float[,] _cost_field;
    private float[,] _cost_map;
    private bool[,] _obstacle_map;

    public float[,] CostField => _cost_field;
    public int Width => _width;
    public int Height => _height;

    public FastSweepingSolver(int width, int height)
    {
        _width = width;
        _height = height;
        _cost_field = new float[height, width];
        _cost_map = new float[height, width];
        _obstacle_map = new bool[height, width];

        // 初始化F为1（标准成本）
        for (int y = 0; y < height; y++)
            for (int x = 0; x < width; x++)
                _cost_map[y, x] = 1.0f;
    }

    // 设置慢度场/成本
    public void SetCost(int x, int y, float cost)
    {
        if (cost <= 0)
        {
            _obstacle_map[y, x] = true;
            _cost_map[y, x] = 0;
        }
        else
        {
            _obstacle_map[y, x] = false;
            _cost_map[y, x] = cost;
        }
    }

    // 设置障碍物
    public void SetObstacle(int x, int y, bool isObstacle)
    {
        _obstacle_map[y, x] = isObstacle;
        _cost_map[y, x] = isObstacle ? 0 : 1.0f;
    }

    public void ClearObstacle()
    {
        _obstacle_map = new bool[_height, _width];
    }

    // 核心：求解局部Eikonal方程
    private float SolveLocal(int x, int y)
    {
        if (_obstacle_map[y, x]) return float.MaxValue;

        float Fval = _cost_map[y, x];
        if (Fval <= 0) return float.MaxValue;

        // 获取四个邻居的值
        float left = (x > 0 && !_obstacle_map[y, x - 1]) ? _cost_field[y, x - 1] : float.MaxValue;
        float right = (x < _width - 1 && !_obstacle_map[y, x + 1]) ? _cost_field[y, x + 1] : float.MaxValue;
        float up = (y > 0 && !_obstacle_map[y - 1, x]) ? _cost_field[y - 1, x] : float.MaxValue;
        float down = (y < _height - 1 && !_obstacle_map[y + 1, x]) ? _cost_field[y + 1, x] : float.MaxValue;

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
                if (_obstacle_map[y, x]) continue;

                float oldValue = _cost_field[y, x];
                float newValue = SolveLocal(x, y);

                if (newValue < oldValue)
                {
                    _cost_field[y, x] = newValue;
                    float change = oldValue - newValue;
                    if (change > maxChange) maxChange = change;
                }
            }
        }

        return maxChange;
    }

    public void Solve(List<Vector2Int> goals, int maxIterations = 8, float tolerance = 0.001f)
    {
        for (int y = 0; y < _height; y++)
            for (int x = 0; x < _width; x++)
                _cost_field[y, x] = _obstacle_map[y, x] ? float.MaxValue : float.MaxValue;

        foreach (var goal in goals)
        {
            int goalX = goal.x;
            int goalY = goal.y;
            if (!_obstacle_map[goalY, goalX])
                _cost_field[goalY, goalX] = 0;
            else
            {
                SetNearestValidGoal(ref goalX, ref goalY);
                _cost_field[goalY, goalX] = 0;
            }
        }

        for (int iter = 0; iter < maxIterations; iter++)
        {
            float maxChange = 0;

            maxChange = Mathf.Max(maxChange,
                PerformSweep(0, _height, 1, 0, _width, 1));

            maxChange = Mathf.Max(maxChange,
                PerformSweep(0, _height, 1, _width - 1, -1, -1));

            maxChange = Mathf.Max(maxChange,
                PerformSweep(_height - 1, -1, -1, 0, _width, 1));

            maxChange = Mathf.Max(maxChange,
                PerformSweep(_height - 1, -1, -1, _width - 1, -1, -1));

            if (maxChange < tolerance)
            {
                Debug.Log($"FSS converged after {iter + 1} iterations");
                break;
            }
        }
    }

    public void Solve(int goalX, int goalY, int maxIterations = 8, float tolerance = 0.001f)
    {
        Solve(new List<Vector2Int> { new Vector2Int(goalX, goalY) }, maxIterations, tolerance);
    }

    // 寻找最近的可通行点作为目标
    private void SetNearestValidGoal(ref int goalX, ref int goalY)
    {
        // 简单的BFS寻找最近可通行点
        bool[,] visited = new bool[_height, _width];
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

            if (!_obstacle_map[current.y, current.x])
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

                if (nx >= 0 && nx < _width && ny >= 0 && ny < _height && !visited[ny, nx])
                {
                    visited[ny, nx] = true;
                    queue.Enqueue(new Vector2Int(nx, ny));
                }
            }
        }

        // 如果找不到，使用默认点
        goalX = _width / 2;
        goalY = _height / 2;
        _obstacle_map[goalY, goalX] = false;
        _cost_map[goalY, goalX] = 1.0f;
        Debug.LogWarning("No valid goal found, using center");
    }

    public Vector2 GetGrad(int x, int y)
    {
        if (_obstacle_map[y, x] || _cost_field[y, x] == float.MaxValue)
        {
            return Vector2.zero;
        }

        // 计算梯度
        float dx = 0, dy = 0;

        if (x > 0 && !_obstacle_map[y, x - 1] && _cost_field[y, x - 1] != float.MaxValue)
            dx += _cost_field[y, x] - _cost_field[y, x - 1];
        if (x < _width - 1 && !_obstacle_map[y, x + 1] && _cost_field[y, x + 1] != float.MaxValue)
            dx += _cost_field[y, x + 1] - _cost_field[y, x];

        if (y > 0 && !_obstacle_map[y - 1, x] && _cost_field[y - 1, x] != float.MaxValue)
            dy += _cost_field[y, x] - _cost_field[y - 1, x];
        if (y < _height - 1 && !_obstacle_map[y + 1, x] && _cost_field[y + 1, x] != float.MaxValue)
            dy += _cost_field[y + 1, x] - _cost_field[y, x];

        return new Vector2(dx, dy);
    }
}

public class NavFlowField : IHeatMap
{
    private Rect _mapRect;
    private Vector2Int _goal;

    FastSweepingSolver _solver;

    public event HeatMapChangeDelegate HeatMapChangeEvent;

    public void Initialize(Rect mapRect, int numXCells, int numYCells)
    {
        _mapRect = mapRect;
        _solver = new FastSweepingSolver(numXCells, numYCells);
    }

    public void Update()
    {
        _solver.ClearObstacle();

        GridManager gridManager = GridManager.Instance;
        for (int x = 0; x < gridManager.Length; x++)
        {
            for (int y = 0; y < gridManager.Width; y++)
            {
                Grid grid = gridManager.GetGridXY(x, y);
                if (grid.IsObstacle)
                {
                    _solver.SetObstacle(x, y, true);
                }
            }
        }

        List<Vector2Int> goals = new List<Vector2Int>();
        if (gridManager != null && gridManager.Grids != null)
        {
            for (int x = 0; x < gridManager.Length; x++)
            {
                for (int y = 0; y < gridManager.Width; y++)
                {
                    Grid grid = gridManager.GetGridXY(x, y);
                    if (grid != null)
                    {
                        foreach (GameObject factoryObj in grid.FactoryTowers)
                        {
                            if (factoryObj != null)
                            {
                                Vector3 worldPos = factoryObj.transform.position;
                                int cellX = Mathf.RoundToInt((worldPos.x - _mapRect.xMin) / _mapRect.width * _solver.Width);
                                int cellY = Mathf.RoundToInt((worldPos.z - _mapRect.yMin) / _mapRect.height * _solver.Height);
                                cellX = Mathf.Clamp(cellX, 0, _solver.Width - 1);
                                cellY = Mathf.Clamp(cellY, 0, _solver.Height - 1);
                                goals.Add(new Vector2Int(cellX, cellY));
                            }
                        }
                    }
                }
            }
        }

        if (goals.Count > 0)
        {
            _solver.Solve(goals, maxIterations: 8);
        }

        HeatMapChangeEvent?.Invoke();
    }

    public float GetValue(float worldX, float worldZ)
    {
        int width = _solver.CostField.GetLength(1);
        int length = _solver.CostField.GetLength(0);
        int cellX = Mathf.Clamp(Mathf.RoundToInt((worldX - _mapRect.xMin) / _mapRect.width * _solver.Width), 0, width - 1);
        int cellY = Mathf.Clamp(Mathf.RoundToInt((worldZ - _mapRect.yMin) / _mapRect.height * _solver.Height), 0, length - 1);
        return _solver.CostField[cellY, cellX];
    }

    public Vector2 GetGradient(float worldX, float worldZ)
    {
        int width = _solver.CostField.GetLength(1);
        int length = _solver.CostField.GetLength(0);
        int cellX = Mathf.Clamp(Mathf.RoundToInt((worldX - _mapRect.xMin) / _mapRect.width * _solver.Width), 0, width - 1);
        int cellY = Mathf.Clamp(Mathf.RoundToInt((worldZ - _mapRect.yMin) / _mapRect.height * _solver.Height), 0, length - 1);
        return _solver.GetGrad(cellX, cellY);
    }

    public void SetGoal(float worldX, float worldZ)
    {
        int width = _solver.CostField.GetLength(1);
        int length = _solver.CostField.GetLength(0);
        int cellX = Mathf.Clamp(Mathf.RoundToInt((worldX - _mapRect.xMin) / _mapRect.width * _solver.Width), 0, width - 1);
        int cellY = Mathf.Clamp(Mathf.RoundToInt((worldZ - _mapRect.yMin) / _mapRect.height * _solver.Height), 0, length - 1);
        _goal = new Vector2Int(cellX, cellY);
    }
}
