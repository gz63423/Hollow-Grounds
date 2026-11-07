using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using static WaveSpawner;

public class WaveSpawner : MonoBehaviour
{
    [System.Serializable]
    public class EnemySpawn
    {
        public GameObject enemyPrefab;
        public int amount = 1;
    }

    [System.Serializable]
    public class Wave
    {
        public string waveName;
        public List<EnemySpawn> enemies;
    }

    // ============================================================
    // WAVES
    // ============================================================

    [Header("Waves")]
    public List<Wave> waves = new List<Wave>();


    // ============================================================
    // BETA SETTINGS
    // ============================================================

    [Header("Beta Version")]

    [Tooltip("If enabled, the spawner will end the official demo at Beta Max Wave and then enter Endless Mode.")]
    public bool betaVersion = false;

    [Tooltip("The final official demo wave. Example: 10 means Wave 10 is the finale.")]
    [Min(1)]
    public int betaMaxWave = 10;


    // ============================================================
    // ENDLESS MODE
    // ============================================================

    [Header("Endless Mode")]

    [Tooltip("First enemy prefab used in Endless Mode.")]
    public GameObject endlessEnemyPrefab1;

    [Tooltip("Second enemy prefab used in Endless Mode.")]
    public GameObject endlessEnemyPrefab2;

    [Tooltip("Chance for Endless Enemy 1 to spawn.")]
    [Range(0f, 1f)]
    public float endlessEnemy1Chance = 0.40f;

    [Tooltip("Chance for Endless Enemy 2 to spawn.")]
    [Range(0f, 1f)]
    public float endlessEnemy2Chance = 0.60f;

    [Tooltip("Time between Endless Mode enemy spawns.")]
    public float endlessSpawnDelay = 1f;


    // ============================================================
    // WAVE UI
    // ============================================================

    [Header("Wave UI")]

    [Tooltip("Optional. Assign the TMP text that displays the current wave.")]
    public TMP_Text waveText;


    // ============================================================
    // SPAWN SETTINGS
    // ============================================================

    [Header("Spawn Settings")]

    [Tooltip("Spawn points used by all waves and Endless Mode.")]
    public Transform[] spawnPoints;

    [Tooltip("Time between enemy spawns during normal waves.")]
    public float spawnDelay = 1f;

    [Tooltip("Time between enemy spawns during the Finale.")]
    public float finaleSpawnDelay = 0.5f;

    [Tooltip("Delay before the first enemy of the run spawns.")]
    [Min(0f)]
    public float firstEnemySpawnDelay = 1f;

    private bool isFirstEnemySpawn = true;


    // ============================================================
    // INTERNAL
    // ============================================================

    private int currentWaveIndex = 0;

    private List<GameObject> aliveEnemies =
        new List<GameObject>();

    private bool endlessMode = false;


    // ============================================================
    // PROGRESS TRACKING
    // ============================================================

    private int totalEnemiesThisWave = 0;
    private int enemiesKilledThisWave = 0;

    private WaveTransition waveTransition;


    // ============================================================
    // SPAWN TRACKING
    // ============================================================

    private int globalSpawnCounter = 0;

    private Dictionary<int, int> spawnPointCooldown =
        new Dictionary<int, int>();

    private const int SPAWN_COOLDOWN = 10;


    // ============================================================
    // COMPLETION
    // ============================================================

    [Header("Completion")]
    public string completionSceneName;


    // ============================================================
    // FREEZE / SPAWN PAUSE
    // ============================================================

    private bool spawnPaused = false;

    public bool IsSpawnPaused
    {
        get
        {
            return spawnPaused;
        }
    }


    // ============================================================
    // START
    // ============================================================

    void Start()
    {
        waveTransition =
            FindFirstObjectByType<WaveTransition>();


        // ========================================================
        // DETERMINE STARTING WAVE
        // ========================================================

        if (
            RebirthManager.Instance != null &&
            RebirthManager.Instance.ConsumeStartAtWaveOne()
        )
        {
            currentWaveIndex = 0;

            Debug.Log(
                "WAVE SPAWNER: Rebirth detected. " +
                "Starting current run at Wave 1."
            );
        }
        else
        {
            int currentRunHighestWave =
                PlayerPrefs.GetInt(
                    "CurrentRunHighestWave",
                    0
                );

            currentWaveIndex =
                currentRunHighestWave;

            Debug.Log(
                "WAVE SPAWNER: Starting at Wave " +
                (currentWaveIndex + 1) +
                " because current run highest completed wave is " +
                currentRunHighestWave
            );
        }


        // ========================================================
        // DETERMINE MAXIMUM OFFICIAL WAVE
        // ========================================================

        int maximumWaveCount =
            GetMaximumWaveCount();


        // ========================================================
        // CHECK IF PLAYER HAS ALREADY REACHED DEMO FINALE
        // ========================================================

        if (
            betaVersion &&
            currentWaveIndex >= maximumWaveCount
        )
        {
            Debug.Log(
                "WAVE SPAWNER: Demo Finale already completed. " +
                "Entering Endless Mode."
            );

            StartEndlessMode();

            return;
        }


        // ========================================================
        // CHECK NORMAL VERSION WAVES
        // ========================================================

        if (
            !betaVersion &&
            currentWaveIndex >= maximumWaveCount
        )
        {
            Debug.Log(
                "WAVE SPAWNER: All available waves have already " +
                "been completed."
            );

            return;
        }


        // ========================================================
        // START WAVES
        // ========================================================

        StartCoroutine(
            SpawnWaveRoutine()
        );
    }


