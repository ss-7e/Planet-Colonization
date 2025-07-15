using UnityEngine;
using Game.Turret;
using System.Collections.Generic;
using Game.Ammo;

namespace Game.Modules
{
    [CreateAssetMenu(fileName = "FireControlModule", menuName = "Tower Modules/Fire Control Module", order = 2)]
    public class FireControlModule : TurretModule
    {
        public void FireControlUpdate(TurretBase turret)
        {
            
            List<Vector3> targets = new List<Vector3>();
            foreach (RadarModuleData radar in turret.radarInstances)
            {
                if (radar != null)
                {
                    RadarModule radarModule = radar.module as RadarModule; 
                    if (radarModule != null)
                    {
                        List<Vector3> searchTargets = radarModule.SearchEnemy(turret.transform);
                        if (searchTargets != null)
                        {
                            targets.AddRange(searchTargets);
                        } 
                    }
                }
            }


            Vector3 targetPosition = Vector3.zero;
            float count = 0f;
            for (int i = 0; i < turret.maxTargetsPerAttack && i < targets.Count; i++)
            {
                targetPosition += targets[i];
                count++;
            }
            if (count > 1)
            {
                targetPosition /= count;
            }
            Vector3 targetDirection = targetPosition - turret.transform.position;


            ReloaderModule reloader = turret.reloaderInstance.module as ReloaderModule;
            ShellData shell = null;
            if (reloader != null)
            {
                reloader.ReloaderUpdate(turret.reloaderInstance, turret.ammoStorage);
                shell = turret.reloaderInstance.GetFiringShell();
            }
            else
            {
                Debug.LogWarning("Reloader is null.");
            }
            if(shell != null)
            {
                DirectionCalculate(turret, shell, ref targetDirection);
            }

            if (targetDirection.magnitude < 0.1f)
            {
                targetDirection = turret.transform.forward; 
            }
            MotorModule motor = turret.motorInstance.module as MotorModule;
            if (motor != null)
            {
                motor.RotateTurret(turret.rotatePart, targetDirection, turret);
            }
        }
        
        public virtual void DirectionCalculate(TurretBase turret, ShellData shell, ref Vector3 targetDirection)
        {
            float speed = shell.propellantEnergy / 1000f;
            Vector3 gravity = PlanetDataManager.instance.gravity;
            float timeToTarget = targetDirection.magnitude / speed;
            Vector3 gravityCompensation = 0.5f * gravity * timeToTarget * timeToTarget;
            targetDirection -= gravityCompensation;
        }
    }
}
