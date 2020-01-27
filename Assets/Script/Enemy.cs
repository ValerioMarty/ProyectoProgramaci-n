using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]

public class Enemy : MonoBehaviour
{

    [Header("Stats")]
    [SerializeField]
    private float hp;
    public float damage;
    public float attackRate;
    public float speed;
    public float rotationSpeed;
    [Space(5)]
    [Range(0, 100)]
    [Tooltip("Varianza en porcentaje de las estadísticas")]
    public float percentualVariance;
    [Space(5)]
    [Range(0, 500)]
    [Tooltip("0 significa que no aumenta, 50 es un aumento del 50% por piso")]
    public int percentualMultiplierPerFloor;

    [Header("Behaviour")]
    public LayerMask whatBlocksVisionPlusPlayer;
    public float searchRange;
    public float searchRate;
    [Tooltip("Distancia a la que deja de perseguir al jugador y ataca")]
    [SerializeField]
    private float killZone;
    public bool isSearching;
    public bool playerOnSight;
    public bool overrideTarget;

    [Header("Other")]
    public float knockedDuration;
    public float groundCheckDistance = 0.1f;
    public LayerMask whatIsGround;

    protected NavMeshAgent agent;
    protected Rigidbody rb;
    protected CapsuleCollider coll;

    public float Hp
    {
        get => hp;
        set
        {
            if (value <= 0)
            {
                value = 0;
                this.Death();
            }
            hp = value;

        }
    }

    public bool OverrideTarget
    {
        get => overrideTarget;
        set
        {
            if (value)
            {
                if (agent.isOnNavMesh)
                    agent.destination = (PlayerInfo.instance.gameObject.transform.position);
            }
            else
            {
                if (agent.isOnNavMesh)
                    agent.destination = transform.position;
            }
            overrideTarget = value;
        }
    }

    public float KillZone
    {
        get => killZone;
        set
        {
            killZone = value;
            agent.stoppingDistance = value;
        }
    }

    private void Awake()
    {
        rb = this.GetComponent<Rigidbody>();
        agent = GetComponent<NavMeshAgent>();
        coll = GetComponent<CapsuleCollider>();
    }

    void Start()
    {
        agent.stoppingDistance = KillZone;
        //el navmesh agent mueve sin rigidbody, asi que en general debe estar en kinematico para que no genere jitter
        //hay que ponerlo no kinematico cuando quieras que le afecten fisicas como explosiones
        rb.isKinematic = true;

        if (OverrideTarget)
        {
            if (agent.isOnNavMesh)
                agent.destination = PlayerInfo.instance.gameObject.transform.position;
        }

        isSearching = true;
        StartCoroutine(SearchForPlayer());

        RandomizeStats(ref hp);
        RandomizeStats(ref damage);
        RandomizeStats(ref speed);
    }

    //al pasar por referencia puedo modificar una variable definida fuera
    public void RandomizeStats(ref float stat)
    {
        stat = Random.Range(stat - stat * percentualVariance / 100f, stat + stat * percentualVariance / 100f) * (1 + percentualMultiplierPerFloor / 100f * GameManager.instance.currentLevel);
    }

    public virtual void Attack()
    {
        // Debug.Log("Attack");
    }

    public void ReceiveDamage(float damage)
    {
        StartCoroutine(Knocked());
        // Debug.Log("received damage");
        Hp -= damage;
    }

    public IEnumerator Knocked()
    {
        agent.enabled = false;
        yield return new WaitForSeconds(knockedDuration);
        while (!IsGrounded())
        {
            yield return null;
        }
        rb.isKinematic = true;
        agent.enabled = true;
    }

    public bool IsGrounded()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, coll.height / 2 + groundCheckDistance, whatIsGround, QueryTriggerInteraction.Ignore))
        {
            return true;
        }
        else return false;
    }

    public virtual void Death()
    {
        Destroy(this.gameObject);
    }

    public IEnumerator SearchForPlayer()
    {
        while (isSearching)
        {
            RaycastHit hit;
            Debug.DrawRay(transform.position, PlayerInfo.instance.transform.position - transform.position, Color.green);
            if (Physics.Raycast(transform.position, PlayerInfo.instance.transform.position - transform.position, out hit, searchRange, whatBlocksVisionPlusPlayer, QueryTriggerInteraction.Ignore))
            {
                if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Player"))
                {
                    if (agent.isOnNavMesh)
                        agent.destination = hit.point;

                    isSearching = false;
                    playerOnSight = true;
                    StartCoroutine(TrackPlayer());
                }
            }
            yield return new WaitForSeconds(searchRate);
        }
    }

    //Sugerencia: checkear que ha llegado a una determinada distancia del enemigo y atacar, asi lo que distingue ranger y melee es la distancia y el tipo de ataque
    public IEnumerator TrackPlayer()
    {
        StartCoroutine(UpdatePlayerPosition());
        StartCoroutine(AttackCycle());
        while (playerOnSight)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, PlayerInfo.instance.transform.position - transform.position, out hit, searchRange, whatBlocksVisionPlusPlayer, QueryTriggerInteraction.Ignore))
            {
                if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Player") || hit.collider.gameObject.layer == LayerMask.NameToLayer("Cover"))
                {
                    //tambien checkeo con cover para que te puedas cubrir y no le den la vuelta a las rocas
                    //no hace falta actualizar la posicion del jugador, esta funcion es solo para comprobar cuando lo pierde de vista
                    //dejo esto aqui por si es util mas adelante
                }
                else
                {
                    isSearching = true;
                    playerOnSight = false;
                    StartCoroutine(SearchForPlayer());
                }

            }
            else
            {
                isSearching = true;
                playerOnSight = false;
                StartCoroutine(SearchForPlayer());
            }
            yield return new WaitForSeconds(searchRate);
        }
    }

    //esta funcion actualiza de forma eficiente la posicion del jugador
    public IEnumerator UpdatePlayerPosition()
    {
        while (playerOnSight)
        {
            //se lo pongo a un vector primero para no pasar la posicion por referencia
            Vector3 position = PlayerInfo.instance.transform.position;
            Vector3 direction = (position - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(Vector3.right * direction.x + Vector3.forward * direction.z);
            // new Vector3(direction.x, 0, direction.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);

            if (agent.isOnNavMesh)
                agent.destination = position;
            yield return null;
        }
    }

    public IEnumerator AttackCycle()
    {
        while (playerOnSight)
        {
            if (agent.pathStatus == NavMeshPathStatus.PathComplete)
            {
                Attack();
            }
            yield return new WaitForSeconds(attackRate);
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        // Debug.Log(other.name);
        if (other.gameObject.layer == LayerMask.NameToLayer("OutOfBounds"))
        {
            Death();
        }
    }
}