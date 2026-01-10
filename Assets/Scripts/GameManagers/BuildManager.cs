using Game.Towers.Mine;
using Game.Towers.Turrets;
using Game.Towers.Factory;
using Game.Towers;
using Factory;
using System.Collections.Generic;
using UnityEngine;
using Factory;

public class BuildManager : MonoBehaviour
{
    public static BuildManager instance;

    public List<GameObject> canveyerBelts;


    private List<TurretBase> turretList = new List<TurretBase>();
    private List<Miner> miners = new List<Miner>();
    private List<StorageTower> storageTowers = new List<StorageTower>();
    private List<FactoryTowerBase> factoryTowers = new List<FactoryTowerBase>();
    private GameObject objectToBuild;
    private CanveyerBeltBuild canveyerBeltBuild = null;

    void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("More than one BuildManager in scene!");
            return;
        }
        instance = this;
    }

    private void Update()
    {
        if(canveyerBeltBuild != null) {
            canveyerBeltBuild.Update();
        }
    }

    public void SetCanveyerBeltBuild()
    {
        canveyerBeltBuild = new CanveyerBeltBuild();
        canveyerBeltBuild.Awake();
    }

    public void SetObjectToBuild(GameObject obj)
    {
        if (objectToBuild != null) {
            Destroy(objectToBuild);
        }
        objectToBuild = Instantiate(obj);
        objectToBuild.SetActive(false);
    }

    public void ClearObjectToBuild()
    {
        if (objectToBuild != null)
        {
            Destroy(objectToBuild);
        }
        objectToBuild = null;
        canveyerBeltBuild = null;
    }

    public GameObject GetObjectToBuild()
    {
        return objectToBuild;
    }

    /// <summary>
    /// 点击格子尝试建造塔，或许应该改成观察者模式
    /// </summary>
    /// <param name="grid"></param>
    public void ConfirmBuildOnGrid(Grid grid)
    {
        if (objectToBuild == null || !grid.canBuild()) return;
        if (objectToBuild.GetComponent<Tower>())
        {
            BuildTowerOnGrid(grid);
        }
        else if (objectToBuild.GetComponent<Factory.FactorySquare>())
        {
            BuildFactoryOnGrid(grid);
        }
    }


    public void TryBuildingOnGrid(Grid grid, bool set)
    {
        if (objectToBuild == null) return;
        if(set && grid.canBuild())
        {
            objectToBuild.SetActive(true);
            if (objectToBuild.GetComponent<Tower>())
            {
                Vector3 towerPos = grid.pos + new Vector3(0, 0.5f, 0);
                objectToBuild.transform.position = towerPos;
            }
            else if (objectToBuild.GetComponent<Factory.FactorySquare>())
            {
                objectToBuild.transform.position = grid.pos + new Vector3(0, 1f, 0);
            }
        }
        else
        {
            objectToBuild.SetActive(false);
        }
    }
    

    private void BuildFactoryOnGrid(Grid grid)
    {
        objectToBuild.SetActive(true);
        objectToBuild.transform.position = grid.pos + new Vector3(0, 1f, 0);

        GameObject Factories = GameObject.Find("Factories");
        if (Factories == null)
        {
            Factories = new GameObject("Factories");
        }
        objectToBuild.transform.parent = Factories.transform;
        SelectFactory.instance.OnCancelSelect();
        objectToBuild.GetComponent<Factory.FactorySquare>().ConfirmBuild();
        objectToBuild.GetComponent<BuildingProcess>().ConfirmBuild();
        grid.AddFactoryToGrid(objectToBuild);
        objectToBuild = null;

    }


    private void BuildTowerOnGrid(Grid grid)
    {
        if (objectToBuild == null)
        {
            return;
        }
        if (grid.canBuild())
        {
            //if (!Cargo.instance.FindTower(objectToBuild.GetComponent<Tower>())) 
            //{
            //    return;
            //}
            Vector3 towerPos = grid.pos + new Vector3(0, 0.5f, 0);
            objectToBuild.SetActive(true);
            objectToBuild.transform.position = towerPos;
            GameObject tower = objectToBuild;
            GameObject Towers =  GameObject.Find("Towers");
            if (Towers == null)
            {
                Towers = new GameObject("Towers");
            }
            //finish set GameObject

            objectToBuild = null;
            tower.GetComponent<Tower>().BuildOnGrid(grid);
            AddToTowerList(tower);

            tower.transform.parent = Towers.transform;
            Transform quad = tower.transform.Find("Quad");
            if (quad != null)
            {
                quad.gameObject.SetActive(false);
            }
            grid.AssignTurretToGrid(tower);
            UIManager.instance.setHealthBar(tower);
            UIManager.instance.downSelectionBarFrame.gameObject.SetActive(false);
            
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
                turretList.Add(turret);
                SetTowerStorage(turret);
                SetFactoryAffectTurret(turret);
                break;
            case Miner miner:
                miners.Add(miner);
                SetTowerStorage(miner);
                break;
            case StorageTower storage:
                storageTowers.Add(storage);
                UpdateTowersStorage();
                break;
            case FactoryTowerBase factory:
                factoryTowers.Add(factory);
                SetTowerStorage(factory);
                SetFactoryTurretList(factory);
                break;
        }

    }


    void SetTowerStorage(Tower tower)
    {
        Vector3 TowerPos = tower.onGrid.pos;
        foreach (StorageTower storageTower in storageTowers)
        { 
            Vector3 storagePos = storageTower.onGrid.pos;
            float distance = (TowerPos - storagePos).magnitude;
            if (distance < 10)
            {
                tower.addStorage(storageTower, storageTower.GetStorage(), distance);
            }
        }
    }

    void SetFactoryAffectTurret(TurretBase turret)
    {
        Vector3 TowerPos = turret.onGrid.pos;
        foreach (FactoryTowerBase factory in factoryTowers)
        {
            Vector3 factoryPos = factory.onGrid.pos;
            float distance = (TowerPos - factoryPos).magnitude;
            if (distance < 10)
            {
                factory.AddTurrettoList(turret);
            }
        }
    }
    void SetFactoryTurretList(FactoryTowerBase factory)
    {
        Vector3 factoryPos = factory.onGrid.pos;
        foreach (TurretBase turret in turretList)
        {
            Vector3 TowerPos = turret.onGrid.pos;
            float distance = (TowerPos - factoryPos).magnitude;
            if (distance < 10)
            {
                factory.AddTurrettoList(turret);
            }
        }
    }


    void UpdateTowersStorage()
    {
        foreach (TurretBase turret in turretList)
        {
            SetTowerStorage(turret);
            Debug.LogWarning($"Updated storage for turret: {turret.name}");
        }
        foreach (Miner miner in miners)
        {
            SetTowerStorage(miner);
            Debug.LogWarning($"Updated storage for miner: {miner.name}");
        }
        foreach (FactoryTowerBase factory in factoryTowers)
        {
            Debug.LogWarning($"Updated storage for factory: {factory.name}");
            SetTowerStorage(factory);
        }
    }

}


