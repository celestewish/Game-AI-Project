using UnityEngine;

public class TestManager : MonoBehaviour
{
    public int numberOfTrials = 500;
    int currentTrial = 0;

    float mapWidth = 6f;
    float mapDepth = 4.7f;

    public Transform player;
    public Transform enemy;

    public WalllControl[] walllControl;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (currentTrial == numberOfTrials)
        {
            Application.Quit();
        }
    }

    public void SpawnRandomPositions()
    {
        Vector3 playerPosition = new Vector3(Random.Range(0, mapWidth), 0.5f, Random.Range(0, mapDepth) + 2);
        // Change this V
        Vector3 enemyPosition = new Vector3(-Random.Range(0, mapWidth), 0.5f, -Random.Range(0, mapDepth) - 2);

        //player.position = playerPosition;
        //enemy.position = enemyPosition;
        for (int i = 0; i < walllControl.Length; i++)
        {
            walllControl[i].MoveWalls();
        }
        currentTrial++;
    }
}
