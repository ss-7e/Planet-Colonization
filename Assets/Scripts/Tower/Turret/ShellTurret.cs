using Game.Ammo;
using Game.Modules;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Turret
{
    public class ShellTurret : TurretBase
    {
        [SerializeField]
        protected GameObject shellPrefab;
        [SerializeField]
        protected float barrelLengthInCalibers;
        protected Grid posGrid;
        protected BarrelModule barrelModule;

        List<GameObject> targets;

        public void AddAmmo(ShellData shell, int count)
        {
            ammoStorage.AddAmmo(shell, count);
        }

        protected virtual void Fire()
        {
            GameObject shotedShellPrefab = Instantiate(shellPrefab, muzzle.position, muzzle.rotation);
            ShellData shellData = ammoStorage.GetAmmo();
            if (shellData == null)
            {
                Debug.LogWarning("No ammo available to fire.");
                return;
            }
            Shell shotedShell = shotedShellPrefab.GetComponent<Shell>();
            shotedShell.GetComponent<Shell>().ShellData = shellData;
            shotedShell.OnFire(muzzle.forward, muzzle.position, barrelLengthInCalibers);
        }
    }
}
