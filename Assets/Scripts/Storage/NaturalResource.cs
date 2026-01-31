using UnityEngine;
using UnityEngine.UI;
public enum NaturalResourceType
{
    IronOre,
    CopperOre,
    Coal,
    Titanium,
    Magnetite,
    Rubidium
}
[CreateAssetMenu(fileName = "NaturalResource", menuName = "Storage/NaturalResource")]
public class NaturalResource : ScriptableObject, IStorable
{

    //-----------------------------------------------------------
    // IStorable Implementation
    public int Id { get; private set; }

    public int MaxCount => _maxCount;
    [SerializeField] protected int _maxCount;

    public int CurrentCount { get; set; } = 1;

    public Sprite Icon { get => _icon; set => _icon = value; }
    [SerializeField] protected Sprite _icon;

    public ItemTypeA ItemType => ItemTypeA.NaturalResource;
    public bool SameItem(IStorable other)
    {
        if (other is NaturalResource naturalResource)
        {
            return naturalResource.resourceType == resourceType;
        }
        return false;
    }
    //-----------------------------------------------------------


    public string resourceName;
    public NaturalResourceType resourceType;


    public void SetId(int id)
    {
        Id = id;
    }
    public void IncrementCount(int amount)
    {
        CurrentCount += amount;
        if (CurrentCount > MaxCount)
        {
            CurrentCount = MaxCount; // Ensure we don't exceed max count
        }
    }
    public void DecrementCount(int amount)
    {
        CurrentCount -= amount;
        if (CurrentCount < 0)
        {
            CurrentCount = 0; // Ensure we don't go below zero
        }
    }
}
