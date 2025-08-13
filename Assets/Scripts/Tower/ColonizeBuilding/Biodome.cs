using Game.Towers.ColonizeBuilding;
using UnityEngine;

namespace Game.Towers.ColonizeBuilding
{
    public class Biodome : ColonizeBuildingBase
    {
        public override void TakeDamage(float damage)
        {
            healthComp.TakeDamage(damage);
            // UI update logic can be added here if needed
            if (healthComp.health < 0)
            {
                Destroy(gameObject);
            }
        }

        
    }
}
