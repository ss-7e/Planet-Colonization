using Game.Turret;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Turret
{
    public class LazerTurret : TurretBase
    {
        [SerializeField]
        float energyConsumptionRate;
        [SerializeField]
        float maxEnergy;
        [SerializeField]
        float currentEnergy;


    }
}
