using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shuttle : MonoBehaviour, IDamageable
{
    public HealthComponent healthComp { get; private set; }
    public float defaltHealth;
    public HealthBarControl healthBarControl;
    public float wearResistance { get; private set; }
    [SerializeField] protected ParticleSystem explosion;
    private void Awake()
    {
        healthComp = new HealthComponent(defaltHealth, defaltHealth);
        if (healthComp == null)
        {
            Debug.LogError("HealthComponent is missing on Shuttle.");
        }
    }

    public void TakeDamage(float damage)
    {
        healthComp.TakeDamage(damage);
        healthBarControl.UpdateHealthBar(healthComp.health, healthComp.maxHealth);
        if (healthComp.health <= 0)
        {
            Die();
        }
    }
    void Die()
    {
        Debug.Log("Shuttle destroyed.");
        Instantiate(explosion, transform.position, transform.rotation);
        Destroy(gameObject);
    }
}
