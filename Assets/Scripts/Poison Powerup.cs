using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// =============================================================
// POISON POWERUP
// =============================================================

public class PoisonPowerup : MonoBehaviour
{
    // =========================================================
    // POWERUP
    // =========================================================

    [Header("Powerup")]

    [Tooltip("Poison = 1")]
    public int powerupIndex = 1;

    [Tooltip("Poison cooldown in seconds.")]
    public float cooldown = 30f;


    // =========================================================
    // POISON SETTINGS
    // =========================================================

    [Header("Poison Settings")]

    [Tooltip("Poison damage per second at Level 1.")]
    public float unlockValue = 0.5f;

    [Tooltip("Poison damage per second at maximum level.")]
    public float maxValue = 5f;

    [Tooltip("How long enemies remain poisoned.")]
    public float poisonDuration = 5f;

    [Tooltip("Maximum Poison level.")]
    public int maxLevel = 10;

    [Tooltip("Maximum number of enemies Poison can affect.")]
    public int maxTargets = 5;


    // =========================================================
    // IGNORED ENEMIES
    // =========================================================

    [Header("Ignored Enemies")]

    [Tooltip("Enemy GameObjects that Poison will completely ignore.")]
    public List<GameObject> ignoredEnemies =
        new List<GameObject>();


    // =========================================================
    // HUD
    // =========================================================

    [Header("HUD")]

    public ActiveHUD activeHUD;


    // =========================================================
    // POISON VISUALS
    // =========================================================

    [Header("Poison Visuals")]

    [Tooltip("PoisonPowerupEffects component that controls the poison visuals and sound.")]
    public PoisonPowerupEffects poisonEffects;


    // =========================================================
    // INTERNAL
    // =========================================================

    private float cooldownTimer = 0f;

    private bool isPoisonActive = false;

    private Coroutine poisonCoroutine;


    // =========================================================
    // UNITY
    // =========================================================

    private void Start()
    {
        RefreshHUD();
    }


