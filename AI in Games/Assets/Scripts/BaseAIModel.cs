using UnityEngine;

public class BaseAIModel : MonoBehaviour
{
    public float chaseDistance = 5f;
    public float investigateTime = 5f;
    public float episodeTimeLimit = 30f;
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
            // Penalize for running out of time
            reward = -0.5f;
            totalReward += reward;
            EndEpisode();
        }
    }

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

        // Distance-based rewards (run once, per new minimum distance reached)
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

    void Idle()
    {
        // Roam randomly in general area
        // Implement simple random movement if desired

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        if (distanceToPlayer < chaseDistance)
            currentState = State.Chasing;
    }

    void Chasing()
    {
        // Move toward the player using NavMesh or direct movement
        // Implement your pathfinding here (e.g., NavMeshAgent)

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        if (distanceToPlayer > chaseDistance)
            currentState = State.Patrolling;
    }

    void Patrolling()
    {
        // Return to last known player position and explore the area
        investigateTimer += Time.deltaTime;
        // Implement random wandering or systematic search
    }

    // Simple player detection (can be as advanced as you want)
    bool IsPlayerInSight()
    {
        float distance = Vector3.Distance(transform.position, player.position);
        // Use Raycasting for obstacles if needed
        return distance < chaseDistance;
    }

    // Collision handling
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
                reward = -0.0005f;
                totalReward += reward;
                wallPoints += reward;
            }
        }
    }

    void EndEpisode()
    {
        // Clamp reward between -1 and 1
        totalReward = Mathf.Clamp(totalReward, -1f, 1f);

        episodeActive = false;

        // Save stats here - e.g., using PlayerPrefs, ScriptableObject, or file I/O
        SavePerformanceData();

        // Prepare for next run (could be reload scene or reposition agent)
        Invoke("RestartEpisode", 1.0f);
    }

    void SavePerformanceData()
    {
        // Example: PlayerPrefs, replace with your own system as needed
        PlayerPrefs.SetFloat("LastReward", totalReward);
        PlayerPrefs.Save();
        Debug.Log("Performance saved: " + totalReward);
    }

    void RestartEpisode()
    {
        // Reset variables and state, respawn as needed
        InitializeEpisode();

        // Respawn logic: Move agent/player, reset timers, etc.
    }

    void InitializeEpisode()
    {
        currentState = State.Idle;
        episodeTimer = 0f;
        totalReward = 0f;
        reward = 0f;
        wallPoints = 0f;
        episodeActive = true;
    }
}
