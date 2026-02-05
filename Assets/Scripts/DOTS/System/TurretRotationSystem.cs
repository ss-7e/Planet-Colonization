using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Collections;

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

                // 计算目标方向
                float3 toTarget = enemyTransform.Position - turretTransform.Position;

                // --- 1. 水平旋转 (Yaw) ---
                float3 horizontalDirection = new float3(toTarget.x, 0, toTarget.z);
                float horizontalDistance = math.length(horizontalDirection); // 保存距离用于Pitch计算
                horizontalDirection = math.normalize(horizontalDirection);

                // 计算水平旋转角度
                float currentHorizontalAngle = aimer.TargetingYaw;
                float targetHorizontalAngle = CalculateAngle(
                    turret.InitialForward,
                    horizontalDirection,
                    new float3(0, 1, 0)
                );

                // 角度限制
                targetHorizontalAngle = math.clamp(
                    targetHorizontalAngle,
                    -turret.MaxRotationAngle,
                    turret.MaxRotationAngle
                );

                // 插值旋转 Yaw
                aimer.TargetingYaw = math.lerp(
                    currentHorizontalAngle,
                    targetHorizontalAngle,
                    turret.RotationSpeed * DeltaTime
                );

                // --- 2. 垂直俯仰 (Pitch) ---
                // 计算目标俯仰角 (atan2(y, horizontalDistance))
                // 假设：正值为向上抬起 (Up)
                float targetPitchAngle = math.atan2(toTarget.y, horizontalDistance);

                // 简单的角度限制 (通常俯仰角限制不同于水平角，这里暂时复用或给死值，例如 -45 到 45 度)
                // 注意：atan2返回的是弧度，Update中使用的是弧度插值，如果RotationSpeed是度，要注意单位转换
                // 原代码 calculateAngle 返回弧度，lerp 直接用。我们保持一致，targetPitchAngle 也是弧度。
                // 限制在 +/- 45度 (0.78弧度)
                float maxPitchRad = math.radians(45); 
                targetPitchAngle = math.clamp(targetPitchAngle, -maxPitchRad, maxPitchRad);

                float currentPitchAngle = aimer.TargetingPitch;
                
                // 插值旋转 Pitch
                aimer.TargetingPitch = math.lerp(
                    currentPitchAngle,
                    targetPitchAngle,
                    turret.RotationSpeed * DeltaTime
                );
            }

            private float CalculateAngle(float3 from, float3 to, float3 axis)
            {
                float3 cross = math.cross(from, to);
                float sign = math.sign(math.dot(cross, axis));
                float angle = math.acos(math.clamp(math.dot(math.normalize(from), math.normalize(to)), -1f, 1f));
                return angle * sign;
            }
        }
    }
}