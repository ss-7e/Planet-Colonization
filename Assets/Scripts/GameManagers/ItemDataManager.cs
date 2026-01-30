using UnityEngine;
using System.Collections.Generic;

public class Item : IItem // TODO:有用吗？
{
    public int Id { get; private set; }
    public ItemTypeA ItemType { get; private set; }
    public Item(int id = 0, ItemTypeA itemType = ItemTypeA.NaturalResource)
    {
        Id = id;
        ItemType = itemType;
    }
}


/// <summary>
/// 记录游戏中物品数据
///     （物品预制体）
///     （物品堆叠数量）
/// </summary>
public class ItemDataManager 
{
    private static readonly ItemDataManager _instance = new();
    public static ItemDataManager Instance => _instance;

    private Dictionary<ItemIDPrev, GameObject> _itemPrefabs = new();
    private Dictionary<ItemIDPrev, int> _itemMaxStackCounts = new();
    public IReadOnlyDictionary<ItemIDPrev, GameObject> ItemPrefabs => _itemPrefabs;
    public IReadOnlyDictionary<ItemIDPrev, int> ItemMaxStackCounts => _itemMaxStackCounts;

    ItemDataManager()
    {
        _itemMaxStackCounts = new Dictionary<ItemIDPrev, int>
        {
            { ItemIDPrev.Raw_IronOre, 5 },
            { ItemIDPrev.Refined_IronIngot, 5 },
        };
    }


    public void SetItemStackMaxCountByID(ItemIDPrev itemID, int maxCount)
    {
        if (_itemMaxStackCounts.ContainsKey(itemID))
        {
            _itemMaxStackCounts[itemID] = maxCount;
        }
        else
        {
            _itemMaxStackCounts.Add(itemID, maxCount);
        }
    }

}