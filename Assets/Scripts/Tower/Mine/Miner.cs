using System.Collections.Generic;
using UnityEditor.iOS;
using UnityEngine;
namespace Game.Towers.Mine
{
    public class Miner : Tower, IDamageable
    {
        [SerializeField]protected float mineSpeed = 0.2f; // unit per second
        [SerializeField]protected float outputSpeed = 0f;
        HealthComponent healthComp;
        ResourceGrid placedGrid;
        public float outputTime{ get; protected set; } = 0;
        public override void BuildOnGrid(Grid grid)
        {
            onGrid = grid;
            placedGrid = grid as ResourceGrid;
        }
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
            float timeToMine = 1f / mineSpeed;
            if(outputTime < timeToMine)
            {
                outputTime += Time.deltaTime;
                return; 
            }
            outputTime = 0f; 
            Output(placedGrid.GetMineOnGrid());
        }



        protected virtual void Output(NaturalResource resource)
        {
            if (storageList.Count > 0)
            {
                foreach (var storage in storageList)
                {
                    if (storage.AddItem(resource))
                    {
                        Debug.Log($"Resource {resource.resourceName} added to storage.");
                        return; // Exit after successfully adding to one storage
                    }
                }

            }
        }
    }
}
