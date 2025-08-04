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
    public Mesh[] highGroundMeshes;
    public Mesh[] trees;
    public Mesh[] resourses;

    public void Regenerate()
    {
        MapGenerator mapGenerate = GetComponent<MapGenerator>();
        if(GridManager.instance != null)
        {
            GridManager.instance.grid = null;
            mapGenerate.RunTimeGenerate();
        }
        else
        {
            GenerateGridMesh();
        }
    }

    public void GenerateGridMesh()
    {

        if (config != null)
        {
            length = config.length;
            width = config.width;
            tileSize = config.tileSize;
        }
        Grid[] grids = null;
        if (GridManager.instance != null)
        {
            grids = GridManager.instance.grid;
        }
        else 
        { 
            grids = GetComponent<MapGenerator>().GridGenerate();
        }
        List<CombineInstance> combineList = new List<CombineInstance>();
        Mesh tile = Instantiate(tileMesh);
        Mesh[] trees = this.trees.Select(m => Instantiate(m)).ToArray();
        Mesh[] resourses = this.resourses.Select(m => Instantiate(m)).ToArray();

        for (int x = 0; x < length; x++)
        {
            for (int y = 0; y < width; y++)
            {
                Grid grid = grids[y * length + x];
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
                if (grid.isObstacle)
                {
                    ci.mesh = GenerateHighGround();
                }

                Vector3 pos = grid.pos;
                ci.transform = Matrix4x4.TRS(
                    pos,
                    Quaternion.identity,
                    Vector3.one * tileSize
                );
                combineList.Add(ci);
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

    Mesh GenerateHighGround()
    {
        Mesh highGroundMesh = highGroundMeshes[0];
        return highGroundMesh;
    }
}
