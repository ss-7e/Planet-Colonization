using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Ammo
{
    public class Shell : MonoBehaviour
    {
        private IAmmoState currentState;

        // Shell states
        public bool fired = false;
        public Vector3 speedVec = Vector3.zero;
        public float speed = 0f;
        public float timetoLive = 10f;
        public GameObject hitTarget { get; private set; }
        public Vector3 Pos;

        public ShellData ShellData;

        public void SetState(IAmmoState newState)
        {
            currentState?.Exit(this);
            currentState = newState;
            currentState?.Enter(this);
        }

        private void Start()
        {
            Pos = transform.position;
            InvokeRepeating("ProjectileTracer", 0f, ShellData.deltaTime);           
        }

        private void Update()
        {
            //add prefab movement logic
            currentState?.Update(this);
            timetoLive -= Time.deltaTime;
            if (timetoLive <= 0f)
            {
                Destroy(gameObject);
            }
        }

        private void ProjectileTracer()
        {
            float deltaTime = ShellData.deltaTime; 
            // simulate trajectory with multiple raycasts  
            if (!(currentState is FiredState)) { return; }
            Vector3 gravity = OnPlanetDataManager.instance.gravity;
            Vector3 startPos = transform.position;
            Vector3 endPos = startPos + speedVec * deltaTime + 0.5f * gravity * deltaTime * deltaTime;
            LayerMask mask = LayerMask.GetMask("Enemy", "GridMap");
            float maxDistance = Vector3.Distance(startPos, endPos);
            RaycastHit[] hits = Physics.RaycastAll(startPos, speedVec, maxDistance, mask.value);
            Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));
            if (hits.Length > 0)
            {
                hitTarget = hits[0].collider.gameObject;
                SetState(new HitState());
            }
            else
            {
                Debug.DrawLine(startPos, endPos, Color.green);
            }
        }

        public void ShellHit()
        {
            Instantiate(ShellData.explosion, transform.position, transform.rotation);
            if (hitTarget != null)
            {
                IEnemyDamageable damageable = hitTarget.GetComponent<IEnemyDamageable>();
                if (damageable != null)
                {
                    damageable.TakeDamage(ShellData.DamageCount());
                }
            }
            SetState(null);
            Destroy(gameObject);
        }
    }
}





