// 简单敌人Authoring
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace DOTS
{
    public class EnemyAuthoring : MonoBehaviour
    {
        public float Priority = 1f;
        public float Speed = 5f;
    }

    public class EnemyBaker : Baker<EnemyAuthoring>
    {
        public override void Bake(EnemyAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new Enemy { Priority = authoring.Priority });
            AddComponent(entity, new EnemyMovement 
            { 
                Speed = authoring.Speed,
                Timer = 0f,
                TargetPosition = float3.zero
            });
        }
    }

    // 敌人移动组件
    public struct EnemyMovement : IComponentData
    {
        public float Speed;
        public float3 TargetPosition;
        public float Timer;
    }

    // 敌人移动系统
    [BurstCompile]
    public partial struct EnemyMovementSystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var movementJob = new EnemyMovementJob
            {
                DeltaTime = SystemAPI.Time.DeltaTime,
                Random = new Unity.Mathematics.Random((uint)((SystemAPI.Time.ElapsedTime + 1.0) * 1000))
            };

            movementJob.ScheduleParallel();
        }

        [BurstCompile]
        public partial struct EnemyMovementJob : IJobEntity
        {
            public float DeltaTime;
            public Unity.Mathematics.Random Random;

            public void Execute(
                ref LocalTransform transform,
                ref EnemyMovement movement)
            {
                movement.Timer += DeltaTime;

                // 如果达到2秒或者尚未初始化目标，选择新随机目标并重置计时器
                if (movement.Timer >= 2.0f || math.all(movement.TargetPosition == float3.zero))
                {
                    movement.TargetPosition = Random.NextFloat3(new float3(-20, 0, -20), new float3(20, 0, 20));
                    movement.Timer = 0f;
                }

                // 计算向目标的向量
                float3 dir = movement.TargetPosition - transform.Position;
                dir.y = 0; // 保持在水平面上
                // 规范化方向并移动 (避开零向量)
                if (math.lengthsq(dir) > 0.001f)
                {
                    float3 moveDir = math.normalize(dir);
                    transform.Position += moveDir * movement.Speed * DeltaTime;
                    // 转向移动方向
                    transform.Rotation = quaternion.LookRotationSafe(moveDir, math.up());
                }
            }
        }
    }
}
