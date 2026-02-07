using Unity.Entities;
using Unity.Mathematics;



namespace DOTS
{
    public struct LifeTimeComponent : IComponentData
    {
        public float Lifetime;         // 生命周期
        public float ElapsedTime;      // 已经过的时间
    }

    public struct AmmoData : IComponentData
    {
        public float Damage;           // 子弹伤害
        public float Speed;            // 子弹速度
        public float Radius;           // 子弹碰撞半径
    }


}