using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public int currentLevel;

    private void Awake()
    {
        if (!instance)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    private void Start()
    {
        currentLevel = 0;
        PlayerInfo.instance.transform.position = Vector3.zero;
        LevelGenerator.instance.Generate();
    }

    public void Restart()
    {
        Start();
    }

    public void NextLevel()
    {
        currentLevel++;
        PlayerInfo.instance.transform.position = Vector3.zero;
        LevelGenerator.instance.Generate();
    }

}
