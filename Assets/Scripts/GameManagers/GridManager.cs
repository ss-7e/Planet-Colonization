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
    public Grid[] grid;
    public int length { get; set; }
    public int width { get; set; }
    void Awake()
    {
        instance = this;
        grid = null;
        LoadGridFromJson();
    }

    void LoadGridFromJson()
    {
        string path = Application.dataPath + "/grid_data.json";

        if (!System.IO.File.Exists(path))
        {
            Debug.LogError("Grid JSON not FOund！");
            return;
        }

        string json = System.IO.File.ReadAllText(path);
        GridData[] gridDataArray = Newtonsoft.Json.JsonConvert.DeserializeObject<GridData[]>(json);
        width = 1000;
        length = 1000;

        grid = new Grid[gridDataArray.Length];

        for (int i = 0; i < gridDataArray.Length; i++)
        {
            var data = gridDataArray[i];
            grid[i] = new Grid(data.GetPosition());
        }

        Debug.Log($"成功加载 {grid.Length} 个格子数据");
    }

    public Grid GetGridXY(int x, int y)
    {
        return grid[y * width + x];
    }

    public void SetGridPos(int x, int y, Grid value)
    {
        grid[y * width + x] = value;
    }
}
