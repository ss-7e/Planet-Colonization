using UnityEngine;

public enum MapType
{
    Square,
    Hexagon
}
public class GridManager : MonoBehaviour
{
    public static GridManager Instance;
    public MapType MapType;
    public Grid[] Grids;
    public int Length { get; set; }
    public int Width { get; set; }
    void Awake()
    {
        Instance = this;
        Grids = null;
        LoadGridFromJson();
    }

    void LoadGridFromJson()
    {
        string path = Application.dataPath + "/grid_data.json";

        if (!System.IO.File.Exists(path))
        {
            return;
        }

        string json = System.IO.File.ReadAllText(path);
        GridData[] gridDataArray = Newtonsoft.Json.JsonConvert.DeserializeObject<GridData[]>(json);
        Width = 1000;
        Length = 1000;

        Grids = new Grid[gridDataArray.Length];

        for (int i = 0; i < gridDataArray.Length; i++)
        {
            var data = gridDataArray[i];
            Grids[i] = new Grid(data.GetPosition());
        }

    }

    public Grid GetGridXY(int x, int y)
    {
        if (x < 0 || x >= Length || y < 0 || y >= Width)
        {
            return null;
        }
        return Grids[y * Width + x];
    }

    public void SetGridPos(int x, int y, Grid value)
    {
        Grids[y * Width + x] = value;
    }

    public Vector2Int GetGridXYValue(Vector3 pos)
    {
        pos.x += Length / 2;
        pos.z += Width / 2;
        int x = Mathf.RoundToInt(pos.x);
        int z = Mathf.RoundToInt(pos.z);
        return new Vector2Int(x, z);
    }
    public Grid GetGridByXZ(float x, float z, out Vector2Int gridXY)
    {
        Vector3 pos = new(x, 0, z);
        gridXY = GetGridXYValue(pos);
        return GetGridXY(gridXY.x, gridXY.y);
    }

    public Grid GetGridByPos(Vector3 pos, out Vector2Int gridXY)
    {
        gridXY = GetGridXYValue(pos);
        return GetGridXY(gridXY.x, gridXY.y);
    }

    public Rect GetMapRectInWorld()
    {
        return new Rect(-Length / 2 - 0.5f, -Width / 2 - 0.5f, Length, Width);
    }
}
