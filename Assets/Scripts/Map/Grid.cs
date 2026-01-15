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
    public GameObject BuildingOnGrid = null;
    public List<GameObject> FactoryTowers = new List<GameObject>();
    public bool IsObstacle = false;
    public bool IsShipyard = false;
    public GridType GridType;

    internal IConnectTo ConnectableBuilding = null; //可连接物品，如工厂输入输出口，传送带等
    public FactoryProducer ProducerFrom = null;             //工厂输出接口前方的格子
    public FactoryProducer ProducerTo = null;               //TODO: 后续需要做成抽象
    public CanveyerBeltUnit CanveyerBeltUnitFrom = null;    //传送带输出口前方格子
    public CanveyerBeltUnit CanveyerBeltUnitTo = null;      //传送带输入口前方格子

    public Grid(Vector3 gridPos)
    {
        Pos = gridPos;
        FactoryTowers = new List<GameObject>();
    }

    public Vector3 Pos { get; private set; }

    public void AddFactoryToGrid(GameObject factory)
    {
        FactoryTowers.Add(factory);
    }

    public void RemoveFactoryFromGrid(GameObject factory)
    {
        FactoryTowers.Remove(factory);
    }

    public void AssignBuildingToGrid(GameObject tower)
    {
        this.BuildingOnGrid = tower;
    }

    public void destroyTower()
    {
        if (BuildingOnGrid != null)
        {
            BuildingOnGrid = null;
        }
    }

    public bool canBuild()
    {
        if (BuildingOnGrid == null)
        {
            return true && !IsObstacle && !IsShipyard ;
        }
        return false;
    }
    public bool hasTower()
    {
        return BuildingOnGrid != null;
    }

    public bool hasTurret()
    {
        return BuildingOnGrid != null && BuildingOnGrid.GetComponent<TurretBase>() != null;
    }
}
