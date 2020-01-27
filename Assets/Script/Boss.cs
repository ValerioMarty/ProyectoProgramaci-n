using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : Melee
{
    [Header("Boss Settings")]
    public GameObject bossChestPrefab;

    public override void Death()
    {
        base.Death();
        GameObject portal = Instantiate(LevelGenerator.instance.portalPrefab, this.transform.position, transform.rotation, LevelGenerator.instance.temporalAssets);
        GameObject[] chests = new GameObject[3];
        chests[0] = Instantiate(bossChestPrefab, this.transform.position + transform.forward * 2f, transform.rotation, LevelGenerator.instance.temporalAssets);
        chests[1] = Instantiate(bossChestPrefab, this.transform.position + transform.forward * 2f + transform.right * 2f, transform.rotation, LevelGenerator.instance.temporalAssets);
        chests[1] = Instantiate(bossChestPrefab, this.transform.position + transform.forward * 2f - transform.right * 2f, transform.rotation, LevelGenerator.instance.temporalAssets);
    }
}
