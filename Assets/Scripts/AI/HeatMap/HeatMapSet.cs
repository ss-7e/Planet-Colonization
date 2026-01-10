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
        int mapWidth = GridManager.instance.width;
        int mapLength = GridManager.instance.length;
        Rect mapRectInWorld = GridManager.instance.GetMapRectInWorld();
        NavFlowField.Initialize(mapRectInWorld, mapLength, mapWidth);
    }

    public void Refresh()
    {
        NavFlowField.Update();
    }
}