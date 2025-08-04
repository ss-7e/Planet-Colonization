using UnityEngine;
using Game.Turret;
using NUnit.Framework;
using System.Collections.Generic;

namespace Game.Modules
{
    [CreateAssetMenu(fileName = "RadarModule", menuName = "Tower Modules/Radar Module", order = 1)]
    public class RadarModule : TurretModule
    {
        [SerializeField]
        protected SensorBase searchSensor;
        [SerializeField]
        protected SensorBase lockSensor;
        [SerializeField]
        int maxTargets = 1;

        public float GetRadarRange()
        {
            if (searchSensor == null)
            {
                Debug.LogError("Search sensor is not assigned.");
                return 0f;
            }
            return searchSensor.range;
        }

        public virtual List<Vector3> SearchEnemy(Transform turretTransform)
        {
            if (searchSensor == null)
            {
                Debug.LogError("Search sensor is not assigned.");
                return null;
            }
            return searchSensor.DetectTargets(turretTransform);
        }

        public virtual List<Vector3> LockEnemy(Transform turretTransform)
        {
            if (lockSensor == null)
            {
                Debug.LogError("Lock sensor is not assigned.");
                return null;
            }
            return searchSensor.DetectTargets(turretTransform);
        }


    }
}
