using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemiesInfo : MonoBehaviour
{
    public List<GameObject> availableEnemies;
    public static EnemiesInfo instance;
    public Transform enemyParent;
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
}
