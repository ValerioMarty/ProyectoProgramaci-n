using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombController : MonoBehaviour
{
    public Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Timer());
    }

    IEnumerator Timer()
    {
        yield return new WaitForSeconds(BombManager.instance.bombTimer);
        //explota
        Destroy(this.gameObject);
        Collider[] colliders = Physics.OverlapSphere(transform.position, BombManager.instance.BombRange, PlayerInfo.instance.whatIsBombable, QueryTriggerInteraction.Collide);
        foreach (var collider in colliders)
        {
            //el navmesh agent mueve sin rigidbody, asi que en general debe estar en kinematico para que no genere jitter
            //hay que ponerlo no kinematico cuando quieras que le afecten fisicas como explosioness
            collider.attachedRigidbody.isKinematic = false;
            collider.attachedRigidbody.AddExplosionForce(BombManager.instance.bombKnockback, transform.position, BombManager.instance.BombRange, BombManager.instance.bombKnockbackHeightCorrection, ForceMode.Impulse);
            //bastante ineficiente, habria que ver por que el QueryTriggerInteraction.Collide no funciona
            collider.gameObject.SendMessage("ReceiveDamage", PlayerInfo.instance.oneShotEnemies ? 999999 : BombManager.instance.bombDamage, SendMessageOptions.DontRequireReceiver);
        }
        colliders = Physics.OverlapSphere(transform.position, BombManager.instance.BombRange, BombManager.instance.whatIsDestroyable, QueryTriggerInteraction.Collide);
        foreach (var collider in colliders)
        {
            collider.gameObject.SetActive(false);
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        rb.isKinematic = true;
        rb.useGravity = false;
        this.transform.SetParent(other.transform);
    }

}
