using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockEnermy : MonoBehaviour
{
    public float rotationSpeed = 10f;
    public void PointAtTarget(Transform enermy)
    {
        Vector3 targetDir = enermy.position - transform.position;
        targetDir.y = 0;
        float step = rotationSpeed * Time.deltaTime;
        if (targetDir.magnitude < 0.1f)
        {
            return;
        }
        Vector3 newDir = Vector3.RotateTowards(transform.forward, targetDir, step, 0.0f);
        transform.rotation = Quaternion.LookRotation(newDir);
    }
}
