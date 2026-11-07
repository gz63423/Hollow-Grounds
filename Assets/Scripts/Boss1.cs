using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Collider2D))]
public class Boss1 : MonoBehaviour
{
    [Header("Stats")]
    public float moveSpeed = 2f;
    public int damage = 1;
    public float attackCooldown = 1f;
    public float maxHealth = 3f;

    [Header("Attack Ranges")]
    public float playerAttackRange = 0.25f;

    [Tooltip("Extra distance the player can move away before an attack is cancelled.")]
    public float attackForgivenessRange = 0.5f;

    [Header("Health Bar")]
    public Slider healthBarPrefab;

    [Header("Targets")]
    public Player player;
    public House house;

    [Header("Movement")]
    public float stoppingDistance = 0.5f;

    [Header("Knockback")]
    public float knockbackForce = 4f;
    public float knockbackDuration = 0.15f;

    private Coroutine knockbackCoroutine;

    //Poison

    private Coroutine poisonCoroutine;





    private long spawnOrder;

    public long GetSpawnOrder()
    {
        return spawnOrder;
    }

    //Knockback

    [Header("Player Knockback")]
    public float playerKnockbackForce = 6f;
    public float playerKnockbackDuration = 0.2f;



    [Header("Death Fade")]
    public float fadeDuration = 1.5f;



    //Bounty

    [Header("Money Reward")]
    public float moneyReward = 0.01f;



    [Header("Money Popup")]
    public GameObject moneyPopupPrefab;
    public Transform moneyPopupAnchor;
    public bool flipMoneyPopupAnchor = true;

    private float moneyPopupAnchorOriginalX;



    [Header("Enemy Unlock Popup")]
    public GameObject enemyUnlockPopupPrefab;
    public Transform enemyUnlockPopupAnchor;




    [Header("Soulstone Reward")]
    [Range(0f, 100f)]
    public float soulstoneDropChance = 100f;

    public int soulstoneDropAmount = 10;




    [Header("Soulstone Popup")]
    public GameObject soulstonePopupPrefab;
    public Transform soulstonePopupAnchor;
    public bool flipSoulstonePopupAnchor = true;

    private float soulstonePopupAnchorOriginalX;



    //Components

    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private Collider2D myCollider;

    private EnemyHitFlash hitFlash;



    //AI Logic

    public Transform currentTarget { get; private set; }
    public float Health { get; private set; }

    private float attackTimer;
    private bool isDead;
    private bool facingRight = true;

    private bool isAttacking;
    private bool aggroLocked;
    private Transform lockedTarget;

    private EnemyHealthBar healthBarInstance;

    public event System.Action OnEnemyDestroyed;

    [HideInInspector]
    public WaveSpawner waveSpawner;

    private bool targetingHouse = false;



    //Freeze

    private bool isFrozen = false;
    private float previousAnimatorSpeed = 1f;





    void Awake()
    {
        // Lower spawnOrder = spawned earlier.
        spawnOrder =
            EnemySpawnOrder.GetNext();

        rb =
            GetComponent<Rigidbody2D>();

        animator =
            GetComponent<Animator>();

        spriteRenderer =
            GetComponent<SpriteRenderer>();

        myCollider =
            GetComponent<Collider2D>();

        hitFlash =
            GetComponent<EnemyHitFlash>();

        rb.gravityScale =
            0f;

        rb.freezeRotation =
            true;

        if (player == null)
        {
            player =
                FindFirstObjectByType<Player>();
        }

        if (house == null)
        {
            house =
                FindFirstObjectByType<House>();
        }
    }



    void Start()
    {
        Health =
            maxHealth;

        if (healthBarPrefab != null)
        {
            healthBarInstance =
                gameObject.AddComponent<EnemyHealthBar>();

            healthBarInstance.healthBarPrefab =
                healthBarPrefab;
        }

        if (moneyPopupAnchor != null)
        {
            moneyPopupAnchorOriginalX =
                moneyPopupAnchor.localPosition.x;
        }

        if (soulstonePopupAnchor != null)
        {
            soulstonePopupAnchorOriginalX =
                soulstonePopupAnchor.localPosition.x;
        }

        ChooseTarget();

        if (currentTarget != null)
        {
            FlipTowards(
                currentTarget.position
            );
        }
    }



