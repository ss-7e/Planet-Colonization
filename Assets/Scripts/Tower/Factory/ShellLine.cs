using Game.Ammo;
using UnityEngine;
namespace Game.Towers.Factory
{
    [CreateAssetMenu(fileName = "ShellLine", menuName = "AssembleLine/ShellLine")]
    public class ShellAssembleLine : AssembleLine
    {
        [SerializeField] ShellData _shellData;
        public ShellData shellData => _shellData;
    }
}
