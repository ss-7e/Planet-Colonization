using System.Collections.Generic;
using UnityEngine;

public class GenericObjectPool<T> where T : MonoBehaviour
{
    private Queue<T> pool;
    private T prefab;
    private Transform parent;

    public GenericObjectPool(T prefab, int initialSize = 10, Transform parent = null)
    {
        this.prefab = prefab;
        this.parent = parent;
        pool = new Queue<T>();

        for (int i = 0; i < initialSize; i++)
        {
            T obj = CreateNewObject();
            pool.Enqueue(obj);
            obj.gameObject.SetActive(false);
        }
    }

    public T GetObject()
    {
        T obj;

        if (pool.Count > 0)
        {
            obj = pool.Dequeue();
        }
        else
        {
            obj = CreateNewObject();
        }

        return obj;
    }

    public void ReturnObject(T obj)
    {
        pool.Enqueue(obj);
    }

    private T CreateNewObject()
    {
        T newObj = Object.Instantiate(prefab, parent);
        return newObj;
    }
}