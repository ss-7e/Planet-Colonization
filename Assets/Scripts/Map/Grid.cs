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
    public List<GameObject> FactoryTowers = new();
    public bool IsObstacle = false;
    public bool IsShipyard = false;
    public GridType GridType;


    private List<(IItemOutput, PortDir)> _itemOutputFromBuildingDir = new();        //建筑输出标记格子
    private List<(IItemInput, PortDir)> _itemInputToBuildingDir = new();            //可接受物品输入的建筑

    public IItemOutput ItemOutputFromBuilding => _itemOutputFromBuildingDir.Count > 0 ? _itemOutputFromBuildingDir[0].Item1 : null;
    public IItemInput ItemInputToBuilding => _itemInputToBuildingDir.Count > 0 ? _itemInputToBuildingDir[0].Item1 : null;


    public IReadOnlyList<(IItemOutput, PortDir)> ItemOutputFromBuildingDir => _itemOutputFromBuildingDir;
    public IReadOnlyList<(IItemInput, PortDir)> ItemInputToBuildingDir => _itemInputToBuildingDir;


    //方向含义：从当前格子的dir方向的格子输入到当前格子的建筑
    public void AddItemOutputFromBuildingDir(IItemOutput itemOutput, PortDir dir)
    {
        _itemOutputFromBuildingDir.Add((itemOutput, dir));
    }
    //方向含义：向当前格子的dir方向的格子输出（由本格建筑输出）
    public void AddItemInputToBuildingDir(IItemInput itemInput, PortDir dir)
    {
        _itemInputToBuildingDir.Add((itemInput, dir));
    }
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

    public bool CanBuild()
    {
        if (BuildingOnGrid == null)
        {
            return true && !IsObstacle && !IsShipyard ;
        }
        return false;
    }
    public bool HasTower()
    {
        return BuildingOnGrid != null;
    }

    public bool HasTurret()
    {
        return BuildingOnGrid != null && BuildingOnGrid.GetComponent<TurretBase>() != null;
    }
}
