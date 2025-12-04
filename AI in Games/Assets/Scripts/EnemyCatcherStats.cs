using TMPro;
using UnityEngine;

public class EnemyCatcherStats : MonoBehaviour
{
    [Header("Detection Settings")]
    [Tooltip("Tag used on the Player GameObject.")]
    public string playerTag = "Player";

    [Tooltip("Use trigger colliders (OnTriggerEnter) or normal colliders (OnCollisionEnter)?")]
    public bool useTrigger = true;

    [Header("Stats")]
    [Tooltip("How many times the enemy has caught the player.")]
    public int timesPlayerCaught = 0;

    [Tooltip("Time (in seconds since scene start) when the player was last caught.")]
    public float lastCatchTime = -1f;


    [Header("Optional UI")]
    public TMP_Text caughtCounterText;   // Assign in Inspector (optional)
    public TMP_Text lastCatchTimeText;   // Assign in Inspector (optional)



    private void Start()
    {
        UpdateUI();
    }


    // Use this if your enemy/player colliders are set as triggers
    private void OnTriggerEnter(Collider other)
    {
        if (!useTrigger) return;
        TryRegisterCatch(other.gameObject);
    }

    // Use this if you're using normal colliders (non-trigger)
    private void OnCollisionEnter(Collision collision)
    {
        if (useTrigger) return;
        TryRegisterCatch(collision.gameObject);
    }

    private void TryRegisterCatch(GameObject other)
    {
        if (!other.CompareTag(playerTag)) return;

        timesPlayerCaught++;
        lastCatchTime = Time.time;

        Debug.Log($"Player caught! Total times caught: {timesPlayerCaught}");

        UpdateUI();

        
    }

    private void UpdateUI()
    {
        if (caughtCounterText != null)
        {
            caughtCounterText.text = $"Caught: {timesPlayerCaught}";
        }

        if (lastCatchTimeText != null)
        {
            if (lastCatchTime >= 0f)
                lastCatchTimeText.text = $"Last Catch Time: {lastCatchTime:F1}s";
            else
                lastCatchTimeText.text = "Last Catch Time: N/A";
        }
    }
}
