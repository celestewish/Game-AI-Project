using UnityEngine;

public class NewAIModel : MonoBehaviour
{
    public float chaseDistance = 5f;
    public float investigateTime = 5f;
    public float episodeTimeLimit = 5f;
    public Transform player;

    private enum State { Idle, Chasing, Patrolling }
    private State currentState = State.Idle;

    private float episodeTimer = 0f;
    private float investigateTimer = 0f;
    private Vector3 lastKnownPlayerPos;

    // Reward-related variables
    private float totalReward = 0f;
    private float reward = 0f;
    private float wallPoints = 0f;
    private bool episodeActive = true;

    // Random roaming settings
    public float roamSpeed = 2f;
    public float roamTime = 2f;
    public float roamRadius = 5f;

    private Vector3 roamDirection;
    private float roamTimer;

    // Chase state variables
    public float chaseSpeed = 3f;

    // Patrol state variables
    public float patrolSpeed = 2f;
    public float patrolRadius = 5f;

    private Vector3 patrolDirection;
    private float patrolTimer;
    public float patrolMoveDuration = 2f;

    // Adjustment limits and rates
    public float maxRoamSpeed = 5f;
    public float minRoamSpeed = 1f;
    public float roamSpeedAdjustRate = 0.1f;

    public float maxChaseSpeed = 6f;
    public float minChaseSpeed = 2f;
    public float chaseSpeedAdjustRate = 0.2f;

    public float maxPatrolMoveDuration = 5f;
    public float minPatrolMoveDuration = 1f;
    public float patrolDurationAdjustRate = 0.1f;

    public float maxWallAvoidanceSensitivity = 1f;
    public float minWallAvoidanceSensitivity = 0.01f;
    public float wallAvoidanceAdjustRate = 0.001f;

    private float wallAvoidanceSensitivity = 0.05f;

    public float wallAvoidanceDistance = 1.0f;

    public TestManager testManager;



    void Start()
    {
        InitializeEpisode();
    }

    void Update()
    {
        if (!episodeActive) return;

        episodeTimer += Time.deltaTime;

        StateMachine();

        if (episodeTimer > episodeTimeLimit)
        {
            reward = -0.5f;
            totalReward += reward;
            EndEpisode();
        }
    }

    // Title
    void StateMachine()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        switch (currentState)
        {
            case State.Idle:
                Idle();
                if (IsPlayerInSight())
                    currentState = State.Chasing;
                break;

            case State.Chasing:
                Chasing();
                if (!IsPlayerInSight())
                {
                    if (player == null)
                    {
                        Debug.LogWarning("Player Transform is not assigned.");
                        return;
                    }
                    lastKnownPlayerPos = player.position;
                    investigateTimer = 0f;
                    currentState = State.Patrolling;
                }
                break;

            case State.Patrolling:
                Patrolling();
                if (investigateTimer > investigateTime)
                    currentState = State.Idle;
                break;
        }

