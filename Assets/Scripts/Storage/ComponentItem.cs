using UnityEngine;
using UnityEngine.UI;

public class Component : ScriptableObject, IStorable
{

    //-----------------------------------------------------------
    // IStorable Implementation
    public int Id { get; private set; }

    public int maxCount => _maxCount;
    [SerializeField] protected int _maxCount;

    public int currentCount { get; set; } = 1;

    public Sprite icon { get => _icon; set => _icon = value; }
    [SerializeField] protected Sprite _icon;

    public ItemType itemType => ItemType.Component;

    public bool SameItem(IStorable other)
    {
        return false;
    }
    //-----------------------------------------------------------


    public string resourceName;


    public void SetId(int id)
    {
        Id = id;
    }
    public void IncrementCount(int amount)
    {
        currentCount += amount;
        if (currentCount > maxCount)
        {
            currentCount = maxCount; // Ensure we don't exceed max count
        }
    }
    public void DecrementCount(int amount)
    {
        currentCount -= amount;
        if (currentCount < 0)
        {
            currentCount = 0; // Ensure we don't go below zero
        }
    }
}
