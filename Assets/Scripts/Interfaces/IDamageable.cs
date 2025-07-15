using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
    void TakeDamage(float damage);
}

public interface IEnemyDamageable : IDamageable
{
    float wearResistance { get; }
}