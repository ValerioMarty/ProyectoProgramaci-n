using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Melee : Enemy
{
    [Header("Attack Settings")]
    public float meleeRange;
    public float meleeWidth;
    Vector3 halfExtents;

    public override void Attack()
    {
        base.Attack();
        halfExtents = new Vector3(meleeWidth / 2f, meleeWidth / 2f, meleeRange / 2f);
        Collider[] colliders = Physics.OverlapBox(this.transform.position + transform.forward * (coll.radius + meleeRange / 2f), halfExtents, this.transform.rotation, LayerMask.GetMask("Player"), QueryTriggerInteraction.Collide);
        Debug.DrawLine(this.transform.position + transform.forward * (coll.radius), this.transform.position + transform.forward * (coll.radius + meleeRange));
        foreach (var item in colliders)
        {
            item.gameObject.SendMessage("ReceiveDamage", damage, SendMessageOptions.DontRequireReceiver);
        }
    }
    private void OnDrawGizmosSelected()
    {
        // Gizmos.DrawCube(this.transform.position + transform.forward * (coll.radius + meleeRange / 2f), halfExtents * 2f);
    }
}