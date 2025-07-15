using NPBehave;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public float detectionRange = 10f;
    public float attackRange = 2f;
    public float moveSpeed = 3f;
    public Transform playerTransform;
    private Vector3 initialPosition;
    private void Start()
    {
        initialPosition = transform.position;
    }
    private void Update()
    {
        
    }
}

public class EnemyBTAI : EnemyAI
{
    private Root tree;

    private void Awake()
    {
        tree = CreateBehaviorTree();
        if (tree == null)
        {
            Debug.LogError("Behavior tree is not created. Please implement CreateBehaviorTree method.");
            return;
        }
        tree.Start();
    }
    protected virtual Root CreateBehaviorTree()
    {
        Root root = new Root(new Selector(
            new Sequence(
                new Wait(0.5f)
            )
        ));
        return root;
    }
    private void TurnBack()
    {

    }
}


