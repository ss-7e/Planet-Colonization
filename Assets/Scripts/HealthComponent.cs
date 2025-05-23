using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class HealthComponent
{
    public int maxHealth { get; private set; }
    public int health { get; private set; }

    public HealthComponent(int H, int maxH)
    {
        health = H;
        maxHealth = maxH;
    }
    public void SetHealth(int newHealth)
    {
        health = newHealth;
    }
    public void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            //add destroy logic
        }
    }
}

public class temp : MonoBehaviour
{
    
}