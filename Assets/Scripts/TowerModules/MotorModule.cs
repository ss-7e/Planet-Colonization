using UnityEngine;
using Game.Turret;

namespace Game.Modules
{
    [CreateAssetMenu(fileName = "MotorModule", menuName = "Tower Modules/Motor Module", order = 1)]
    public class MotorModule : TurretModule
    {
        public float rotatePower = 1f; 
        public void RotateTurret(Transform turretRotatePart, Vector3 targetDirection, TurretBase turret)
        {
            float rotationSpeed = turret.totalWeight / rotatePower; 
            if (turretRotatePart == null)
            {
                Debug.LogError("Turret rotate part is not assigned.");
                return;
            }
            Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
            turretRotatePart.rotation = Quaternion.Slerp(turretRotatePart.rotation, targetRotation, Time.deltaTime * rotationSpeed);
        }
    }
}
