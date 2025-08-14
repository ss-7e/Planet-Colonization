using System.Collections.Generic;
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
        foreach (IStorable existingItem in storableItems)
        {
            if (existingItem.SameItem(item))
            {
                if (existingItem.currentCount + item.currentCount <= existingItem.maxCount)
                {
                    existingItem.currentCount += item.currentCount;
                    return true; 
                }
                else
                {
                    item.currentCount -= (existingItem.maxCount - existingItem.currentCount);
                }
            }
        }
        if (item.currentCount > 0) { return AddNewItem(item); } 
        return false;
    }
    bool AddNewItem(IStorable item)
    {
        if (storableItems.Count >= maxCapacity)
        {
            Debug.LogWarning("Storage is full!");
            return false;
        }
        storableItems.Add(item);
        return true;
    }

    public IStorable GetItem(IStorable item)
    {
        foreach (IStorable existingItem in storableItems)
        {
            if (existingItem.SameItem(item))
            {
                storableItems.Remove(existingItem);
                return existingItem;
            }
        }
        return null;
    }

    public bool GetItem(IStorable item, int count)
    {
        List<IStorable> itemsToRemove = new List<IStorable>();
        foreach (IStorable existingItem in storableItems)
        {
            if (existingItem.SameItem(item))
            {
                if (existingItem.currentCount >= count)
                {
                    existingItem.currentCount -= count;
                    if (existingItem.currentCount <= 0)
                    {
                        storableItems.Remove(existingItem);
                    }
                    item.currentCount += count; 
                    count = 0;
                    break;
                }
                else
                {
                    count -= existingItem.currentCount;
                    item.currentCount += existingItem.currentCount;
                    itemsToRemove.Add(existingItem);
                }
            }
        }
        if (count <= 0)
        {
            foreach (IStorable removeItem in itemsToRemove)
            {
                storableItems.Remove(removeItem);
            }
            return true;
        }
        return false;
    }

    public List<IStorable> GetAllItems()
    {
        return new List<IStorable>(storableItems);
    }

    public List<IStorable> GetItemsByType(ItemType itemType)
    {
        List<IStorable> itemsOfType = new List<IStorable>();
        foreach (IStorable item in storableItems)
        {
            if (item.itemType == itemType)
            {
                itemsOfType.Add(item);
            }
        }
        return itemsOfType;
    }

    public void GetItemsByType(ItemType itemType, out List<IStorable> items)
    {
        items = GetItemsByType(itemType);
    }


    public void OranizeStorage()
    {
        Dictionary<IStorable, int> itemCounts = new Dictionary<IStorable, int>();
        foreach (IStorable item in storableItems)
        {
            if (itemCounts.ContainsKey(item))
            {
                itemCounts[item] += item.currentCount;
            }
            else
            {
                itemCounts[item] = item.currentCount;
            }
        }
        storableItems.Clear();
    }
}
