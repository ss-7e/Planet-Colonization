using UnityEngine;
using UnityEngine.EventSystems;

public class GridSelector : MonoBehaviour
{
    public GameObject selectionIndicator;
    public float maxDistance = 20f;
    private Grid gridSelected;
    private void Start()
    {
    }

    private void Update()
    {
        if (Input.GetMouseButton(0))
        {
            HighlightGrid();
        }
        if(Input.GetMouseButtonUp(0))
        {
            selectionIndicator.SetActive(false);
            buildTowerAt();
        }
        //HighlightGrid();
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
                hitPoint = GridManager.instance.grid[x, z].pos;
                hitPoint.y += 0.5f;
                selectionIndicator.SetActive(true);
                selectionIndicator.transform.position = hitPoint;
                gridSelected = GridManager.instance.grid[x, z];
            }
        }
    }
    private void buildTowerAt()
    {
        BuildManager.instance.BuildTurret(gridSelected);
    }
}