    void Update()
    {
        if (isFrozen)
        {
            rb.linearVelocity =
                Vector2.zero;

            animator.SetFloat(
                "Speed",
                0f
            );

            return;
        }

        if (isDead)
        {
            rb.linearVelocity =
                Vector2.zero;

            animator.SetFloat(
                "Speed",
                0f
            );

            return;
        }

        attackTimer -=
            Time.deltaTime;

        ChooseTarget();

        TryAttack();

        CheckAttackCancel();

        animator.SetFloat(
            "Speed",
            rb.linearVelocity.magnitude
        );

        if (currentTarget != null)
        {
            FlipTowards(
                currentTarget.position
            );
        }
    }



    void FixedUpdate()
    {
        if (isFrozen)
        {
            rb.linearVelocity =
                Vector2.zero;

            return;
        }

        if (isDead)
        {
            rb.linearVelocity =
                Vector2.zero;

            return;
        }

        if (knockbackCoroutine != null)
        {
            return;
        }

        if (currentTarget == null)
        {
            rb.linearVelocity =
                Vector2.zero;

            return;
        }

        Vector2 direction =
            currentTarget.position -
            transform.position;

        float distance =
            direction.magnitude;

        if (!targetingHouse)
        {
            if (player == null ||
                player.IsDead)
            {
                rb.linearVelocity =
                    Vector2.zero;

                return;
            }

            float effectiveStoppingDistance =
                Mathf.Max(
                    stoppingDistance,
                    playerAttackRange
                );

            if (distance <=
                effectiveStoppingDistance)
            {
                rb.linearVelocity =
                    Vector2.zero;

                return;
            }

            if (isAttacking)
            {
                rb.linearVelocity =
                    Vector2.zero;

                return;
            }

            rb.linearVelocity =
                direction.normalized *
                moveSpeed;

            return;
        }

        if (targetingHouse &&
            house != null)
        {
            if (IsTouchingHouse())
            {
                rb.linearVelocity =
                    Vector2.zero;
            }
            else
            {
                rb.linearVelocity =
                    direction.normalized *
                    moveSpeed;
            }
        }
    }



    public void SetFrozen(
        bool frozen)
    {
        if (isDead)
            return;

        if (isFrozen == frozen)
            return;

        isFrozen =
            frozen;

        if (isFrozen)
        {
            rb.linearVelocity =
                Vector2.zero;

            rb.angularVelocity =
                0f;

            previousAnimatorSpeed =
                animator.speed;

            animator.speed =
                0f;
        }
        else
        {
            rb.linearVelocity =
                Vector2.zero;

            rb.angularVelocity =
                0f;

            animator.speed =
                previousAnimatorSpeed;
        }
    }


    //AI
    public void ChooseTarget()
    {
        if (!targetingHouse &&
            player != null &&
            player.IsDead)
        {
            isAttacking =
                false;

            animator.ResetTrigger(
                "Attack"
            );

            lockedTarget =
                null;

            aggroLocked =
                false;
        }

        if (aggroLocked &&
            lockedTarget != null)
        {
            currentTarget =
                lockedTarget;

            targetingHouse =
                lockedTarget ==
                house?.transform;

            return;
        }

        Transform closest =
            null;

        float closestDist =
            float.MaxValue;

        if (house != null)
        {
            closest =
                house.transform;

            closestDist =
                Vector2.Distance(
                    transform.position,
                    house.transform.position
                );
        }

        if (player != null &&
            !player.IsDead)
        {
            float dist =
                Vector2.Distance(
                    transform.position,
                    player.transform.position
                );

            if (dist < closestDist)
            {
                closest =
                    player.transform;

                closestDist =
                    dist;
            }
        }

        currentTarget =
            closest;

        targetingHouse =
            currentTarget ==
            house?.transform;
    }



