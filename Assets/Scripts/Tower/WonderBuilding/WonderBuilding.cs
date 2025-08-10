using UnityEngine;

namespace Game.Towers.Wonders
{
    public class WonderBuilding : MonoBehaviour, IDamageable
    {
        HealthComponent healthComp;
        [SerializeField]
        int wonderLevel;

        float countDownTime;
        Grid[] occupiedGrids;
        public void TakeDamage(float damage)
        {
            healthComp.TakeDamage(damage);
            if (healthComp.health <= 0)
            {
                Destroy(gameObject);
            }
        }


    }
}
