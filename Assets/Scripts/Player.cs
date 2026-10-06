using UnityEngine;
using System;
using System.Collections;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody2D))]
public class Player : MonoBehaviour
{
    [Header("Movement")]
    public float walkSpeed = 3f;
    public float runSpeed = 6f;
    public KeyCode runKey = KeyCode.LeftShift;
    public Animator animator;

    // ============================================================
    // TUTORIAL / CONTROL TOGGLES
    // ============================================================

    [Header("Tutorial / Control Toggles")]
    [Tooltip("If disabled, the player cannot move.")]
    public bool movementEnabled = true;

    [Tooltip("If disabled, the player cannot attack.")]
    public bool attacksEnabled = true;

    [Tooltip("If disabled, the player cannot sprint.")]
    public bool sprintEnabled = true;

    [Tooltip("If enabled, stamina stays full and sprint does not drain stamina.")]
    public bool unlimitedStamina = false;

    [Header("Stamina")]
    public float maxStamina = 100f;
    public float stamina;
    public float runStaminaDrainPerSecond = 10f;
    public float staminaRegenDelay = 3f;
    public float staminaRegenDuration = 5f;

    [Header("Stats")]
    public float baseMaxHealth = 3f;
    public float maxHealth;
    public float health;

    [Header("Armor Bonuses")]
    public float helmetHPBonus;
    public float chestplateHPBonus;
    public float leggingsHPBonus;
    public float bootsHPBonus;

    // ============================================================
    // ENEMY ATTACK KNOCKBACK
    // ============================================================

    [Header("Enemy Attack Knockback")]
    private Coroutine enemyKnockbackCoroutine;

    [Header("Helmet")]
    public HelmetAnimationsManager helmetManager;

    [Header("Chestplate")]
    public ChestplateAnimationsManager chestplateManager;

    [Header("Leggings")]
    public LeggingsAnimationsManager leggingsManager;

    [Header("Boots")]
    public BootsAnimationsManager bootsManager;

    [Header("Money")]
    public float money = 0f;

    public float Money
    {
        get => money;
        private set
        {
            money = Mathf.Round(value * 100f) / 100f;

            PlayerPrefs.SetFloat(
                "PlayerMoney",
                money
            );

            PlayerPrefs.Save();

            OnMoneyChanged?.Invoke(money);
        }
    }

    public event System.Action<float> OnMoneyChanged;

    [Header("Respawn")]
    public Transform spawnPoint;
    public float respawnTime = 3f;

    // ============================================================
    // SCENE RECONNECT
    // ============================================================

    [Header("Scene Reconnect")]
    [Tooltip("The name of the spawn point in the scene. Leave this as the name of your existing spawn point.")]
    public string spawnPointName = "PlayerSpawnPoint";

    private bool reconnectingToScene;

    // Prevents old death/respawn routines from surviving a
    // house-caused scene reload.
    private bool sceneResetting;

    [Header("References")]
    public House house;

    [Header("Combat - Hurt")]
    public float hurtAnimLockTime = 0.4f;
    public float invincibilityTime = 3f;

    [Header("Combat - Attack")]
    public float attackDamage = 1f;
    public float walkAttackCooldown = 0.5f;
    public float runAttackCooldown = 0.8f;
    public float attackRadius = 0.4f;
    public LayerMask enemyLayer;

    [Header("Attack Input Buffer")]
    [Tooltip("How long an early attack click is remembered.")]
    public float attackBufferTime = 0.2f;

    private float attackBufferTimer;
    private bool bufferedAttackRunning;

    [Header("Attack Animation")]
    [Tooltip("Attack speed multiplier. 1 = normal speed, 1.25 = 25% faster, 2 = twice as fast.")]
    public float attackSpeedMultiplier = 1f;

    // ============================================================
    // ATTACK KNOCKBACK
    // ============================================================

    [Header("Attack Knockback")]
    public float attackKnockbackForce = 1.5f;
    public float attackKnockbackDuration = 0.1f;

    private Coroutine attackKnockbackCoroutine;

    private bool isHurt;
    private bool isInvincible;
    private bool isDead;
    private bool isAttacking;

    // ============================================================
    // DEATH / RESPAWN
    // ============================================================

    private Coroutine deathRespawnCoroutine;

    // ============================================================
    // GAMEPLAY DISABLED
    // ============================================================

    private bool gameplayDisabled;
    public bool GameplayDisabled => gameplayDisabled;

    private float attackTimer;
    private float lastRunTime;

    public bool IsDead => isDead;

