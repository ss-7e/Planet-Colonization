using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using Unity.Jobs;
using Unity.Transforms;
using Unity.Collections;

namespace DOTS
{
    [BurstCompile]
    public partial struct TurretFireSystem : ISystem
    {
        private EntityQuery _turretQuery;
        // 声明 Lookup
        private ComponentLookup<LocalToWorld> _localToWorldLookup;

        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<Turret>();
            state.RequireForUpdate<TurretAimer>();
            state.RequireForUpdate<TurretShooter>();

            _turretQuery = SystemAPI.QueryBuilder()
                .WithAll<Turret, TurretAimer, TurretShooter>()
                .Build();
            
            // 初始化 Lookup
            _localToWorldLookup = state.GetComponentLookup<LocalToWorld>(true);
        }

        public void OnUpdate(ref SystemState state)
        {
            var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
            var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

            // 更新 Lookup 的当前帧状态
            _localToWorldLookup.Update(ref state);

            var fireJob = new TurretFireJob
            {
                DeltaTime = SystemAPI.Time.DeltaTime,
                ECB = ecb.AsParallelWriter(),
                LocalToWorldLookup = _localToWorldLookup // 使用 LocalToWorld
            };

            state.Dependency = fireJob.ScheduleParallel(_turretQuery, state.Dependency);
        }

        [BurstCompile]
        public partial struct TurretFireJob : IJobEntity
        {
            public float DeltaTime;
            public EntityCommandBuffer.ParallelWriter ECB;
            [ReadOnly] public ComponentLookup<LocalToWorld> LocalToWorldLookup;

            public void Execute(
                [EntityIndexInQuery] int sortKey,
                ref Turret turret,
                ref TurretAimer aimer,
                ref TurretShooter shooter)
            {
                shooter.TimeSinceLastShot += DeltaTime;

                if (turret.HasTarget && shooter.TimeSinceLastShot >= 1f / shooter.FireRate)
                {
                    if (shooter.ShellPrefab == Entity.Null) return;

                    // 创建子弹实体
                    Entity shell = ECB.Instantiate(sortKey, shooter.ShellPrefab);

                    // 使用 LocalToWorld 获取炮口在世界空间的位置和旋转
                    if (shooter.MuzzleEntity != Entity.Null &&
                        LocalToWorldLookup.TryGetComponent(shooter.MuzzleEntity, out LocalToWorld muzzleLtw))
                    {
                        // 将 LocalToWorld 转换为 LocalTransform (世界空间)
                        var shellTransform = LocalTransform.FromPositionRotation(
                            muzzleLtw.Position, 
                            quaternion.LookRotation(muzzleLtw.Forward, muzzleLtw.Up)
                        );
                        MovementComponent ammoMovement = new MovementComponent
                        {
                            Direction = muzzleLtw.Forward,
                            ElapsedTime = 0f
                        };
                        ECB.SetComponent(sortKey, shell, shellTransform);
                        ECB.SetComponent(sortKey, shell, ammoMovement);
                    }
                    
                    shooter.TimeSinceLastShot = 0f;
                }
            }
        }
    }
}