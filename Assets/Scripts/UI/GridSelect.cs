using Game.Towers;
using Game.Towers.Turrets;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public class GridSelector : MonoBehaviour
{
    public GameObject ConnectTowerUIPrefab;
    public GameObject selectionIndicator;
    public float maxDistance = 20f;
    public Vector3 targetSize = new Vector3(1f, 1f, 1f);
    private Mesh mesh;
    private Vector3[] originalVertices;
    private Grid gridSelected;
    private TurretBase previousTurret = null;
    private Tower previousTower = null;

    private void Start()
    {
        mesh = selectionIndicator.GetComponent<MeshFilter>().mesh;
        originalVertices = mesh.vertices;
        selectionIndicator.SetActive(false);
    }
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            //Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            //if (EventSystem.current.IsPointerOverGameObject() || Physics.Raycast(ray, out RaycastHit hitInfo, maxDistance, LayerMask.GetMask("Build")))
            //{
            //    return;
            //}
            ClickGrid();
        }
        HighlightGrid();
        ResizeFrame(targetSize);
    }

    public void ResizeFrame(Vector3 newSize)
    {
        Vector3[] vertices = new Vector3[originalVertices.Length];
        float halfX = newSize.x / 2f;
        float halfY = newSize.y / 2f;
        float halfZ = newSize.z / 2f;

        for (int i = 0; i < vertices.Length; i++)
        {
            Vector3 v = originalVertices[i];

            float signX = Mathf.Sign(v.x);
            float signY = Mathf.Sign(v.y);
            float signZ = Mathf.Sign(v.z);


            float absX = Mathf.Abs(v.x);
            float absY = Mathf.Abs(v.y);
            float absZ = Mathf.Abs(v.z);

            if(signX > 0)
                v.x = halfX + absX - 0.5f;

            if(signZ > 0)
                v.z = halfZ + absZ - 0.5f;


            vertices[i] = v;
        }

        mesh.vertices = vertices;
        mesh.RecalculateBounds();
    }

    private void HighlightGrid()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit[] hits = Physics.RaycastAll(ray, maxDistance, LayerMask.GetMask("Default"));
        if(Physics.Raycast(ray, out RaycastHit hitInfo, maxDistance, LayerMask.GetMask("Build")))
        {
            BuildManager.Instance.TryBuildingOnGrid(gridSelected, false);
            return;
        }
        
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }
        foreach (RaycastHit hit in hits)
        {
            if (hit.collider.gameObject.tag == "GridMap")
            {
                Vector3 hitPoint = hit.point;
                hitPoint.x += GridManager.Instance.Length / 2;
                hitPoint.z += GridManager.Instance.Width / 2;
                int x = Mathf.RoundToInt(hitPoint.x);
                int z = Mathf.RoundToInt(hitPoint.z);
                Vector3 pos = GridManager.Instance.GetGridXY(x, z).Pos;
                hitPoint = pos;
                hitPoint.y += 0.5f;
                selectionIndicator.SetActive(true);
                selectionIndicator.transform.position = hitPoint;
                gridSelected = GridManager.Instance.GetGridXY(x, z);

                BuildManager.Instance.TryBuildingOnGrid(gridSelected, true);
                return;
            }
        }
        BuildManager.Instance.TryBuildingOnGrid(gridSelected, false);
    }
    private void ConfirmBuild()
    {
        BuildManager.Instance.ConfirmBuildOnGrid(gridSelected);
    }
    private void ClickGrid()
    {
        if (gridSelected != null)
        {
            if (previousTower != null)
            {
                UIManager.instance.HideTowerUI(previousTower);
                previousTurret = null;
            }
            if (gridSelected.hasTower())
            {
                Tower tower = gridSelected.BuildingOnGrid.GetComponent<Tower>();
                SetConnectTowerUI(tower);
                UIManager.instance.SetTowerUI(tower, isLeft: Input.mousePosition.x < Screen.width / 2f);
                previousTower = tower;
            }
            if(gridSelected.IsObstacle)
            {
                return;
            }
            ConfirmBuild();
        }
        else
        {
            Debug.Log("No grid selected.");
        }
    }
    void SetConnectTowerUI(Tower tower)
    {
        List<StoragePrev> storageList = tower.GetStorageList();
        Dictionary<StoragePrev, Tower> storageTowerList = tower.GetStorageTowerList();
        GameObject connectTowerUIParent = GameObject.Find("ConnectTower");
        if (connectTowerUIParent == null)
        {
            Debug.LogError("ConnectTower UI not found!");
            return;
        }
        foreach (Transform child in connectTowerUIParent.transform)
        {
            Destroy(child.gameObject);
        }
        foreach (StoragePrev storage in storageList)
        {
            if (storageTowerList.ContainsKey(storage))
            {
                Tower storageTower = storageTowerList[storage];
                if (storageTower != null)
                {
                    Vector3 UIPos = storageTower.onGrid.Pos + new Vector3(0, 3f, 0);
                    GameObject connectTowerUI = Instantiate(ConnectTowerUIPrefab, UIPos, Quaternion.identity);
                    connectTowerUI.transform.SetParent(connectTowerUIParent.transform);
                }
            }
        }
    }

}