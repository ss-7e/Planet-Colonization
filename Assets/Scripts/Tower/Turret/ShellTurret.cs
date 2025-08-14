using Game.Ammo;
using Game.Modules;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Towers.Turrets
{
    public class ShellTurret : TurretBase
    {
        [SerializeField]
        protected float barrelLengthInCalibers;
        protected BarrelModule barrelModule;


        public void AddAmmo(ShellData shell, int count)
        {
            ammoStorage.AddAmmo(shell, count);
        }
    }
}
