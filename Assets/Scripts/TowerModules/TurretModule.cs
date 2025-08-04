using Game.Turret;
using UnityEngine;

public enum ItemRarity
{
    Common,
    Uncommon,
    Rare,
    Epic,
    Legendary
}

namespace Game.Modules
{
    public abstract class TurretModule : ScriptableObject
    {
        public string moduleName;
        public string description;

        public bool stackable = true;
        public float weight = 1f;   // affect turret's rotation speed

        public ItemRarity rarity = ItemRarity.Common;
        [Header("Rarity Icon")]
        public Sprite rarityIcon;

        public virtual void OnAttach(TurretBase turret) { turret.WeightChange(weight); }
        public virtual void OnDetach(TurretBase turret) { turret.WeightChange(-weight); }
    }
}

