using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeLauncherManager : Weapon
{
    [Header("Grenade Launcher Settings")]
    public int magazineSize;
    public float reloadDelay;
    [Space(5)]
    public float grenadeRange;
    public float grenadeDamage;
    public float grenadeKnockback;
    public float grenadeKnockbackHeightCorrection;
    public float grenadeThrowPower;
    public float grenadeThrowHeight;
    [Header("Prefab Settings")]
    public GameObject grenadePrefab;

    public static GrenadeLauncherManager instance;
    private Camera cam;
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

            GameObject obj = Instantiate(grenadePrefab, this.transform.position + Vector3.up, Quaternion.identity, LevelGenerator.instance.temporalAssets);
            Rigidbody rb = obj.GetComponent<Rigidbody>();
            if (addPlayerVelocity)
                rb.velocity = PlayerInfo.instance.rb.velocity;
            rb.AddForce(grenadeThrowPower * (cam.transform.forward + grenadeThrowHeight * 0.01f * Vector3.up), ForceMode.Impulse);

        }
        return true;
    }


    // public override void SecondaryFire()
    // {

    // }

    public IEnumerator Reload()
    {
        isReloading = true;
        yield return new WaitForSeconds(reloadDelay);
        CurrentAmmo = magazineSize;
        isReloading = false;
    }

    public override bool CheckAmmo()
    {
        if (CurrentAmmo <= 0)
        {
            Debug.Log("reloading");
            if (!isReloading)
            {
                StartCoroutine(Reload());
            }
            return false;
        }
        return true;
    }

}