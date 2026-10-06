using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Collider2D))]
public class Enemy : MonoBehaviour
{
    public enum EnemyType { Scout, Ravager, Stalker }

    [Header("Enemy Encyclopedia")]
    public EnemyType enemyType;

    [Header("Stats")]
    public float moveSpeed = 2f;
    public int damage = 1;
    public float attackCooldown = 1f;
    public float maxHealth = 3;

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

    // =========================================================
    // POISON
    // =========================================================

    private Coroutine poisonCoroutine;

    // =========================================================
    // SPAWN ORDER
    // =========================================================

    private long spawnOrder;

    [Header("Player Knockback")]
    public float playerKnockbackForce = 3f;
    public float playerKnockbackDuration = 0.15f;

    [Header("Death Fade")]
    [Tooltip("How long the enemy takes to completely fade out after the death animation event.")]
    public float fadeDuration = 1.5f;

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
    public int soulstoneDropAmount = 1;

    [Header("Soulstone Popup")]
    public GameObject soulstonePopupPrefab;
    public Transform soulstonePopupAnchor;
    public bool flipSoulstonePopupAnchor = true;

    private float soulstonePopupAnchorOriginalX;

    // =========================================================
    // COMPONENTS
    // =========================================================

    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private Collider2D myCollider;

    private EnemyHitFlash hitFlash;

    // =========================================================
    // BLOOD EFFECT
    // =========================================================

    [Header("Blood Effect")]
    [Tooltip("Automatically finds EnemyBloodEffect on this enemy.")]
    private EnemyBloodEffect bloodEffect;

    // =========================================================
    // TARGET
    // =========================================================

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

    // =========================================================
    // FREEZE
    // =========================================================

    private bool isFrozen = false;
    private float previousAnimatorSpeed = 1f;

    // =========================================================
    // AWAKE
    // =========================================================

    void Awake()
    {
        spawnOrder =
            EnemySpawnOrder.GetNext();

        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        myCollider = GetComponent<Collider2D>();
        hitFlash = GetComponent<EnemyHitFlash>();

        // =====================================================
        // BLOOD EFFECT
        // =====================================================

        bloodEffect =
            GetComponent<EnemyBloodEffect>();

        rb.gravityScale = 0f;
        rb.freezeRotation = true;

        if (player == null)
            player = FindFirstObjectByType<Player>();

        if (house == null)
            house = FindFirstObjectByType<House>();
    }

    // =========================================================
    // START
    // =========================================================

    void Start()
    {
        Health = maxHealth;

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
            FlipTowards(currentTarget.position);
    }

    // =========================================================
    // UPDATE
    // =========================================================

    void Update()
    {
        if (isFrozen)
        {
            rb.linearVelocity = Vector2.zero;

            animator.SetFloat(
                "Speed",
                0f
            );

            return;
        }

        if (isDead)
        {
            rb.linearVelocity = Vector2.zero;

            animator.SetFloat(
                "Speed",
                0f
            );

            return;
        }

        attackTimer -= Time.deltaTime;

        ChooseTarget();
        TryAttack();
        CheckAttackCancel();

        animator.SetFloat(
            "Speed",
            rb.linearVelocity.magnitude
        );

        if (currentTarget != null)
            FlipTowards(currentTarget.position);
    }

    // =========================================================
    // FIXED UPDATE
    // =========================================================

    void FixedUpdate()
    {
        if (isFrozen)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        if (isDead)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        if (knockbackCoroutine != null)
        {
            return;
        }

        if (currentTarget == null)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        Vector2 direction =
            currentTarget.position -
            transform.position;

        float distance =
            direction.magnitude;

        // =====================================================
        // PLAYER TARGET
        // =====================================================

        if (!targetingHouse)
        {
            if (player == null || player.IsDead)
            {
                rb.linearVelocity = Vector2.zero;
                return;
            }

            float effectiveStoppingDistance =
                Mathf.Max(
                    stoppingDistance,
                    playerAttackRange
                );

            if (distance <= effectiveStoppingDistance)
            {
                rb.linearVelocity = Vector2.zero;
                return;
            }

            if (isAttacking)
            {
                rb.linearVelocity = Vector2.zero;
                return;
            }

            rb.linearVelocity =
                direction.normalized *
                moveSpeed;

            return;
        }

        // =====================================================
        // HOUSE TARGET
        // =====================================================

        if (targetingHouse && house != null)
        {
            if (IsTouchingHouse())
            {
                rb.linearVelocity = Vector2.zero;
            }
            else
            {
                rb.linearVelocity =
                    direction.normalized *
                    moveSpeed;
            }
        }
    }

