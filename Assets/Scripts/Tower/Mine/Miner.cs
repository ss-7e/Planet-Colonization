using UnityEditor;
using UnityEngine;
namespace Game.Tower.Mine
{
    public class Miner : MonoBehaviour, IDamageable
    {
        HealthComponent healthComp;
        float mineSpeed = 1f;
        float outputSpeed = 0f;
        Grid placedGrid;

        public void TakeDamage(float damage)
        {
            if (healthComp == null)
            {
                healthComp = GetComponent<HealthComponent>();
            }
            healthComp.TakeDamage(damage);
            if (healthComp.health <= 0)
            {
                Destroy(gameObject);
            }
        }

        protected virtual void Mining()
        {
            Resource[] resources = placedGrid.getResources();
            if (resources == null || resources.Length == 0)
            {
                Debug.Log("No resources to mine.");
                return;
            }
            outputSpeed = mineSpeed * Time.deltaTime * resources[0].richness;
            if (resources[0] is RenewableResource renewableResource)
            {
                renewableResource.Regenerate();
                renewableResource.amount -= outputSpeed;
                if (renewableResource.amount < 0)
                {
                    renewableResource.amount = 0;
                }
            }
            else
            {
                Debug.Log("Mining non-renewable resource: " + resources[0].resourceName);
            }
        }
    }
}
