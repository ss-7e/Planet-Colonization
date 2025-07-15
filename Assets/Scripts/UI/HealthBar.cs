using Game.Entites;
using Game.Turret;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBar : MonoBehaviour
{
    void Start()
    {
        InvokeRepeating("UpdateHealthBar", 0f, 0.1f);
    }
    void Update()
    {
        transform.LookAt(Camera.main.transform);
    }
    void UpdateHealthBar()
    {
        float health = 1f;
        TurretBase turret = GetComponentInParent<TurretBase>();
        if (turret != null)
        {
            health = turret.getHealth() / turret.getMaxHealth();
        }
        Enemy enermy = GetComponentInParent<Enemy>();
        if (enermy != null)
        {
            health = enermy.healthComp.health / enermy.healthComp.maxHealth;
        }
        health = Mathf.Clamp01(health);
        Vector3 scale = transform.localScale;
        scale.x = health;
        transform.localScale = scale;
        Vector3 pos = transform.localPosition;
        pos.x = -0.5f + health * 0.5f;
        transform.localPosition = pos;
    }
}
