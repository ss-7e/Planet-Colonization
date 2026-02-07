using System.Diagnostics;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
namespace DOTS
{

    /// <summary>
    /// 直接遍历所有敌人实体，选择最佳目标的炮塔目标选择系统（距离+优先级综合评分）
    /// TODO： 优化：使用空间划分结构（如四叉树或八叉树）来减少每个炮塔需要检查的敌人数量
    /// TODO: 增加目标验证逻辑（如遮挡检测等）
    /// </summary>
    [BurstCompile]
    public partial struct TurretTargetingSystem : ISystem
    {
        private EntityQuery _enemyQuery;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            // Using EntityQueryBuilder avoids the managed array allocation error caused by params ComponentType[]
            var turretQuery = new EntityQueryBuilder(Allocator.Temp)
                .WithAll<Turret, TurretAimer>()
                .Build(ref state);

            state.RequireForUpdate(turretQuery);

            // 敌人查询
            _enemyQuery = new EntityQueryBuilder(Allocator.Temp)
                .WithAll<LocalTransform, Enemy>()
                .Build(ref state);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var targetingJob = new TargetSelectionJob
            {
                EnemyTransforms = _enemyQuery.ToComponentDataArray<LocalTransform>(Allocator.TempJob),
                EnemyEntities = _enemyQuery.ToEntityArray(Allocator.TempJob),
                EnemyPriorities = _enemyQuery.ToComponentDataArray<Enemy>(Allocator.TempJob),
                DeltaTime = SystemAPI.Time.DeltaTime
            };

            targetingJob.Schedule();
        }

        [BurstCompile]
        public partial struct TargetSelectionJob : IJobEntity
        {
            [ReadOnly] public NativeArray<LocalTransform> EnemyTransforms;
            [ReadOnly] public NativeArray<Entity> EnemyEntities;
            [ReadOnly] public NativeArray<Enemy> EnemyPriorities;
            public float DeltaTime;

            public void Execute(
                ref Turret turret,
                ref TurretAimer aimer,
                in LocalTransform turretTransform)
            {
                // 如果没有目标或目标无效，寻找新目标
                if (!turret.HasTarget || !EnemyEntities.Contains(turret.TargetEntity))
                {
                    FindNewTarget(ref turret, ref aimer, turretTransform);
                }
                else
                {
                    // 检查当前目标是否仍在范围内
                    ValidateCurrentTarget(ref turret, ref aimer, turretTransform);
                }
            }

            private void FindNewTarget(
                ref Turret turret,
                ref TurretAimer aimer,
                in LocalTransform turretTransform)
            {
                turret.HasTarget = false;
                turret.TargetEntity = Entity.Null;

                float bestScore = 0f;
                Entity bestTarget = Entity.Null;

                for (int i = 0; i < EnemyEntities.Length; i++)
                {
                    var enemyTransform = EnemyTransforms[i];
                    var enemyPriority = EnemyPriorities[i];

                    // 计算距离
                    float distance = math.distance(turretTransform.Position, enemyTransform.Position);

                    // 检查是否在范围内
                    if (distance < aimer.MinRange || distance > aimer.MaxRange)
                        continue;

                    // 计算角度（是否在可旋转范围内）
                    float3 toEnemy = enemyTransform.Position - turretTransform.Position;
                    toEnemy.y = 0; // 水平面投影

                    float angle = math.acos(math.dot(
                        math.normalize(turret.InitialForward),
                        math.normalize(toEnemy)
                    ));

                    if (angle > turret.MaxRotationAngle)
                        continue;

                    // 综合评分：距离 + 优先级
                    float distanceScore = 1f - math.saturate(distance / aimer.MaxRange);
                    float score = distanceScore * 0.7f + enemyPriority.Priority * 0.3f;

                    if (score > bestScore)
                    {
                        bestScore = score;
                        bestTarget = EnemyEntities[i];
                    }
                }

                if (bestTarget != Entity.Null)
                {
                    turret.TargetEntity = bestTarget;
                    turret.HasTarget = true;
                }
            }

            private void ValidateCurrentTarget(
                ref Turret turret,
                ref TurretAimer aimer,
                LocalTransform turretTransform)
            {
                // 这里可以添加目标验证逻辑
                // 比如目标超出范围、被遮挡等
            }
        }
    }
}