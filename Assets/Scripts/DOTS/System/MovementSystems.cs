using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;


namespace DOTS
{
    public partial struct ProjectileMoveSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<ProjectileMoveComponent>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var movementJob = new ProjectileMoveJob
            {
                DeltaTime = SystemAPI.Time.DeltaTime
            };
            movementJob.ScheduleParallel();
        }
        public partial struct ProjectileMoveJob : IJobEntity
        {
            public float DeltaTime;
            public void Execute(
                ref ProjectileMoveComponent movement,
                ref LifeTimeComponent lifeTime,
                ref LocalTransform transform)
            {
                float3 direction = movement.Direction * movement.Speed + new float3(0, -5f * DeltaTime, 0);
                movement.Speed = math.length(direction);
                movement.Direction = math.normalize(direction);
                transform.Position += direction * DeltaTime;
            }
        }
    }

    public partial struct TargetedMoveSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<TargetedMovementComponent>();
        }
        public void OnUpdate(ref SystemState state)
        {
            var movementJob = new TargetedMoveJob
            {
                DeltaTime = SystemAPI.Time.DeltaTime
            };
            movementJob.ScheduleParallel();
        }
        public partial struct TargetedMoveJob : IJobEntity
        {
            public float DeltaTime;
            public void Execute(
                ref TargetedMovementComponent movement,
                ref LocalTransform transform)
            {
                float3 direction = movement.TargetPosition - transform.Position;
                float distance = math.length(direction);
                if (distance > 0.1f)
                {
                    movement.Direction = math.normalize(direction);
                    transform.Position += movement.Direction * movement.Speed * DeltaTime;
                }
            }
        }
    }
}