    void TryAttack()
    {
        if (attackTimer > 0f ||
            currentTarget == null ||
            isDead ||
            isFrozen)
            return;

        if (!targetingHouse &&
            player != null &&
            !player.IsDead)
        {
            float dist =
                Vector2.Distance(
                    transform.position,
                    player.transform.position
                );

            if (dist <=
                playerAttackRange)
            {
                StartAttack();

                return;
            }
        }

        if (targetingHouse &&
            house != null &&
            IsTouchingHouse())
        {
            StartAttack();
        }
    }

    void StartAttack()
    {
        if (isDead ||
            isFrozen ||
            currentTarget == null)
            return;

        isAttacking =
            true;

        aggroLocked =
            true;

        lockedTarget =
            currentTarget;

        rb.linearVelocity =
            Vector2.zero;

        animator.SetTrigger(
            "Attack"
        );

        attackTimer =
            attackCooldown;
    }

    void CheckAttackCancel()
    {
        if (!isAttacking ||
            lockedTarget == null ||
            isFrozen)
            return;

        if (!targetingHouse &&
            player != null)
        {
            float dist =
                Vector2.Distance(
                    transform.position,
                    player.transform.position
                );

            float cancelDistance =
                playerAttackRange +
                attackForgivenessRange;

            if (dist >
                cancelDistance)
            {
                isAttacking =
                    false;

                aggroLocked =
                    false;

                lockedTarget =
                    null;

                animator.ResetTrigger(
                    "Attack"
                );
            }
        }
    }

    public void DealDamage()
    {
        if (isDead ||
            isFrozen)
            return;

        Transform target =
            currentTarget;

        if (target == null)
            return;

        if (target ==
            house?.transform &&
            house != null)
        {
            if (IsTouchingHouse())
            {
                house.TakeDamage(
                    damage
                );

                if (SoundManager.Instance != null)
                {
                    SoundManager.Instance.PlayEnemyHit();
                }
            }

            return;
        }

        if (target ==
            player?.transform &&
            player != null &&
            !player.IsDead)
        {
            float dist =
                Vector2.Distance(
                    transform.position,
                    player.transform.position
                );

            if (dist <=
                playerAttackRange +
                attackForgivenessRange)
            {
                player.TakeDamage(
                    damage
                );

                if (SoundManager.Instance != null)
                {
                    SoundManager.Instance.PlayEnemyHit();
                }

                Vector2 knockbackDirection =
                    (
                        (Vector2)player.transform.position -
                        (Vector2)transform.position
                    ).normalized;

                player.ApplyEnemyKnockback(
                    knockbackDirection,
                    playerKnockbackForce,
                    playerKnockbackDuration
                );
            }
        }
    }

    public void EndAttack()
    {
        isAttacking =
            false;

        if (!targetingHouse)
        {
            aggroLocked =
                false;

            lockedTarget =
                null;
        }
    }



    void OnCollisionEnter2D(
        Collision2D collision)
    {
        if (isDead)
            return;

        if (collision.collider ==
            house?.houseCollider)
        {
            targetingHouse =
                true;

            currentTarget =
                house.transform;
        }
    }

    void OnCollisionExit2D(
        Collision2D collision)
    {
        if (collision.collider ==
            house?.houseCollider)
        {
            isAttacking =
                false;

            targetingHouse =
                false;

            animator.ResetTrigger(
                "Attack"
            );
        }
    }

    public bool IsTouchingHouse()
    {
        if (house == null ||
            house.houseCollider == null)
            return false;

        return myCollider.IsTouching(
            house.houseCollider
        );
    }



    public void ApplyPoison(
        float damagePerSecond,
        float duration)
    {
        if (isDead)
            return;

        if (poisonCoroutine != null)
        {
            StopCoroutine(
                poisonCoroutine
            );
        }

        poisonCoroutine =
            StartCoroutine(
                PoisonRoutine(
                    damagePerSecond,
                    duration
                )
            );
    }

