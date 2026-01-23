using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class StorageUI : MonoBehaviour
{
    public GameObject itemsParent;
    public List<Transform> itemSlots;
    protected StoragePrev storage;

    private void Start()
    {
        foreach (Transform child in itemsParent.transform)
        {
            itemSlots.Add(child);
        }
        UpdateStorageUI();
    }

    public void SetStorage(StoragePrev newStorage)
    {
        storage = newStorage;
        UpdateStorageUI();
    }
    public void UpdateStorageUI()
    {
        if (storage == null)
        {
            return;
        }
        List<IStorable> items = storage.GetAllItems();
        for (int i = 0; i < items.Count; i++)
        {
            if (i >= itemSlots.Count)
            {
                break;
            }
            Image slotImage = itemSlots[i].Find("Icon").GetComponent<Image>();
            slotImage.sprite = items[i].Icon;
            slotImage.color = Color.white;
        }
    }
}
