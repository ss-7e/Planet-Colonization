using UnityEngine;
namespace Game.Tower.Factory
{
    public abstract class FactroyBase : IDamageable
    {
        [SerializeField]
        protected Vector2Int effectArea;
        protected Storage Storage;
        protected AssembleLine AssembleLine; 

        public void TakeDamage(float damage)
        {
            // Implement damage logic for the factory
            Debug.Log($"Factory took {damage} damage.");
            // You can add more logic here, like reducing health or triggering effects
        }
    }
}
