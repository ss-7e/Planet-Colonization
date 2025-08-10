using Game.Towers.Mine;
using Game.Towers.Turrets;
using Game.Towers.Factory;
using Game.Towers;
using System.Collections.Generic;
using UnityEngine;

public class BuildManager : MonoBehaviour
{
    public static BuildManager instance;
    private List<TurretBase> turretList = new List<TurretBase>();
    private List<Miner> miners = new List<Miner>();
    private List<StorageTower> storageTowers = new List<StorageTower>();
    private List<FactroyTowerBase> factoryTowers = new List<FactroyTowerBase>();
    void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("More than one BuildManager in scene!");
            return;
        }
        instance = this;
    }

    private GameObject towerToBuild;

    public void SetTurretToBuild(GameObject turret)
    {
        towerToBuild = turret;
    }

    public GameObject GetTurretToBuild()
    {
        return towerToBuild;
    }

    public void BuildTower(Grid grid)
    {
        if (towerToBuild == null)
        {
            return;
        }
        if (grid.canBuild())
        {
            Vector3 towerPos = grid.pos + new Vector3(0, 0.5f, 0);
            GameObject tower = (GameObject)Instantiate(towerToBuild, towerPos, Quaternion.identity);
            GameObject Towers =  GameObject.Find("Towers");
            if (Towers == null)
            {
                Towers = new GameObject("Towers");
            }
            //finish set GameObject

            towerToBuild = null;
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
                break;
            case Miner miner:
                miners.Add(miner);
                SetTowerStorage(miner);
                break;
            case StorageTower storage:
                storageTowers.Add(storage);
                UpdateTowersStorage();
                break;
            case FactroyTowerBase factory:
                factoryTowers.Add(factory);
                SetTowerStorage(factory);
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
                tower.addStorage(storageTower.GetStorage(), distance);
            }
        }
    }
    void UpdateTowersStorage()
    {
        foreach (TurretBase turret in turretList)
        {
            SetTowerStorage(turret);
        }
        foreach (Miner miner in miners)
        {
            SetTowerStorage(miner);
        }
        foreach (FactroyTowerBase factory in factoryTowers)
        {
            SetTowerStorage(factory);
        }
    }

}


