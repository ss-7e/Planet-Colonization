using UnityEngine;
using Game.Turret;
using Game.Ammo;

namespace Game.Modules
{
    public class ReloaderModuleData : ModuleData
    {
        public float timeCountdown = 0f;
        private ShellData firingShell = null;
        public ReloaderModuleData(ReloaderModule module)
        {
            this.module = module;
        }
        public void SetFiringShell(ShellData shell)
        {
            firingShell = shell;
        }
        public ShellData GetFiringShell()
        {
            return firingShell;
        }


    }
}
