using UnityEngine;

namespace Game.Towers.ColonizeBuilding
{
    public abstract class ColonizeBuildingBase : Tower, IDamageable
    {
        protected HealthComponent healthComp;
        [SerializeField] protected float coinPerSecond = 10f;
        [SerializeField] protected float defaultHealth = 100f;
        protected float countDownTime;
        protected Grid[] occupiedGrids;
        public abstract void TakeDamage(float damage);
        protected virtual void Start()
        {
            healthComp = new HealthComponent(defaultHealth, defaultHealth);
        }
        protected void Update()
        {
            countDownTime -= Time.deltaTime;
            if (countDownTime <= 0)
            {
                countDownTime = 1f;
                // Logic to generate coins
                GenerateCoins();
            }
        }
        void GenerateCoins()
        {
            GameManager.instance.AddGalacticCredit(coinPerSecond);
        }

    }
}