    // ============================================================
    // GET MAXIMUM WAVE COUNT
    // ============================================================

    private int GetMaximumWaveCount()
    {
        if (!betaVersion)
        {
            return waves.Count;
        }

        int betaWaveCount =
            Mathf.Clamp(
                betaMaxWave,
                1,
                waves.Count
            );

        return betaWaveCount;
    }


    // ============================================================
    // PAUSE SPAWNING
    // ============================================================

    public void PauseSpawning()
    {
        spawnPaused = true;

        Debug.Log(
            "WAVE SPAWNER: Enemy spawning PAUSED."
        );
    }


    // ============================================================
    // RESUME SPAWNING
    // ============================================================

    public void ResumeSpawning()
    {
        spawnPaused = false;

        Debug.Log(
            "WAVE SPAWNER: Enemy spawning RESUMED."
        );
    }


    // ============================================================
    // WAVE ROUTINE
    // ============================================================


    IEnumerator SpawnWaveRoutine()
    {
        int maximumWaveCount = GetMaximumWaveCount();

        while (currentWaveIndex < maximumWaveCount)
        {
            Wave currentWave = waves[currentWaveIndex];

            aliveEnemies.Clear();
            totalEnemiesThisWave = 0;
            enemiesKilledThisWave = 0;

            // Calculate total enemies in this wave.
            foreach (EnemySpawn spawn in currentWave.enemies)
            {
                totalEnemiesThisWave += spawn.amount;
            }

            // Reset wave progress.
            if (waveTransition != null)
            {
                waveTransition.ResetWaveProgress();
            }

            // Update wave text.
            UpdateWaveText();

            Debug.Log(
                "WAVE " + (currentWaveIndex + 1) +
                ": Total enemies = " + totalEnemiesThisWave
            );

            // Wave start sound.
            if (SoundManager.Instance != null)
            {
                SoundManager.Instance.PlayWaveStarted();
            }

            // Determine spawn delay.
            float currentSpawnDelay = spawnDelay;

            if (
                betaVersion &&
                currentWaveIndex == maximumWaveCount - 1
            )
            {
                currentSpawnDelay = finaleSpawnDelay;
            }

            // Spawn enemies.
            foreach (EnemySpawn spawn in currentWave.enemies)
            {
                for (int i = 0; i < spawn.amount; i++)
                {
                    while (spawnPaused)
                    {
                        yield return null;
                    }

                    // Delay the very first enemy spawn to allow the player to initialize.
                    if (isFirstEnemySpawn)
                    {
                        float delayTimer = 0f;

                        while (delayTimer < firstEnemySpawnDelay)
                        {
                            while (spawnPaused)
                            {
                                yield return null;
                            }

                            delayTimer += Time.deltaTime;
                            yield return null;
                        }

                        isFirstEnemySpawn = false;
                    }

                    GameObject newEnemy = SpawnEnemy(spawn.enemyPrefab);

                    if (newEnemy != null)
                    {
                        aliveEnemies.Add(newEnemy);
                    }

                    float timer = 0f;

                    while (timer < currentSpawnDelay)
                    {
                        while (spawnPaused)
                        {
                            yield return null;
                        }

                        timer += Time.deltaTime;
                        yield return null;
                    }
                }
            }

            // Wait until all spawned enemies have died.
            while (aliveEnemies.Count > 0)
            {
                aliveEnemies.RemoveAll(e => e == null);
                yield return null;
            }

            // Make sure wave progress reaches 100%.
            enemiesKilledThisWave = totalEnemiesThisWave;
            UpdateWaveProgress();

            // Save current run wave.
            int completedWave = currentWaveIndex + 1;

            PlayerPrefs.SetInt(
                "CurrentRunHighestWave",
                completedWave
            );

            // Save lifetime highest wave.
            if (HighestWave.Instance != null)
            {
                HighestWave.Instance.ReachedWave(completedWave);
            }

            // Achievement: Defiance.
            AchievementStats.SetHighestWave(completedWave);

            PlayerPrefs.Save();

            // ====================================================
            // DEMO FINALE COMPLETED
            // ====================================================

            if (
                betaVersion &&
                currentWaveIndex >= maximumWaveCount - 1
            )
            {
                Debug.Log(
                    "WAVE SPAWNER: THE FINALE completed at Wave " +
                    completedWave + "."
                );

                // Record completion for the final Demo Journey objective.
                if (DemoJourneyManager.Instance != null)
                {
                    DemoJourneyManager.Instance.CompleteDemoFinale();
                }
                else
                {
                    Debug.LogWarning(
                        "WAVE SPAWNER: DemoJourneyManager.Instance is null. " +
                        "The Demo Finale could not be recorded in Demo Journey."
                    );
                }

                yield return new WaitForSeconds(1f);

                StartEndlessMode();

                yield break;
            }

            // ====================================================
            // NORMAL GAME FINAL WAVE
            // ====================================================

            if (
                !betaVersion &&
                currentWaveIndex >= maximumWaveCount - 1
            )
            {
                Debug.Log("WAVE SPAWNER: Final wave reached.");

                yield return new WaitForSeconds(1f);

                if (!string.IsNullOrEmpty(completionSceneName))
                {
                    SceneScreenFader fader =
                        FindFirstObjectByType<SceneScreenFader>();

                    if (fader != null)
                    {
                        fader.LoadScene(completionSceneName);
                    }
                    else
                    {
                        SceneManager.LoadScene(completionSceneName);
                    }
                }

                yield break;
            }

            // Advance to the next wave.
            currentWaveIndex++;

            yield return new WaitForSeconds(1f);
        }
    }



