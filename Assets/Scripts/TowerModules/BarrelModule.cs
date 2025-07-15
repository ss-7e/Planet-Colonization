using UnityEngine;
using Game.Turret;
using Game.Ammo;

namespace Game.Modules
{
    [CreateAssetMenu(fileName = "BarrelModule", menuName = "Tower Modules/Barrel Module", order = 1)]
    public class BarrelModule : TurretModule
    {
        public float heatCapacitySet = 90; // Maximum heat capacity persetage
        [SerializeField] float heatCapacity = 1000; // Max heat capacity
        [SerializeField] float heatDissipation = 10; // Heat dissipation rate per second
        public float caliber = 100f;
        public float lengthIncaliber = 39f;
        public GameObject newModelPrefab;


        GameObject currentTurret;
        Transform muzzlePos;

        public override void OnAttach(TurretBase turret)
        {
            muzzlePos = turret.muzzle;
            base.OnAttach(turret);
            MeshFilter currentMeshFilter = currentTurret.GetComponent<MeshFilter>();
            MeshRenderer currentRenderer = currentTurret.GetComponent<MeshRenderer>();

            MeshFilter newMeshFilter = newModelPrefab.GetComponent<MeshFilter>();
            MeshRenderer newRenderer = newModelPrefab.GetComponent<MeshRenderer>();

            if (currentMeshFilter != null && newMeshFilter != null)
            {
                currentMeshFilter.mesh = newMeshFilter.sharedMesh;
            }
            if (currentRenderer != null && newRenderer != null)
            {
                currentRenderer.materials = newRenderer.sharedMaterials;
            }
        }
        public bool Onfire(Vector3 direction, float chamberPressure, GameObject shellPrefab, ref float temperature)
        {
            if (temperature >= heatCapacity * heatCapacitySet / 100f)
            {
                return false;
            }
            temperature += chamberPressure * 0.1f; 
            float speed = caculateShellSpeed(chamberPressure);
            GameObject firedShell = Object.Instantiate(shellPrefab, muzzlePos.position, Quaternion.identity);
            Shell shellComponent = firedShell.GetComponent<Shell>();
            shellComponent.OnFire(direction, speed);
            return true;
        }
        public float caculateShellSpeed(float chamberPressure)
        {
            float speed = Mathf.Sqrt(chamberPressure * 1000f * lengthIncaliber);
            return speed;
        }
    }
}
