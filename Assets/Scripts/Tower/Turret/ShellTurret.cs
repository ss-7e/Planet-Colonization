using Game.Ammo;
using Game.Modules;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Turret
{
    public class ShellTurret : TurretBase
    {
        [SerializeField]
        protected float barrelLengthInCalibers;
        protected Grid posGrid;
        protected BarrelModule barrelModule;

        List<GameObject> targets;

        public void AddAmmo(ShellData shell, int count)
        {
            ammoStorage.AddAmmo(shell, count);
        }
    }
}
