using Game.Towers.Mine;
using Game.Towers.Turrets;
using Game.Towers.Factory;
using Game.Towers;
using Factory;
using System.Collections.Generic;
using UnityEngine;

public delegate void OnBuildDelegate();

public class BuildManager : MonoBehaviour
{
    public static BuildManager Instance;

    public List<GameObject> ConveyorBelts;

    /// <summary>
    /// 建造事件
    /// </summary>
    public event OnBuildDelegate OnBuildEvent;

    //TODO 应当存这么多列表吗？
    private List<TurretBase> _turretList = new();
    private List<Miner> _miners = new();
    private List<StorageTower> _storageTowers = new();
    private List<FactoryTowerBase> _factoryTowers = new();

    private GameObject _objectToBuild;
    private ConveyorBeltBuild _conveyorBeltBuild = null;

    void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("More than one BuildManager in scene!");
            return;
        }
        Instance = this;
    }

    private void Update()
    {
        _conveyorBeltBuild?.Update();
    }

    public void SetConveyorBeltBuild()
    {
        _conveyorBeltBuild ??= new ConveyorBeltBuild();
        _conveyorBeltBuild.Awake();
    }


    //以下修改：是否应该不依赖于GameObject？
    public void SetObjectToBuild(GameObject obj)
    {
        if (_objectToBuild != null) {
            Destroy(_objectToBuild);
        }
        _objectToBuild = Instantiate(obj);
        _objectToBuild.SetActive(false);
    }

    public void ClearObjectToBuild()
    {
        if (_objectToBuild != null)
        {
            Destroy(_objectToBuild);
        }
        _objectToBuild = null;
        _conveyorBeltBuild = null;
    }

    public GameObject GetObjectToBuild()
    {
        return _objectToBuild;
    }

    /// <summary>
    /// 点击格子尝试建造塔，或许应该改成观察者模式
    /// TODO: 想办法弄掉getcomponent
    ///
    /// </summary>
    /// <param name="grid"></param>
    public void ConfirmBuildOnGrid(Grid grid)
    {
        if (_objectToBuild == null || !grid.CanBuild()) return;
        if (_objectToBuild.GetComponent<Tower>())
        {
            BuildTowerOnGrid(grid);
        }
        else if (_objectToBuild.GetComponent<FactorySquare>())
        {
            BuildFactoryOnGrid(grid);
        }
    }


    public void TryBuildingOnGrid(Grid grid, bool set)
    {
        if (_objectToBuild == null) return;
        if(set && grid.CanBuild())
        {
            _objectToBuild.SetActive(true);
            if (_objectToBuild.GetComponent<Tower>())
            {
                Vector3 towerPos = grid.Pos + new Vector3(0, 0.5f, 0);
                _objectToBuild.transform.position = towerPos;
            }
            else if (_objectToBuild.GetComponent<Factory.FactorySquare>())
            {
                _objectToBuild.transform.position = grid.Pos + new Vector3(0, 1f, 0);
            }
        }
        else
        {
            _objectToBuild.SetActive(false);
        }
    }
    

    private void BuildFactoryOnGrid(Grid grid)
    {
        _objectToBuild.SetActive(true);
        _objectToBuild.transform.position = grid.Pos + new Vector3(0, 1f, 0);


        GameObject Factories = GameObject.Find("Factories");
        if (Factories == null)
        {
            Factories = new GameObject("Factories");
        }
        _objectToBuild.transform.parent = Factories.transform;


        SelectFactory.Instance.OnCancelSelect();
        _objectToBuild.GetComponent<BuildingProcess>().ConfirmBuild(); //TODO：后续这个都得删掉

        FactorySquare factory = _objectToBuild.GetComponent<FactorySquare>();
        (factory as IItemOutput)?.SetOutputOnGrid(grid);
        (factory as IItemInput)?.SetInputOnGrid(grid);
        (factory as IItemInput)?.ConnetTo(grid);

        grid.AddFactoryToGrid(_objectToBuild);
        _objectToBuild = null;

        OnBuildEvent?.Invoke();
    }


    private void BuildTowerOnGrid(Grid grid)
    {
        if (_objectToBuild == null)
        {
            return;
        }
        if (grid.CanBuild())
        {
            //if (!Cargo.instance.FindTower(objectToBuild.GetComponent<Tower>())) 
            //{
            //    return;
            //}
            Vector3 towerPos = grid.Pos + new Vector3(0, 0.5f, 0);
            _objectToBuild.SetActive(true);
            _objectToBuild.transform.position = towerPos;
            GameObject tower = _objectToBuild;
            GameObject Towers =  GameObject.Find("Towers");
            if (Towers == null)
            {
                Towers = new GameObject("Towers");
            }
            //finish set GameObject

            _objectToBuild = null;
            tower.GetComponent<Tower>().BuildOnGrid(grid);
            AddToTowerList(tower);

            tower.transform.parent = Towers.transform;
            Transform quad = tower.transform.Find("Quad");
            if (quad != null)
            {
                quad.gameObject.SetActive(false);
            }
            grid.AssignBuildingToGrid(tower);
            UIManager.instance.setHealthBar(tower);
            UIManager.instance.downSelectionBarFrame.gameObject.SetActive(false);

            OnBuildEvent?.Invoke();
        }
        else
        {
            //cant build UI
            Debug.LogWarning("Cannot build here!");
        }
    }

    void AddToTowerList(GameObject tower)
    {
        Tower towerData = tower.GetComponent<Tower>();
        switch (towerData)
        {
            case TurretBase turret:
                _turretList.Add(turret);
                SetTowerStorage(turret);
                SetFactoryAffectTurret(turret);
                break;
            case Miner miner:
                _miners.Add(miner);
                SetTowerStorage(miner);
                break;
            case StorageTower storage:
                _storageTowers.Add(storage);
                UpdateTowersStorage();
                break;
            case FactoryTowerBase factory:
                _factoryTowers.Add(factory);
                SetTowerStorage(factory);
                SetFactoryTurretList(factory);
                break;
        }

    }

    //TODO 移除以下玩意
    void SetTowerStorage(Tower tower)
    {
        Vector3 TowerPos = tower.onGrid.Pos;
        foreach (StorageTower storageTower in _storageTowers)
        { 
            Vector3 storagePos = storageTower.onGrid.Pos;
            float distance = (TowerPos - storagePos).magnitude;
            if (distance < 10)
            {
                tower.addStorage(storageTower, storageTower.GetStorage(), distance);
            }
        }
    }

    void SetFactoryAffectTurret(TurretBase turret)
    {
        Vector3 TowerPos = turret.onGrid.Pos;
        foreach (FactoryTowerBase factory in _factoryTowers)
        {
            Vector3 factoryPos = factory.onGrid.Pos;
            float distance = (TowerPos - factoryPos).magnitude;
            if (distance < 10)
            {
                factory.AddTurrettoList(turret);
            }
        }
    }
    void SetFactoryTurretList(FactoryTowerBase factory)
    {
        Vector3 factoryPos = factory.onGrid.Pos;
        foreach (TurretBase turret in _turretList)
        {
            Vector3 TowerPos = turret.onGrid.Pos;
            float distance = (TowerPos - factoryPos).magnitude;
            if (distance < 10)
            {
                factory.AddTurrettoList(turret);
            }
        }
    }


    void UpdateTowersStorage()
    {
        foreach (TurretBase turret in _turretList)
        {
            SetTowerStorage(turret);
            Debug.LogWarning($"Updated storage for turret: {turret.name}");
        }
        foreach (Miner miner in _miners)
        {
            SetTowerStorage(miner);
            Debug.LogWarning($"Updated storage for miner: {miner.name}");
        }
        foreach (FactoryTowerBase factory in _factoryTowers)
        {
            Debug.LogWarning($"Updated storage for factory: {factory.name}");
            SetTowerStorage(factory);
        }
    }

}


