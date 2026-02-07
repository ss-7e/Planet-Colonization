using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
namespace DOTS
{
    [BurstCompile]
    public partial struct CollideSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            // 确保场景中有弹药数据才运行系统
            state.RequireForUpdate<AmmoData>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var ecbSingleton = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
            // 使用 AsParallelWriter 以支持并行 Job 写入
            var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter();

            // 收集当前所有的敌人位置和实体 ID
            // 根据 EnemyAuthoring，敌人拥有 Enemy 组件
            var enemyQuery = SystemAPI.QueryBuilder().WithAll<Enemy, LocalTransform>().Build();
            var enemies = enemyQuery.ToEntityArray(Allocator.TempJob);
            var enemyTransforms = enemyQuery.ToComponentDataArray<LocalTransform>(Allocator.TempJob);

            var collisionJob = new CollisionJob
            {
                ECB = ecb,
                DeltaTime = SystemAPI.Time.DeltaTime,
                EnemyEntities = enemies,
                EnemyTransforms = enemyTransforms
            };

            // 调度并行 Job
            state.Dependency = collisionJob.ScheduleParallel(state.Dependency);
            
            // 在 Job 完成后自动释放临时数组
            enemies.Dispose(state.Dependency);
            enemyTransforms.Dispose(state.Dependency);
        }

        [BurstCompile]
        public partial struct CollisionJob : IJobEntity
        {
            public EntityCommandBuffer.ParallelWriter ECB;
            public float DeltaTime;
            
            [ReadOnly] public NativeArray<Entity> EnemyEntities;
            [ReadOnly] public NativeArray<LocalTransform> EnemyTransforms;

            public void Execute(
                Entity bulletEntity, 
                [EntityIndexInQuery] int sortKey, 
                in LocalTransform transform, 
                in ProjectileMoveComponent movement)
            {
                // 1. 同步 ProjectileMoveSystem 的逻辑计算下一帧的预测位置
                float3 currentPos = transform.Position;
                float3 velocity = movement.Direction * movement.Speed;
                float3 nextPos = currentPos - velocity * DeltaTime;

                // 2. 准备线段碰撞检测所需向量
                float3 segment = nextPos - currentPos;
                float segmentLenSq = math.lengthsq(segment) / 2;

                // 3. 遍历所有敌人进行距离判定
                for (int i = 0; i < EnemyEntities.Length; i++)
                {
                    float3 enemyPos = EnemyTransforms[i].Position;
                    float3 startToEnemy = enemyPos - currentPos;

                    // 计算点到线段的最短距离对应的投影系数 t
                    // 如果位移极小，t 设为 0；否则计算投影并限制在 [0, 1] 之间
                    float t = segmentLenSq < 0.0001f ? 0f : math.saturate(math.dot(startToEnemy, segment) / segmentLenSq);
                    
                    // 线段上离敌人最近的点
                    float3 closestPoint = currentPos + t * segment;

                    // 4. 判定是否命中（最短距离 < 0.5f）
                    if (math.distance(enemyPos, closestPoint) < 0.5f)
                    {

                        // ==========================================
                        // 在此处编写碰撞后的事件逻辑（如扣血、产生火花等）
                        // ==========================================

                        // 销毁子弹
                        ECB.DestroyEntity(sortKey, bulletEntity);
                        
                        // 销毁命中目标

                        
                        // 一颗子弹只判定一次碰撞，跳出循环
                        break;
                    }
                }
            }
        }
    }
}