    private Rigidbody2D rb;
    private Vector2 movement;
    private Collider2D col;

    public SpriteRenderer sr;

    private Vector2 animDirection = Vector2.down;

    private bool upHeld;
    private bool downHeld;
    private bool leftHeld;
    private bool rightHeld;

    public static Player Instance { get; private set; }

    // ============================================================
    // AWAKE
    // ============================================================

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        DontDestroyOnLoad(gameObject);

        sr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();

        // Automatically find the armor managers on child objects
        helmetManager = GetComponentInChildren<HelmetAnimationsManager>(true);
        chestplateManager = GetComponentInChildren<ChestplateAnimationsManager>(true);
        leggingsManager = GetComponentInChildren<LeggingsAnimationsManager>(true);
        bootsManager = GetComponentInChildren<BootsAnimationsManager>(true);

        // ========================================================
        // REMEMBER THE EXISTING SPAWN POINT NAME
        // ========================================================

        if (spawnPoint != null)
        {
            spawnPointName = spawnPoint.name;
        }

        stamina = maxStamina;

        RespawnInstant();

        Money = PlayerPrefs.GetFloat(
            "PlayerMoney",
            0f
        );

        LoadArmorStats();

        // ========================================================
        // LOAD ATTACK SPEED UPGRADE
        // ========================================================

        LoadAttackSpeedStats();

        // ========================================================
        // LOAD ATTACK DAMAGE UPGRADE
        // ========================================================

        LoadAttackDamageStats();

        Debug.Log(
            "Helmet Manager: " + helmetManager
        );

        Debug.Log(
            "Chestplate Manager: " + chestplateManager
        );

        Debug.Log(
            "Leggings Manager: " + leggingsManager
        );

