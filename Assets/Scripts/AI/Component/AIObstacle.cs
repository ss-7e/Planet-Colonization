using UnityEngine;

/// <summary>
/// AI 障碍物组件
/// </summary>
public class AIObstacle : MonoBehaviour, IObstacle
{
    /// <summary>
    /// 障碍物尺寸
    /// </summary>
    [SerializeField]
    private Vector2Int _obstacleRect;

    /// <summary>
    /// 障碍物强度
    /// </summary>
    [SerializeField]
    private float _obstacleStrength;

    // 这里还要处理世界坐标到地图坐标的转换...应该加一个转换层才对
    public RectInt ObstacleRect
    {
        get
        {
            Vector2Int gridPos = GridManager.Instance.GetGridXYValue(transform.position);
            return new RectInt(gridPos, _obstacleRect);
        }
    }

    public float ObstacleStrength => _obstacleStrength;

    private void OnEnable()
    {
        AIModule.Instance.HeatMapSet.NavFlowField.AddObstacle(this);
    }

    private void OnDisable()
    {
        AIModule.Instance.HeatMapSet.NavFlowField.RemoveObstacle(this);
    }
}