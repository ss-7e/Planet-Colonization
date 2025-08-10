using UnityEngine;

namespace Game.Towers.Factory
{
    public class ShellFactory : IDamageable
    {
        HealthComponent healthComp;
        public void TakeDamage(float damage) 
        {
            healthComp.TakeDamage(damage);
            if (healthComp.health <= 0)
            {
                DestroyFactory();
            }
        }
        private void DestroyFactory()
        {
            // Logic to destroy the factory, e.g., play an animation, remove from scene, etc.
            Debug.Log("Shell Factory destroyed.");
            // Optionally, you can destroy the game object if this is a MonoBehaviour
            // GameObject.Destroy(gameObject);
        }



    }
}
