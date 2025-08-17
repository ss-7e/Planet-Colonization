using Game.Towers;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;


public class Cargo : MonoBehaviour
{
    public static Cargo instance;
    [SerializeField] private int cargoCapacity = 24;
    [SerializeField] Dictionary<IStorable, float> galaxyShopItemPrices;
    [SerializeField] List<TowerPackedItem> defaltTowerItems;
    [SerializeField] StorageUI storageUI;
    protected Storage storage;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("More than one Cargo in scene!");
            return;
        }
        instance = this;
        storage = new Storage(cargoCapacity);
        foreach (IStorable item in defaltTowerItems)
        {
            if (item != null)
            {
                storage.AddItem(item);
            }
        }
        storageUI.SetStorage(storage);
    }
    public void BuyItem(IStorable item, float price)
    {
        if(GameManager.instance.CostGalacticCredit(price))
        {
            if(AddItem(item))
            {
                Debug.Log($"Bought item {item.Id} for {price} Galactic Credits.");
            }
        }
    }
    public bool AddItem(IStorable item)
    {
        return storage.AddItem(item);
    }

    public bool FindTower(Tower tower)
    {
        foreach (IStorable item in storage.GetAllItems())
        {
            if(item is TowerPackedItem packedItem)
            {
                if(packedItem.towerData == tower)
                {
                    storage.RemoveItem(packedItem);
                    return true;
                }
            }
        }
        return false;
    }

    public int GetTowerCount(Tower tower)
    {
        int count = 0;
        foreach (IStorable item in storage.GetAllItems())
        {
            if (item is TowerPackedItem packedItem && packedItem.towerData == tower)
            {
                count += packedItem.currentCount;
            }
        }
        return count;
    }   
    public Storage GetStorage()
    {
        return storage;
    }

    public void UpdateUI()
    {
        storageUI.UpdateStorageUI();
    }
}