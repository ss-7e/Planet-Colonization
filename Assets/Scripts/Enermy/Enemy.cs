using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Game.Entites 
{
    public class Enemy : MonoBehaviour, IEnemyDamageable
    {
        public ParticleSystem explosion;
        [SerializeField]protected float maxHeight;
        [SerializeField]protected float health = 100f;
        [SerializeField]protected float speed = 5f;
        public float armorValue { get; private set; } = 0f;
        public float wearResistance { get; private set; } = 0f;
        public float attackValue { get; private set; } = 10f;
        public HealthComponent healthComp { get; private set; }

        public SensorBase sensor;  
        public float ttl = 20f;  // 20s 

        private void Awake()
        {
            sensor = ScriptableObject.CreateInstance<SensorFOV>();
            healthComp = new HealthComponent(health, health);
        }

        protected void Update()
        {
            if(transform.position.y > maxHeight)
            {
                Vector3 h = transform.position;
                h.y -= 0.1f;
                transform.position = h;
            }
            if (healthComp.health <= 0)
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

        public void Die()
        {
            Debug.Log("Enermy died.");
            Instantiate(explosion, transform.position, transform.rotation);
            Destroy(gameObject);
        }
    }
}

