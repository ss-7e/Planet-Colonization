using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class GridMeshGenerator : MonoBehaviour
{
    public int length = 100;
    public int width = 100;
    public float tileSize = 1f;
    public float heightMapScale = 1f;
    public Mesh temp;
    void Start()
    {
        GridManager.instance.grid = new Grid[10000, 10000];
        GridManager.instance.length = (int)(length * tileSize);
        GridManager.instance.width = (int)(width * tileSize);
        GenerateGridMesh();
    }

    public void GenerateGridMesh()
    {
        List<CombineInstance> combineList = new List<CombineInstance>();

        Mesh tileMesh = temp;
        for (int x = 0; x < length; x++)
        {
            for (int y = 0; y < width; y++)
            {
                CombineInstance ci = new CombineInstance();
                ci.mesh = tileMesh;
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
        transform.position = Vector3.zero;

        MeshCollider meshCollider = GetComponent<MeshCollider>();
        if (meshCollider == null)
        {
            meshCollider = gameObject.AddComponent<MeshCollider>();
        }
        meshCollider.sharedMesh = combined;
    }
    float yposGengerate(float x, float y)
    {
        float yPos = Mathf.PerlinNoise(x * 0.1f * heightMapScale, y * 0.1f * heightMapScale) * 3f;
        yPos = (float)Mathf.Round(yPos) / 10;
        return yPos;
    }
}
