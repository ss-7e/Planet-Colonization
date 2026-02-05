using Unity.Entities;
using Unity.Mathematics;



namespace DOTS
{
    public struct AmmoData : IComponentData
    {
        public float Damage;           // 子弹伤害
        public float Speed;            // 子弹速度
        public float Lifetime;         // 子弹生命周期
    }

    public struct MovementComponent : IComponentData
    {
        public float3 Direction;       // 移动方向
        public float ElapsedTime;      // 已经过的时间
    }
}