using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreezePowerup : MonoBehaviour
{
    // =========================================================
    // POWERUP
    // =========================================================

    [Header("Powerup")]

    [Tooltip("Freeze = 0")]
    public int powerupIndex = 0;

    [Tooltip("Freeze cooldown in seconds.")]
    public float cooldown = 30f;


    // =========================================================
    // FREEZE SETTINGS
    // =========================================================

    [Header("Freeze Settings")]

    [Tooltip("Freeze duration at Level 1.")]
    public float unlockValue = 0.5f;

    [Tooltip("Freeze duration at maximum level.")]
    public float maxValue = 5f;

    [Tooltip("Maximum Freeze level.")]
    public int maxLevel = 10;


    // =========================================================
    // EFFECTS
    // =========================================================

    [Header("Effects")]

    public FreezePowerupEffects freezeEffects;


    // =========================================================
    // HUD
    // =========================================================

    [Header("HUD")]

    public ActiveHUD activeHUD;


    // =========================================================
    // INTERNAL
    // =========================================================

    private float cooldownTimer = 0f;

    private bool isFrozen = false;

    private Coroutine freezeCoroutine;


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

        /*
         * There is only ONE active slot now.
         *
         * Alpha1 = Slot 0
         *
         * Alpha2 / Alpha3 are intentionally no longer used.
         */

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


        cooldownTimer -= Time.deltaTime;


        if (cooldownTimer <= 0f)
        {
            cooldownTimer = 0f;

            if (activeHUD != null)
            {
                activeHUD.SetReady();
            }

            return;
        }


        if (activeHUD != null && cooldown > 0f)
        {
            float elapsedTime =
                cooldown -
                cooldownTimer;


            float fillAmount =
                elapsedTime /
                cooldown;


            fillAmount =
                Mathf.Clamp01(fillAmount);


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
        // =====================================================
        // MANAGER CHECK
        // =====================================================

        if (ActivePowerupManager.Instance == null)
        {
            Debug.LogWarning(
                "[FreezePowerup] Cannot activate Freeze because " +
                "ActivePowerupManager.Instance is null."
            );

            return;
        }


        // =====================================================
        // CHECK SLOT 0
        // =====================================================

        int equippedPowerup =
            ActivePowerupManager.Instance.GetPowerupInSlot(0);


        // =====================================================
        // EMPTY
        // =====================================================

        if (equippedPowerup == -1)
        {
            Debug.Log(
                "[FreezePowerup] Slot 0 is empty."
            );

            return;
        }


        // =====================================================
        // WRONG POWERUP
        // =====================================================

        if (equippedPowerup != powerupIndex)
        {
            Debug.Log(
                "[FreezePowerup] Slot 0 contains " +
                ActivePowerupManager.Instance.GetPowerupName(
                    equippedPowerup
                ) +
                ", not Freeze."
            );

            return;
        }


        // =====================================================
        // FIND HUD
        // =====================================================

        FindActiveHUD();


        // =====================================================
        // ACTIVATE
        // =====================================================

        ActivateFreeze();
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


        for (int i = 0; i < huds.Length; i++)
        {
            if (huds[i] == null)
            {
                continue;
            }


            if (huds[i].slotIndex == 0)
            {
                activeHUD = huds[i];
                return;
            }
        }
    }


    // =========================================================
    // ACTIVATE FREEZE
    // =========================================================

    private void ActivateFreeze()
    {
        // =====================================================
        // ALREADY ACTIVE
        // =====================================================

        if (isFrozen)
        {
            Debug.Log(
                "[FreezePowerup] Freeze is already active."
            );

            return;
        }


        // =====================================================
        // COOLDOWN
        // =====================================================

        if (cooldownTimer > 0f)
        {
            Debug.Log(
                "[FreezePowerup] Freeze is on cooldown: " +
                cooldownTimer.ToString("0.0") +
                " seconds remaining."
            );

            return;
        }


        // =====================================================
        // CHECK UNLOCK
        // =====================================================

        const string saveKey =
            "Active_Freeze";


        bool unlocked =
            PlayerPrefs.GetInt(
                saveKey + "_Unlocked",
                0
            ) == 1;


        if (!unlocked)
        {
            Debug.Log(
                "[FreezePowerup] Freeze is not unlocked."
            );

            return;
        }


        // =====================================================
        // GET LEVEL
        // =====================================================

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


        // =====================================================
        // GET DURATION
        // =====================================================

        float duration =
            GetDurationFromLevel(level);


        // =====================================================
        // START COOLDOWN
        // =====================================================

        cooldownTimer =
            cooldown;


        if (activeHUD != null)
        {
            activeHUD.StartCooldown();
        }


        // =====================================================
        // VISUAL EFFECT
        // =====================================================

        if (freezeEffects != null)
        {
            freezeEffects.PlayFreezeEffect(
                duration
            );
        }


        // =====================================================
        // DEBUG
        // =====================================================

        Debug.Log(
            "[FreezePowerup] FREEZE ACTIVATED | " +
            "Level: " +
            level +
            " | Duration: " +
            duration.ToString("0.0") +
            "s | Cooldown: " +
            cooldown.ToString("0.0") +
            "s"
        );


        // =====================================================
        // START FREEZE
        // =====================================================

        if (freezeCoroutine != null)
        {
            StopCoroutine(freezeCoroutine);
        }


        freezeCoroutine =
            StartCoroutine(
                FreezeEnemies(duration)
            );
    }


    // =========================================================
    // GET DURATION
    // =========================================================

    private float GetDurationFromLevel(int level)
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
    // FREEZE EVERYTHING
    // =========================================================

    private IEnumerator FreezeEnemies(float duration)
    {
        isFrozen = true;


        // =====================================================
        // PAUSE WAVE SPAWNING
        // =====================================================

        WaveSpawner waveSpawner =
            FindFirstObjectByType<WaveSpawner>();


        if (waveSpawner != null)
        {
            waveSpawner.PauseSpawning();
        }
        else
        {
            Debug.LogWarning(
                "[FreezePowerup] WaveSpawner was not found."
            );
        }


        // =====================================================
        // FREEZE EXISTING ENEMIES
        // =====================================================

        GameObject[] enemies =
            GameObject.FindGameObjectsWithTag(
                "Enemy"
            );


        List<EnemyFreezeData> frozenEnemies =
            new List<EnemyFreezeData>();


        for (int i = 0; i < enemies.Length; i++)
        {
            if (enemies[i] == null)
            {
                continue;
            }


            EnemyFreezeData data =
                FreezeEnemy(
                    enemies[i]
                );


            if (data != null)
            {
                frozenEnemies.Add(data);
            }
        }


        Debug.Log(
            "[FreezePowerup] Frozen " +
            frozenEnemies.Count +
            " enemies for " +
            duration.ToString("0.0") +
            " seconds."
        );


        // =====================================================
        // FREEZE EXISTING BULLETS
        // =====================================================

        EnemyBullet[] bullets =
            FindObjectsByType<EnemyBullet>(
                FindObjectsInactive.Include,
                FindObjectsSortMode.None
            );


        int frozenBulletCount = 0;


        for (int i = 0; i < bullets.Length; i++)
        {
            if (bullets[i] == null)
            {
                continue;
            }


            bullets[i].SetFrozen(true);

            frozenBulletCount++;
        }


        Debug.Log(
            "[FreezePowerup] Frozen " +
            frozenBulletCount +
            " enemy bullets."
        );


        // =====================================================
        // WAIT
        // =====================================================

        yield return new WaitForSeconds(
            duration
        );


        // =====================================================
        // RESTORE ENEMIES
        // =====================================================

        for (int i = 0; i < frozenEnemies.Count; i++)
        {
            if (frozenEnemies[i] != null)
            {
                frozenEnemies[i].Restore();
            }
        }


        // =====================================================
        // RESTORE BULLETS
        // =====================================================

        EnemyBullet[] bulletsToRestore =
            FindObjectsByType<EnemyBullet>(
                FindObjectsInactive.Include,
                FindObjectsSortMode.None
            );


        for (int i = 0; i < bulletsToRestore.Length; i++)
        {
            if (bulletsToRestore[i] == null)
            {
                continue;
            }


            bulletsToRestore[i].SetFrozen(false);
        }


        // =====================================================
        // RESUME WAVE SPAWNING
        // =====================================================

        if (waveSpawner != null)
        {
            waveSpawner.ResumeSpawning();
        }


        // =====================================================
        // END FREEZE
        // =====================================================

        isFrozen = false;

        freezeCoroutine = null;


        Debug.Log(
            "[FreezePowerup] FREEZE ENDED. " +
            "Enemies restored and wave spawning resumed."
        );
    }


    // =========================================================
    // FREEZE ENEMY
    // =========================================================

    private EnemyFreezeData FreezeEnemy(
        GameObject enemyObject)
    {
        if (enemyObject == null)
        {
            return null;
        }


        EnemyFreezeData data =
            new EnemyFreezeData();


        data.enemyObject =
            enemyObject;


        // =====================================================
        // NORMAL ENEMY
        // =====================================================

        Enemy enemy =
            enemyObject.GetComponent<Enemy>();


        if (enemy != null)
        {
            data.enemy =
                enemy;


            enemy.SetFrozen(true);

            return data;
        }


        // =====================================================
        // RANGED ENEMY
        // =====================================================

        EnemyRanged rangedEnemy =
            enemyObject.GetComponent<EnemyRanged>();


        if (rangedEnemy != null)
        {
            data.rangedEnemy =
                rangedEnemy;


            rangedEnemy.SetFrozen(true);

            return data;
        }


        // =====================================================
        // BOSS
        // =====================================================

        Boss1 boss =
            enemyObject.GetComponent<Boss1>();


        if (boss != null)
        {
            data.boss =
                boss;


            boss.SetFrozen(true);

            return data;
        }


        // =====================================================
        // UNKNOWN
        // =====================================================

        Debug.LogWarning(
            "[FreezePowerup] Enemy-tagged object has no " +
            "Enemy, EnemyRanged, or Boss1 component: " +
            enemyObject.name
        );


        return null;
    }


    // =========================================================
    // RESET FREEZE
    // =========================================================

    public void ResetFreeze()
    {
        // =====================================================
        // STOP FREEZE COROUTINE
        // =====================================================

        if (freezeCoroutine != null)
        {
            StopCoroutine(freezeCoroutine);
            freezeCoroutine = null;
        }


        StopAllCoroutines();


        // =====================================================
        // UNFREEZE ENEMIES
        // =====================================================

        GameObject[] enemies =
            GameObject.FindGameObjectsWithTag(
                "Enemy"
            );


        for (int i = 0; i < enemies.Length; i++)
        {
            if (enemies[i] == null)
            {
                continue;
            }


            Enemy enemy =
                enemies[i].GetComponent<Enemy>();


            if (enemy != null)
            {
                enemy.SetFrozen(false);
                continue;
            }


            EnemyRanged rangedEnemy =
                enemies[i].GetComponent<EnemyRanged>();


            if (rangedEnemy != null)
            {
                rangedEnemy.SetFrozen(false);
                continue;
            }


            Boss1 boss =
                enemies[i].GetComponent<Boss1>();


            if (boss != null)
            {
                boss.SetFrozen(false);
            }
        }


        // =====================================================
        // UNFREEZE BULLETS
        // =====================================================

        EnemyBullet[] bullets =
            FindObjectsByType<EnemyBullet>(
                FindObjectsInactive.Include,
                FindObjectsSortMode.None
            );


        for (int i = 0; i < bullets.Length; i++)
        {
            if (bullets[i] == null)
            {
                continue;
            }


            bullets[i].SetFrozen(false);
        }


        // =====================================================
        // RESUME SPAWNING
        // =====================================================

        WaveSpawner waveSpawner =
            FindFirstObjectByType<WaveSpawner>();


        if (waveSpawner != null)
        {
            waveSpawner.ResumeSpawning();
        }


        // =====================================================
        // RESET STATE
        // =====================================================

        isFrozen = false;

        cooldownTimer = 0f;


        // =====================================================
        // RESET HUD
        // =====================================================

        RefreshHUD();


        Debug.Log(
            "[FreezePowerup] FREEZE FULLY RESET."
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

    private class EnemyFreezeData
    {
        public GameObject enemyObject;

        public Enemy enemy;

        public EnemyRanged rangedEnemy;

        public Boss1 boss;


        public void Restore()
        {
            if (enemy != null)
            {
                enemy.SetFrozen(false);
            }


            if (rangedEnemy != null)
            {
                rangedEnemy.SetFrozen(false);
            }


            if (boss != null)
            {
                boss.SetFrozen(false);
            }
        }
    }
}

