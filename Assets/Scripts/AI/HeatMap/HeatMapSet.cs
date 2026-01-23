using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum HeatMapType
{
    NavFlowField,
}
    
public class HeatMapSet
{
    public NavFlowField NavFlowField = new();

    public void Initialize()
    {
        int mapWidth = GridManager.Instance.Width;
        int mapLength = GridManager.Instance.Length;
        Rect mapRectInWorld = GridManager.Instance.GetMapRectInWorld();
        NavFlowField.Initialize(mapRectInWorld, mapLength, mapWidth);
    }

    public void Refresh()
    {
        NavFlowField.Update();
    }
}