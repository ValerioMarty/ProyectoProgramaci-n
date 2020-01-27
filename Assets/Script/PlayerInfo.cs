using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInfo : MonoBehaviour
{
    [Header("PlayerSettings")]
    [SerializeField]
    private float hp;

    [Header("Destruction Settings")]
    public LayerMask whatIsBombable;

    [Header("Debug")]
    public bool inmortal;
    public bool oneShotEnemies;
    public Rigidbody rb;
    public static PlayerInfo instance;

    public float Hp
    {
        get => hp;
        set
        {
            if (value <= 0)
            {
                // value = 0;
                this.Death();
                value = 10;
            }
            hp = value;

        }
    }

    private void Awake()
    {
        if (!instance)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    public void ReceiveDamage(float damage)
    {
        // Debug.Log("received damage");
        if (!inmortal)
            Hp -= damage;
    }

    public void Death()
    {
        Debug.Log("Has muerto");
        GameManager.instance.Restart();
        Hp = 10;

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("OutOfBounds"))
        {
            Death();
        }
    }
}
