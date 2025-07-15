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
            GameObject Towers =  GameObject.Find("Towers");
            if (Towers == null)
            {
                Towers = new GameObject("Towers");
            }
            turret.transform.parent = Towers.transform;
            Transform quad = turret.transform.Find("Quad");
            if (quad != null)
            {
                quad.gameObject.SetActive(false);
            }
            grid.AssignTurretToGrid(turret);
            UIManager.instance.setHealthBar(turret);
            turretToBuild = null;
        }
        else
        {
            //cant build UI
            Debug.Log("Cannot build here!");
        }
    }
    public bool CanBuild { get { return turretToBuild != null; } }
}


