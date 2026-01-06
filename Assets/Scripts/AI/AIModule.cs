using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIModule : MonoBehaviour
{
    public static AIModule Instance { get; private set; }

    public readonly HeatMapSet HeatMapSet = new();

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        HeatMapSet.Initialize();
        HeatMapSet.Refresh();
        EntityBase entity = EntityManager.Instance.CreateEntity("HeatMapVisualizer", typeof(HeatMapVisualizer));
        entity.gameObject.SetActive(false);
    }
}
