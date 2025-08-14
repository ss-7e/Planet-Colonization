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

    public int maxCount => _maxCount;
    [SerializeField] protected int _maxCount;

    public int currentCount { get; set; } = 1;

    public Image Icon { get => _icon; set => _icon = value; }
    [SerializeField] protected Image _icon;

    public ItemType itemType => ItemType.NaturalResource;
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
