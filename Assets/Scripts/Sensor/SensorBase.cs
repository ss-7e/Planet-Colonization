using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public abstract class SensorBase : ScriptableObject
{
    public float range = 20f;
    public float updateInterval = 0.5f;
    public float accuracy = 0.1f; 
    public abstract List<Vector3> DetectTargets(Transform turretTransform);

    /// <summary>
    /// Processes a list of target GameObjects to extract unique, rounded world positions,
    /// sorted by distance to the given turret transform.
    /// </summary>
    /// <param name="targets">List of target GameObjects.</param>
    /// <param name="turretTransform">The transform of the turret (used as distance reference).</param>
    /// <returns>
    /// A list of rounded, unique Vector3 positions, sorted from nearest to farthest relative to the turret.
    /// </returns>
    protected List<Vector3> CalculateTargetPositions(List<GameObject> targets, Transform turretTransform)
    {
        List<Vector3> targetPositions = new List<Vector3>();
        if (accuracy < 0.0001f)
        {
            accuracy = 0.01f; // fallback default
            Debug.LogWarning("Accuracy too small. Falling back to 0.1");
        }
        foreach (GameObject target in targets)
        {
            Vector3 targetPosition = target.transform.position;
            float x = Mathf.Round(targetPosition.x / accuracy) * accuracy;
            float y = Mathf.Round(targetPosition.y / accuracy) * accuracy;
            float z = Mathf.Round(targetPosition.z / accuracy) * accuracy;
            Vector3 roundedPosition = new Vector3(x, y, z);
            if (!targetPositions.Contains(roundedPosition))
            {
                targetPositions.Add(roundedPosition);
            }
        }
        if (targetPositions.Count > 0)
        {
            targetPositions = targetPositions.OrderBy(p => Vector3.Distance(turretTransform.position, p)).ToList();
        }
        return targetPositions;
    }
}



[CreateAssetMenu(fileName = "SensorMaskOptical", menuName = "Sensors/SensorMaskOptical")]
public class SensorMaskOptical : SensorBase
{
    public LayerMask targetMask;
    public float fieldOfView = 45f;
    public override List<Vector3> DetectTargets(Transform turretTransform)
    {
        Collider[] hits = Physics.OverlapSphere(turretTransform.position, range, targetMask);
        List<GameObject> visibleTargets = new List<GameObject>();
        foreach (var hit in hits)
        {
            Vector3 dirToTarget = (hit.transform.position - turretTransform.position).normalized;
            if (Vector3.Angle(turretTransform.forward, dirToTarget) < fieldOfView / 2)
            {
                if (!Physics.Linecast(turretTransform.position, hit.transform.position, targetMask))
                {
                    visibleTargets.Add(hit.gameObject);
                }
            }
        }
        return CalculateTargetPositions(visibleTargets, turretTransform);
    }
}
