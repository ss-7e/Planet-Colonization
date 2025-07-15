using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Ammo
{
    public class Shell : MonoBehaviour
    {
        private ShellData shellData;
        // Shell states
        public bool fired = false;
        public Vector3 speedVec = Vector3.zero;
        private IAmmoState currentState;
        private float timetoLive = 10f;

        public ShellData ShellData
        {
            get { return shellData; }
            set
            {
                shellData = value;
            }
        }
        public void SetState(IAmmoState newState)
        {
            currentState?.Exit(this);
            currentState = newState;
            currentState.Enter(this);
        }

        private void Start()
        {
            shellData = new PiercingShellData(); // or new ExplosiveShellData();
            InvokeRepeating("ProjectileTracer", 0f, shellData.deltaTime);           
        }

        private void Update()
        {
            //add prefab movement logic
            currentState?.Update(this);
            timetoLive -= Time.deltaTime;
            if (timetoLive <= 0f)
            {
                SetState(new HitState());
                Destroy(gameObject);
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                Vector3 fireDirection = transform.forward;
                Vector3 pos = transform.position;
                OnFire(fireDirection, pos, 1);
            }
        }
        private void ProjectileTracer()
        {
            float deltaTime = shellData.deltaTime; 
            // simulate trajectory with multiple raycasts  
            if (!fired) { return; }
            Vector3 gravity = PlanetDataManager.instance.gravity;
            Vector3 startPos = transform.position;
            Vector3 endPos = startPos + speedVec * deltaTime + 0.5f * gravity * deltaTime * deltaTime;
            LayerMask mask = LayerMask.GetMask("Enermy", "GridMap");
            float maxDistance = Vector3.Distance(startPos, endPos);
            RaycastHit[] hits = Physics.RaycastAll(startPos, speedVec, maxDistance, mask.value);
            Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));
            if (hits.Length > 0)
            {
                ShellData.damageType?.applyDamage(this, hits);
            }
            else
            {
                Debug.DrawLine(startPos, endPos, Color.green);
            }
        }
        public void OnFire(Vector3 direction, Vector3 muzzlePos, float barrelLengthInCalibers)
        {
            Debug.Log("Shell fired in direction: " + direction + " from position: " + muzzlePos);
            float speed = SpeedCalculation(shellData.propellantEnergy, barrelLengthInCalibers); 
            speedVec = direction.normalized * speed;
            fired = true;
            transform.rotation = Quaternion.LookRotation(speedVec);
            transform.position = muzzlePos;
            SetState(new FiredState());
        }

        public void OnFire(Vector3 direction, float speed)
        {
            transform.rotation = Quaternion.LookRotation(speedVec);
            speedVec = direction.normalized * speed;
            fired = true;
            SetState(new FiredState());
        }
        protected virtual float SpeedCalculation(float barrelPressure, float barrelLengthInCalibers)
        {
            float basicSpeed = 1.0f;
            return basicSpeed * barrelPressure * barrelLengthInCalibers * 0.1f;
        }
    }
}





