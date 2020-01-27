using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeController : MonoBehaviour
{
    private void OnCollisionEnter(Collision other)
    {
        //explota
        Destroy(this.gameObject);
        Collider[] colliders = Physics.OverlapSphere(transform.position, GrenadeLauncherManager.instance.grenadeRange, PlayerInfo.instance.whatIsBombable, QueryTriggerInteraction.Collide);
        foreach (var collider in colliders)
        {
            //el navmesh agent mueve sin rigidbody, asi que en general debe estar en kinematico para que no genere jitter
            //hay que ponerlo no kinematico cuando quieras que le afecten fisicas como explosioness
            collider.attachedRigidbody.isKinematic = false;
            collider.attachedRigidbody.AddExplosionForce(GrenadeLauncherManager.instance.grenadeKnockback, transform.position, GrenadeLauncherManager.instance.grenadeRange, GrenadeLauncherManager.instance.grenadeKnockbackHeightCorrection, ForceMode.Impulse);
            //bastante ineficiente, habria que ver por que el QueryTriggerInteraction.Collide no funciona
            collider.gameObject.SendMessage("ReceiveDamage", PlayerInfo.instance.oneShotEnemies ? 999999 : GrenadeLauncherManager.instance.grenadeDamage, SendMessageOptions.DontRequireReceiver);
        }
        colliders = Physics.OverlapSphere(transform.position, GrenadeLauncherManager.instance.grenadeRange, GrenadeLauncherManager.instance.whatIsDestroyable, QueryTriggerInteraction.Collide);
        foreach (var collider in colliders)
        {
            collider.gameObject.SetActive(false);
        }
    }

}