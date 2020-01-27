using System.Collections;
using System.Collections.Generic;
using UnityEngine;




public class BombManager : Weapon
{
    [Header("Bomb Settings")]
    public float bombTimer;
    public float BombRange { get => GrenadeLauncherManager.instance.grenadeRange * (2 / 3f); }
    public float bombDamage;
    public float bombKnockback;
    public float bombKnockbackHeightCorrection;
    public float bombThrowPower;
    public float bombThrowHeight;

    private Camera cam;

    [Header("Prefab Settings")]
    public GameObject bombPrefab;

    public static BombManager instance;



    private void Awake()
    {
        if (!instance)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
            return;
        }
    }

    private void Start()
    {
        cam = Camera.main;
    }

    public override bool PrimaryFire()
    {

        if (base.PrimaryFire())
        {
            GameObject obj = Instantiate(bombPrefab, this.transform.position + Vector3.up, Quaternion.identity, LevelGenerator.instance.temporalAssets);
            Rigidbody rb = obj.GetComponent<Rigidbody>();
            if (addPlayerVelocity)
                rb.velocity = PlayerInfo.instance.rb.velocity;
            rb.AddForce(bombThrowPower * (cam.transform.forward + bombThrowHeight * 0.01f * Vector3.up), ForceMode.Impulse);
        }
        //esto ya no sirve de nada
        return true;
    }

    public override bool SecondaryFire()
    {
        if (base.SecondaryFire())
        {
            Instantiate(bombPrefab, this.transform.position, Quaternion.identity, LevelGenerator.instance.temporalAssets);
        }
        return true;
    }

    public override bool CheckAmmo()
    {
        if (CurrentAmmo <= 0)
        {
            Debug.Log("no ammo");
            return false;
        }
        return true;
    }

}
