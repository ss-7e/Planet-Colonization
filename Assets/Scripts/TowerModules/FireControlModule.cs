using Game.Ammo;
using Game.Turret;
using System.Collections.Generic;
using Unity.Android.Gradle.Manifest;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

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
            Vector3 targetDirection = targetPosition - turret.muzzle.position;


            ReloaderModule reloader = turret.reloaderInstance.module as ReloaderModule;
            ShellData shellToFire = null;
            if (reloader != null)
            {
                reloader.ReloaderUpdate(turret.reloaderInstance, turret.ammoStorage);
                shellToFire = turret.reloaderInstance.GetFiringShell();
            }
            else
            {
                Debug.LogWarning("Reloader is null.");
            }

            bool canFire = true;
            if(shellToFire != null && count > 0.1f)
            {
                canFire = AimDirectionCalculate(turret, shellToFire, ref targetDirection);
                float speed = shellToFire.propellantEnergy / 1000f;
            }

            
            if (targetDirection.magnitude < 0.1f || count < 0.1f || !canFire)
            {
                targetDirection = turret.transform.forward; 
            }


            MotorModule motor = turret.motorInstance.module as MotorModule;
            if (motor != null)
            {
                motor.RotateTurret(turret.rotatePart, targetDirection, turret);
            }

            if((turret.rotatePart.forward - targetDirection.normalized).magnitude < 0.1f 
                && shellToFire != null && canFire)
            {
                Fire(shellToFire, turret, targetDirection);
                reloader.Fire(turret.reloaderInstance);
                canFire = false; 
            }
        }
        
        public virtual void Fire(ShellData shellToFire, TurretBase turret, Vector3 fireDirection)
        {
            GameObject shellObject = Instantiate(turret.shellPrefab,turret.muzzle.position, turret.muzzle.rotation);
            Shell shell = shellObject.GetComponent<Shell>();
            shell.ShellData = shellToFire;
            fireDirection += Random.insideUnitSphere / 5.0f;
            fireDirection.Normalize();
            shell.speedVec = fireDirection * shellToFire.propellantEnergy / 1000f; 
            shell.transform.SetParent(turret.transform);
            shell.SetState(new FiredState());
            shell.fired = true;
        }


        public virtual bool AimDirectionCalculate(TurretBase turret, ShellData shell, ref Vector3 targetDirection)
        {
            Vector3 targetDirectionXZ = new Vector3(targetDirection.x, 0f, targetDirection.z);
            float speed = shell.propellantEnergy / 1000f;
            float distance = targetDirectionXZ.magnitude;
            float height = targetDirection.y;
            float g =  -PlanetDataManager.instance.gravity.y;

            float speedSquared = speed * speed;

            float underSqrt = speedSquared * speedSquared - g * (g * distance * distance + 2 * height * speedSquared);
            if (underSqrt < 0f)
            {
                targetDirection = turret.transform.forward; // fallback to forward direction
                return false;
            }
            float sqrt = Mathf.Sqrt(underSqrt);

            targetDirection.y = (speedSquared - sqrt) / g;
            return true;
        }
    }
}
