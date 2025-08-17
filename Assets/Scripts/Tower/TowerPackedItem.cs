using UnityEngine;
using UnityEngine.UI;
using Game.Towers;

[CreateAssetMenu(fileName = "TowerPackedItem", menuName = "Items/Tower", order = 1)]
public class TowerPackedItem : ScriptableObject, IStorable
{
    //-----------------------------------------------------------
    // IStorable Implementation
    public int Id { get; private set; }

    public int maxCount => _maxCount;
    [SerializeField] protected int _maxCount = 1;

    public int currentCount { get; set; } = 1;

    public Sprite icon { get => _icon; set => _icon = value; }
    [SerializeField] protected Sprite _icon;

    public ItemType itemType => ItemType.Tower;

    public bool SameItem(IStorable other)
    {
        return false;
    }
    //-----------------------------------------------------------
    public Tower towerData;
}