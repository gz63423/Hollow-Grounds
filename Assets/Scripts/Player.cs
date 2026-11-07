using UnityEngine;
using System;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

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

    public bool IsSceneReady =>
    !sceneResetting &&
    !reconnectingToScene;

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
    public event Action OnPlayerAttack;

    [Header("Respawn")]
    public Transform spawnPoint;
    public float respawnTime = 3f;


    [Header("Scene Reconnect")]
    [Tooltip("The name of the spawn point in the scene. Leave this as the name of your existing spawn point.")]
    public string spawnPointName = "PlayerSpawnPoint";

    private bool reconnectingToScene;


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

    [Header("Buffered Attack Synchronization")]
    [Tooltip("Small delay before a buffered attack starts, giving all armor animation managers time to receive the same trigger.")]
    public float bufferedAttackStartDelay = 0.06f;

    [Tooltip("If enabled, all armor attack triggers are sent together after the buffered-attack delay.")]
    public bool synchronizeBufferedArmorAttacks = true;

    private Coroutine bufferedAttackStartCoroutine;
    private int attackRequestVersion;

    [Header("Attack Animation")]
    [Tooltip("Attack speed multiplier. 1 = normal speed, 1.25 = 25% faster, 2 = twice as fast.")]
    public float attackSpeedMultiplier = 1f;

    //Knockback

    [Header("Attack Knockback")]
    public float attackKnockbackForce = 1.5f;
    public float attackKnockbackDuration = 0.1f;

    private Coroutine attackKnockbackCoroutine;

    private bool isHurt;
    private bool isInvincible;
    private bool isDead;
    private bool isAttacking;


    private Coroutine deathRespawnCoroutine;


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

        //Attack Speed

        LoadAttackSpeedStats();

        //Attack Damage

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

        CancelBufferedAttackStart();
    }

    // ============================================================
    // SCENE LOADED
    // ============================================================

    private void OnSceneLoaded(
        Scene scene,
        LoadSceneMode mode
    )
    {
        sceneResetting = true;

        CancelPendingDeathRespawn();
        CancelBufferedAttackStart();

        if (reconnectingToScene)
            return;

        StartCoroutine(
            ReconnectToScene()
        );
    }

    private void LateUpdate()
    {
        // Armor animation synchronization is handled by the
        // individual armor animation manager scripts.
        // Do not synchronize the armor animators here.
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

        yield return null;
        yield return new WaitForEndOfFrame();

        LoadAttackSpeedStats();
        LoadAttackDamageStats();

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

        if (newSpawnPoint != null)
        {
            spawnPoint = newSpawnPoint;
        }

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

        helmetManager =
            GetComponentInChildren<HelmetAnimationsManager>(true);

        chestplateManager =
            GetComponentInChildren<ChestplateAnimationsManager>(true);

        leggingsManager =
            GetComponentInChildren<LeggingsAnimationsManager>(true);

        bootsManager =
            GetComponentInChildren<BootsAnimationsManager>(true);

        if (spawnPoint != null)
        {
            RespawnInstant();
        }

        sceneResetting = false;
        reconnectingToScene = false;
    }

    //Attack Speed (Can be changed with upgrades)

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

    //DMG

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

        if (attackTimer > 0f)
            attackTimer -= Time.deltaTime;

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

        bool isWalking =
            movement.sqrMagnitude > 0.001f;

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

        //Attack Input

        if (
            attacksEnabled &&
            Input.GetMouseButtonDown(0)
        )
        {
            if (
                !isAttacking &&
                attackTimer <= 0f &&
                bufferedAttackStartCoroutine == null
            )
            {
                StartAttack(isRunning);
            }
            else
            {
                attackBufferTimer =
                    attackBufferTime;

                bufferedAttackRunning =
                    isRunning;
            }
        }

        //Attack logic

        if (
            attacksEnabled &&
            attackBufferTimer > 0f
        )
        {
            attackBufferTimer -=
                Time.deltaTime;

            if (
                !isAttacking &&
                attackTimer <= 0f &&
                bufferedAttackStartCoroutine == null
            )
            {
                int requestVersion = attackRequestVersion;

                bufferedAttackStartCoroutine =
                    StartCoroutine(
                        StartBufferedAttackAfterDelay(
                            bufferedAttackRunning,
                            requestVersion
                        )
                    );

                attackBufferTimer = 0f;
            }
        }

        if (!attacksEnabled)
        {
            attackBufferTimer = 0f;
            bufferedAttackRunning = false;
            CancelBufferedAttackStart();
        }

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

    void UpdateFacing()
    {
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
        if (unlimitedStamina)
        {
            stamina = maxStamina;
            return;
        }

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

    //Player Attack

    void StartAttack(bool running)
    {
        if (
            !attacksEnabled ||
            gameplayDisabled ||
            sceneResetting ||
            isDead ||
            isHurt
        )
        {
            return;
        }

        // A normal and buffered attack both enter through this method, so the player and all armor managers receive their attack triggers from one synchronized point.
        CancelBufferedAttackStart();

        Debug.Log(chestplateManager);

        isAttacking = true;

        OnPlayerAttack?.Invoke();

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

        if (synchronizeBufferedArmorAttacks)
        {
            // Trigger armor managers together, after the same player attack setup. Their own scripts still control their animator synchronization.
            TriggerAllArmorAttacks();
        }
        else
        {
            TriggerAllArmorAttacks();
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

    private IEnumerator StartBufferedAttackAfterDelay(
        bool running,
        int requestVersion
    )
    {
        float delay =
            Mathf.Max(
                0f,
                bufferedAttackStartDelay
            );

        if (delay > 0f)
        {
            yield return new WaitForSeconds(delay);
        }
        else
        {
            yield return null;
        }

        bufferedAttackStartCoroutine = null;

        if (
            requestVersion != attackRequestVersion ||
            !attacksEnabled ||
            gameplayDisabled ||
            sceneResetting ||
            reconnectingToScene ||
            isDead ||
            isHurt ||
            isAttacking ||
            attackTimer > 0f
        )
        {
            yield break;
        }

        StartAttack(running);
    }

    private void CancelBufferedAttackStart()
    {
        attackRequestVersion++;

        if (bufferedAttackStartCoroutine != null)
        {
            StopCoroutine(bufferedAttackStartCoroutine);
            bufferedAttackStartCoroutine = null;
        }
    }

    private void TriggerAllArmorAttacks()
    {
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
    }

    //DMG

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

    //Knockback

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

    //DMG

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
        CancelBufferedAttackStart();

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

    //Death

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

        CancelBufferedAttackStart();
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

    //Death

    IEnumerator DeathRoutine()
    {
        yield return new WaitForSeconds(1f);

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

    //Rspawning

    void RespawnInstant()
    {
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

        CancelBufferedAttackStart();

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

        LoadArmorStats();

        health = maxHealth;
        stamina = maxStamina;

        isDead = false;
        isHurt = false;
        isInvincible = false;
        isAttacking = false;

        attackBufferTimer = 0f;
        bufferedAttackRunning = false;

        animator.speed = 1f;

        if (attackKnockbackCoroutine != null)
        {
            StopCoroutine(
                attackKnockbackCoroutine
            );

            attackKnockbackCoroutine = null;
        }

        if (enemyKnockbackCoroutine != null)
        {
            StopCoroutine(
                enemyKnockbackCoroutine
            );

            enemyKnockbackCoroutine = null;
        }

        rb.linearVelocity =
            Vector2.zero;

        transform.position =
            spawnPoint.position;

        transform.rotation =
            Quaternion.identity;

        col.enabled = true;

        animDirection =
            Vector2.down;

        upHeld = false;
        downHeld = false;
        leftHeld = false;
        rightHeld = false;

        movement =
            Vector2.zero;

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

    //KnockBack

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

        attackBufferTimer = 0f;
        bufferedAttackRunning = false;
        CancelBufferedAttackStart();

        animator.speed = 1f;

        if (attackKnockbackCoroutine != null)
        {
            StopCoroutine(
                attackKnockbackCoroutine
            );

            attackKnockbackCoroutine = null;
        }

        if (enemyKnockbackCoroutine != null)
        {
            StopCoroutine(
                enemyKnockbackCoroutine
            );

            enemyKnockbackCoroutine = null;
        }

        movement =
            Vector2.zero;

        upHeld = false;
        downHeld = false;
        leftHeld = false;
        rightHeld = false;

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

        attackBufferTimer = 0f;
        bufferedAttackRunning = false;
        CancelBufferedAttackStart();

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

    //HP

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

    //Money
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

