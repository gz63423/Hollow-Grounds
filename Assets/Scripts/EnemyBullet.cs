using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyBullet : MonoBehaviour
{


    public int damage = 1;
    public float speed = 8f;
    public float lifeTime = 5f;



    private Transform target;
    private Rigidbody2D rb;

    private EnemyRanged rangedEnemy;

    private bool isFrozen = false;
    private Vector2 frozenVelocity;


    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }


    void Start()
    {
        Destroy(gameObject, lifeTime);

        if (target != null)
        {
            Vector2 dir =
                (
                    (Vector2)target.position -
                    (Vector2)transform.position
                ).normalized;

            rb.linearVelocity =
                dir * speed;
        }
    }


    public void SetTarget(
        Transform newTarget)
    {
        target = newTarget;
    }


    public void SetRangedEnemy(
        EnemyRanged enemy)
    {
        rangedEnemy = enemy;
    }


    public void SetFrozen(
        bool frozen)
    {
        if (rb == null)
            return;


        if (frozen)
        {
            if (isFrozen)
                return;

            isFrozen = true;

            // Save current velocity
            frozenVelocity =
                rb.linearVelocity;

            // Completely stop the bullet
            rb.linearVelocity =
                Vector2.zero;

            return;
        }



        if (!frozen)
        {
            if (!isFrozen)
                return;

            isFrozen = false;

            rb.linearVelocity =
                frozenVelocity;
        }
    }



    void Update()
    {
        // Keep the bullet completely stationary while frozen
        if (isFrozen)
        {
            rb.linearVelocity =
                Vector2.zero;
        }
    }




    void OnTriggerEnter2D(
        Collider2D other)
    {


        Player player =
            other.GetComponent<Player>();

        if (player != null &&
            !player.IsDead)
        {
            player.TakeDamage(
                damage
            );


            if (rangedEnemy != null)
            {
                Vector2 knockbackDirection =
                    (
                        (Vector2)player.transform.position -
                        (Vector2)transform.position
                    ).normalized;

                player.ApplyEnemyKnockback(
                    knockbackDirection,
                    rangedEnemy.playerKnockbackForce,
                    rangedEnemy.playerKnockbackDuration
                );
            }

            Destroy(gameObject);

            return;
        }




        House house =
            other.GetComponent<House>();

        if (house != null)
        {
            house.TakeDamage(
                damage
            );

            Destroy(gameObject);

            return;
        }



        if (!other.isTrigger)
        {
            Destroy(gameObject);
        }
    }
}