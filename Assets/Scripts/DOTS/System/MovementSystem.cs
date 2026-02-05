using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;


namespace DOTS
{
    public partial struct MovementSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<MovementComponent>();
            state.RequireForUpdate<AmmoData>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var movementJob = new MovementJob
            {
                DeltaTime = SystemAPI.Time.DeltaTime
            };
            movementJob.ScheduleParallel();
        }
        public partial struct MovementJob : IJobEntity
        {
            public float DeltaTime;
            public void Execute(
                ref MovementComponent movement, 
                ref AmmoData ammo,
                ref LocalTransform transform)
            {
                float3 direction = movement.Direction;
                transform.Position += direction * ammo.Speed * DeltaTime;
            }
        }
    }
}