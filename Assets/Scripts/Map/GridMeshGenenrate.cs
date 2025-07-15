using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Game.Map;


[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class GridMeshGenerator : MonoBehaviour
{
    public int length = 120;
    public int width = 120;
    public float tileSize = 1f;
    public float heightMapScale = 1f;
    public MapGenerateConfig config;
    public Mesh tileMesh;
    public Mesh[] trees;
    public Mesh[] resourses;
    private void Start()
    {
        //float length = config.length;
        //float width = config.width;
        //float tileSize = config.tileSize;
        //GridManager.instance.grid = new Grid[10000, 10000];
        //GridManager.instance.length = (int)(length * tileSize);
        //GridManager.instance.width = (int)(width * tileSize);
        //GenerateGridMesh();
    }

    public void GenerateGridMesh()
    {
        if (config != null)
        {
            length = config.length;
            width = config.width;
            tileSize = config.tileSize;
        }

        List<CombineInstance> combineList = new List<CombineInstance>();
        Mesh tile = Instantiate(tileMesh);
        Mesh[] trees = this.trees.Select(m => Instantiate(m)).ToArray();
        Mesh[] resourses = this.resourses.Select(m => Instantiate(m)).ToArray();


        for (int x = 0; x < length; x++)
        {
            for (int y = 0; y < width; y++)
            {
                CombineInstance ci = new CombineInstance();
                ci.mesh = tile;
                if (Random.value < 0.01f)
                {
                    ci.mesh = trees[Random.Range(0, trees.Length)];
                }
                else if (Random.value > 0.99f)
                {
                    ci.mesh = resourses[Random.Range(0, resourses.Length)];
                }
                float yPos = yposGengerate(x, y);
                Vector3 pos = new Vector3(x * tileSize - length * tileSize / 2, yPos, y * tileSize - width * tileSize / 2);
                ci.transform = Matrix4x4.TRS(
                    pos,
                    Quaternion.identity,
                    Vector3.one * tileSize
                );
                combineList.Add(ci);
                if(GridManager.instance)
                {
                    GridManager.instance.grid[x, y] = new Grid(pos);
                }
            }
        }

        Mesh combined = new Mesh();
        combined.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;    
        combined.CombineMeshes(combineList.ToArray(), true, true);
        GetComponent<MeshFilter>().mesh = combined;
        transform.position = new Vector3(0, 0.3f, 0);

        MeshCollider meshCollider = GetComponent<MeshCollider>();
        if (meshCollider == null)
        {
            meshCollider = gameObject.AddComponent<MeshCollider>();
        }
        meshCollider.sharedMesh = combined;
    }
    float yposGengerate(float x, float y)
    {
        if (config != null)
        {
            heightMapScale = config.heightMapScale;
        }
        float yPos = Mathf.PerlinNoise(x * 0.1f * heightMapScale, y * 0.1f * heightMapScale) * 3f;
        yPos = (float)Mathf.Round(yPos) / 10;
        return yPos;
    }
}
