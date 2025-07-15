using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Ammo
{
    public abstract class ShellData : ScriptableObject
    {
        public string shellName;
        public float propellantEnergy;
        public float calibar = 120f; // mm
        public float deltaTime = 0.1f;
        public float timeToLive = 5f; // seconds
        public IDamageType damageType;
    }
    public class PiercingShellData : ShellData
    {
        public float penetrationDepth = 1000f; //mm
        
    }
    public class ExplosiveShellData : ShellData
    {
        public float explosionRadius = 5f; 
        public float explosivePower = 1000f; 
        
    }
}
