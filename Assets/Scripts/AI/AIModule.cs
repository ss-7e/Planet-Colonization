using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIModule : MonoBehaviour
{
    [SerializeField]
    private Transform _centerSpacecraft;

    public static AIModule Instance { get; private set; }

    public readonly HeatMapSet HeatMapSet = new();

    private bool _needRefreshHeatMap = false;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        HeatMapSet.Initialize();
        HeatMapSet.NavFlowField.SetGoal(_centerSpacecraft.position.x, _centerSpacecraft.position.z);
        HeatMapSet.Refresh();
        EntityBase entity = EntityManager.Instance.CreateEntity("HeatMapVisualizer", typeof(HeatMapVisualizer));
        entity.gameObject.SetActive(false);

        BuildManager.Instance.OnBuildEvent += OnBuild;
    }

    private void LateUpdate()
    {
        if (_needRefreshHeatMap)
        {
            HeatMapSet.Refresh();
            _needRefreshHeatMap = false;
        }
    }

    private void OnDestroy()
    {
        BuildManager.Instance.OnBuildEvent -= OnBuild;
    }

    private void OnBuild()
    {
        _needRefreshHeatMap = true;
    }
}