        if (distanceToPlayer <= 6f)
        {
            reward = 0.2f;
            totalReward += reward;
        }
        if (distanceToPlayer <= 3f)
        {
            reward = 0.3f;
            totalReward += reward;
        }
    }
    // Title
    void PickNewRoamDirection()
    {
        Vector2 randomCircle = Random.insideUnitCircle * roamRadius;
        roamDirection = new Vector3(randomCircle.x, 0, randomCircle.y).normalized;
        roamTimer = roamTime;
    }
    // Idle roaming
    void Idle()
    {
        if (player == null)
        {
            Debug.LogWarning("Player Transform is not assigned.");
            return;
        }
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (IsWallAhead(roamDirection))
            PickNewRoamDirection();

        if (roamTimer <= 0f)
            PickNewRoamDirection();

        transform.position += roamDirection * roamSpeed * Time.deltaTime;
        roamTimer -= Time.deltaTime;

        if (distanceToPlayer < chaseDistance)
            currentState = State.Chasing;
    }
    // Chases player
    void Chasing()
    {
        if (player == null)
        {
            Debug.LogWarning("Player Transform is not assigned.");
            return;
        }
        Vector3 directionToPlayer = (player.position - transform.position).normalized;

        if (IsWallAhead(directionToPlayer))
        {
            PickNewRoamDirection();
            transform.position += roamDirection * roamSpeed * Time.deltaTime;
        }
        else
        {
            transform.position += directionToPlayer * chaseSpeed * Time.deltaTime;
        }

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        if (distanceToPlayer > chaseDistance)
            currentState = State.Patrolling;
    }
    // Looks for player
    void Patrolling()
    {
        investigateTimer += Time.deltaTime;

        if (IsWallAhead(patrolDirection))
            PickNewPatrolDirection();

        if (patrolTimer <= 0f)
            PickNewPatrolDirection();

        transform.position += patrolDirection * patrolSpeed * Time.deltaTime;
        patrolTimer -= Time.deltaTime;

        if (Vector3.Distance(transform.position, lastKnownPlayerPos) > patrolRadius)
            PickNewPatrolDirection();
    }
    // Picks a new direction based on a radius
    void PickNewPatrolDirection()
    {
        Vector2 randomCircle = Random.insideUnitCircle * patrolRadius;
        Vector3 target = lastKnownPlayerPos + new Vector3(randomCircle.x, 0, randomCircle.y);
        patrolDirection = (target - transform.position).normalized;
        patrolTimer = patrolMoveDuration;
    }

    // Title
    bool IsPlayerInSight()
    {
        float distance = Vector3.Distance(transform.position, player.position);
        return distance < chaseDistance;
    }
    // Checks to see if collided with wall
    void OnCollisionEnter(Collision collision)
    {
        if (collision.transform == player)
        {
            reward = 0.5f;
            totalReward += reward;
            EndEpisode();
        }
        else if (collision.gameObject.CompareTag("Wall"))
        {
            if (wallPoints < -0.5f)
            {
                Debug.Log(wallPoints);
                reward = -0.0005f * wallAvoidanceSensitivity * 10f;
                totalReward += reward;
                wallPoints += reward;
            }
        }
    }
    // Title
    bool IsWallAhead(Vector3 direction)
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, direction, out hit, wallAvoidanceDistance))
        {
            if (hit.collider.CompareTag("Wall"))
                return true;
        }

        float scaledAvoidanceDistance = wallAvoidanceDistance * wallAvoidanceSensitivity;

        return false;
    }

    // Gives the bot his final rewards
    void EndEpisode()
    {
        totalReward = Mathf.Clamp(totalReward, -1f, 5f);

        AdaptBehavior();

        SavePerformanceData();

        RestartEpisode();
    }
    // Title
    void AdaptBehavior()
    {
        float adjustmentFactor = Mathf.Abs(totalReward);

        //if doing well try to become easier
        if (totalReward > 0)
        {
            roamSpeed -= adjustmentFactor * roamSpeedAdjustRate * 0.5f;
            chaseSpeed -= adjustmentFactor * chaseSpeedAdjustRate * 0.5f;
            patrolMoveDuration -= adjustmentFactor * patrolDurationAdjustRate * 0.5f;
            wallAvoidanceSensitivity -= adjustmentFactor * wallAvoidanceAdjustRate * 0.5f;
        }
        //if doing poorly become harder to escape
        else
        {
            roamSpeed += adjustmentFactor * roamSpeedAdjustRate;
            chaseSpeed += adjustmentFactor * chaseSpeedAdjustRate;
            patrolMoveDuration += adjustmentFactor * patrolDurationAdjustRate;
            wallAvoidanceSensitivity += adjustmentFactor * wallAvoidanceAdjustRate;
        }

        // Clamp all parameters to their valid ranges
        roamSpeed = Mathf.Clamp(roamSpeed, minRoamSpeed, maxRoamSpeed);
        chaseSpeed = Mathf.Clamp(chaseSpeed, minChaseSpeed, maxChaseSpeed);
        patrolMoveDuration = Mathf.Clamp(patrolMoveDuration, minPatrolMoveDuration, maxPatrolMoveDuration);
        wallAvoidanceSensitivity = Mathf.Clamp(wallAvoidanceSensitivity, minWallAvoidanceSensitivity, maxWallAvoidanceSensitivity);

        Debug.Log($"Behavior adapted: roamSpeed={roamSpeed}, chaseSpeed={chaseSpeed}, patrolDuration={patrolMoveDuration}, wallAvoidance={wallAvoidanceSensitivity}");
    }


    void SavePerformanceData()
    {
        // Rn is on player prefs but we can replace this later so we can get data from like an excel graph
        PlayerPrefs.SetFloat("LastReward", totalReward);
        PlayerPrefs.Save();
        Debug.Log("Performance saved: " + totalReward);
    }
    // Restarts episode
    void RestartEpisode()
    {
        totalReward = 0f;
        reward = 0f;
        wallPoints = 0f;
        episodeTimer = 0f;
    }
    // Resets all stats
    void InitializeEpisode()
    {
        currentState = State.Idle;
        episodeTimer = 0f;
        totalReward = 0f;
        reward = 0f;
        wallPoints = 0f;
        episodeActive = true;
        roamTimer = 0f;
        roamDirection = Vector3.zero;
        patrolTimer = 0f;
        patrolDirection = Vector3.zero;
        investigateTimer = 0f;
    }
}

