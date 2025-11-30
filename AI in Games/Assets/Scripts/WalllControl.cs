using UnityEngine;
using System.Collections;

public class WalllControl : MonoBehaviour
{
    public int randomAngle;
    public int randomNum;
    public int min = 0;
    public int max = 180;
    void Start()
    {
        MoveWalls();
    }
    public void WallVisible()
    {
        randomNum = Random.Range(min, max);
        //if even set false
        if (randomNum % 2 == 0)
        {
            gameObject.SetActive(false);
        }
        else
        {
            gameObject.SetActive(true);
        }

    }
    public void MoveWalls()
    {
        WallVisible();
        randomAngle = Random.Range(min, max);
        //random wall y position at game start   
        transform.rotation = Quaternion.Euler(new Vector3(0, randomAngle, 0));
    }
}
