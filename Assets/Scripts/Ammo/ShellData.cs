using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Ammo
{
    public abstract class ShellData : ScriptableObject
    {
        public string shellName;
        public int maxCount = 20;
        public float propellantEnergy;
        public float calibar = 120f; // mm
        public float deltaTime = 0.1f;
        public float timeToLive = 5f; // seconds
        public IDamageType damageType;
        public GameObject explosion;



        public virtual float DamageCount()
        {
            return calibar * 0.5f;
        }
    }

}
