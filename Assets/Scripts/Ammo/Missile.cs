using UnityEngine;

//public class Missile : MonoBehaviour
//{
//    private Transform target;
 
//    public int damage = 50;
//    public float damageRadius = 5f;
//    public float buringTime = 2f;
//    public float maxAcceleration = 10f;
//    public float speed = 20f;
//    public void SetTarget(Transform _target)
//    {
//        target = _target;
//    }

//    private void Update()
//    {
//        Vector3 dir = target.position - transform.position;
//        float distanceThisFrame = speed * Time.deltaTime;

//        if (dir.magnitude <= distanceThisFrame)
//        {
//            HitTarget();
//            return;
//        }
//        transform.rotation = Quaternion.LookRotation(dir);
//        transform.Translate(dir.normalized * distanceThisFrame, Space.World);
//    }

//    private void HitTarget()
//    {

//        Collider[] colliders = Physics.OverlapSphere(transform.position, damageRadius);
//        foreach (Collider collider in colliders)
//        {
//            if (collider.TryGetComponent<IDamageable>(out IDamageable damageable))
//            {
//                damageable.TakeDamage(damage);
//            }
//        }
//    }
//}
