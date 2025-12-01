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



    private int wallCollisionCount = 0;
    private int enemyCollisionCount = 0;
    private int collisionCount = 0; // or  => wallCollisionCount + enemyCollisionCount;


    private void Start()
    {
        UpdateUI();
    }

    private void OnCollisionEnter(Collision collision)
    {
            
            string tag = collision.gameObject.tag;

            if (!ShouldCountCollision(tag))
            return;
        
            if (tag == "Wall")
            {
                wallCollisionCount++;

                collisionCount = wallCollisionCount + enemyCollisionCount;

               
            }
            else if (tag == "Enemy")
            {
                enemyCollisionCount++;

                collisionCount = enemyCollisionCount + wallCollisionCount;

                
            }

        UpdateUI();


    }


    //if using OnTriggerEnter

    /*
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
    */

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
        if (counterTextTotal != null)
            counterTextTotal.text = "Hits: " + collisionCount;

        if (counterTextWall != null)
            counterTextWall.text = "Wall: " + wallCollisionCount;

        if (counterTextEnemy != null)
            counterTextEnemy.text = "Enemies: " + enemyCollisionCount;
    }
}


