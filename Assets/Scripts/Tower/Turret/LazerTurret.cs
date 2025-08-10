using Game.Towers.Turrets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Towers.Turrets
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
