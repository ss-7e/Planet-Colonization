using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;


namespace DOTS
{
    /// <summary>
    /// 炮弹组件的Authoring，用于在编辑器中设置炮弹属性
    /// </summary>
    public class ShellAuthoring : MonoBehaviour
    {
        public float Damage = 10f;        // 子弹伤害
        public float Speed = 20f;         // 子弹速度
        public float Lifetime = 5f;       // 子弹生命周期

        public class ShellBaker : Baker<ShellAuthoring>
        {
            public override void Bake(ShellAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new AmmoData
                {
                    Damage = authoring.Damage,
                    Speed = authoring.Speed
                });
                AddComponent(entity, new ProjectileMoveComponent
                {
                    Direction = float3.zero // 初始方向可以在发射时设置
                });
                AddComponent(entity, new LifeTimeComponent
                {
                    Lifetime = authoring.Lifetime,
                    ElapsedTime = 0f
                });
            }
        }
    }
}