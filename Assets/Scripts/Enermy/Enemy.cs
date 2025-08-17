using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Game.Entites 
{
    public class Enemy : MonoBehaviour, IEnemyDamageable
    {
        public ParticleSystem explosion;
        public float randomForceInterval = 3f;
        public float randomForceScale = 0.1f;
        Vector3 moveForce;
        Vector3 moveDirection;
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
            InvokeRepeating(nameof(AddRandomForce), 0f, randomForceInterval);
        }
        
        protected void Update()
        {
            Moving();
            if (healthComp.health <= 0)
            {
                Die();
            }
        }
        void Moving()
        {
            moveDirection += moveForce * Time.deltaTime;
            moveDirection = Vector3.ClampMagnitude(moveDirection, 1f);
            Vector3 newPosition = transform.position + moveDirection * speed * Time.deltaTime;
            newPosition.y = Mathf.Clamp(newPosition.y, 0, maxHeight);
            transform.position = newPosition;
            if(moveDirection != Vector3.zero)
                transform.rotation = Quaternion.LookRotation(moveDirection);
        }

        void AddRandomForce()
        {
            Vector3 randomForce = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f));
            AddMoveForce(randomForce, randomForceScale);
        }

        public void TakeDamage(float damage)
        {
            healthComp.TakeDamage(damage);
        }

        public void SetwearResistance(float resistance)
        {
            wearResistance = resistance;
        }

        public void AddMoveForce(Vector3 force, float scale)
        {
            moveForce += force.normalized * scale;
            moveForce = Vector3.ClampMagnitude(moveForce, 1f); 
        }
        public void Die()
        {

            Instantiate(explosion, transform.position, transform.rotation);
            Destroy(gameObject);
        }
    }
}

