using UnityEngine;
using Game.Turret;
using Game.Ammo;

namespace Game.Modules
{
    [CreateAssetMenu(fileName = "AutoloaderModule", menuName = "Tower Modules/Reloader Module", order = 2)]
    public class ReloaderModule : TurretModule
    {
        public float reloadTime = 2f; 
        public void ReloaderUpdate(ReloaderModuleData reloader, TurretAmmoStorage ammoStorate)
        {

            if(reloader.GetFiringShell() != null) {return; }

            float time = reloader.timeCountdown;
            if (time <= 0f)
            {
                time = reloadTime;
                reloader.SetFiringShell(ammoStorate.GetAmmo());
            }
            else
            {
                time -= Time.deltaTime;
            }
            reloader.timeCountdown = time;
        }

        public void Fire(ReloaderModuleData reloader)
        {
            reloader.SetFiringShell(null);
        }

    }
}
