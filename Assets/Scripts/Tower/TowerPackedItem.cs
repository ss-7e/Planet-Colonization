using UnityEngine;
using UnityEngine.UI;
using Game.Towers;

[CreateAssetMenu(fileName = "TowerPackedItem", menuName = "Items/Tower", order = 1)]
public class TowerPackedItem : ScriptableObject, IStorable
{
    //-----------------------------------------------------------
    // IStorable Implementation
    public int Id { get; private set; }

    public int MaxCount => _maxCount;
    [SerializeField] protected int _maxCount = 1;

    public int CurrentCount { get; set; } = 1;

    public Sprite Icon { get => _icon; set => _icon = value; }
    [SerializeField] protected Sprite _icon;

    public ItemTypeA ItemType => ItemTypeA.Tower;

    public bool SameItem(IStorable other)
    {
        return false;
    }
    //-----------------------------------------------------------
    public Tower towerData;
}