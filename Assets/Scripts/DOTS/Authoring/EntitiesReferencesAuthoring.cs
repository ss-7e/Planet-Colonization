using Unity.Entities;
using UnityEngine;

namespace DOTS
{
    using Unity.Entities;
    using Unity.Collections;
    using UnityEngine;

    namespace DOTS
    {
        public class EntitiesReferencesAuthoring : MonoBehaviour
        {
            [SerializeField]
            private GameObject[] _turretPrefabs;

            public class Baker : Baker<EntitiesReferencesAuthoring>
            {
                public override void Bake(EntitiesReferencesAuthoring authoring)
                {
                    var entity = GetEntity(TransformUsageFlags.None);

                    // 使用动态缓冲区存储所有预制件
                    var turretBuffer = AddBuffer<TurretPrefabBuffer>(entity);

                    for (int i = 0; i < authoring._turretPrefabs.Length; i++)
                    {
                        turretBuffer.Add(new TurretPrefabBuffer
                        {
                            PrefabEntity = GetEntity(authoring._turretPrefabs[i], TransformUsageFlags.Dynamic),
                            PrefabIndex = i
                        });
                    }

                    // 添加一个管理组件
                    AddComponent(entity, new TurretPrefabManager
                    {
                        PrefabCount = authoring._turretPrefabs.Length
                    });
                }
            }
        }

        // 缓冲区元素
        public struct TurretPrefabBuffer : IBufferElementData
        {
            public Entity PrefabEntity;
            public int PrefabIndex;
        }

        // 管理组件
        public struct TurretPrefabManager : IComponentData
        {
            public int PrefabCount;
        }
    }
}