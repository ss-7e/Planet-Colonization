using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Collections;

namespace DOTS
{
    [BurstCompile]
    public partial struct RotatablePartSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<TurretAimer>();
            state.RequireForUpdate<RotatablePart>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var job = new RotatePartJob
            {
                TransformLookup = SystemAPI.GetComponentLookup<LocalTransform>(false)
            };

            state.Dependency = job.ScheduleParallel(state.Dependency);
        }

        [BurstCompile]
        public partial struct RotatePartJob : IJobEntity
        {
            [NativeDisableParallelForRestriction]
            public ComponentLookup<LocalTransform> TransformLookup;

            public void Execute(in TurretAimer aimer, in RotatablePart part)
            {
                Entity targetEntity = part.TurretEntity;

                if (!TransformLookup.HasComponent(targetEntity))
                    return;

                var transform = TransformLookup[targetEntity];

                // 计算目标旋转
                quaternion targetRotation = quaternion.AxisAngle(part.RotationAxisY, aimer.TargetingYaw);
                targetRotation = math.mul(targetRotation, quaternion.AxisAngle(part.RotationAxisZ, - aimer.TargetingPitch));

                // 更新 Transform
                transform.Rotation = targetRotation;


                TransformLookup[targetEntity] = transform;
            }
        }
    }
}