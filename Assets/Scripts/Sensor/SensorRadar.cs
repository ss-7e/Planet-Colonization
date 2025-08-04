using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "SensorRadar", menuName = "Sensors/SensorRadar")]
public class SensorRadar : SensorBase
{
    public override List<Vector3> DetectTargets(Transform turretTransform)
    {
        Collider[] hits = Physics.OverlapSphere(turretTransform.position, range);
        return CalculateTargetPositions(hits.Select(hit => hit.gameObject).ToList(), turretTransform);

    }
}