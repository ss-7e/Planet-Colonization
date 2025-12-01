using UnityEngine;

public enum MapType
{
    Square,
    Hexagon
}
public class GridManager : MonoBehaviour
{
    public static GridManager instance;
    public MapType mapType;
    public Grid[] grids;
    public GridPathDirection[] gridPathDirections;
    public int length { get; set; }
    public int width { get; set; }
    void Awake()
    {
        instance = this;
        grids = null;
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
        width = 1000;
        length = 1000;

        grids = new Grid[gridDataArray.Length];

        for (int i = 0; i < gridDataArray.Length; i++)
        {
            var data = gridDataArray[i];
            grids[i] = new Grid(data.GetPosition());
        }

    }

    public Grid GetGridXY(int x, int y)
    {
        return grids[y * width + x];
    }

    public void SetGridPos(int x, int y, Grid value)
    {
        grids[y * width + x] = value;
    }

    public Vector2Int GetGridXY(Vector3 pos)
    {
        pos.x += length / 2;
        pos.z += width / 2;
        int x = Mathf.RoundToInt(pos.x);
        int z = Mathf.RoundToInt(pos.z);
        return new Vector2Int(x, z);
    }
}
