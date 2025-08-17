using UnityEngine;
using UnityEngine.UI;

namespace Game.Modules
{
    public class TurretModuleItem : IStorable
    {
        public TurretModule module;


        //----------------------------------------------------------
        // IStorable Implementation
        public int currentCount { get; set; }
        public int maxCount { get; private set; }
        public int Id { get; }
        public Sprite icon { get => _icon; set => _icon = value; }
        [SerializeField] protected Sprite _icon;
        public ItemType itemType => ItemType.Module;

        public bool SameItem(IStorable other)
        {
            return false;
        }
        //---------------------------------------------------------

        public TurretModuleItem(TurretModule module, int count = 1)
        {
            this.module = module;
            this.Id = Id;
            this.currentCount = count;
            this.maxCount = 1;
        }
    }
}

