using Game.Turret;
using UnityEngine;

namespace Game.Modules
{
    public abstract class TurretModule : ScriptableObject
    {
        public string moduleName;
        public string description;

        public bool stackable = true;
        public float weight = 1f;   // affect turret's rotation speed


        public virtual void OnAttach(TurretBase turret) { turret.WeightChange(weight); }
        public virtual void OnDetach(TurretBase turret) { turret.WeightChange(-weight); }
    }
}

