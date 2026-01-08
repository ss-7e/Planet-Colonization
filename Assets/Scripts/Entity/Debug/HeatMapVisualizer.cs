using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class HeatMapVisualizer : EntityBase
{
    [SerializeField] private HeatMapType _heatMapToPaint = HeatMapType.NavFlowField;
    [SerializeField] private float _maxHeatValue = 50.0f;
        
    private HeatMapSet _heatMapSet;
    private Mesh _mesh;
    private Vector2Int _mapSize;
    private Rect _mapRectInWorld;

    private void Awake()
    {
        _mesh = GetComponent<MeshFilter>().mesh;
    }

    private void Start()
    {
        _heatMapSet = AIModule.Instance.HeatMapSet;
        _heatMapSet.NavFlowField.OnHeatMapChange += Repaint;
        _mapSize = new Vector2Int(GridManager.instance.length, GridManager.instance.width);
        _mapRectInWorld = GridManager.instance.GetMapRectInWorld();

        MeshRenderer renderer = GetComponent<MeshRenderer>();
        renderer.material.color = Color.white;
        renderer.material.shader = Shader.Find("Custom/VertexColor");

        Repaint();
    }

    private void Repaint()
    {
        Vector3[] vertices = new Vector3[(_mapSize.x + 1) * (_mapSize.y + 1)];
        int[] triangles = new int[_mapSize.x * _mapSize.y * 6];
        Color[] colors = new Color[vertices.Length];
            
        float heatValueScale = 1.0f / _maxHeatValue;

        for (int x = 0; x < _mapSize.x + 1; x++)
        {
            for (int y = 0; y < _mapSize.y + 1; y++)
            {
                float worldX = y + _mapRectInWorld.y;
                float worldZ = x + _mapRectInWorld.x;
                vertices[x + y * (_mapSize.x + 1)] = new Vector3(worldX, 0.6f, worldZ);
                // 目前只支持 NavFlowField
                float heatValue = _heatMapSet.NavFlowField.GetValue(worldX, worldZ) * heatValueScale;
                Color color = Utils.MathUtils.ColorHSVInterpClamped(Color.green, Color.red, heatValue);
                colors[x + y * (_mapSize.x + 1)] = color;
            }
        }

        for (int x = 0; x < _mapSize.x; x++)
        {
            for (int y = 0; y < _mapSize.y; y++)
            {
                int triangleIndex = x + y * _mapSize.x;
                int vertexIndex = triangleIndex * 6;
                triangles[vertexIndex + 0] = x + y * (_mapSize.x + 1);
                triangles[vertexIndex + 1] = x + 1 + y * (_mapSize.x + 1);
                triangles[vertexIndex + 2] = x + (y + 1) * (_mapSize.x + 1);
                triangles[vertexIndex + 3] = x + 1 + y * (_mapSize.x + 1);
                triangles[vertexIndex + 4] = x + 1 + (y + 1) * (_mapSize.x + 1);
                triangles[vertexIndex + 5] = x + (y + 1) * (_mapSize.x + 1);
            }
        }
            
        _mesh.Clear();
        _mesh.vertices = vertices;
        _mesh.triangles = triangles;
        _mesh.colors = colors;
    }
}