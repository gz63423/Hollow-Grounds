using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyBullet : MonoBehaviour
{
    // =========================================================
    // SETTINGS
    // =========================================================

    public int damage = 1;
    public float speed = 8f;
    public float lifeTime = 5f;


    // =========================================================
    // INTERNAL
    // =========================================================

    private Transform target;
    private Rigidbody2D rb;

    private EnemyRanged rangedEnemy;

    private bool isFrozen = false;
    private Vector2 frozenVelocity;


    // =========================================================
    // AWAKE
    // =========================================================

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }


    // =========================================================
    // START
    // =========================================================

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


    // =========================================================
    // SET TARGET
    // =========================================================

    public void SetTarget(
        Transform newTarget)
    {
        target = newTarget;
    }


    // =========================================================
    // SET RANGED ENEMY
    // =========================================================

    public void SetRangedEnemy(
        EnemyRanged enemy)
    {
        rangedEnemy = enemy;
    }


    // =========================================================
    // FREEZE
    // =========================================================

    public void SetFrozen(
        bool frozen)
    {
        if (rb == null)
            return;


        // =====================================================
        // FREEZE
        // =====================================================

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


        // =====================================================
        // UNFREEZE
        // =====================================================

        if (!frozen)
        {
            if (!isFrozen)
                return;

            isFrozen = false;

            // Restore the exact velocity
            rb.linearVelocity =
                frozenVelocity;
        }
    }


    // =========================================================
    // UPDATE
    // =========================================================

    void Update()
    {
        // Keep the bullet completely stationary
        // while Freeze is active.
        if (isFrozen)
        {
            rb.linearVelocity =
                Vector2.zero;
        }
    }


    // =========================================================
    // COLLISION
    // =========================================================

    void OnTriggerEnter2D(
        Collider2D other)
    {
        // =====================================================
        // PLAYER
        // =====================================================

        Player player =
            other.GetComponent<Player>();

        if (player != null &&
            !player.IsDead)
        {
            player.TakeDamage(
                damage
            );


            // =================================================
            // PLAYER KNOCKBACK
            // =================================================

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


        // =====================================================
        // HOUSE
        // =====================================================

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


        // =====================================================
        // OTHER COLLIDERS
        // =====================================================

        if (!other.isTrigger)
        {
            Destroy(gameObject);
        }
    }
}