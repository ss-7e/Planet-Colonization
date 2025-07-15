using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Game.Entites 
{
    public class Enemy : MonoBehaviour, IEnemyDamageable
    {
        public ParticleSystem explosion;
        public float armorValue { get; private set; } = 0f;
        public float wearResistance { get; private set; } = 0f;
        public float attackValue { get; private set; } = 10f;
        public HealthComponent healthComp { get; private set; }

        public SensorBase sensor;  
        public float ttl = 20f;  // 20s 

        private void Awake()
        {
            sensor = ScriptableObject.CreateInstance<SensorFOV>();
            healthComp = new HealthComponent(100f, 100f);
        }

        protected void Update()
        {
            if (healthComp.health <= 0)
            {
                Die();
            }
            ttl -= Time.deltaTime;
            if (ttl <= 0)
            {
                Die();
            }
        }

        public void TakeDamage(float damage)
        {
            healthComp.TakeDamage(damage);
            Debug.Log("Enermy took " + damage + " damage.");
        }

        public void SetwearResistance(float resistance)
        {
            wearResistance = resistance;
            Debug.Log("Enermy wear resistance set to " + wearResistance);
        }

        void Die()
        {
            Debug.Log("Enermy died.");
            Instantiate(explosion, transform.position, transform.rotation);
            Destroy(gameObject);
        }
    }
}

