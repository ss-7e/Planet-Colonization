using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
namespace DOTS
{
    /// <summary>
    /// 将所有ECS炮塔相关的基础组件集中在此文件中定义
    /// 64字节以内
    /// </summary>


    // 1. 炮塔基础数据组件
    public struct Turret : IComponentData
    {
        public float RotationSpeed;      // 旋转速度（度/秒）
        public float MaxRotationAngle;   // 最大旋转角度限制（相对于初始朝向）
        public Entity TargetEntity;      // 当前目标敌人
        public bool HasTarget;           // 是否有有效目标
        public float3 InitialForward;    // 初始朝向（用于角度限制）
    }

    // 2. 可旋转部位标记组件
    public struct RotatablePart : IComponentData
    {
        public Entity TurretEntity;      // 所属的炮塔实体
        public float3 RotationAxisY;      // 旋转轴（通常是 Vector3.up 或 Vector3.right）
        public float3 RotationAxisZ;
        public bool UseLocalSpace;       // 是否使用局部空间旋转
    }

    // 3. 炮塔瞄准器组件（用于计算瞄准）
    public struct TurretAimer : IComponentData
    {
        public float MaxRange;           // 最大瞄准距离
        public float MinRange;           // 最小瞄准距离
        public float TargetingYaw;     // 当前瞄准偏航角（Yaw，相对于初始朝向）
        public float TargetingPitch;     // 当前瞄准俯仰角（Pitch，向上为正）
    }

    // 4. 敌人标记组件
    public struct Enemy : IComponentData
    {
        public float Priority;           // 目标优先级
    }

    // 5. 炮塔目标选择请求（事件组件）
    public struct TargetSelectionRequest : IComponentData
    {
        public Entity RequestingTurret;
        public float3 SearchCenter;
        public float SearchRadius;
    }

    // 6. 炮塔射击组件
    public struct TurretShooter : IComponentData
    {
        public float FireRate;           // 射击频率（每秒射击次数）
        public float TimeSinceLastShot;  // 距离上次射击的时间
        public Entity ShellPrefab;      // 子弹预制体实体
        public Entity MuzzleEntity;      // 发射点实体
    }

    // 7. 炮塔炮弹存储组件
    public struct TurretAmmo : IComponentData
    {
        public int CurrentAmmo;          // 当前弹药数量
        public int MaxAmmo;              // 最大弹药容量
        public float ReloadTime;         // 装填时间
        public float TimeSinceLastReload; // 距离上次装填的时间
    }
}