    private void Update()
    {
        // =====================================================
        // COOLDOWN
        // =====================================================

        UpdateCooldown();


        // =====================================================
        // ACTIVATION
        // =====================================================

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            TryActivate();
        }
    }


    // =========================================================
    // COOLDOWN
    // =========================================================

    private void UpdateCooldown()
    {
        if (cooldownTimer <= 0f)
        {
            return;
        }


        cooldownTimer -=
            Time.deltaTime;


        if (cooldownTimer <= 0f)
        {
            cooldownTimer = 0f;

            if (activeHUD != null)
            {
                activeHUD.SetReady();
            }

            return;
        }


        if (activeHUD != null &&
            cooldown > 0f)
        {
            float elapsedTime =
                cooldown -
                cooldownTimer;


            float fillAmount =
                elapsedTime /
                cooldown;


            fillAmount =
                Mathf.Clamp01(
                    fillAmount
                );


            activeHUD.SetCooldownFill(
                fillAmount
            );
        }
    }


    // =========================================================
    // TRY ACTIVATE
    // =========================================================

    private void TryActivate()
    {
        if (ActivePowerupManager.Instance == null)
        {
            Debug.LogWarning(
                "[PoisonPowerup] Cannot activate Poison because " +
                "ActivePowerupManager.Instance is null."
            );

            return;
        }


        int equippedPowerup =
            ActivePowerupManager.Instance.GetPowerupInSlot(0);


        if (equippedPowerup == -1)
        {
            Debug.Log(
                "[PoisonPowerup] Slot 0 is empty."
            );

            return;
        }


        if (equippedPowerup != powerupIndex)
        {
            Debug.Log(
                "[PoisonPowerup] Slot 0 contains " +
                ActivePowerupManager.Instance.GetPowerupName(
                    equippedPowerup
                ) +
                ", not Poison."
            );

            return;
        }


        FindActiveHUD();

        ActivatePoison();
    }


    // =========================================================
    // FIND HUD
    // =========================================================

    private void FindActiveHUD()
    {
        if (ActivePowerupManager.Instance == null)
        {
            return;
        }


        ActiveHUD[] huds =
            FindObjectsByType<ActiveHUD>(
                FindObjectsInactive.Include,
                FindObjectsSortMode.None
            );


        for (int i = 0;
             i < huds.Length;
             i++)
        {
            if (huds[i] == null)
            {
                continue;
            }


            if (huds[i].slotIndex == 0)
            {
                activeHUD =
                    huds[i];

                return;
            }
        }
    }


    // =========================================================
    // ACTIVATE POISON
    // =========================================================

    private void ActivatePoison()
    {
        if (isPoisonActive)
        {
            Debug.Log(
                "[PoisonPowerup] Poison is already active."
            );

            return;
        }


        if (cooldownTimer > 0f)
        {
            Debug.Log(
                "[PoisonPowerup] Poison is on cooldown: " +
                cooldownTimer.ToString("0.0") +
                " seconds remaining."
            );

            return;
        }


        const string saveKey =
            "Active_Poison";


        bool unlocked =
            PlayerPrefs.GetInt(
                saveKey + "_Unlocked",
                0
            ) == 1;


        if (!unlocked)
        {
            Debug.Log(
                "[PoisonPowerup] Poison is not unlocked."
            );

            return;
        }


        int level =
            PlayerPrefs.GetInt(
                saveKey + "_Level",
                1
            );


        level =
            Mathf.Clamp(
                level,
                1,
                maxLevel
            );


        float damagePerSecond =
            GetDamageFromLevel(
                level
            );


        cooldownTimer =
            cooldown;


        if (activeHUD != null)
        {
            activeHUD.StartCooldown();
        }


        if (poisonEffects != null)
        {
            poisonEffects.PlayPoisonActivationSound();
        }


        Debug.Log(
            "[PoisonPowerup] POISON ACTIVATED | " +
            "Level: " +
            level +
            " | Damage: " +
            damagePerSecond.ToString("0.0") +
            " per second | Duration: " +
            poisonDuration.ToString("0.0") +
            "s | Targets: " +
            maxTargets +
            " | Cooldown: " +
            cooldown.ToString("0.0") +
            "s"
        );


        if (poisonCoroutine != null)
        {
            StopCoroutine(
                poisonCoroutine
            );
        }


        poisonCoroutine =
            StartCoroutine(
                PoisonEnemies(
                    damagePerSecond
                )
            );
    }


    // =========================================================
    // GET DAMAGE
    // =========================================================

    private float GetDamageFromLevel(
        int level)
    {
        if (level <= 1)
        {
            return unlockValue;
        }


        if (level >= maxLevel)
        {
            return maxValue;
        }


        float progress =
            (level - 1f) /
            (maxLevel - 1f);


        return Mathf.Lerp(
            unlockValue,
            maxValue,
            progress
        );
    }


    // =========================================================
    // POISON ENEMIES
    // =========================================================

    private IEnumerator PoisonEnemies(
        float damagePerSecond)
    {
        isPoisonActive = true;


        // =====================================================
        // FIND NORMAL ENEMIES
        // =====================================================

        Enemy[] enemies =
            FindObjectsByType<Enemy>(
                FindObjectsInactive.Exclude,
                FindObjectsSortMode.None
            );


        // =====================================================
        // FIND RANGED ENEMIES
        // =====================================================

        EnemyRanged[] rangedEnemies =
            FindObjectsByType<EnemyRanged>(
                FindObjectsInactive.Exclude,
                FindObjectsSortMode.None
            );


        // =====================================================
        // FIND BOSSES
        // =====================================================

        Boss1[] bosses =
            FindObjectsByType<Boss1>(
                FindObjectsInactive.Exclude,
                FindObjectsSortMode.None
            );


        // =====================================================
        // BUILD TARGET LIST
        // =====================================================

        List<PoisonTarget> targets =
            new List<PoisonTarget>();


        // =====================================================
        // NORMAL ENEMIES
        // =====================================================

        for (int i = 0;
             i < enemies.Length;
             i++)
        {
            if (enemies[i] == null)
            {
                continue;
            }


            if (ignoredEnemies.Contains(
                enemies[i].gameObject))
            {
                continue;
            }


            targets.Add(
                new PoisonTarget(
                    enemies[i]
                )
            );
        }


        // =====================================================
        // RANGED ENEMIES
        // =====================================================

        for (int i = 0;
             i < rangedEnemies.Length;
             i++)
        {
            if (rangedEnemies[i] == null)
            {
                continue;
            }


            if (ignoredEnemies.Contains(
                rangedEnemies[i].gameObject))
            {
                continue;
            }


            targets.Add(
                new PoisonTarget(
                    rangedEnemies[i]
                )
            );
        }


        // =====================================================
        // BOSSES
        // =====================================================

        for (int i = 0;
             i < bosses.Length;
             i++)
        {
            if (bosses[i] == null)
            {
                continue;
            }


            if (ignoredEnemies.Contains(
                bosses[i].gameObject))
            {
                continue;
            }


            targets.Add(
                new PoisonTarget(
                    bosses[i]
                )
            );
        }


        // =====================================================
        // SORT
        //
        // PRIMARY:
        // Highest current health first
        //
        // SECONDARY:
        // Oldest spawned first when health is equal
        // =====================================================

        targets.Sort(
            (a, b) =>
            {
                int healthComparison =
                    b.GetHealth().CompareTo(
                        a.GetHealth()
                    );


                if (healthComparison != 0)
                {
                    return healthComparison;
                }


                return a.GetSpawnOrder().CompareTo(
                    b.GetSpawnOrder()
                );
            }
        );


        // =====================================================
        // APPLY TO TOP TARGETS
        // =====================================================

        int targetCount =
            Mathf.Min(
                maxTargets,
                targets.Count
            );


        int poisonedCount = 0;


        for (int i = 0;
             i < targetCount;
             i++)
        {
            if (targets[i] == null)
            {
                continue;
            }


            targets[i].ApplyPoison(
                damagePerSecond,
                poisonDuration
            );


            if (poisonEffects != null)
            {
                GameObject targetObject =
                    targets[i].GetGameObject();


                if (targetObject != null)
                {
                    poisonEffects.PlayPoisonEffect(
                        targetObject
                    );
                }
            }


            poisonedCount++;
        }


        // =====================================================
        // DEBUG
        // =====================================================

        Debug.Log(
            "[PoisonPowerup] Poisoned " +
            poisonedCount +
            " enemies for " +
            poisonDuration.ToString("0.0") +
            " seconds at " +
            damagePerSecond.ToString("0.0") +
            " damage per second."
        );


        // =====================================================
        // WAIT
        // =====================================================

        yield return new WaitForSeconds(
            poisonDuration
        );


        // =====================================================
        // END
        // =====================================================

        isPoisonActive =
            false;

        poisonCoroutine =
            null;


        Debug.Log(
            "[PoisonPowerup] POISON EFFECT ENDED."
        );
    }


    // =========================================================
    // RESET POISON
    // =========================================================

    public void ResetPoison()
    {
        if (poisonCoroutine != null)
        {
            StopCoroutine(
                poisonCoroutine
            );

            poisonCoroutine =
                null;
        }


        StopAllCoroutines();


        isPoisonActive =
            false;

        cooldownTimer =
            0f;


        if (poisonEffects != null)
        {
            poisonEffects.ClearPoisonVisuals();
        }


        RefreshHUD();


        Debug.Log(
            "[PoisonPowerup] POISON FULLY RESET."
        );
    }


    // =========================================================
    // HUD REFRESH
    // =========================================================

    private void RefreshHUD()
    {
        if (activeHUD == null)
        {
            FindActiveHUD();
        }


        if (activeHUD != null)
        {
            if (cooldownTimer <= 0f)
            {
                activeHUD.SetReady();
            }
            else
            {
                activeHUD.StartCooldown();
            }
        }
    }


    // =========================================================
    // DATA
    // =========================================================

    private class PoisonTarget
    {
        public Enemy enemy;

        public EnemyRanged rangedEnemy;

        public Boss1 boss;


        // -----------------------------------------------------
        // NORMAL ENEMY
        // -----------------------------------------------------

        public PoisonTarget(
            Enemy enemy)
        {
            this.enemy =
                enemy;
        }


        // -----------------------------------------------------
        // RANGED ENEMY
        // -----------------------------------------------------

        public PoisonTarget(
            EnemyRanged enemy)
        {
            this.rangedEnemy =
                enemy;
        }


        // -----------------------------------------------------
        // BOSS
        // -----------------------------------------------------

        public PoisonTarget(
            Boss1 boss)
        {
            this.boss =
                boss;
        }


        // -----------------------------------------------------
        // HEALTH
        // -----------------------------------------------------

        public float GetHealth()
        {
            if (enemy != null)
            {
                return enemy.Health;
            }


            if (rangedEnemy != null)
            {
                return rangedEnemy.Health;
            }


            if (boss != null)
            {
                return boss.Health;
            }


            return 0f;
        }


        // -----------------------------------------------------
        // SPAWN ORDER
        // -----------------------------------------------------

        public long GetSpawnOrder()
        {
            if (enemy != null)
            {
                return enemy.GetSpawnOrder();
            }


            if (rangedEnemy != null)
            {
                return rangedEnemy.GetSpawnOrder();
            }


            if (boss != null)
            {
                return boss.GetSpawnOrder();
            }


            return long.MaxValue;
        }


        // -----------------------------------------------------
        // APPLY POISON
        // -----------------------------------------------------

        public void ApplyPoison(
            float damagePerSecond,
            float duration)
        {
            if (enemy != null)
            {
                enemy.ApplyPoison(
                    damagePerSecond,
                    duration
                );

                return;
            }


            if (rangedEnemy != null)
            {
                rangedEnemy.ApplyPoison(
                    damagePerSecond,
                    duration
                );

                return;
            }


            if (boss != null)
            {
                boss.ApplyPoison(
                    damagePerSecond,
                    duration
                );
            }
        }


        // -----------------------------------------------------
        // GAME OBJECT
        // -----------------------------------------------------

        public GameObject GetGameObject()
        {
            if (enemy != null)
            {
                return enemy.gameObject;
            }


            if (rangedEnemy != null)
            {
                return rangedEnemy.gameObject;
            }


            if (boss != null)
            {
                return boss.gameObject;
            }


            return null;
        }
    }
}