    // ============================================================
    // START ENDLESS MODE
    // ============================================================

    private void StartEndlessMode()
    {
        if (endlessMode)
            return;

        endlessMode = true;

        StopAllCoroutines();

        aliveEnemies.Clear();

        totalEnemiesThisWave = 0;
        enemiesKilledThisWave = 0;

        UpdateWaveText();

        if (waveTransition != null)
        {
            waveTransition.HideProgressBar();
        }

        Debug.Log(
            "WAVE SPAWNER: ENDLESS MODE STARTED."
        );

        StartCoroutine(
            EndlessModeRoutine()
        );
    }


    // ============================================================
    // ENDLESS MODE ROUTINE
    // ============================================================

    IEnumerator EndlessModeRoutine()
    {
        while (endlessMode)
        {
            while (spawnPaused)
            {
                yield return null;
            }

            GameObject enemyPrefab =
                GetEndlessEnemyPrefab();

            if (enemyPrefab != null)
            {
                GameObject newEnemy =
                    SpawnEnemy(enemyPrefab);

                if (newEnemy != null)
                {
                    aliveEnemies.Add(newEnemy);
                }
            }

            float timer = 0f;

            while (timer < endlessSpawnDelay)
            {
                while (spawnPaused)
                {
                    yield return null;
                }

                timer += Time.deltaTime;

                yield return null;
            }
        }
    }


    // ============================================================
    // GET ENDLESS ENEMY PREFAB
    // ============================================================

    private GameObject GetEndlessEnemyPrefab()
    {
        if (
            endlessEnemyPrefab1 == null &&
            endlessEnemyPrefab2 == null
        )
        {
            return null;
        }


        if (endlessEnemyPrefab1 == null)
        {
            return endlessEnemyPrefab2;
        }


        if (endlessEnemyPrefab2 == null)
        {
            return endlessEnemyPrefab1;
        }


        float roll =
            UnityEngine.Random.value;


        if (roll < endlessEnemy1Chance)
        {
            return endlessEnemyPrefab1;
        }


        return endlessEnemyPrefab2;
    }


    // ============================================================
    // UPDATE WAVE TEXT
    // ============================================================

    private void UpdateWaveText()
    {
        if (waveText == null)
            return;


        if (endlessMode)
        {
            waveText.text =
                "Endless Mode";

            return;
        }


        if (
            betaVersion &&
            currentWaveIndex ==
            GetMaximumWaveCount() - 1
        )
        {
            waveText.text =
                "The Finale";

            return;
        }


        waveText.text =
            "Wave " +
            (currentWaveIndex + 1);
    }


    // ============================================================
    // REGISTER ENEMY DEATH
    // ============================================================

    public void RegisterEnemyDeath(
        GameObject enemy
    )
    {
        if (enemy == null)
            return;


        if (!aliveEnemies.Remove(enemy))
            return;


        if (endlessMode)
        {
            if (waveTransition != null)
            {
                waveTransition.SetWaveProgress(
                    1f
                );
            }

            return;
        }


        enemiesKilledThisWave++;


        enemiesKilledThisWave =
            Mathf.Min(
                enemiesKilledThisWave,
                totalEnemiesThisWave
            );


        UpdateWaveProgress();


        Debug.Log(
            "ENEMY KILLED: " +
            enemiesKilledThisWave +
            "/" +
            totalEnemiesThisWave
        );
    }


