using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shuttle : MonoBehaviour, IDamageable
{
    public HealthComponent healthComp { get; private set; }
    public float wearResistance { get; private set; }

    private void Awake()
    {
        healthComp = new HealthComponent(100f, 100f);
        if (healthComp == null)
        {
            Debug.LogError("HealthComponent is missing on Shuttle.");
        }
    }

    public void TakeDamage(float damage)
    {
        healthComp.TakeDamage(damage);
        if (healthComp.health <= 0)
        {
            // Handle destruction or other logic here  
        }
    }
}
