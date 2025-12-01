using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ObjectPoolManager : MonoBehaviour
{
    public static ObjectPoolManager Instance { get; private set; }

    private Dictionary<string, object> pools = new Dictionary<string, object>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void CreatePool<T>(T prefab, int initialSize = 10, Transform parent = null) where T : MonoBehaviour
    {
        string key = typeof(T).Name;

        if (!pools.ContainsKey(key))
        {
            var pool = new GenericObjectPool<T>(prefab, initialSize, parent);
            pools.Add(key, pool);
        }
    }

    public T GetObject<T>(T prefab) where T : MonoBehaviour
    { 
        string key = typeof(T).Name;

        if (!pools.ContainsKey(key))
        {
            CreatePool(prefab);
        }

        var pool = pools[key] as GenericObjectPool<T>;
        return pool.GetObject();
    }

    public void ReturnObject<T>(T obj) where T : MonoBehaviour
    {
        string key = typeof(T).Name;

        if (pools.ContainsKey(key))
        {
            var pool = pools[key] as GenericObjectPool<T>;
            pool.ReturnObject(obj);
        }
        else
        {
            Debug.LogWarning($"No pool found for type {typeof(T).Name}");
            Destroy(obj);
        }
    }

    public void PreloadObjects<T>(T prefab, int count) where T : MonoBehaviour
    {
        string key = typeof(T).Name;

        if (!pools.ContainsKey(key))
        {
            CreatePool(prefab, count);
        }
    }
}