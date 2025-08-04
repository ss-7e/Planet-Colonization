using Game.Tower;
using Game.Turret;
using UnityEngine;
using UnityEngine.EventSystems;

public class GridSelector : MonoBehaviour
{
    public GameObject selectionIndicator;
    public float maxDistance = 20f;

    private Grid gridSelected;
    private TurretBase previousTurret = null;
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }
            ClickGrid();
            buildTowerAt();
        }
        HighlightGrid();
    }
    private void HighlightGrid()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit[] hits = Physics.RaycastAll(ray, maxDistance);
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }
        foreach (RaycastHit hit in hits)
        {
            if (hit.collider.gameObject.tag == "GridMap")
            {
                Vector3 hitPoint = hit.point;
                hitPoint.x += GridManager.instance.length / 2;
                hitPoint.z += GridManager.instance.width / 2;
                int x = Mathf.RoundToInt(hitPoint.x);
                int z = Mathf.RoundToInt(hitPoint.z);
                Vector3 pos = GridManager.instance.GetGridXY(x, z).pos;
                hitPoint = pos;
                hitPoint.y += 0.5f;
                selectionIndicator.SetActive(true);
                selectionIndicator.transform.position = hitPoint;
                gridSelected = GridManager.instance.GetGridXY(x, z);
            }
        }
    }
    private void buildTowerAt()
    {
        BuildManager.instance.BuildTurret(gridSelected);
    }
    private void ClickGrid()
    {
        if (gridSelected != null)
        {
            if (gridSelected.hasTower())
            {
                TurretBase turret = gridSelected.tower.GetComponent<TurretBase>();
                Vector2 clickPosition = Input.mousePosition;
                float screenWidth = Screen.width;
                bool isLeft = clickPosition.x < screenWidth / 2f;
                UIManager.instance.turretUI.SetUI(turret, isLeft);
                previousTurret = turret;
            }
            else if (previousTurret != null)
            {   
                UIManager.instance.turretUI.HideUI(previousTurret);
                previousTurret = null;
            }
        }
        else
        {
            Debug.Log("No grid selected.");
        }
    }
}