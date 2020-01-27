using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ranger : Enemy
{
    [Header("Attack Settings")]
    public GameObject projectilePrefab;

    public override void Attack()
    {
        base.Attack();
        GameObject projectile = Instantiate(projectilePrefab, transform.position + transform.forward, transform.rotation, LevelGenerator.instance.temporalAssets);
        projectilePrefab.GetComponent<ProjectileController>().damage = damage;
    }
}