using Game.Ammo;
using UnityEngine;
using UnityEngine.UI;

public enum ItemType
{
    NaturalResource,
    Module,
    ProcessingResource,
    Component,
    Ammo,
    Tower
}
public interface IStorable
{
    int Id { get; }
    int maxCount { get; }
    int currentCount { get; set; }
    Sprite icon { get; set; }

    bool SameItem(IStorable other);
    ItemType itemType { get; }
}
