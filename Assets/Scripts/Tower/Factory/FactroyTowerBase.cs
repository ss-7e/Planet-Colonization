using UnityEngine;
using System.Collections.Generic;
namespace Game.Towers.Factory
{
    public class FactroyTowerBase : Tower, IDamageable
    {
        [SerializeField]protected List<Vector2Int> effectArea;
        [SerializeField] protected int towerStorageCapacity;
        [SerializeField] protected int assembleLineCount;
        protected AssembleLine[] assembleLines; 
        protected HealthComponent healthComponent;

        public void TakeDamage(float damage)
        {
            healthComponent.TakeDamage(damage);
            if(healthComponent.health <= 0)
            {
                // Need to be fixed
            }
        }
        public void Start()
        {
            assembleLines = new AssembleLine[assembleLineCount];
            storageList = new List<Storage>
            {
                new Storage(towerStorageCapacity)
            };
        }

    }
}