        Debug.Log(
            "Boots Manager: " + bootsManager
        );
    }

    // ============================================================
    // ENABLE
    // ============================================================

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    // ============================================================
    // DISABLE
    // ============================================================

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // ============================================================
    // SCENE LOADED
    // ============================================================

    private void OnSceneLoaded(
        Scene scene,
        LoadSceneMode mode
    )
    {
        // ========================================================
        // IMPORTANT
        // ========================================================
        //
        // A new scene means any pending death respawn from the
        // previous scene is now invalid.
        //
        // This is especially important when the HOUSE dies while
        // the player is already dead.
        // ========================================================

        sceneResetting = true;

        CancelPendingDeathRespawn();

        // Don't try to reconnect during the special rebirth
        // dialogue transition.

        if (reconnectingToScene)
            return;

        StartCoroutine(
            ReconnectToScene()
        );
    }

    // ============================================================
    // CANCEL PENDING DEATH RESPAWN
    // ============================================================

    private void CancelPendingDeathRespawn()
    {
        if (deathRespawnCoroutine != null)
        {
            StopCoroutine(
                deathRespawnCoroutine
            );

            deathRespawnCoroutine = null;
        }
    }

    // ============================================================
    // RECONNECT TO NEW SCENE
    // ============================================================

    private IEnumerator ReconnectToScene()
    {
        reconnectingToScene = true;

        // Wait until the new scene has finished creating
        // all of its objects.

        yield return null;
        yield return new WaitForEndOfFrame();

        // ========================================================
        // LOAD ATTACK SPEED UPGRADE
        // ========================================================

        LoadAttackSpeedStats();

        // ========================================================
        // LOAD ATTACK DAMAGE UPGRADE
        // ========================================================

        LoadAttackDamageStats();

        // ========================================================
        // FIND NEW SPAWN POINT
        // ========================================================

        Transform newSpawnPoint = null;

        if (!string.IsNullOrEmpty(spawnPointName))
        {
            GameObject spawnObject =
                GameObject.Find(spawnPointName);

            if (spawnObject != null)
            {
                newSpawnPoint =
                    spawnObject.transform;
            }
        }

        // ========================================================
        // RECONNECT SPAWN POINT
        // ========================================================

        if (newSpawnPoint != null)
        {
            spawnPoint = newSpawnPoint;
        }

        // ========================================================
        // RECONNECT HOUSE
        // ========================================================

        House newHouse =
            FindFirstObjectByType<House>();

        if (newHouse != null)
        {
            house = newHouse;
        }
        else
        {
            Debug.LogWarning(
                "Player could not find House in the new scene."
            );
        }

        // ========================================================
        // RECONNECT ARMOR MANAGERS
        // ========================================================

        helmetManager =
            GetComponentInChildren<HelmetAnimationsManager>(true);

        chestplateManager =
            GetComponentInChildren<ChestplateAnimationsManager>(true);

        leggingsManager =
            GetComponentInChildren<LeggingsAnimationsManager>(true);

        bootsManager =
            GetComponentInChildren<BootsAnimationsManager>(true);

        // ========================================================
        // RESET PLAYER
        // ========================================================

        if (spawnPoint != null)
        {
            RespawnInstant();
        }

        sceneResetting = false;
        reconnectingToScene = false;
    }

    // ============================================================
    // ATTACK SPEED
    // ============================================================

    public void LoadAttackSpeedStats()
    {
        int attackSpeedLevel =
            PlayerPrefs.GetInt(
                "AttackSpeedUpgradeLevel",
                0
            );

        attackSpeedLevel =
            Mathf.Clamp(
                attackSpeedLevel,
                0,
                10
            );

        float attackTime =
            0.8f - (attackSpeedLevel * 0.05f);

        attackTime =
            Mathf.Clamp(
                attackTime,
                0.3f,
                0.8f
            );

        // Your original animation is 1 second.
        // This makes Level 0 = 0.80 seconds.

        attackSpeedMultiplier =
            1f / attackTime;

        Debug.Log(
            "Attack Speed Level: " +
            attackSpeedLevel +
            " | Attack Time: " +
            attackTime.ToString("0.00") +
            " sec" +
            " | Animator Speed: " +
            attackSpeedMultiplier.ToString("0.00") +
            "x"
        );
    }

    // ============================================================
    // ATTACK DAMAGE
    // ============================================================

    public void LoadAttackDamageStats()
    {
        int attackDamageLevel =
            PlayerPrefs.GetInt(
                "AttackDamageUpgradeLevel",
                0
            );

        attackDamageLevel =
            Mathf.Clamp(
                attackDamageLevel,
                0,
                10
            );

        // Level 0 = 1 damage
        // Level 1 = 2 damage
        // Level 2 = 3 damage
        // ...
        // Level 10 = 11 damage

        attackDamage =
            1f + attackDamageLevel;

        Debug.Log(
            "Attack Damage Level: " +
            attackDamageLevel +
            " | Damage: " +
            attackDamage.ToString("0")
        );
    }

    // ============================================================
    // UPDATE
    // ============================================================

    void Update()
    {
        // ========================================================
        // GAMEPLAY DISABLED
        // ========================================================

        if (gameplayDisabled)
            return;

        if (
            isDead ||
            isHurt ||
            reconnectingToScene ||
            sceneResetting
        )
        {
            return;
        }

        // ========================================================
        // ATTACK COOLDOWN
        // ========================================================

        if (attackTimer > 0f)
            attackTimer -= Time.deltaTime;

        // ========================================================
        // MOVEMENT INPUT - WASD ONLY
        // ========================================================

        if (movementEnabled)
        {
            float rawX = 0f;
            float rawY = 0f;

            if (Input.GetKey(KeyCode.A))
                rawX -= 1f;

            if (Input.GetKey(KeyCode.D))
                rawX += 1f;

            if (Input.GetKey(KeyCode.S))
                rawY -= 1f;

            if (Input.GetKey(KeyCode.W))
                rawY += 1f;

            movement =
                new Vector2(
                    rawX,
                    rawY
                ).normalized;
        }
        else
        {
            movement = Vector2.zero;
        }

        // ========================================================
        // WALKING STATE
        // ========================================================

        bool isWalking =
            movement.sqrMagnitude > 0.001f;

        // ========================================================
        // SPRINT
        // ========================================================

        bool wantsToRun =
            sprintEnabled &&
            Input.GetKey(runKey);

        bool canRun =
            unlimitedStamina ||
            stamina > 0f;

        bool isRunning =
            isWalking &&
            wantsToRun &&
            canRun;

        animator.SetBool(
            "isWalking",
            isWalking
        );

        animator.SetBool(
            "isRunning",
            isRunning
        );

        HandleStamina(
            isRunning
        );

        // ========================================================
        // ATTACK INPUT
        // ========================================================

        if (
            attacksEnabled &&
            Input.GetMouseButtonDown(0)
        )
        {
            // Attack immediately if available

            if (
                !isAttacking &&
                attackTimer <= 0f
            )
            {
                StartAttack(isRunning);
            }
            else
            {
                // Attack is not available yet.
                // Remember the click temporarily.

                attackBufferTimer =
                    attackBufferTime;

                bufferedAttackRunning =
                    isRunning;
            }
        }

        // ========================================================
        // ATTACK INPUT BUFFER
        // ========================================================

        if (
            attacksEnabled &&
            attackBufferTimer > 0f
        )
        {
            attackBufferTimer -=
                Time.deltaTime;

            // Attack became available while
            // the buffered input is still active.

            if (
                !isAttacking &&
                attackTimer <= 0f
            )
            {
                StartAttack(
                    bufferedAttackRunning
                );

                // Clear the buffered input

                attackBufferTimer = 0f;
            }
        }

        // ========================================================
        // IF ATTACKS ARE DISABLED, CLEAR BUFFER
        // ========================================================

        if (!attacksEnabled)
        {
            attackBufferTimer = 0f;
            bufferedAttackRunning = false;
        }

        // ========================================================
        // FACING
        // ========================================================

        UpdateFacing();

        animator.SetFloat(
            "InputX",
            animDirection.x
        );

        animator.SetFloat(
            "InputY",
            animDirection.y
        );

        animator.SetFloat(
            "LastInputX",
            animDirection.x
        );

        animator.SetFloat(
            "LastInputY",
            animDirection.y
        );
    }

    // ============================================================
    // FIXED UPDATE
    // ============================================================

    void FixedUpdate()
    {
        if (
            gameplayDisabled ||
            sceneResetting
        )
        {
            rb.linearVelocity =
                Vector2.zero;

            return;
        }

        if (
            isDead ||
            isHurt ||
            reconnectingToScene
        )
        {
            return;
        }

        if (attackKnockbackCoroutine != null)
        {
            return;
        }

        if (!movementEnabled)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        bool running =
            sprintEnabled &&
            Input.GetKey(runKey) &&
            (
                unlimitedStamina ||
                stamina > 0f
            );

        float speed =
            running
                ? runSpeed
                : walkSpeed;

        rb.linearVelocity =
            movement * speed;
    }

    // ============================================================
    // UPDATE FACING
    // ============================================================

    // ============================================================
    // UPDATE FACING
    // ============================================================

    void UpdateFacing()
    {
        // If movement is disabled, completely lock the
        // current facing direction.
        if (!movementEnabled)
        {
            return;
        }

        upHeld =
            Input.GetKey(KeyCode.W);

        downHeld =
            Input.GetKey(KeyCode.S);

        leftHeld =
            Input.GetKey(KeyCode.A);

        rightHeld =
            Input.GetKey(KeyCode.D);

        if (Input.GetKeyDown(KeyCode.W))
        {
            animDirection =
                Vector2.up;
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            animDirection =
                Vector2.down;
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            animDirection =
                Vector2.left;
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            animDirection =
                Vector2.right;
        }

        if (
            animDirection == Vector2.up &&
            !upHeld
        )
        {
            SetFacingFromHeld();
        }
        else if (
            animDirection == Vector2.down &&
            !downHeld
        )
        {
            SetFacingFromHeld();
        }
        else if (
            animDirection == Vector2.left &&
            !leftHeld
        )
        {
            SetFacingFromHeld();
        }
        else if (
            animDirection == Vector2.right &&
            !rightHeld
        )
        {
            SetFacingFromHeld();
        }
    }

    // ============================================================
    // SET FACING FROM HELD
    // ============================================================

    void SetFacingFromHeld()
    {
        if (upHeld)
            animDirection = Vector2.up;
        else if (downHeld)
            animDirection = Vector2.down;
        else if (leftHeld)
            animDirection = Vector2.left;
        else if (rightHeld)
            animDirection = Vector2.right;
    }

    // ============================================================
    // STAMINA
    // ============================================================

    void HandleStamina(bool isRunning)
    {
        // ========================================================
        // UNLIMITED STAMINA
        // ========================================================

        if (unlimitedStamina)
        {
            stamina = maxStamina;
            return;
        }

        // ========================================================
        // NORMAL STAMINA
        // ========================================================

        if (isRunning)
        {
            stamina -=
                runStaminaDrainPerSecond *
                Time.deltaTime;

            stamina =
                Mathf.Clamp(
                    stamina,
                    0f,
                    maxStamina
                );

            lastRunTime =
                Time.time;
        }
        else
        {
            if (
                Time.time >=
                lastRunTime +
                staminaRegenDelay
            )
            {
                float regenRate =
                    maxStamina /
                    staminaRegenDuration;

                stamina +=
                    regenRate *
                    Time.deltaTime;

                stamina =
                    Mathf.Clamp(
                        stamina,
                        0f,
                        maxStamina
                    );
            }
        }
    }

    // ============================================================
    // ATTACK
    // ============================================================

    void StartAttack(bool running)
    {
        if (
            !attacksEnabled ||
            gameplayDisabled ||
            sceneResetting
        )
        {
            return;
        }

        Debug.Log(chestplateManager);

        isAttacking = true;

        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlayPlayerAttack();
        }

        animator.SetTrigger("Attack");

        animator.speed =
            Mathf.Max(
                0.01f,
                attackSpeedMultiplier
            );

        Debug.Log(
            "Attack Animation Speed: " +
            attackSpeedMultiplier +
            "x"
        );

        if (helmetManager != null)
        {
            helmetManager.TriggerAttack();
        }

        if (chestplateManager != null)
        {
            chestplateManager.TriggerAttack();
        }

        if (leggingsManager != null)
        {
            leggingsManager.TriggerAttack();
        }

        if (bootsManager != null)
        {
            bootsManager.TriggerAttack();
        }

        animator.SetFloat(
            "LastInputX",
            animDirection.x
        );

        animator.SetFloat(
            "LastInputY",
            animDirection.y
        );

        attackTimer =
            running
                ? runAttackCooldown
                : walkAttackCooldown;
    }

    // ============================================================
    // ATTACK DAMAGE
    // ============================================================

    public void DealAttackDamage()
    {
        if (
            !attacksEnabled ||
            gameplayDisabled ||
            sceneResetting
        )
        {
            return;
        }

        Vector2 center =
            (Vector2)transform.position +
            animDirection *
            attackRadius;

        Collider2D[] hits =
            Physics2D.OverlapCircleAll(
                center,
                attackRadius,
                enemyLayer
            );

        bool successfullyHit = false;

        foreach (Collider2D hit in hits)
        {
            EnemyHitbox hitbox =
                hit.GetComponent<EnemyHitbox>();

            if (hitbox != null)
            {
                bool damageDealt =
                    hitbox.TryTakeDamage(
                        attackDamage,
                        hit
                    );

                if (damageDealt)
                {
                    successfullyHit = true;
                }
            }
        }

        if (successfullyHit)
        {
            if (SoundManager.Instance != null)
            {
                SoundManager.Instance.PlayPlayerHit();
            }

            ApplyAttackKnockback();

            bool cameraShakeAllowed =
                SettingsManager.Instance == null ||
                SettingsManager.Instance.cameraShakeEnabled;

            if (cameraShakeAllowed)
            {
                if (CameraShake.Instance != null)
                {
                    CameraShake.Instance.ShakeOnHit();
                }
            }
        }
    }

    // ============================================================
    // ATTACK KNOCKBACK
    // ============================================================

    void ApplyAttackKnockback()
    {
        if (
            isDead ||
            gameplayDisabled ||
            sceneResetting
        )
        {
            return;
        }

        Vector2 knockbackDirection =
            -animDirection;

        if (
            knockbackDirection ==
            Vector2.zero
        )
        {
            return;
        }

        if (attackKnockbackCoroutine != null)
        {
            StopCoroutine(
                attackKnockbackCoroutine
            );
        }

        attackKnockbackCoroutine =
            StartCoroutine(
                AttackKnockbackRoutine(
                    knockbackDirection
                )
            );
    }

    IEnumerator AttackKnockbackRoutine(
        Vector2 direction
    )
    {
        if (
            isDead ||
            gameplayDisabled ||
            sceneResetting
        )
        {
            yield break;
        }

        direction =
            direction.normalized;

        float elapsed = 0f;

        while (
            elapsed <
            attackKnockbackDuration
        )
        {
            if (
                isDead ||
                gameplayDisabled ||
                sceneResetting
            )
            {
                yield break;
            }

            float progress =
                elapsed /
                attackKnockbackDuration;

            float strength =
                Mathf.Lerp(
                    attackKnockbackForce,
                    0f,
                    progress
                );

            rb.linearVelocity =
                direction * strength;

            elapsed +=
                Time.fixedDeltaTime;

            yield return
                new WaitForFixedUpdate();
        }

        rb.linearVelocity =
            Vector2.zero;

        attackKnockbackCoroutine = null;
    }

    public void EndAttack()
    {
        isAttacking = false;
        animator.speed = 1f;
    }

    // ============================================================
    // DAMAGE / DEATH
    // ============================================================

    public void TakeDamage(int amount)
    {
        if (
            gameplayDisabled ||
            sceneResetting
        )
        {
            return;
        }

        if (
            isDead ||
            isInvincible
        )
        {
            return;
        }

        health -= amount;

        health =
            Mathf.Max(
                0,
                health
            );

        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlayPlayerDamaged();
        }

        if (CombatEffects.Instance != null)
        {
            CombatEffects.Instance.FreezeFrame();
        }

        ForceCancelAttack();

        animator.SetTrigger("Hurt");

        if (helmetManager != null)
        {
            helmetManager.TriggerHurt();
        }

        if (chestplateManager != null)
        {
            chestplateManager.TriggerHurt();
        }

        if (leggingsManager != null)
        {
            leggingsManager.TriggerHurt();
        }

        if (bootsManager != null)
        {
            bootsManager.TriggerHurt();
        }

        StartCoroutine(
            HurtRoutine()
        );

        if (health <= 0)
            Die();
    }

    public bool TrySpendMoney(float amount)
    {
        if (Money >= amount)
        {
            Money -= amount;
            return true;
        }

        return false;
    }

    void ForceCancelAttack()
    {
        if (!isAttacking)
            return;

        isAttacking = false;

        animator.speed = 1f;

        animator.ResetTrigger("Attack");
    }

    IEnumerator HurtRoutine()
    {
        isHurt = true;
        isInvincible = true;

        rb.linearVelocity =
            Vector2.zero;

        yield return new WaitForSeconds(
            hurtAnimLockTime
        );

        if (sceneResetting)
            yield break;

        isHurt = false;

        yield return new WaitForSeconds(
            invincibilityTime -
            hurtAnimLockTime
        );

        if (sceneResetting)
            yield break;

        isInvincible = false;
    }

    // ============================================================
    // DIE
    // ============================================================

    void Die()
    {
        if (
            isDead ||
            sceneResetting
        )
        {
            return;
        }

        isDead = true;

        // Make absolutely sure there isn't an older
        // respawn routine still waiting.

        CancelPendingDeathRespawn();

        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlayPlayerDeath();
        }

        if (PlayerDeaths.Instance != null)
        {
            PlayerDeaths.Instance.AddDeath();
        }

        rb.linearVelocity =
            Vector2.zero;

        col.enabled = false;

        animator.speed = 1f;

        animator.SetTrigger("Die");

        if (helmetManager != null)
        {
            helmetManager.TriggerDie();
        }

        if (chestplateManager != null)
        {
            chestplateManager.TriggerDie();
        }

        if (leggingsManager != null)
        {
            leggingsManager.TriggerDie();
        }

        if (bootsManager != null)
        {
            bootsManager.TriggerDie();
        }

        if (RebirthIntroManager.Instance != null)
        {
            bool introTriggered =
                RebirthIntroManager.Instance
                    .TryPlayRebirthIntro();

            if (introTriggered)
            {
                return;
            }
        }

        deathRespawnCoroutine =
            StartCoroutine(
                DeathRoutine()
            );
    }

    // ============================================================
    // DEATH ROUTINE
    // ============================================================

    IEnumerator DeathRoutine()
    {
        yield return new WaitForSeconds(1f);

        // The house may have been destroyed while the player
        // was dead. If the scene is resetting, this coroutine
        // must NOT continue.

        if (
            sceneResetting ||
            reconnectingToScene
        )
        {
            deathRespawnCoroutine = null;
            yield break;
        }

        if (
            house != null &&
            house.health > 0
        )
        {
            yield return new WaitForSeconds(
                respawnTime
            );

            // Check AGAIN after the respawn delay.
            //
            // The house could have been destroyed during
            // those three seconds.

            if (
                sceneResetting ||
                reconnectingToScene
            )
            {
                deathRespawnCoroutine = null;
                yield break;
            }

            if (
                house == null ||
                house.health <= 0
            )
            {
                deathRespawnCoroutine = null;
                yield break;
            }

            RespawnInstant();

            if (SoundManager.Instance != null)
            {
                SoundManager.Instance.PlayPlayerRespawn();
            }
        }

        deathRespawnCoroutine = null;
    }

    // ============================================================
    // RESPAWN
    // ============================================================

    void RespawnInstant()
    {
        // ========================================================
        // CANCEL ANY OLD DEATH RESPAWN
        // ========================================================

        if (
            deathRespawnCoroutine != null &&
            !sceneResetting
        )
        {
            StopCoroutine(
                deathRespawnCoroutine
            );

            deathRespawnCoroutine = null;
        }

        // ========================================================
        // FIND SPAWN POINT
        // ========================================================

        if (spawnPoint == null)
        {
            if (!string.IsNullOrEmpty(spawnPointName))
            {
                GameObject spawnObject =
                    GameObject.Find(
                        spawnPointName
                    );

                if (spawnObject != null)
                {
                    spawnPoint =
                        spawnObject.transform;
                }
            }
        }

        if (spawnPoint == null)
        {
            Debug.LogError(
                "RESPAWN FAILED: No spawn point was found. " +
                "Make sure the scene contains an object named '" +
                spawnPointName +
                "'."
            );

            return;
        }

        // ========================================================
        // LOAD ARMOR
        // ========================================================

        LoadArmorStats();

        // ========================================================
        // RESET PLAYER STATE
        // ========================================================

        health = maxHealth;
        stamina = maxStamina;

        isDead = false;
        isHurt = false;
        isInvincible = false;
        isAttacking = false;

        // Clear any buffered attack when respawning

        attackBufferTimer = 0f;
        bufferedAttackRunning = false;

        animator.speed = 1f;

        // ========================================================
        // STOP ATTACK KNOCKBACK
        // ========================================================

        if (attackKnockbackCoroutine != null)
        {
            StopCoroutine(
                attackKnockbackCoroutine
            );

            attackKnockbackCoroutine = null;
        }

        // ========================================================
        // STOP ENEMY KNOCKBACK
        // ========================================================

        if (enemyKnockbackCoroutine != null)
        {
            StopCoroutine(
                enemyKnockbackCoroutine
            );

            enemyKnockbackCoroutine = null;
        }

        // ========================================================
        // RESET PHYSICS
        // ========================================================

        rb.linearVelocity =
            Vector2.zero;

        transform.position =
            spawnPoint.position;

        transform.rotation =
            Quaternion.identity;

        col.enabled = true;

        // ========================================================
        // RESET FACING
        // ========================================================

        animDirection =
            Vector2.down;

        upHeld = false;
        downHeld = false;
        leftHeld = false;
        rightHeld = false;

        movement =
            Vector2.zero;

        // ========================================================
        // RESET ANIMATOR
        // ========================================================

        animator.SetBool(
            "isWalking",
            false
        );

        animator.SetBool(
            "isRunning",
            false
        );

        animator.SetFloat(
            "InputX",
            0f
        );

        animator.SetFloat(
            "InputY",
            -1f
        );

        animator.SetFloat(
            "LastInputX",
            0f
        );

        animator.SetFloat(
            "LastInputY",
            -1f
        );

        animator.ResetTrigger("Attack");
        animator.ResetTrigger("Hurt");
        animator.ResetTrigger("Die");

        animator.Rebind();

        animator.Update(0f);

        animator.Play(
            "Idle",
            0,
            0f
        );

        if (helmetManager != null)
        {
            helmetManager.ResetAnimator();
        }

        if (chestplateManager != null)
        {
            chestplateManager.ResetAnimator();
        }

        if (leggingsManager != null)
        {
            leggingsManager.ResetAnimator();
        }

        if (bootsManager != null)
        {
            bootsManager.ResetAnimator();
        }

        Debug.Log(
            "PLAYER SUCCESSFULLY RESPAWNED AT: " +
            spawnPoint.position
        );
    }

    // ============================================================
    // ENEMY KNOCKBACK
    // ============================================================

    public void ApplyEnemyKnockback(
        Vector2 direction,
        float force,
        float duration
    )
    {
        if (
            isDead ||
            gameplayDisabled ||
            sceneResetting
        )
        {
            return;
        }

        if (enemyKnockbackCoroutine != null)
        {
            StopCoroutine(
                enemyKnockbackCoroutine
            );
        }

        enemyKnockbackCoroutine =
            StartCoroutine(
                EnemyKnockbackRoutine(
                    direction,
                    force,
                    duration
                )
            );
    }

    private IEnumerator EnemyKnockbackRoutine(
        Vector2 direction,
        float force,
        float duration
    )
    {
        if (
            isDead ||
            gameplayDisabled ||
            sceneResetting
        )
        {
            yield break;
        }

        direction =
            direction.normalized;

        float elapsed = 0f;

        while (elapsed < duration)
        {
            if (
                isDead ||
                gameplayDisabled ||
                sceneResetting
            )
            {
                yield break;
            }

            float progress =
                elapsed /
                duration;

            float strength =
                Mathf.Lerp(
                    force,
                    0f,
                    progress
                );

            rb.linearVelocity =
                direction * strength;

            elapsed +=
                Time.fixedDeltaTime;

            yield return
                new WaitForFixedUpdate();
        }

        rb.linearVelocity =
            Vector2.zero;

        enemyKnockbackCoroutine = null;
    }

    // ============================================================
    // GAMEPLAY ENABLE / DISABLE
    // ============================================================

    public void DisableGameplay()
    {
        gameplayDisabled = true;

        movement =
            Vector2.zero;

        rb.linearVelocity =
            Vector2.zero;

        isAttacking = false;
        attackTimer = 0f;

        // ========================================================
        // CLEAR ATTACK INPUT BUFFER
        // ========================================================

        attackBufferTimer = 0f;
        bufferedAttackRunning = false;

        animator.speed = 1f;

        // ========================================================
        // STOP ATTACK KNOCKBACK
        // ========================================================

        if (attackKnockbackCoroutine != null)
        {
            StopCoroutine(
                attackKnockbackCoroutine
            );

            attackKnockbackCoroutine = null;
        }

        // ========================================================
        // STOP ENEMY KNOCKBACK
        // ========================================================

        if (enemyKnockbackCoroutine != null)
        {
            StopCoroutine(
                enemyKnockbackCoroutine
            );

            enemyKnockbackCoroutine = null;
        }

        // ========================================================
        // CLEAR MOVEMENT INPUT
        // ========================================================

        movement =
            Vector2.zero;

        upHeld = false;
        downHeld = false;
        leftHeld = false;
        rightHeld = false;

        // ========================================================
        // RESET ANIMATOR
        // ========================================================

        animator.ResetTrigger("Attack");

        animator.SetBool(
            "isWalking",
            false
        );

        animator.SetBool(
            "isRunning",
            false
        );

        animator.SetFloat(
            "InputX",
            0f
        );

        animator.SetFloat(
            "InputY",
            -1f
        );

        animator.SetFloat(
            "LastInputX",
            0f
        );

        animator.SetFloat(
            "LastInputY",
            -1f
        );

        Debug.Log(
            "PLAYER GAMEPLAY DISABLED."
        );
    }

    public void EnableGameplay()
    {
        gameplayDisabled = false;

        movement =
            Vector2.zero;

        rb.linearVelocity =
            Vector2.zero;

        isAttacking = false;
        attackTimer = 0f;

        // Clear any buffered attack

        attackBufferTimer = 0f;
        bufferedAttackRunning = false;

        animator.speed = 1f;

        animator.SetBool(
            "isWalking",
            false
        );

        animator.SetBool(
            "isRunning",
            false
        );

        Debug.Log(
            "PLAYER GAMEPLAY ENABLED."
        );
    }

    // ============================================================
    // HEALTH
    // ============================================================

    public void UpdateMaxHealth()
    {
        maxHealth =
            baseMaxHealth +
            helmetHPBonus +
            chestplateHPBonus +
            leggingsHPBonus +
            bootsHPBonus;

        health = maxHealth;
    }

    public void SetHelmetBonus(float amount)
    {
        helmetHPBonus = amount;

        PlayerPrefs.SetFloat(
            "HelmetHPBonus",
            amount
        );

        PlayerPrefs.Save();

        UpdateMaxHealth();
    }

    public void LoadArmorStats()
    {
        helmetHPBonus =
            PlayerPrefs.GetFloat(
                "HelmetHPBonus",
                0f
            );

        chestplateHPBonus =
            PlayerPrefs.GetFloat(
                "ChestplateHPBonus",
                0f
            );

        leggingsHPBonus =
            PlayerPrefs.GetFloat(
                "LeggingsHPBonus",
                0f
            );

        bootsHPBonus =
            PlayerPrefs.GetFloat(
                "BootsHPBonus",
                0f
            );

        maxHealth =
            baseMaxHealth +
            helmetHPBonus +
            chestplateHPBonus +
            leggingsHPBonus +
            bootsHPBonus;

        health = maxHealth;

        Debug.Log(
            "Armor HP Total: " +
            (
                helmetHPBonus +
                chestplateHPBonus +
                leggingsHPBonus +
                bootsHPBonus
            )
        );

        Debug.Log(
            "Helmet: " +
            helmetHPBonus +
            " Chestplate: " +
            chestplateHPBonus +
            " Total HP: " +
            maxHealth
        );
    }

    // ============================================================
    // MONEY FUNCTIONS
    // ============================================================

    public void AddMoney(float amount)
    {
        Money += amount;

        PlayerPrefs.SetFloat(
            "PlayerMoney",
            Money
        );

        PlayerPrefs.Save();
    }

    public bool SpendMoney(float amount)
    {
        if (Money >= amount)
        {
            Money -= amount;

            PlayerPrefs.SetFloat(
                "PlayerMoney",
                Money
            );

            PlayerPrefs.Save();

            return true;
        }

        return false;
    }
}