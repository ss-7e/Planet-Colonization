using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class HeatMapVisualizer : EntityBase
{
    [SerializeField] private HeatMapType _heatMapToPaint = HeatMapType.NavFlowField;
    [SerializeField] private float _maxHeatValue = 50.0f;
    [SerializeField] private Color _zeroHeatColor = new Color(0, 1, 0, 0.5f);
    [SerializeField] private Color _maxHeatColor = new Color(1, 0, 0, 0.5f);
    [SerializeField] private float _lineWidth = 0.02f;
    [SerializeField] private float _arrowSize = 0.6f;
        
    private HeatMapSet _heatMapSet;
    private Mesh _mesh;
    private MeshRenderer _meshRenderer;
    private Vector2Int _mapSize;
    private Texture2D _heatMapTexture;
    private Material _material;

    private void Awake()
    {
        _mesh = GetComponent<MeshFilter>().mesh;
        _meshRenderer = GetComponent<MeshRenderer>();
    }

    private void Start()
    {
        _heatMapSet = AIModule.Instance.HeatMapSet;
        _heatMapSet.NavFlowField.OnHeatMapChange += Repaint;
        _mapSize = new Vector2Int(GridManager.Instance.Length, GridManager.Instance.Width);

        CreateTexture();
        SetupMaterial();
        Repaint();
    }

    private void OnDestroy()
    {
        if (_heatMapTexture != null)
        {
            Destroy(_heatMapTexture);
        }
        if (_material != null)
        {
            Destroy(_material);
        }
    }

    private void CreateTexture()
    {
        if (_heatMapTexture != null)
        {
            Destroy(_heatMapTexture);
        }
        _heatMapTexture = new Texture2D(_mapSize.x, _mapSize.y, TextureFormat.RGBAHalf, false);
        _heatMapTexture.filterMode = FilterMode.Point;
        _heatMapTexture.wrapMode = TextureWrapMode.Clamp;
    }

    private void SetupMaterial()
    {
        _material = new Material(Shader.Find("Custom/HeatMapVisualizer"));
        _meshRenderer.material = _material;
    }

    private void Repaint()
    {
        if (_heatMapSet == null || _heatMapTexture == null)
            return;

        Rect mapRectInWorld = GridManager.Instance.GetMapRectInWorld();

        Vector3[] vertices = new Vector3[] {
            new Vector3(mapRectInWorld.xMin, 0.6f, mapRectInWorld.yMin),
            new Vector3(mapRectInWorld.xMin, 0.6f, mapRectInWorld.yMax),
            new Vector3(mapRectInWorld.xMax, 0.6f, mapRectInWorld.yMin),
            new Vector3(mapRectInWorld.xMax, 0.6f, mapRectInWorld.yMax),
        };
        int[] triangles = new int[]{ 0, 1, 2, 2, 1, 3 };
        Vector2[] uv = new Vector2[] {
            Vector2.zero,
            Vector2.up,
            Vector2.right,
            Vector2.one
        };
            
        _mesh.Clear();
        _mesh.vertices = vertices;
        _mesh.triangles = triangles;
        _mesh.uv = uv;

        UpdateTextureData(mapRectInWorld);
        UpdateMaterialProperties();
    }

    private void UpdateTextureData(Rect mapRectInWorld)
    {
        NavFlowField navFlowField = _heatMapSet.NavFlowField;

        for (int x = 0; x < _mapSize.x; x++)
        {
            for (int y = 0; y < _mapSize.y; y++)
            {
                float worldX = x + mapRectInWorld.x;
                float worldZ = y + mapRectInWorld.y;

                float heatValue = navFlowField.GetValue(worldX, worldZ);
                Vector2 gradient = navFlowField.GetGradient(worldX, worldZ).normalized;

                float normalizedHeat = Mathf.Clamp01(heatValue / _maxHeatValue);

                _heatMapTexture.SetPixel(x, y, new Color(normalizedHeat, gradient.x, gradient.y, 0));
            }
        }

        _heatMapTexture.Apply();
    }

    private void UpdateMaterialProperties()
    {
        _material.SetTexture("_HeatMapTex", _heatMapTexture);
        _material.SetVector("_GridCount", new Vector4(_mapSize.x, _mapSize.y, 0, 0));
        _material.SetColor("_ZeroHeatColor", _zeroHeatColor);
        _material.SetColor("_MaxHeatColor", _maxHeatColor);
        _material.SetFloat("_LineWidth", _lineWidth);
        _material.SetFloat("_ArrowSize", _arrowSize);
    }
}