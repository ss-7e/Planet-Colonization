using System;
using UnityEngine;

public class EntityManager : MonoBehaviour
{
    public static EntityManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    public EntityBase CreateEntity(String entityName, Type entityClass)
    {
        if (entityClass.IsSubclassOf(typeof(EntityBase)))
        {
            GameObject gameObject = new(entityName, entityClass);
            return gameObject.GetComponent<EntityBase>();
        }
        else
        {
            throw new ArgumentException("Provided class does not inherit from EntityBase");
        }
    }

    // TODO 其它重写方法，如从 Prefab 创建实体等
    // TODO EntityData
    // TODO EntityPool
}