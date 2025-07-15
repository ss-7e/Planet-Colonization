using Game.Turret;
using UnityEngine;

namespace Game.Modules
{
    public class TurretModuleItem : IStorable
    {
        public TurretModule module;

        public int currentCount { get; set; }
        public int maxCount { get; private set; }
        public int Id { get; }

        public TurretModuleItem(TurretModule module, int count = 1)
        {
            this.module = module;
            this.Id = Id;
            this.currentCount = count;
            this.maxCount = 1;
        }
    }
}

