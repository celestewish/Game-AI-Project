using UnityEngine;

public class PlayerAI : MonoBehaviour
{
    public Transform enemy;
    public float moveSpeed = 3f;
    public float runSpeed = 6f;
    public float detectionRange = 8f;
    public float wanderRadius = 5f;
    private Vector3 wanderDirection;
    private float wanderTimer = 0f;
    public float directionChangeInterval = 2f;

    public float avoidanceTime = 0.5f;
    private Rigidbody rb;
    private enum BotState { Wander, RunFromEnemy }
    private BotState currentState;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        PickNewWanderDirection();
    }

    //controls the different states
    void FixedUpdate()
    {
        float distToEnemy = Vector3.Distance(transform.position, enemy.position);

        if (distToEnemy < detectionRange)
            currentState = BotState.RunFromEnemy;
        else
            currentState = BotState.Wander;

        if (currentState == BotState.Wander)
        {
            wanderTimer -= Time.fixedDeltaTime;
            if (wanderTimer <= 0f)
                PickNewWanderDirection();

            MoveTowards(transform.position + wanderDirection, moveSpeed);
        }
        else if (currentState == BotState.RunFromEnemy)
        {
            Vector3 runDir = (transform.position - enemy.position).normalized;
            MoveTowards(transform.position + runDir, runSpeed);
        }
    }
    //title
    void PickNewWanderDirection()
    {
        wanderDirection = Random.onUnitSphere;
        wanderDirection.y = 0;
        wanderDirection.Normalize();
        wanderTimer = directionChangeInterval;
    }
    //movement
    void MoveTowards(Vector3 target, float speed)
    {
        Vector3 direction = (target - transform.position).normalized;
        rb.linearVelocity = direction * speed;
    }
}
