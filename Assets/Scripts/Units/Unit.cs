using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public enum UnitType
{
    Farmer = 0,
    Knight = 1,
    GoldKnight = 2,
    Golem = 3,
    Archer = 4,
    Wizard = 5,
    Catapult = 6,
    ALL = 7 //For debuff purposes
}

public class Unit : MonoBehaviour
{

    [Header("Unit Stats")]
    [SerializeField] private float baseMaximumHealth;
    private float MaximumHealth { get { if (isAlly) { return baseMaximumHealth -= UnitTrustManager.Instance.GetPlayerHealth(unitType); } else { return baseMaximumHealth; } } }
    public float currentHealth;
    [SerializeField]
    private float baseMoveSpeed = 4f;
    private float moveSpeed { get { if (isAlly) { return baseMoveSpeed *= UnitTrustManager.Instance.GetPlayerMoveSpeed(unitType); } else { return baseMoveSpeed; } } }
    [SerializeField]
    private float baseAttackDamage = 4f;
    private float attackDamage { get { if (isAlly) { return Mathf.Clamp(baseAttackDamage -= UnitTrustManager.Instance.GetPlayerDamage(unitType),1,Mathf.Infinity); } else { return baseAttackDamage += UnitTrustManager.Instance.GetEnemyUnitDamage(UnitType); } } }
    [SerializeField]
    private float attackRange = 1f;
    [SerializeField]
    private float baseAttackCooldown = 1f;
    private float attackCooldown { get { if (isAlly) { return baseAttackCooldown *= UnitTrustManager.Instance.GetPlayerAttackSpeed(unitType); } else { return baseAttackCooldown; } } }
    private bool attackReady = true;



    public Animator anim;
    private Rigidbody2D rb;

    public bool isAlly;
    private Vector2 positionOffset;
    Sprite sprite;

    [SerializeField]
    private UnitType UnitType;
    public UnitType unitType { get { return UnitType; } }
    [SerializeField]
    bool rangedUnit;


    CircleCollider2D hitRadius;
    BoxCollider2D hitCollider;

    private Transform currentTarget;
    List<Transform> validTargets = new List<Transform>();


