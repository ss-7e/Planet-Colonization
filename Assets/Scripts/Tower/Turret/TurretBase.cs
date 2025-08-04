using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using Game.Ammo;
using Game.Modules;

namespace Game.Turret
{
    public class TurretBase : MonoBehaviour, IDamageable
    {
        [SerializeField] protected Transform _muzzle;
        public Transform muzzle { 
            get=> _muzzle;
            protected set { _muzzle = value; }
        }

        [SerializeField]protected Transform turretRotatePart;
        public Transform rotatePart => turretRotatePart;

        protected IAttackStrategy attackStrategy;
        protected HealthComponent healthComp;
        [SerializeField]protected int radarCount = 1;

        [SerializeField] protected RadarModule defaltRadar;
        [SerializeField] protected ReloaderModule defaltReloader;
        [SerializeField] protected MotorModule defaltMotor;
        [SerializeField] protected FireControlModule defaltFireControl;
        public RadarModuleData[] radarInstances { get; private set; }
        public ReloaderModuleData reloaderInstance { get; private set; }
        public MotorModuleData motorInstance { get; private set; }

        public TurretAmmoStorage ammoStorage { get; private set; }
        [SerializeField] protected PackedAmmo defaultAmmoPack;

        [SerializeField] protected int _maxTargetsPerAttack = 1;
        public int maxTargetsPerAttack
        {
            get => _maxTargetsPerAttack;
            private set
            {
                if (value < 1)
                {
                    Debug.LogWarning("maxTargetsPerAttack cannot be less than 1");
                    return;
                }
                _maxTargetsPerAttack = value;
            }
        }

        [SerializeField] private float _totalWeight = 0f;
        public float totalWeight
        {
            get => _totalWeight;
            private set
            {
                _totalWeight = value;
            }
        }


        public GameObject shellPrefab;

        protected void Start()
        {
            healthComp = new HealthComponent(100f, 100f);
            radarInstances = new RadarModuleData[radarCount];
            radarInstances[0] = new RadarModuleData(defaltRadar);
            reloaderInstance = new ReloaderModuleData(defaltReloader);
            motorInstance = new MotorModuleData(defaltMotor);
            ammoStorage = new TurretAmmoStorage();
            totalWeight = 5f;
            if (defaultAmmoPack != null)
            {
                defaultAmmoPack.InitTurretStorage(ammoStorage);
            }
        }

        protected void Update()
        {
            defaltFireControl?.FireControlUpdate(this);
        }
        
        public void TakeDamage(float damage)
        {
            healthComp.TakeDamage(damage);
            if (healthComp.health <= 0)
            {
                Destroy(gameObject);
            }
        }
        public float getHealth()
        {
            return healthComp.health;
        }
        public float getMaxHealth()
        {
            return healthComp.maxHealth;
        }
        public void WeightChange(float addweight)
        {
            totalWeight += addweight;
        }
        public bool ModifyModule(ModuleData moduledata)
        {
            moduledata.module.OnAttach(this);
            switch (moduledata)
            {
                case ReloaderModuleData reloaderData:
                    if (reloaderInstance != null)
                    {
                        reloaderInstance.module.OnDetach(this);
                    }
                    reloaderInstance = reloaderData;
                    return true;
                case MotorModuleData motorData:
                    if (motorInstance != null)
                    {
                        motorInstance.module.OnDetach(this);
                    }
                    motorInstance = motorData;
                    return true;
                default:
                    Debug.LogWarning("Unknown module type");
                    break;
            }
            return false;
        }
        public bool ModifyRadar(RadarModuleData radarData, int index)
        {
            if (index < 0 || index >= radarInstances.Length)
            {
                Debug.LogWarning("Radar index out of range");
                return false;
            }
            if (radarInstances[index] != null)
            {
                radarInstances[index].module.OnDetach(this);
            }
            radarInstances[index] = radarData;
            radarData.module.OnAttach(this);
            return true;
        }


    }
}
