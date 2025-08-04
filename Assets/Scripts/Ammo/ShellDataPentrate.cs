using UnityEngine;

namespace Game.Ammo
{
    [CreateAssetMenu(fileName = "ShellDataPiercing", menuName = "Ammo/ShellDataPiercing", order = 3)]
    public class PiercingShellData : ShellData
    {
        public float penetrationDepth = 1000f; //mm
        
    }
}
