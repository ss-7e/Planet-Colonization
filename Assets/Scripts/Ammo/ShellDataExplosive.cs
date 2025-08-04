using UnityEngine;

namespace Game.Ammo
{
    [CreateAssetMenu(fileName = "ShellDataExplosive", menuName = "Ammo/ShellDataExplosive", order = 2)]
    public class ShellDataExplosive : ShellData
    {
        public float explosionRadius = 5f;
        public float explosivePower = 1000f;

    }
}
