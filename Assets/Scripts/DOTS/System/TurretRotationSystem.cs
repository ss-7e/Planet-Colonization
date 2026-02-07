using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Collections;
using Game.UI;

namespace DOTS
{

    /// <summary>
    /// 通过插值旋转炮塔以瞄准目标敌人的系统
    /// TODO: 优化：考虑使用更复杂的旋转插值方法（如 Slerp）以获得更平滑的旋转效果
    /// TODO: 增加预测目标移动的功能，以提高命中率
    /// TODO: 考虑炮塔旋转时的物理限制（如惯性、加速度等）
    /// TODO: 判断是否瞄准到目标，增加瞄准完成事件
    /// TODO：增加不同类型炮塔的特殊旋转行为（如跟踪速度更快的目标）
    /// </summary>
    [BurstCompile]
    public partial struct TurretRotationSystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var rotationJob = new TurretRotationJob
            {
                EnemyTransforms = SystemAPI.GetComponentLookup<LocalTransform>(true),
                DeltaTime = SystemAPI.Time.DeltaTime
            };

            rotationJob.ScheduleParallel();
        }

        [BurstCompile]
        public partial struct TurretRotationJob : IJobEntity
        {
            [ReadOnly] public ComponentLookup<LocalTransform> EnemyTransforms;
            public float DeltaTime;

            public void Execute(
                ref Turret turret,
                ref TurretAimer aimer,
                in LocalTransform turretTransform)
            {
                if (!turret.HasTarget)
                    return;

                if (!EnemyTransforms.TryGetComponent(turret.TargetEntity, out var enemyTransform))
                    return;

                // --- 0. Setup Targets ---、
                //float3 targetTemp = PointAt.Instance.gridHit.Pos;
                //float3 toTarget = targetTemp - turretTransform.Position;
                float3 targetPos = enemyTransform.Position;
                float3 toTarget = targetPos - turretTransform.Position;

                // --- 1. 水平旋转 (Yaw) ---
                float3 horizontalDirection = new float3(toTarget.x, 0, toTarget.z);
                float horizontalDistance = math.length(horizontalDirection);
                
                if (horizontalDistance < 0.001f) return;

                float3 normalizedHorizontal = horizontalDirection / horizontalDistance;

                float targetHorizontalAngle = CalculateAngle(
                    turret.InitialForward,
                    normalizedHorizontal,
                    new float3(0, 1, 0)
                );

                targetHorizontalAngle = math.clamp(
                    targetHorizontalAngle,
                    -turret.MaxRotationAngle,
                    turret.MaxRotationAngle
                );

                float deltaYaw = targetHorizontalAngle - aimer.TargetingYaw;
                deltaYaw = math.atan2(math.sin(deltaYaw), math.cos(deltaYaw));
                aimer.TargetingYaw += deltaYaw * math.min(1.0f, turret.RotationSpeed * DeltaTime);

                // --- 2. 垂直俯仰 (Pitch) ---
                float v = 20f; // Projectile Speed (should be a variable in Turret component)
                float g = 5f; // Gravity
                float x = horizontalDistance;
                float y = toTarget.y; // Vertical displacement

                float v2 = v * v;
                float v4 = v2 * v2;
                float root = v4 - g * (g * (x * x) + 2 * y * v2);

                float targetPitchAngle;
                if (root >= 0)
                {
                    // Calculate the low-arc trajectory angle
                    targetPitchAngle = math.atan((v2 - math.sqrt(root)) / (g * x));
                    aimer.HitTime = x / (v * math.cos(targetPitchAngle));
                }
                else
                {
                    // Target is out of range, point towards it as best as possible
                    targetPitchAngle = math.atan2(y, x);
                    aimer.HitTime = 0;
                }

                float maxPitchRad = math.radians(45);
                targetPitchAngle = math.clamp(targetPitchAngle, -maxPitchRad, maxPitchRad);

                float deltaPitch = targetPitchAngle - aimer.TargetingPitch;
                deltaPitch = math.atan2(math.sin(deltaPitch), math.cos(deltaPitch));
                aimer.TargetingPitch += deltaPitch * math.min(1.0f, turret.RotationSpeed * DeltaTime);
            }

            private readonly float CalculateAngle(float3 from, float3 to, float3 axis)
            {
                float dot = math.dot(from, to);
                float det = math.dot(axis, math.cross(from, to));
                return math.atan2(det, dot);
            }
        }
    }
}