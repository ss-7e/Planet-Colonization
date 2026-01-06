using UnityEngine;

public delegate void OnHeatMapChange();
    
/// <summary>
/// 热力图接口
/// </summary>
public interface IHeatMap
{
    public event OnHeatMapChange OnHeatMapChange;

    /// <summary>
    /// 更新 tick
    /// </summary>
    public void Update();
        
    /// <summary>
    /// 获取热力值
    /// </summary>
    /// <param name="worldX">x坐标</param>
    /// <param name="worldY">y坐标</param>
    /// <returns>热力值</returns>
    public float GetValue(float worldX, float worldY);
        
    /// <summary>
    /// 获取热力梯度
    /// </summary>
    /// <param name="worldX">x坐标</param>
    /// <param name="worldY">y坐标</param>
    /// <returns>热力梯度</returns>
    public Vector2 GetGradient(float worldX, float worldY);
}