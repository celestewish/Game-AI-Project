using System;
using UnityEngine;

public class TestManager : MonoBehaviour
{
    public int numberOfTrials = 500;
    int currentTrial = 0;

    public float minimumDistance = 4f;

    float mapWidth;
    float mapDepth;

    public Transform player;
    public Transform enemy;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameObject plane = GameObject.Find("Plane");

        Transform planeTransform = plane.GetComponent<Transform>();
        MeshFilter meshFilter = plane.GetComponent<MeshFilter>();

        Vector3 bound = meshFilter.mesh.bounds.size;

        mapWidth = (bound.x * planeTransform.localScale.x) - 4f;
        mapDepth = (bound.z * planeTransform.localScale.z) - 4f;

        Debug.Log("mapwidth: " + mapWidth);
        Debug.Log("mapdepth: " + mapDepth);

    }

    // Update is called once per frame
    void Update()
    {
    }

    public void SpawnRandomPositions()
    {
        float rangeX = mapWidth / 2;
        float rangeZ = mapDepth / 2;

        Math.Floor(rangeX);
        Math.Floor(rangeZ);

        Debug.Log("RangeX: " + rangeX);
        Debug.Log("RangeZ: " + rangeZ);


        float playerX = UnityEngine.Random.Range(-rangeX, rangeX);
        float playerZ = UnityEngine.Random.Range(-rangeZ, rangeZ);
        player.position = new Vector3(playerX, 0.5f, playerZ);

        int count = 0;

        do
        {
            float enemyX = UnityEngine.Random.Range(-rangeX, rangeX);
            float enemyZ = UnityEngine.Random.Range(-rangeZ, rangeZ);
            enemy.position = new Vector3(enemyX, 0.5f, enemyZ);
            count++;
        }
        while(Vector3.Distance(player.position, enemy.position) < minimumDistance && count < 50);

        // player.position = new Vector3(20f, 0.5f, 20f);
        // enemy.position = new Vector3(-20f, 0.5f, -20f);


    }

}
