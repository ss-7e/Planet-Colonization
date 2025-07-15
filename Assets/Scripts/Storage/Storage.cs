using Game.Ammo;
using Game.Modules;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
public class Storage
{
    int maxCapacity;
    List<IStorable> storableItems = new List<IStorable>();

    public Storage(int capacity)
    {
        maxCapacity = capacity;
    }
    public bool AddItem(IStorable item)
    {
        if (storableItems.Count >= maxCapacity)
        {
            Debug.LogWarning("Storage is full!");
            return false;
        }
        else
        {
            storableItems.Add(item);
        }
        return true;
    }
    public bool RemoveItem(IStorable item)
    {
        if (storableItems.Contains(item))
        {
            storableItems.Remove(item);
            return true;
        }
        else
        {
            Debug.LogWarning("Item not found in storage!");
            return false;
        }
    }
    public bool TakeIndexItem(int index, out IStorable item)
    {
        if (index < 0 || index >= storableItems.Count)
        {
            Debug.LogWarning("Index out of range!");
            item = null;
            return false;
        }
        item = storableItems[index];
        storableItems.RemoveAt(index);
        return true;
    }
    public bool PutBackIndexItem(int index, IStorable item)
    {
        if (index < 0 || index >= storableItems.Count)
        {
            Debug.LogWarning("Index out of range!");
            return false;
        }
        if (storableItems.Contains(item))
        {
            Debug.LogWarning("Item already exists in storage!");
            return false;
        }

        storableItems[index] = item;
        return true;
    }
}
