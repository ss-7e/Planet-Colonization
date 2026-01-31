using UnityEngine;
using UnityEngine.UI;

namespace Game.Modules
{
    public class TurretModuleItem : IStorable
    {
        public TurretModule module;


        //----------------------------------------------------------
        // IStorable Implementation
        public int CurrentCount { get; set; }
        public int MaxCount { get; private set; }
        public int Id { get; }
        public Sprite Icon { get => _icon; set => _icon = value; }
        [SerializeField] protected Sprite _icon;
        public ItemTypeA ItemType => ItemTypeA.Module;

        public bool SameItem(IStorable other)
        {
            return false;
        }
        //---------------------------------------------------------

        public TurretModuleItem(TurretModule module, int count = 1)
        {
            this.module = module;
            this.Id = Id;
            this.CurrentCount = count;
            this.MaxCount = 1;
        }
    }
}

