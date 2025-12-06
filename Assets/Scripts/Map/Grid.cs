using UnityEngine;
using System.Collections.Generic;
using Game.Towers.Turrets;
public enum GridType
{
    Grass,
    Water,
    Sand,
    Tree,
    Mine,
    Lava
}

public class Grid
{
    public GameObject tower = null;
    public List<GameObject> factoryTowers = new List<GameObject>();
    public bool isObstacle = false;
    public bool isShipyard = false;
    public GridType gridType;

    public Grid(Vector3 gridPos)
    {
        pos = gridPos;
        factoryTowers = new List<GameObject>();
    }

    public Vector3 pos { get; private set; }

    public void AddFactoryToGrid(GameObject factory)
    {
        factoryTowers.Add(factory);
    }

    public void RemoveFactoryFromGrid(GameObject factory)
    {
        factoryTowers.Remove(factory);
    }

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
            return true && !isObstacle && !isShipyard && factoryTowers.Count == 0;
        }
        return false;
    }
    public bool hasTower()
    {
        return tower != null;
    }

    public bool hasTurret()
    {
        return tower != null && tower.GetComponent<TurretBase>() != null;
    }
}
