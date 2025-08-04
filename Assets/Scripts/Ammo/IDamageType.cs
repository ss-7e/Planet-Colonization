using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Entites;
using Game.Managers;
using Game.EffectsEnums;

namespace Game.Ammo
{
    public interface IDamageType
    {
        void applyDamage(Shell shell, RaycastHit[] hits);
    }
    public class PiercingDamage : IDamageType
    {
        private float piercingDepthCount = 0f; //mm
        public void applyDamage(Shell shell, RaycastHit[] hits)
        {
            foreach (var hit in hits)
            {
                if (hit.collider.CompareTag("Enermy"))
                {
                    float speed = shell.speedVec.magnitude;
                    float damage = DamageCalculation(shell.ShellData.calibar, speed);
                }
                else if (hit.collider.CompareTag("GridMap"))
                {
                    HitEffectManager.Instance.CreateHitEffect(HitEffectType.Piercing, hit.point, hit.normal);
                }
            }
        }
        private float DamageCalculation(float calibar, float speed)
        {
            return calibar * calibar * speed * 0.1f;
        }
    }
    public class ExplosiveDamage : IDamageType
    {
        public void applyDamage(Shell shell, RaycastHit[] hits)
        {
            ShellDataExplosive explosiveData = shell.ShellData as ShellDataExplosive;
            float explosionRadius = explosiveData.explosionRadius;
            float explosivePower = explosiveData.explosivePower;
            LayerMask enemyMask = LayerMask.GetMask("Enermy");
            Collider[] hitEnemies = Physics.OverlapSphere(hits[0].transform.position, explosionRadius, enemyMask);
            foreach (Collider enermy in hitEnemies)
            {
                enermy.GetComponent<Enemy>().TakeDamage(explosivePower);
            }

        }
    }
}