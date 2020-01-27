using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ProjectileController : MonoBehaviour
{

    public float speed;
    public float lifespan;
    private Rigidbody rb;
    [HideInInspector]
    public float damage;

    private void Awake()
    {
        rb = this.GetComponent<Rigidbody>();
    }
    // Start is called before the first frame update
    void Start()
    {
        rb.velocity = transform.forward * speed;
        Invoke("Death", lifespan);
    }

    public void Death()
    {
        CancelInvoke();
        Destroy(this.gameObject);
    }
    private void OnTriggerEnter(Collider other)
    {
        // Debug.Log(this.gameObject.name + ": " + other.name);
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            other.gameObject.SendMessage("ReceiveDamage", damage, SendMessageOptions.DontRequireReceiver);
        }
        Death();

    }
}
