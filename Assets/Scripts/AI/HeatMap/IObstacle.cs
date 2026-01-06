using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 障碍物接口
/// </summary>
public interface IObstacle
{
    /// <summary>
    /// 障碍物占据的地图矩形范围
    /// </summary>
    RectInt ObstacleRect { get; }
        
    /// <summary>
    /// 障碍物的强度，影响寻路权重
    /// </summary>
    float ObstacleStrength { get; }
}