    private void Awake()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
        hitRadius = gameObject.GetComponent<CircleCollider2D>();
        hitCollider = gameObject.GetComponent<BoxCollider2D>();
        currentHealth = MaximumHealth;
    }


    private void Start()
    {
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.freezeRotation = true;
        rb.gravityScale = 0f;
        hitRadius.radius = attackRange;

        if (isAlly)
        {
            currentTarget = KingController.Instance.transform;
        }

        print(attackDamage);
        print(attackCooldown);

    }

    private void Update()
    {

        if(validTargets.Count > 0)
        {
            currentTarget = validTargets[0].transform;
        }
        else
        {
            currentTarget = null;
        }


        if (rangedUnit)
        {
            if (attackReady && currentTarget != null)
            {
                attackReady = false;
                RangedUnitAttack();
                StartCoroutine(StartAttackCooldown());
            }
        }

    }

    private void FixedUpdate()
    {

        if (isAlly)
        {
            ProcessAllyMovement();
        }
        else
        {

            if (validTargets.Count > 0)
            {
                if (unitType == UnitType.Catapult || unitType == UnitType.Archer || unitType == UnitType.Wizard)
                {
                    rb.linearVelocity = Vector2.zero;
                } else
                {
                    rb.linearVelocity = (currentTarget.position - transform.position) * moveSpeed * Time.deltaTime * 5f;
                }
            }
            else
            {
                rb.linearVelocity = (KingController.Instance.kingPosition - (Vector2)transform.position) * moveSpeed * Time.deltaTime * 5f;
            }
        }

        if (anim) {
            if (rb.linearVelocity == Vector2.zero) {
                anim.SetBool("isWalking", true);
            } else {
                anim.SetBool("isWalking", false);
            }
        }
    }

    private void ProcessAllyMovement()
    {
        if (currentTarget != null && !(unitType == UnitType.Catapult || unitType == UnitType.Archer || unitType == UnitType.Wizard)) //checks if there is a target (melee only)
        {
            if (Vector2.Distance(currentTarget.position, transform.position) < DistanceToKing()) //checks if the target is closer than the king
            {
                rb.linearVelocity = (currentTarget.position - transform.position) * moveSpeed * Time.deltaTime * 5f; //moves to the target if so
            }
            else
            {
                rb.linearVelocity = (KingController.Instance.kingPosition + positionOffset - (Vector2)transform.position) * moveSpeed * 5f;
            }

        }
        else
        {
            rb.linearVelocity = (KingController.Instance.kingPosition + positionOffset - (Vector2)transform.position) * moveSpeed * 5f;
        }
    }

    private void RangedUnitAttack()
    {

        switch (unitType)
        {
            case UnitType.Archer:
                Debug.DrawLine(transform.position, currentTarget.position, Color.red, 0.3f);
                if (currentTarget.tag == "King")
                {
                    KingController.Instance.TakeDamage(attackDamage);
                }
                else
                {
                    currentTarget.GetComponent<Unit>().TakeDamage(attackDamage);
                }
                break;


            case UnitType.Wizard:


                Debug.DrawRay(transform.position, currentTarget.position - transform.position, Color.green, 0.25f);
                foreach (RaycastHit2D hitInfo in Physics2D.RaycastAll(transform.position, currentTarget.position - transform.position, 5f))
                {

                    if(hitInfo.collider is CircleCollider2D)
                    {
                        continue;
                    }
                    if (isAlly)
                    {
                        if (hitInfo.transform.tag == "EnemyUnit")
                        {
                            hitInfo.collider.GetComponent<Unit>().TakeDamage(attackDamage);
                        }
                    }
                    else
                    {
                        if (hitInfo.collider.tag == "King")
                        {
                            KingController.Instance.TakeDamage(attackDamage);
                        }
                        else if (hitInfo.collider.tag == "PlayerUnit")
                        {
                            hitInfo.collider.GetComponent<Unit>().TakeDamage(attackDamage);
                        }
                    }
                }
                break;
            case UnitType.Catapult:

                foreach (Collider2D collision in Physics2D.OverlapCircleAll(currentTarget.position, 1f))
                {
                    if (isAlly)
                    {
                        if(collision.tag == "EnemyUnit")
                        {
                            collision.GetComponent<Unit>().TakeDamage(attackDamage);
                        }
                    }
                    else
                    {
                        if(collision.tag == "PlayerUnit")
                        {
                            collision.GetComponent<Unit>().TakeDamage(attackDamage);
                        }
                        else if(collision.tag == "King")
                        {
                            KingController.Instance.TakeDamage(attackDamage);
                        }
                    }
                }
                break;
            default:
                break;
        }


    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            Die();
        }
    }



    IEnumerator StartAttackCooldown()
    {
        yield return new WaitForSeconds(attackCooldown);
        attackReady = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (isAlly)
        {
            if (collision.tag == "EnemyUnit")
            {
                validTargets.Add(collision.transform);
            }
        }
        else
        {
            if (collision.tag == "King")
            {
                validTargets.Insert(0,collision.transform);

            }
            else if (collision.tag == "PlayerUnit")
            {
                validTargets.Add(collision.transform);
            }


        }

    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (isAlly)
        {
            if (collision.tag == "EnemyUnit")
            {
                validTargets.Remove(collision.transform);
            }
        }
        else
        {
            if (collision.tag == "King")
            {
                validTargets.Remove(collision.transform);
                if (validTargets.Count != 0)
                {
                    currentTarget = validTargets[0].transform;
                }
            }

            else if (collision.tag == "PlayerUnit")
            {
                validTargets.Remove(collision.transform);
            }


        }
    }

    //===============================
    //  ATTACKS FOR MELEE UNITS ONLY
    //===============================


    private void OnCollisionStay2D(Collision2D collision)
    {

        if (attackReady && !rangedUnit)
        {

            if (isAlly)
            {
                if (collision.transform == currentTarget)
                {
                    currentTarget.GetComponent<Unit>().TakeDamage(attackDamage);
                }
            }
            else
            {
                if(collision.transform == currentTarget)
                {
                    if(collision.transform.tag == "King")
                    {
                        KingController.Instance.TakeDamage(attackDamage);
                    }
                    else
                    {
                       currentTarget.GetComponent<Unit>().TakeDamage(attackDamage);
                    }
                }
            }
            attackReady = false;
            StartCoroutine(StartAttackCooldown());
        }
    }


    private void Die()
    {
        validTargets.Clear();
        if (isAlly)
        {
            UnitController.Instance.RemovePlayerUnit(this);
        }

        Destroy(gameObject);

    }


    public void PlacePlayerUnit(int index)
    {

        Quaternion rotation = Quaternion.AngleAxis(15f * Mathf.Floor(index / 8), Vector3.forward);
        positionOffset = rotation * ((1f + 0.35f * Mathf.Floor(index / 8)) * CardinalVector(index % 8));
        transform.position = KingController.Instance.kingPosition + positionOffset;
    }


    private Vector2 CardinalVector(int num)
    {

        switch (num)
        {
            case 0:
                return Vector2.up;
            case 1:
                return new Vector2(0.7071f, 0.7071f);
            case 2:
                return Vector2.right;
            case 3:
                return new Vector2(0.7071f, -0.7071f);
            case 4:
                return Vector2.down;
            case 5:
                return new Vector2(-0.7071f, -0.7071f);
            case 6:
                return Vector2.left;
            case 7:
                return new Vector2(-0.7071f, 0.7071f);
            default:
                return Vector2.zero;

        }
    }




    private float DistanceToKing()
    {
        return Vector2.Distance(KingController.Instance.kingPosition, transform.position);
    }






    //if (isAlly)
    //{
    //    if (currentTarget == KingController.Instance.transform) { //Checks if the king is the current target

    //        rb.linearVelocity = (KingController.Instance.kingPosition + positionOffset - (Vector2)transform.position) * moveSpeed * Time.deltaTime * 20f;

    //    }
    //    else
    //    {
    //        rb.linearVelocity = (currentTarget.position - transform.position) * moveSpeed * Time.deltaTime * 20f;



    //        if ( DistanceToKing() < Vector2.Distance(currentTarget.position, transform.position))
    //        {
    //            currentTarget = KingController.Instance.transform;
    //        }


    //    }
    //}
}
