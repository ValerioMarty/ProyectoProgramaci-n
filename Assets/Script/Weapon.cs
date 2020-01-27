using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public bool addPlayerVelocity;
    [Header("Ammo")]
    [SerializeField]
    private int currentAmmo;
    public float attackRate;
    [Header("Effect Settings")]
    public LayerMask whatIsDestroyable;

    protected bool isReloading;
    protected bool isOnAttackDelay;

    public int CurrentAmmo
    {
        get => currentAmmo;
        set
        {
            if (value <= 0)
            {
                value = 0;
                CheckAmmo();
            }
            currentAmmo = value;

        }
    }


    //true para que todo va sin problemas, false para que no ataque
    public virtual bool PrimaryFire()
    {
        if (!CheckAmmo())
        {
            return false;
        }
        if (isOnAttackDelay)
        {
            return false;
        }
        CurrentAmmo--;
        StartCoroutine(AttackDelay());
        return true;
    }

    public virtual bool SecondaryFire()
    {
        if (!CheckAmmo())
        {
            return false;
        }

        if (isOnAttackDelay)
        {
            return false;
        }
        CurrentAmmo--;
        StartCoroutine(AttackDelay());
        return true;
    }

    //es como si fuera abstracta, hay que hacer override siempre
    public virtual bool CheckAmmo()
    {
        return true;
    }

    public IEnumerator AttackDelay()
    {
        isOnAttackDelay = true;
        yield return new WaitForSeconds(attackRate);
        isOnAttackDelay = false;
    }
}
