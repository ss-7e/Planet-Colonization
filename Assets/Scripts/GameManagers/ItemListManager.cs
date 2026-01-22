using UnityEngine;
using System.Collections.Generic;

public class Item : IItem
{
    public int Id { get; private set; }
    public ItemTypeA ItemType { get; private set; }
    public Item(int id = 0, ItemTypeA itemType = ItemTypeA.NaturalResource)
    {
        Id = id;
        ItemType = itemType;
    }
}

public class ItemListManager
{
    private static readonly ItemListManager _instance = new();
    public static ItemListManager Instance => _instance;
    public readonly Dictionary<ItemType, GameObject> ItemPrefabs;


}