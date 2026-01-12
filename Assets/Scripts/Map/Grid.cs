using UnityEngine;
using System.Collections.Generic;
using Game.Towers.Turrets;
using Factory;
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
    public GameObject _buildingOnGrid = null;
    public List<GameObject> factoryTowers = new List<GameObject>();
    public bool isObstacle = false;
    public bool isShipyard = false;
    public GridType gridType;
    public FactoryProducer ProducerFrom = null;// 输出工厂目标格子，暂时这么写TODO 后续改进

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
        this._buildingOnGrid = tower;
    }

    public void destroyTower()
    {
        if (_buildingOnGrid != null)
        {
            _buildingOnGrid = null;
        }
    }

    public bool canBuild()
    {
        if (_buildingOnGrid == null)
        {
            return true && !isObstacle && !isShipyard && factoryTowers.Count == 0;
        }
        return false;
    }
    public bool hasTower()
    {
        return _buildingOnGrid != null;
    }

    public bool hasTurret()
    {
        return _buildingOnGrid != null && _buildingOnGrid.GetComponent<TurretBase>() != null;
    }
}