    private IEnumerator PoisonRoutine(
        float damagePerSecond,
        float duration)
    {
        int numberOfTicks =
            Mathf.CeilToInt(
                duration
            );

        for (int i = 0;
             i < numberOfTicks;
             i++)
        {
            if (isDead)
            {
                poisonCoroutine =
                    null;

                yield break;
            }

            yield return new WaitForSeconds(
                1f
            );

            if (isDead)
            {
                poisonCoroutine =
                    null;

                yield break;
            }

            TakeDamage(
                damagePerSecond
            );

            if (isDead)
            {
                poisonCoroutine =
                    null;

                yield break;
            }
        }

        poisonCoroutine =
            null;
    }


    //Sprite Fli[

    public void FlipTowards(
        Vector2 targetPos)
    {
        if (targetPos.x >
            transform.position.x &&
            !facingRight)
        {
            facingRight =
                true;

            spriteRenderer.flipX =
                false;
        }
        else if (
            targetPos.x <
            transform.position.x &&
            facingRight)
        {
            facingRight =
                false;

            spriteRenderer.flipX =
                true;
        }

        if (flipMoneyPopupAnchor &&
            moneyPopupAnchor != null)
        {
            Vector3 anchorPosition =
                moneyPopupAnchor.localPosition;

            anchorPosition.x =
                facingRight
                    ? moneyPopupAnchorOriginalX
                    : -moneyPopupAnchorOriginalX;

            moneyPopupAnchor.localPosition =
                anchorPosition;
        }

        if (flipSoulstonePopupAnchor &&
            soulstonePopupAnchor != null)
        {
            Vector3 anchorPosition =
                soulstonePopupAnchor.localPosition;

            anchorPosition.x =
                facingRight
                    ? soulstonePopupAnchorOriginalX
                    : -soulstonePopupAnchorOriginalX;

            soulstonePopupAnchor.localPosition =
                anchorPosition;
        }
    }

    
    
    //DMG

    public void TakeDamage(
        float amount)
    {
        if (isDead)
            return;

        hitFlash?.Flash();

        float actualDamage =
            Mathf.Min(
                amount,
                Health
            );

        Health -=
            actualDamage;

        if (DamageDealt.Instance != null)
        {
            DamageDealt.Instance.AddDamage(
                actualDamage
            );
        }

        if (healthBarInstance != null)
        {
            healthBarInstance.Show();
        }

        if (Health <= 0f)
        {
            Die();
        }
    }





    public void ApplyKnockback(
        Vector2 direction)
    {
        if (isDead)
            return;

        if (knockbackCoroutine != null)
        {
            StopCoroutine(
                knockbackCoroutine
            );
        }

        knockbackCoroutine =
            StartCoroutine(
                KnockbackRoutine(
                    direction
                )
            );

        if (CombatEffects.Instance != null)
        {
            CombatEffects.Instance.FreezeFrame();
        }
    }

    private IEnumerator KnockbackRoutine(
        Vector2 direction)
    {
        isAttacking =
            false;

        aggroLocked =
            false;

        lockedTarget =
            null;

        animator.ResetTrigger(
            "Attack"
        );

        direction =
            direction.normalized;

        float elapsed =
            0f;

        while (elapsed <
               knockbackDuration)
        {
            if (isDead)
                yield break;

            if (isFrozen)
            {
                rb.linearVelocity =
                    Vector2.zero;

                yield return
                    new WaitForFixedUpdate();

                continue;
            }

            float progress =
                elapsed /
                knockbackDuration;

            float strength =
                Mathf.Lerp(
                    knockbackForce,
                    0f,
                    progress
                );

            rb.linearVelocity =
                direction *
                strength;

            elapsed +=
                Time.fixedDeltaTime;

            yield return
                new WaitForFixedUpdate();
        }

        rb.linearVelocity =
            Vector2.zero;

        knockbackCoroutine =
            null;

        if (!isDead &&
            !isFrozen)
        {
            ChooseTarget();

            if (currentTarget != null)
            {
                FlipTowards(
                    currentTarget.position
                );
            }
        }
    }

    

