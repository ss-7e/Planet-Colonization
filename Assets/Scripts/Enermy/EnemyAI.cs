using NPBehave;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public float detectionRange = 4f;
    public float attackRange = 1f;
    public float moveSpeed = 3f;
    public Transform target;
    private void Start()
    {

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
    
}


