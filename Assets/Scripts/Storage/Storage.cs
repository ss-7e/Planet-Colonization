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
        if (item == null) { return false; }
        foreach (IStorable existingItem in storableItems)
        {
            if (existingItem.SameItem(item))
            {
                if (existingItem.CurrentCount + item.CurrentCount <= existingItem.MaxCount)
                {
                    existingItem.CurrentCount += item.CurrentCount;
                    return true; 
                }
                else
                {
                    item.CurrentCount -= (existingItem.MaxCount - existingItem.CurrentCount);
                }
            }
        }
        if (item.CurrentCount > 0) { return AddNewItem(item); } 
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

    public void RemoveItem(IStorable item)
    {
        for (int i = 0; i < storableItems.Count; i++)
        {
            if (storableItems[i] == item)
            {
                storableItems.RemoveAt(i);
                return;
            }
        }
        Debug.LogWarning("Item not found in storage.");
    }

    public bool GetItem(IStorable item, int count)
    {
        List<IStorable> itemsToRemove = new List<IStorable>();
        foreach (IStorable existingItem in storableItems)
        {
            if (existingItem.SameItem(item))
            {
                if (existingItem.CurrentCount >= count)
                {
                    existingItem.CurrentCount -= count;
                    if (existingItem.CurrentCount <= 0)
                    {
                        storableItems.Remove(existingItem);
                    }
                    item.CurrentCount += count; 
                    count = 0;
                    break;
                }
                else
                {
                    count -= existingItem.CurrentCount;
                    item.CurrentCount += existingItem.CurrentCount;
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

    public List<IStorable> GetItemsByType(ItemTypeA itemType)
    {
        List<IStorable> itemsOfType = new List<IStorable>();
        foreach (IStorable item in storableItems)
        {
            if (item.ItemType == itemType)
            {
                itemsOfType.Add(item);
            }
        }
        return itemsOfType;
    }

    public void GetItemsByType(ItemTypeA itemType, out List<IStorable> items)
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
                itemCounts[item] += item.CurrentCount;
            }
            else
            {
                itemCounts[item] = item.CurrentCount;
            }
        }
        storableItems.Clear();
    }
}
