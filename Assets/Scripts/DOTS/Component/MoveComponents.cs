using Unity.Entities;
using Unity.Mathematics;


namespace DOTS
{
    public struct ProjectileMoveComponent : IComponentData
    {
        public float3 Direction;       // 移动方向
        public float Speed;           // 移动速度
    }

    public struct TargetedMovementComponent : IComponentData
    {
        public float3 Direction;       // 移动方向
        public float3 TargetPosition;  // 目标位置
        public float Speed;            // 移动速度
    }
}