using System.Collections;
using System.Collections.Generic;
using UnityEngine;
enum GridType
{
    Empty,
    Wall,
    Start,
    End,
    Path
}
public class Grid
{
    GameObject tower;
    public Grid(Vector3 gridPos)
    {
        pos = gridPos;
    }
    public Vector3 pos { get; set; }
    public void buildTower(GameObject tower)
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
    GridType type;
    
}
