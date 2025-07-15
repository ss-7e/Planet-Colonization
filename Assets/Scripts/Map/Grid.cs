using System.Collections;
using System.Collections.Generic;
using UnityEngine;
enum ResourceType
{
    Null,
    Wood,
    Stone,
    Iron,
    Gold,
    Oil,
    Crystal
}
public class Grid
{
    public GameObject tower = null;
    Resource[] resource;

    public Grid(Vector3 gridPos)
    {
        pos = gridPos;
    }

    public Vector3 pos { get; set; }

    public void AssignTurretToGrid(GameObject tower)
    {
        this.tower = tower;
    }

    public void destroyTower()
    {
        if (tower != null)
        {
            tower = null;
        }
    }

    public bool canBuild()
    {
        if (tower == null)
        {
            return true;
        }
        return false;
    }
    public bool hasTower()
    {
        return tower != null;
    }
    public Resource[] getResources()
    {
        return resource;
    }
}