    //Death

    void Die()
    {
        if (isDead)
            return;

        isDead =
            true;

        isFrozen =
            false;

        animator.speed =
            1f;

        if (waveSpawner != null)
        {
            waveSpawner.RegisterEnemyDeath(
                gameObject
            );
        }

        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlayEnemyDeath();
        }

        bool wasAlreadyUnlocked =
            EnemyInfoUI.IsEnemyUnlocked(4);

        EnemyInfoUI.UnlockEnemy(
            4,
            transform.position
        );

        if (!wasAlreadyUnlocked &&
            enemyUnlockPopupPrefab != null)
        {
            GameObject popup =
                Instantiate(
                    enemyUnlockPopupPrefab,
                    Vector3.zero,
                    Quaternion.identity
                );

            EnemyUnlockPopup unlockPopup =
                popup.GetComponent<EnemyUnlockPopup>();

            if (unlockPopup != null)
            {
                Vector3 popupPosition =
                    enemyUnlockPopupAnchor != null
                        ? enemyUnlockPopupAnchor.position
                        : transform.position;

                unlockPopup.Setup(
                    "NEW SPECIMEN DISCOVERED!",
                    popupPosition
                );
            }
        }

        if (!wasAlreadyUnlocked &&
            SoundManager.Instance != null)
        {
            SoundManager.Instance.PlayEnemyUnlocked();
        }

        float finalMoneyReward =
            moneyReward;

        if (RebirthManager.Instance != null)
        {
            finalMoneyReward *=
                RebirthManager.Instance.CurrentMultiplier;
        }

        if (Player.Instance != null)
        {
            Player.Instance.AddMoney(
                finalMoneyReward
            );
        }

        if (moneyPopupPrefab != null)
        {
            GameObject popup =
                Instantiate(
                    moneyPopupPrefab,
                    Vector3.zero,
                    Quaternion.identity
                );

            MoneyPopup moneyPopup =
                popup.GetComponent<MoneyPopup>();

            if (moneyPopup != null)
            {
                Vector3 popupPosition =
                    moneyPopupAnchor != null
                        ? moneyPopupAnchor.position
                        : transform.position;

                moneyPopup.Setup(
                    finalMoneyReward,
                    popupPosition
                );
            }
        }

        if (EnemiesKilled.Instance != null)
        {
            EnemiesKilled.Instance.AddKill();
        }

        if (NetWorth.Instance != null)
        {
            NetWorth.Instance.AddMoneyEarned(
                finalMoneyReward
            );
        }

        if (BossesKilled.Instance != null)
        {
            BossesKilled.Instance.AddBossKill();
        }

        GiveXP giveXP =
            GetComponent<GiveXP>();

        if (giveXP != null)
        {
            giveXP.Give();
        }

        animator.SetTrigger(
            "Die"
        );

        myCollider.enabled =
            false;

        rb.linearVelocity =
            Vector2.zero;

        OnEnemyDestroyed?.Invoke();

        Destroy(
            gameObject,
            fadeDuration
        );
    }



    public void FinishDeath()
    {
        StartCoroutine(
            FadeOutAndDestroy()
        );
    }

    IEnumerator FadeOutAndDestroy()
    {
        float elapsed =
            0f;

        Color c =
            spriteRenderer.color;

        while (elapsed <
               fadeDuration)
        {
            elapsed +=
                Time.deltaTime;

            spriteRenderer.color =
                new Color(
                    c.r,
                    c.g,
                    c.b,
                    Mathf.Lerp(
                        1f,
                        0f,
                        elapsed /
                        fadeDuration
                    )
                );

            yield return null;
        }

        Destroy(
            gameObject
        );
    }
}