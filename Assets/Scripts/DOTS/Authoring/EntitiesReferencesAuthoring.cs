using Unity.Entities;
using UnityEngine;

namespace DOTS
{
    // 用于在Authoring中引用实体预制件
    public class EntitiesReferencesAuthoring : MonoBehaviour
    {
        [SerializeField]
        private GameObject _turretPrefab;
        public class Baker : Baker<EntitiesReferencesAuthoring>
        {
            public override void Bake(EntitiesReferencesAuthoring authoring)
            {
                Entity entity = GetEntity(authoring._turretPrefab, TransformUsageFlags.Dynamic);
                AddComponent<EntitesReferencesComponent>(entity, new EntitesReferencesComponent
                {

                });
            }
        }
    }

    public struct EntitesReferencesComponent : IComponentData
    {
        public Entity TurretPrefabEntity;
    }
}