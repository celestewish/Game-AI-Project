using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DisplayData : MonoBehaviour
{


    [Header("What counts as a hit? (Tags)")]
    public string[] countTags = { "Wall", "Enemy" };

    [Header("UI")]
    public TMP_Text counterTextTotal;   // Assign in Inspector (optional)
    public TMP_Text counterTextWall;
    public TMP_Text counterTextEnemy;

    private int collisionCount = 0;
    private int wallCollisionCount = 0;
    private int enemyCollisionCount = 0;

    private void Start()
    {
        UpdateUI();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (ShouldCountCollision(collision.gameObject.tag))
        {
            if (collision.gameObject.tag == "Walls")
            {
                wallCollisionCount++;
                
                collisionCount +=  wallCollisionCount;
                
                UpdateUI();
            }
            else if (collision.gameObject.tag == "Enemy")
            {
                enemyCollisionCount++;

                collisionCount += enemyCollisionCount;

                UpdateUI();
            }

            

        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (ShouldCountCollision(other.gameObject.tag))
        {
            if (other.gameObject.tag == "Walls")
            {
                wallCollisionCount++;

                collisionCount += wallCollisionCount;

                UpdateUI();
            }
            else if (other.gameObject.tag == "Enemy")
            {
                enemyCollisionCount++;

                collisionCount += enemyCollisionCount;

                UpdateUI();
            }
        }
    }

    private bool ShouldCountCollision(string otherTag)
    {
        // Check if the other object's tag is in the list of countable tags
        for (int i = 0; i < countTags.Length; i++)
        {
            if (otherTag == countTags[i])
                return true;
        }
        return false;
    }

    private void UpdateUI()
    {
        if (counterTextTotal != null || counterTextEnemy != null || counterTextWall != null)
        {
            counterTextTotal.text = "Hits: " + collisionCount;
            counterTextWall.text = "Wall: " + counterTextWall;
            counterTextEnemy.text = "Enemies: " + counterTextEnemy;
        }
    }
}


