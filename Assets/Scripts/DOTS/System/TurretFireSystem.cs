using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using Unity.Jobs;
using Unity.Transforms;
using Unity.Collections;
using Game.Ammo;

namespace DOTS
{
    [BurstCompile]
    public partial struct TurretFireSystem : ISystem
    {
        private EntityQuery _turretQuery;
        private ComponentLookup<LocalToWorld> _localToWorldLookup;
        private ComponentLookup<AmmoData> _shellDataLookup;

        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<Turret>();
            state.RequireForUpdate<TurretAimer>();
            state.RequireForUpdate<TurretShooter>();

            _turretQuery = SystemAPI.QueryBuilder()
                .WithAll<Turret, TurretAimer, TurretShooter>()
                .Build();
            
            _localToWorldLookup = state.GetComponentLookup<LocalToWorld>(true);
            _shellDataLookup = state.GetComponentLookup<AmmoData>(true);
        }

        public void OnUpdate(ref SystemState state)
        {
            var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
            var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

            _localToWorldLookup.Update(ref state);
            _shellDataLookup.Update(ref state);

            var fireJob = new TurretFireJob
            {
                DeltaTime = SystemAPI.Time.DeltaTime,
                ECB = ecb.AsParallelWriter(),
                LocalToWorldLookup = _localToWorldLookup,
                ShellDataLookup = _shellDataLookup
            };

            state.Dependency = fireJob.ScheduleParallel(_turretQuery, state.Dependency);
        }

        [BurstCompile]
        public partial struct TurretFireJob : IJobEntity
        {
            public float DeltaTime;
            public EntityCommandBuffer.ParallelWriter ECB;
            [ReadOnly] public ComponentLookup<LocalToWorld> LocalToWorldLookup;
            [ReadOnly] public ComponentLookup<AmmoData> ShellDataLookup;

            public void Execute(
                [EntityIndexInQuery] int sortKey,
                ref Turret turret,
                ref TurretShooter shooter)
            {
                shooter.TimeSinceLastShot += DeltaTime;

                if (turret.HasTarget && shooter.TimeSinceLastShot >= 1f / shooter.FireRate)
                {
                    if (shooter.ShellPrefab == Entity.Null) return;

                    Entity shell = ECB.Instantiate(sortKey, shooter.ShellPrefab);
                    
                    if (shooter.MuzzleEntity != Entity.Null &&
                        LocalToWorldLookup.TryGetComponent(shooter.MuzzleEntity, out LocalToWorld muzzleLtw))
                    {
                        var shellTransform = LocalTransform.FromPositionRotation(
                            muzzleLtw.Position, 
                            quaternion.LookRotation(muzzleLtw.Forward, muzzleLtw.Up)
                        );

                        float initialSpeed = 0f;
                        if (ShellDataLookup.TryGetComponent(shooter.ShellPrefab, out var shellData))
                        {
                            initialSpeed = shellData.Speed;
                        }

                        ProjectileMoveComponent ammoMovement = new ProjectileMoveComponent
                        {
                            Direction = math.normalize(muzzleLtw.Forward),
                            Speed = initialSpeed
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