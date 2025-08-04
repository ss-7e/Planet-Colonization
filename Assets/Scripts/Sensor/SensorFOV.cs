using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "SensorOptical", menuName = "Sensors/SensorOptical")]
public class SensorFOV : SensorBase
{
    public float fieldOfView = 45f;

    public override List<Vector3> DetectTargets(Transform turretTransform)
    {
        Collider[] hits = Physics.OverlapSphere(turretTransform.position, range);
        List<GameObject> visibleTargets = new List<GameObject>();

        foreach (var hit in hits)
        {
            Vector3 dirToTarget = (hit.transform.position - turretTransform.position).normalized;
            if (Vector3.Angle(turretTransform.forward, dirToTarget) < fieldOfView / 2)
            {
                if (!Physics.Linecast(turretTransform.position, hit.transform.position))
                {
                    visibleTargets.Add(hit.gameObject);
                }
            }
        }
        return CalculateTargetPositions(visibleTargets, turretTransform);
    }
}