using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;
using Unity.Mathematics;



namespace DOTS
{
    [BurstCompile]
    public partial struct LifeTimeSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<LifeTimeComponent>();
        }
        public void OnUpdate(ref SystemState state)
        {
            var deltaTime = SystemAPI.Time.DeltaTime;
            var ecb = new EntityCommandBuffer(Unity.Collections.Allocator.TempJob);
            var job = new LifeTimeJob
            {
                DeltaTime = deltaTime,
                ECB = ecb.AsParallelWriter()
            };
            state.Dependency = job.ScheduleParallel(state.Dependency);
            state.Dependency.Complete();
            ecb.Playback(state.EntityManager);
            ecb.Dispose();
        }
        public partial struct LifeTimeJob : IJobEntity
        {
            public float DeltaTime;
            public EntityCommandBuffer.ParallelWriter ECB;
            public void Execute([EntityIndexInQuery] int sortKey, Entity entity, ref LifeTimeComponent lifeTime)
            {
                lifeTime.ElapsedTime += DeltaTime;
                if (lifeTime.ElapsedTime >= lifeTime.Lifetime)
                {
                    ECB.DestroyEntity(sortKey, entity);
                }
            }
        }
    }
}