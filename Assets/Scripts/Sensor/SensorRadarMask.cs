using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "SensorMaskRadar", menuName = "Sensors/SensorMaskRadar")]
public class SensorRadarMask : SensorBase
{
    public LayerMask targetMask;
    public override List<Vector3> DetectTargets(Transform turretTransform)
    {
        Collider[] hits = Physics.OverlapSphere(turretTransform.position, range, targetMask);
        return CalculateTargetPositions(hits.Select(hit => hit.gameObject).ToList(), turretTransform);
    }
}