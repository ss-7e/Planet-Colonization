using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class HealthComponent
{
    public float maxHealth { get; private set; }
    public float health { get; private set; }

    public HealthComponent(float H, float maxH)
    {
        health = H;
        maxHealth = maxH;
    }
    public void SetHealth(float newHealth)
    {
        health = newHealth;
    }
    public void TakeDamage(float damage)
    {
        health -= damage;
        if (health <= 0)
        {
            //add destroy logic
        }
    }
}

