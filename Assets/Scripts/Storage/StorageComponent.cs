using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 任何需要存储物品的塔应当挂载这个组件
/// 后续可以扩展为接口以支持不同的存储类型
/// </summary>
public class StorageComponent
{
    public struct StoredItem
    {
        public ItemIDPrev ID;
        public int Count;
    }

    private List<StoredItem> _storedItems = new();
    private readonly int _maxStorageCapacity;
    public int MaxStorageCapacity => _maxStorageCapacity;

    public StorageComponent(int maxCapacity)
    {
        _maxStorageCapacity = maxCapacity;
    }

 
    public bool AddItem(ItemIDPrev id, int count)
    {
        if (count <= 0) return true;

        int maxStack = ItemDataManager.Instance.ItemMaxStackCounts[id];
        int remaining = count;

        for (int i = 0; i < _storedItems.Count && remaining > 0; i++)
        {
            if (_storedItems[i].ID != id) continue;

            var slot = _storedItems[i];
            int empty = maxStack - slot.Count;
            if (empty <= 0) continue;

            int toAdd = Mathf.Min(empty, remaining);
            slot.Count += toAdd;
            _storedItems[i] = slot;
            remaining -= toAdd;
        }

        while (remaining > 0 && _storedItems.Count < _maxStorageCapacity)
        {
            int toAdd = Mathf.Min(maxStack, remaining);
            _storedItems.Add(new StoredItem { ID = id, Count = toAdd });
            remaining -= toAdd;
        }

        return remaining == 0;
    }

    public bool GetOneItem(ItemIDPrev id)
    {
        for (int i = 0; i < _storedItems.Count; i++)
        {
            if (_storedItems[i].ID == id)
            {
                var slot = _storedItems[i];
                slot.Count = slot.Count - 1;
                _storedItems[i] = slot;
                return true;
            }
        }
        return false;
    }

    public void GetStoredItems(out List<(ItemIDPrev id, int count)> items)
    {
        items = new List<(ItemIDPrev id, int count)>();
        foreach (var storedItem in _storedItems)
        {
            items.Add((storedItem.ID, storedItem.Count));
        }
    }
}