    // =========================================================
    // FREEZE CONTROL
    // =========================================================

    public void SetFrozen(bool frozen)
    {
        if (isDead)
            return;

        if (isFrozen == frozen)
            return;

        isFrozen = frozen;

        if (isFrozen)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;

            previousAnimatorSpeed =
                animator.speed;

            animator.speed = 0f;
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;

            animator.speed =
                previousAnimatorSpeed;
        }
    }

    // =========================================================
    // TARGETING
    // =========================================================

    public void ChooseTarget()
    {
        // =====================================================
        // PLAYER DEAD
        // =====================================================

        if (player != null &&
            player.IsDead)
        {
            // Only clear player aggro if the player was
            // actually the locked target.

            if (lockedTarget == player.transform)
            {
                isAttacking = false;

                animator.ResetTrigger("Attack");

                lockedTarget = null;
                aggroLocked = false;
            }
        }

        // =====================================================
        // LOCKED TARGET
        // =====================================================

        if (aggroLocked &&
            lockedTarget != null)
        {
            // -------------------------------------------------
            // PLAYER LOCK
            // -------------------------------------------------

            if (lockedTarget == player?.transform)
            {
                if (player == null ||
                    player.IsDead)
                {
                    isAttacking = false;

                    animator.ResetTrigger("Attack");

                    lockedTarget = null;
                    aggroLocked = false;
                }
                else
                {
                    currentTarget =
                        player.transform;

                    targetingHouse = false;

                    return;
                }
            }

            // -------------------------------------------------
            // HOUSE LOCK
            // -------------------------------------------------

            if (lockedTarget == house?.transform)
            {
                currentTarget =
                    house.transform;

                targetingHouse = true;

                return;
            }
        }

        // =====================================================
        // FIND CLOSEST TARGET
        // =====================================================

        Transform closestTarget = null;

        float closestDistance =
            float.MaxValue;

        // -----------------------------------------------------
        // HOUSE
        // -----------------------------------------------------

        if (house != null)
        {
            float houseDistance =
                Vector2.Distance(
                    transform.position,
                    house.transform.position
                );

            closestTarget =
                house.transform;

            closestDistance =
                houseDistance;
        }

        // -----------------------------------------------------
        // PLAYER
        // -----------------------------------------------------

        if (player != null &&
            !player.IsDead)
        {
            float playerDistance =
                Vector2.Distance(
                    transform.position,
                    player.transform.position
                );

            if (playerDistance < closestDistance)
            {
                closestTarget =
                    player.transform;

                closestDistance =
                    playerDistance;
            }
        }

        // =====================================================
        // APPLY TARGET
        // =====================================================

        currentTarget =
            closestTarget;

        targetingHouse =
            currentTarget ==
            house?.transform;
    }

    // =========================================================
    // ATTACK
    // =========================================================

    void TryAttack()
    {
        if (attackTimer > 0f ||
            currentTarget == null ||
            isDead ||
            isFrozen)
            return;

        // =====================================================
        // PLAYER
        // =====================================================

        if (!targetingHouse &&
            player != null &&
            !player.IsDead)
        {
            float dist =
                Vector2.Distance(
                    transform.position,
                    player.transform.position
                );

            if (dist <= playerAttackRange)
            {
                StartAttack();
                return;
            }
        }

        // =====================================================
        // HOUSE
        // =====================================================

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

        isAttacking = true;

        aggroLocked = true;
        lockedTarget = currentTarget;

        rb.linearVelocity = Vector2.zero;

        animator.SetTrigger("Attack");

        attackTimer = attackCooldown;
    }

    void CheckAttackCancel()
    {
        if (!isAttacking ||
            lockedTarget == null ||
            isFrozen)
            return;

        // =====================================================
        // PLAYER ATTACK CANCEL
        // =====================================================

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

            if (dist > cancelDistance)
            {
                isAttacking = false;

                aggroLocked = false;
                lockedTarget = null;

                animator.ResetTrigger("Attack");
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

        // =====================================================
        // HOUSE
        // =====================================================

        if (target ==
            house?.transform &&
            house != null)
        {
            if (IsTouchingHouse())
            {
                house.TakeDamage(damage);

                if (SoundManager.Instance != null)
                {
                    SoundManager.Instance.PlayEnemyHit();
                }
            }

            return;
        }

        // =====================================================
        // PLAYER
        // =====================================================

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
                player.TakeDamage(damage);

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
        if (isDead)
            return;

        isAttacking = false;
    }

    // =========================================================
    // COLLISIONS
    // =========================================================

    void OnCollisionEnter2D(
        Collision2D collision)
    {
        if (isDead)
            return;

        if (collision.collider !=
            house?.houseCollider)
            return;

        // =====================================================
        // PLAYER HAS PRIORITY IF CLOSER
        // =====================================================

        if (player != null &&
            !player.IsDead)
        {
            float playerDistance =
                Vector2.Distance(
                    transform.position,
                    player.transform.position
                );

            float houseDistance =
                Vector2.Distance(
                    transform.position,
                    house.transform.position
                );

            // Do NOT force the enemy onto the house
            // if the player is actually closer.

            if (playerDistance < houseDistance)
            {
                currentTarget =
                    player.transform;

                targetingHouse = false;

                return;
            }
        }

        // =====================================================
        // HOUSE TARGET
        // =====================================================

        targetingHouse = true;

        currentTarget =
            house.transform;

        rb.linearVelocity =
            Vector2.zero;
    }

    void OnCollisionExit2D(
        Collision2D collision)
    {
        if (collision.collider !=
            house?.houseCollider)
            return;

        if (isDead)
            return;

        isAttacking = false;

        animator.ResetTrigger("Attack");

        // =====================================================
        // KEEP PLAYER AGGRO IF STILL LOCKED
        // =====================================================

        if (aggroLocked &&
            lockedTarget == player?.transform &&
            player != null &&
            !player.IsDead)
        {
            currentTarget =
                player.transform;

            targetingHouse = false;

            return;
        }

        // =====================================================
        // RE-EVALUATE TARGET
        // =====================================================

        aggroLocked = false;
        lockedTarget = null;

        ChooseTarget();
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

    // =========================================================
    // FLIP
    // =========================================================

    public void FlipTowards(
        Vector2 targetPos)
    {
        if (targetPos.x >
            transform.position.x &&
            !facingRight)
        {
            facingRight = true;
            spriteRenderer.flipX = false;
        }
        else if (
            targetPos.x <
            transform.position.x &&
            facingRight)
        {
            facingRight = false;
            spriteRenderer.flipX = true;
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

    // =========================================================
    // DAMAGE
    // =========================================================

    public void TakeDamage(float amount)
    {
        if (isDead)
            return;

        // =====================================================
        // PLAYER AGGRO
        // =====================================================

        if (player != null &&
            !player.IsDead)
        {
            currentTarget =
                player.transform;

            targetingHouse = false;

            aggroLocked = true;
            lockedTarget =
                player.transform;

            isAttacking = false;

            animator.ResetTrigger("Attack");

            rb.linearVelocity =
                Vector2.zero;
        }

        hitFlash?.Flash();

        float actualDamage =
            Mathf.Min(
                amount,
                Health
            );

        Health -= actualDamage;

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

        // =====================================================
        // BLOOD HIT EFFECT
        // =====================================================

        if (actualDamage > 0f)
        {
            bloodEffect?.PlayHitEffect();
        }

        if (Health <= 0f)
        {
            Die();
        }
    }

    // =========================================================
    // KNOCKBACK
    // =========================================================

    public void ApplyKnockback(Vector2 direction)
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
                KnockbackRoutine(direction)
            );

        if (CombatEffects.Instance != null)
        {
            CombatEffects.Instance.FreezeFrame();
        }
    }

    // =========================================================
    // POISON
    // =========================================================

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
        float elapsed = 0f;

        while (elapsed < duration)
        {
            if (isDead)
            {
                poisonCoroutine = null;
                yield break;
            }

            yield return new WaitForSeconds(1f);

            if (isDead)
            {
                poisonCoroutine = null;
                yield break;
            }

            TakeDamage(
                damagePerSecond
            );

            elapsed += 1f;
        }

        poisonCoroutine = null;
    }

    // =========================================================
    // SPAWN ORDER
    // =========================================================

    public long GetSpawnOrder()
    {
        return spawnOrder;
    }

    // =========================================================
    // KNOCKBACK ROUTINE
    // =========================================================

    private IEnumerator KnockbackRoutine(
        Vector2 direction)
    {
        isAttacking = false;

        aggroLocked = false;
        lockedTarget = null;

        animator.ResetTrigger("Attack");

        direction =
            direction.normalized;

        float elapsed = 0f;

        while (elapsed < knockbackDuration)
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

        knockbackCoroutine = null;

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

    // =========================================================
    // DEATH
    // =========================================================

    void Die()
    {
        if (isDead)
            return;

        isDead = true;

        // =====================================================
        // BLOOD DEATH EFFECT
        // =====================================================

        bloodEffect?.PlayDeathEffect();

        isFrozen = false;

        animator.speed = 1f;

        if (poisonCoroutine != null)
        {
            StopCoroutine(
                poisonCoroutine
            );

            poisonCoroutine = null;
        }

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
            EnemyInfoUI.IsEnemyUnlocked(
                (int)enemyType
            );

        EnemyInfoUI.UnlockEnemy(
            (int)enemyType,
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
                popup.GetComponent<
                    EnemyUnlockPopup>();

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

        // =====================================================
        // MONEY REWARD
        // =====================================================

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

            // =================================================
            // DEMO JOURNEY — MONEY
            // Only count money that was actually awarded.
            // =================================================

            if (DemoJourneyManager.Instance != null)
            {
                DemoJourneyManager.Instance.AddMoneyEarned(
                    finalMoneyReward
                );
            }
        }

        // =====================================================
        // MONEY POPUP
        // =====================================================

        if (moneyPopupPrefab != null)
        {
            GameObject popup =
                Instantiate(
                    moneyPopupPrefab,
                    Vector3.zero,
                    Quaternion.identity
                );

            MoneyPopup moneyPopup =
                popup.GetComponent<
                    MoneyPopup>();

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

        // =====================================================
        // ENEMIES KILLED
        // =====================================================

        if (EnemiesKilled.Instance != null)
        {
            EnemiesKilled.Instance.AddKill();
        }

        AchievementStats.AddEnemiesKilled(1);

        // =====================================================
        // DEMO JOURNEY — ENEMY KILLS
        // =====================================================

        if (DemoJourneyManager.Instance != null)
        {
            if (enemyType == EnemyType.Scout)
            {
                DemoJourneyManager.Instance.AddScoutKill();
            }
            else if (enemyType == EnemyType.Ravager)
            {
                DemoJourneyManager.Instance.AddRavagerKill();
            }
        }

        // =====================================================
        // NET WORTH
        // =====================================================

        if (NetWorth.Instance != null)
        {
            NetWorth.Instance.AddMoneyEarned(
                finalMoneyReward
            );
        }

        // =====================================================
        // XP
        // =====================================================

        GiveXP giveXP =
            GetComponent<GiveXP>();

        if (giveXP != null)
        {
            giveXP.Give();
        }

        // =====================================================
        // SOUL SIPHON
        // =====================================================

        bool soulSiphonUnlocked =
            PlayerPrefs.GetInt(
                "Passive_SoulSiphon_Unlocked",
                0
            ) == 1;

        bool soulSiphonEquipped =
            PassivePowerupManager.IsPassiveEquipped(0);

        if (soulSiphonUnlocked &&
            soulSiphonEquipped)
        {
            int soulSiphonLevel =
                Mathf.Clamp(
                    PlayerPrefs.GetInt(
                        "Passive_SoulSiphon_Level",
                        0
                    ),
                    0,
                    10
                );

            float soulSiphonChance = 0f;

            if (soulSiphonLevel >= 1)
            {
                if (soulSiphonLevel == 1)
                {
                    soulSiphonChance = 2.5f;
                }
                else if (soulSiphonLevel >= 10)
                {
                    soulSiphonChance = 25f;
                }
                else
                {
                    soulSiphonChance =
                        Mathf.Lerp(
                            2.5f,
                            25f,
                            (soulSiphonLevel - 1f) / 9f
                        );
                }
            }

            float roll =
                Random.Range(
                    0f,
                    100f
                );

            Debug.Log(
                "SOUL SIPHON CHECK | " +
                "Unlocked: " +
                soulSiphonUnlocked +
                " | Equipped: " +
                soulSiphonEquipped +
                " | Level: " +
                soulSiphonLevel +
                " | Chance: " +
                soulSiphonChance +
                "% | Roll: " +
                roll
            );

            if (roll < soulSiphonChance)
            {
                if (Soulstones.Instance != null)
                {
                    // =================================================
                    // AWARD SOULSTONES
                    // =================================================

                    Soulstones.Instance.AddSoulstones(
                        soulstoneDropAmount
                    );

                    // =================================================
                    // DEMO JOURNEY — SOUL SIPHON
                    // Count ONE activation for every successful proc.
                    // =================================================

                    if (DemoJourneyManager.Instance != null)
                    {
                        DemoJourneyManager.Instance.AddSoulSiphonActivation();
                    }

                    // =================================================
                    // SOULSTONE POPUP
                    // =================================================

                    if (soulstonePopupPrefab != null)
                    {
                        GameObject popup =
                            Instantiate(
                                soulstonePopupPrefab,
                                Vector3.zero,
                                Quaternion.identity
                            );

                        SoulstonePopup soulstonePopup =
                            popup.GetComponent<
                                SoulstonePopup>();

                        if (soulstonePopup != null)
                        {
                            Vector3 popupPosition =
                                soulstonePopupAnchor != null
                                    ? soulstonePopupAnchor.position
                                    : transform.position;

                            soulstonePopup.Setup(
                                soulstoneDropAmount,
                                popupPosition
                            );
                        }
                    }

                    Debug.Log(
                        "SOUL SIPHON PROC! +" +
                        soulstoneDropAmount +
                        " SOULSTONE!"
                    );
                }
                else
                {
                    Debug.LogError(
                        "SOUL SIPHON PROC FAILED: " +
                        "Soulstones.Instance is NULL!"
                    );
                }
            }
        }

        // =====================================================
        // START DEATH ANIMATION
        // =====================================================

        animator.SetTrigger("Die");

        myCollider.enabled = false;

        rb.linearVelocity =
            Vector2.zero;

        OnEnemyDestroyed?.Invoke();

        // DO NOT DESTROY HERE.
        // The Die animation calls FinishDeath()
        // through an Animation Event.
    }

    // =========================================================
    // ANIMATION EVENT
    // =========================================================

    public void FinishDeath()
    {
        if (!isDead)
            return;

        StartCoroutine(
            FadeOutAndDestroy()
        );
    }

    // =========================================================
    // DEATH FADE
    // =========================================================

    private IEnumerator FadeOutAndDestroy()
    {
        float elapsed = 0f;

        Color startColor =
            spriteRenderer.color;

        while (elapsed < fadeDuration)
        {
            elapsed +=
                Time.deltaTime;

            float progress =
                Mathf.Clamp01(
                    elapsed / fadeDuration
                );

            float alpha =
                Mathf.Lerp(
                    startColor.a,
                    0f,
                    progress
                );

            spriteRenderer.color =
                new Color(
                    startColor.r,
                    startColor.g,
                    startColor.b,
                    alpha
                );

            yield return null;
        }

        spriteRenderer.color =
            new Color(
                startColor.r,
                startColor.g,
                startColor.b,
                0f
            );

        Destroy(gameObject);
    }
}
