using UnityEngine;
using UnityEngine.UI;
using Game.Towers;

public class TowerPackedItem : MonoBehaviour, IStorable
{
    //-----------------------------------------------------------
    // IStorable Implementation
    public int Id { get; private set; }

    public int maxCount => _maxCount;
    [SerializeField] protected int _maxCount;

    public int currentCount { get; set; } = 1;

    public Image Icon { get => _icon; set => _icon = value; }
    [SerializeField] protected Image _icon;

    public ItemType itemType => ItemType.Component;

    public bool SameItem(IStorable other)
    {
        return false;
    }
    //-----------------------------------------------------------

    public Tower towerData;
}