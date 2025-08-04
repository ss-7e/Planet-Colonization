using UnityEngine;
using System.Collections.Generic;

public class GroupEnemyBehavior : MonoBehaviour
{
    public float speed = 3f;
    public float neighborRadius = 5f;
    public float cohesionWeight = 1.0f;
    public float separationWeight = 1.5f;
    public float alignmentWeight = 1.0f;
    public float wanderWeight = 1.0f;
    protected EnemyGroup group;

    private Vector3 velocity;

    private void Start()
    {

    }

    void Update()
    {
        Vector3 force = Vector3.zero;
        if (group != null)
        {
            force += Cohesion(group.enemyPrefab) * cohesionWeight;
        }
        force += Separation(FindNeighbors()) * separationWeight;


        //force += Wander() * wanderWeight;
        force.y = 0f;
        velocity += force;
        velocity = Vector3.ClampMagnitude(velocity, speed);
        transform.position += velocity * Time.deltaTime;
    }
    public void SetGroup(EnemyGroup group)
    {
        this.group = group;
    }
    public EnemyGroup GetGroup()
    {
        return group;
    }
    List<GameObject> FindNeighbors()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, neighborRadius);
        List<GameObject> neighbors = new List<GameObject>();

        foreach (var col in hitColliders)
        {
            if (col.gameObject != this.gameObject && col.CompareTag("Enemy"))
                neighbors.Add(col.gameObject);
        }

        return neighbors;
    }

    Vector3 Cohesion(List<GameObject> neighbors)
    {
        Vector3 center = Vector3.zero;
        foreach (var n in neighbors)
            center += n.transform.position;

        center /= neighbors.Count;
        return (center - transform.position);
    }

    Vector3 Separation(List<GameObject> neighbors)
    {
        Vector3 avoid = Vector3.zero;
        foreach (var n in neighbors)
        {
            Vector3 diff = transform.position - n.transform.position;
            if (diff.sqrMagnitude < 0.01f) { avoid += diff * 120; continue; }
            avoid += diff / diff.sqrMagnitude;
        }

        return avoid;
    }
    Vector3 Alignment(List<GameObject> neighbors)
    {
        Vector3 avgVelocity = Vector3.zero;
        foreach (var n in neighbors)
        {
            GroupEnemyBehavior other = n.GetComponent<GroupEnemyBehavior>();
            if (other != null)
            {
                avgVelocity += other.velocity;
            }
        }

        avgVelocity /= neighbors.Count;
        return avgVelocity.normalized;
    }
}
