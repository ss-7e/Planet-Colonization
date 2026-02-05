
using Unity.Entities;
using UnityEngine;
using Unity.Mathematics;
namespace DOTS
{
    // 炮塔Authoring
    public class TurretAuthoring : MonoBehaviour
    {
        [Header("基础设置")]
        public float RotationSpeed = 90f;
        public float MaxRotationAngle = 180f;
        public float MaxRange = 50f;
        public float MinRange = 5f;


        [Header("旋转部件")]
        public GameObject[] RotatableParts;  // 可旋转的炮管/炮塔部件

        [Header("开火设定")]
        public Transform AimPoint;            // 子弹发射点
        public GameObject BulletPrefab;      // 子弹预制体
        public float FireRate = 1f;          // 射击频率

    }

    // 炮塔Baker
    public class TurretBaker : Baker<TurretAuthoring>
    {
        public override void Bake(TurretAuthoring authoring)
        {
            Entity turretEntity = GetEntity(TransformUsageFlags.Dynamic);

            // 添加炮塔基础组件
            AddComponent(turretEntity, new Turret
            {
                RotationSpeed = math.radians(authoring.RotationSpeed),
                MaxRotationAngle = math.radians(authoring.MaxRotationAngle),
                HasTarget = false,
                TargetEntity = Entity.Null,
                InitialForward = authoring.transform.forward
            });

            // 添加瞄准器组件
            AddComponent(turretEntity, new TurretAimer
            {
                MaxRange = authoring.MaxRange,
                MinRange = authoring.MinRange,
                TargetingYaw = 0f,
                TargetingPitch = 0f
            });

            // 处理所有可旋转部件
            if (authoring.RotatableParts != null)
            {
                foreach (var part in authoring.RotatableParts)
                {
                    if (part == null) continue;

                    Entity partEntity = GetEntity(part, TransformUsageFlags.Dynamic);

                    // 添加可旋转部件组件
                    AddComponent(turretEntity, new RotatablePart
                    {
                        TurretEntity = partEntity,
                        RotationAxisY = Vector3.up,         //转动Yaw
                        RotationAxisZ = Vector3.right,      //俯仰Pitch
                        UseLocalSpace = true
                    });

                    
                }
            }

            // 添加瞄准点组件（如果有）
            if (authoring.AimPoint != null)
            {
                Entity aimPointEntity = GetEntity(authoring.AimPoint, TransformUsageFlags.Dynamic);
                AddComponent(turretEntity, new TurretShooter
                {
                    FireRate = authoring.FireRate,
                    TimeSinceLastShot = 0f,
                    ShellPrefab = GetEntity(authoring.BulletPrefab, TransformUsageFlags.Dynamic),
                    MuzzleEntity = aimPointEntity
                });
            }
        }
    }
}