using Game.Towers;
using Game.Towers.Turrets;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.EventSystems;

public class GridSelector : MonoBehaviour
{
    public GameObject selectionIndicator;
    public float maxDistance = 20f;
    public Vector3 targetSize = new Vector3(1f, 1f, 1f);
    private Mesh mesh;
    private Vector3[] originalVertices;
    private Grid gridSelected;
    private TurretBase previousTurret = null;

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
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }
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

            // 判断顶点在正负 X/Y/Z 方向的符号
            float signX = Mathf.Sign(v.x);
            float signY = Mathf.Sign(v.y);
            float signZ = Mathf.Sign(v.z);

            // 原来是 0.5f 的位置 → 变成 halfX/halfY/halfZ
            // 厚度保持不变：偏离中心的量只调整外框部分
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
        BuildManager.instance.BuildTower(gridSelected);
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
            buildTowerAt();
        }
        else
        {
            Debug.Log("No grid selected.");
        }
    }
}