    // ============================================================
    // UPDATE WAVE PROGRESS
    // ============================================================

    private void UpdateWaveProgress()
    {
        if (waveTransition == null)
            return;


        if (endlessMode)
        {
            waveTransition.SetWaveProgress(
                1f
            );

            return;
        }


        if (totalEnemiesThisWave <= 0)
        {
            waveTransition.SetWaveProgress(
                1f
            );

            return;
        }


        float progress =
            (float)enemiesKilledThisWave /
            totalEnemiesThisWave;


        waveTransition.SetWaveProgress(
            progress
        );
    }


    // ============================================================
    // CURRENT WAVE
    // ============================================================

    public int CurrentWaveNumber
    {
        get
        {
            return currentWaveIndex + 1;
        }
    }


    // ============================================================
    // IS ENDLESS MODE
    // ============================================================

    public bool IsEndlessMode
    {
        get
        {
            return endlessMode;
        }
    }


    // ============================================================
    // SPAWN ENEMY
    // ============================================================

    GameObject SpawnEnemy(
        GameObject enemyPrefab
    )
    {
        // IMPORTANT:
        // spawnPoints is declared above in Spawn Settings.

        if (
            enemyPrefab == null ||
            spawnPoints == null ||
            spawnPoints.Length == 0
        )
        {
            return null;
        }


        int spawnIndex =
            GetValidSpawnPointIndex();


        Transform spawnPoint =
            spawnPoints[spawnIndex];


        GameObject newEnemy =
            Instantiate(
                enemyPrefab,
                spawnPoint.position,
                Quaternion.identity
            );


        spawnPointCooldown[spawnIndex] =
            globalSpawnCounter +
            SPAWN_COOLDOWN;


        globalSpawnCounter++;


        // ========================================================
        // NORMAL ENEMY
        // ========================================================

        Enemy enemy =
            newEnemy.GetComponent<Enemy>();


        if (enemy != null)
        {
            enemy.waveSpawner = this;


            if (enemy.player == null)
            {
                enemy.player =
                    UnityEngine.Object
                        .FindFirstObjectByType<Player>();
            }


            if (enemy.house == null)
            {
                enemy.house =
                    UnityEngine.Object
                        .FindFirstObjectByType<House>();
            }


            enemy.ChooseTarget();


            if (enemy.currentTarget != null)
            {
                enemy.FlipTowards(
                    enemy.currentTarget.position
                );
            }
        }


        // ========================================================
        // BOSS
        // ========================================================

        Boss1 boss =
            newEnemy.GetComponent<Boss1>();


        if (boss != null)
        {
            boss.waveSpawner = this;


            if (boss.player == null)
            {
                boss.player =
                    UnityEngine.Object
                        .FindFirstObjectByType<Player>();
            }


            if (boss.house == null)
            {
                boss.house =
                    UnityEngine.Object
                        .FindFirstObjectByType<House>();
            }


            boss.ChooseTarget();


            if (boss.currentTarget != null)
            {
                boss.FlipTowards(
                    boss.currentTarget.position
                );
            }
        }


        // ========================================================
        // RANGED ENEMY
        // ========================================================

        EnemyRanged rangedEnemy =
            newEnemy.GetComponent<EnemyRanged>();


        if (rangedEnemy != null)
        {
            rangedEnemy.waveSpawner = this;


            if (rangedEnemy.player == null)
            {
                rangedEnemy.player =
                    UnityEngine.Object
                        .FindFirstObjectByType<Player>();
            }


            if (rangedEnemy.house == null)
            {
                rangedEnemy.house =
                    UnityEngine.Object
                        .FindFirstObjectByType<House>();
            }


            rangedEnemy.ChooseTarget();


            if (rangedEnemy.currentTarget != null)
            {
                rangedEnemy.FlipTowards(
                    rangedEnemy.currentTarget.position
                );
            }
        }


        return newEnemy;
    }


    // ============================================================
    // SPAWN POINT
    // ============================================================

    int GetValidSpawnPointIndex()
    {
        List<int> availableIndexes =
            new List<int>();


        for (
            int i = 0;
            i < spawnPoints.Length;
            i++
        )
        {
            if (
                !spawnPointCooldown.ContainsKey(i) ||
                globalSpawnCounter >=
                spawnPointCooldown[i]
            )
            {
                availableIndexes.Add(i);
            }
        }


        if (availableIndexes.Count == 0)
        {
            for (
                int i = 0;
                i < spawnPoints.Length;
                i++
            )
            {
                availableIndexes.Add(i);
            }
        }


        return availableIndexes[
            UnityEngine.Random.Range(
                0,
                availableIndexes.Count
            )
        ];
    }
}
