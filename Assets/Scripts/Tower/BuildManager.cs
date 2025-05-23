using UnityEngine;

public class BuildManager : MonoBehaviour
{
    public static BuildManager instance;

    void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("More than one BuildManager in scene!");
            return;
        }
        instance = this;
    }

    private GameObject turretToBuild;

    public void SetTurretToBuild(GameObject turret)
    {
        turretToBuild = turret;
    }

    public GameObject GetTurretToBuild()
    {
        return turretToBuild;
    }

    public void BuildTurret(Grid grid)
    {
        if (turretToBuild == null)
        {
            return;
        }
        if (grid.canBuild())
        {
            Vector3 towerPos = grid.pos + new Vector3(0, 0.5f, 0);
            GameObject turret = (GameObject)Instantiate(turretToBuild, towerPos, Quaternion.identity);
            grid.buildTower(turret);
            turretToBuild = null;
        }
        else
        {
            Debug.Log("Cannot build here!");
        }
    }
    public bool CanBuild { get { return turretToBuild != null; } }
}


