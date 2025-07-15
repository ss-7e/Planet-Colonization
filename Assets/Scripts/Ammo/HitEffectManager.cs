using Game.EffectsEnums;
using System.Collections.Generic;
using UnityEngine;

namespace Game.EffectsEnums
{
    public enum HitEffectType
    {
        Piercing,
        Explosive
    }
}

namespace Game.Managers
{
    class HitEffectManager : MonoBehaviour
    {
        [SerializeField]
        private GameObject shellPiercingEffect;
        [SerializeField]
        private GameObject shellExplosiveEffect;
        [SerializeField]

        private Dictionary<HitEffectType, GameObject> effectPrefabs;
        public static HitEffectManager Instance { get; private set; }
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
            effectPrefabs = new Dictionary<HitEffectType, GameObject>
            {
                { HitEffectType.Piercing, shellPiercingEffect },
                { HitEffectType.Explosive, shellExplosiveEffect }
            };
        }
        public void CreateHitEffect(HitEffectType type, Vector3 position, Vector3 rotation)
        {
            if (effectPrefabs.TryGetValue(type, out GameObject effectPrefab))
            {
                GameObject effectInstance = Instantiate(effectPrefab, position, Quaternion.LookRotation(rotation));
                Destroy(effectInstance, 2f); 
            }
            else
            {
                Debug.LogWarning($"HitEffect of type {type} not found.");
            }
        }

    }
}
