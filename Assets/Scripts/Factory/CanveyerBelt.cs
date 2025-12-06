using Factory;
using System.Collections.Generic;
using UnityEngine;

namespace Factroy
{
    public class CanveyerBelt : MonoBehaviour
    {
        public float speed = 2f;
        public List<Mesh> beltMeshes;


        private List<CanveyerBeltUnit> beltUnits;

        private void Start()
        {
            beltUnits = new List<CanveyerBeltUnit>();
            float unitLength = 1f; // Assuming each belt unit has a length of 1 unit
            int unitCount = Mathf.CeilToInt(transform.localScale.z / unitLength);
            for (int i = 0; i < unitCount; i++)
            {
                GameObject beltUnitObj = new GameObject("CanveyerBeltUnit_" + i);
                beltUnitObj.transform.parent = transform;
                beltUnitObj.transform.localPosition = new Vector3(0, 0, i * unitLength);
                beltUnitObj.transform.localRotation = Quaternion.identity;
                beltUnitObj.transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, unitLength);
                MeshFilter meshFilter = beltUnitObj.AddComponent<MeshFilter>();
                MeshRenderer meshRenderer = beltUnitObj.AddComponent<MeshRenderer>();
                meshRenderer.material = new Material(Shader.Find("Standard"));
                // Assign a random mesh from the list
                if (beltMeshes.Count > 0)
                {
                    meshFilter.mesh = beltMeshes[Random.Range(0, beltMeshes.Count)];
                }
                CanveyerBeltUnit beltUnit = beltUnitObj.AddComponent<CanveyerBeltUnit>();
                beltUnits.Add(beltUnit);
            }
        }
        private void OnTriggerStay(Collider other)
        {
            other.transform.position += transform.forward * speed * Time.deltaTime;
        }
    }
}
