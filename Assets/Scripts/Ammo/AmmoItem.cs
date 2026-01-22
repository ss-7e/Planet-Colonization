using UnityEngine;
using UnityEngine.UI;

namespace Game.Ammo
{
    public class AmmoItem : IStorable
    {
        //-----------------------------------------------------------
        // IStorable Implementation
        public int Id { get; private set; }

        public int MaxCount => shellData.maxCount;

        public int CurrentCount { get; set; } = 1;

        public Sprite Icon { get => _icon; set => _icon = value; }
        [SerializeField] protected Sprite _icon;

        public ItemTypeA ItemType => ItemTypeA.Ammo;

        public bool SameItem(IStorable other)
        {
            if (other is AmmoItem ammoItem)
            {
                return ammoItem.shellData == shellData;
            }
            return false;
        }
        //-----------------------------------------------------------

        public ShellData shellData;
        
    }
}