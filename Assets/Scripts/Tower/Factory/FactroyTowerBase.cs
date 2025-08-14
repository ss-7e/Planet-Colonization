using UnityEngine;
using System.Collections.Generic;
using Game.Towers.Turrets;
using Game.Ammo;
namespace Game.Towers.Factory
{
    public class FactroyTowerBase : Tower, IDamageable
    {
        [SerializeField]protected List<Vector2Int> effectArea;
        [SerializeField] protected int towerStorageCapacity;
        [SerializeField] protected int assembleLineCount;
        protected List<AssembleLine> assembleLines;
        protected AssembleLine activeAssembleLine;
        protected HealthComponent healthComponent;
        protected List<TurretBase> turretList = new List<TurretBase>();
        protected float produceTimer;
        public AssembleLinePress assblelinePressUI;
        public void TakeDamage(float damage)
        {
            healthComponent.TakeDamage(damage);
            if(healthComponent.health <= 0)
            {
                // Need to be fixed
            }
        }

        public void AddTurrettoList(TurretBase turret)
        {
            if (turretList == null)
            {
                turretList = new List<TurretBase>();
            }
            turretList.Add(turret);
        }
        public void Start()
        {
            assembleLines = TechManager.instance.activeLines;
            Storage towerStorage = new Storage(towerStorageCapacity);
            storageList = new List<Storage>
            {
                towerStorage
            };
            storageDistance[towerStorage] = float.MaxValue;
            storageTowerList[towerStorage] = this;
        }

        private void Update()
        {
            if (activeAssembleLine != null)
            {
                produceTimer += Time.deltaTime;
                if (produceTimer > activeAssembleLine.produceTime)
                {
                    produceTimer = 0f;
                    FinishProduce(activeAssembleLine);
                }
            }
        }

        public void ActiveTurretLine(AssembleLine line)
        {
            activeAssembleLine = line;
            produceTimer = 0f;
        }

        public float GetProducePercent()
        {
            if (activeAssembleLine == null)
            {
                return 0f;
            }
            return produceTimer / activeAssembleLine.produceTime;
        }

        public void AccelerateProduce(float time)
        {
            produceTimer += time;
        }
        

        void FinishProduce(AssembleLine line)
        {
            switch(line)
            {
                case ShellAssembleLine shellLine:
                    AmmoItem shellItem = new AmmoItem
                    {
                        shellData = shellLine.shellData,
                        currentCount = shellLine.produceCountPerCycle
                    };
                    OutputProduct(shellItem);
                    break;

            }
        }
        void OutputProduct(IStorable Item)
        {
            if (Item is AmmoItem ammoItem)
            {
                foreach (TurretBase turret in turretList)
                {
                    if (turret.AddAmmo(ammoItem))
                    {
                        return;
                    }
                }
            }
            else
            {
                foreach (Storage storage in storageList)
                {
                    if (storage.AddItem(Item))
                    {
                        return;
                    }
                }
            }
        }
    }
}
