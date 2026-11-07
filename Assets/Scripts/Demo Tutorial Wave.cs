using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DemoTutorialWave : MonoBehaviour
{
    [System.Serializable]
    public class EnemySpawn
    {
        public GameObject enemyPrefab;
        public int amount = 1;
    }

    // =========================================================
    // TUTORIAL WAVE
    // =========================================================

    [Header("Tutorial Wave")]
    public List<EnemySpawn> enemies = new List<EnemySpawn>();

    // =========================================================
    // PLAYER
    // =========================================================

    [Header("Player")]
    public Player player;

    [Tooltip("Re-enables the player's gameplay when this scene starts.")]
    public bool enablePlayerOnStart = true;

    // =========================================================
    // WAVE UI
    // =========================================================

    [Header("Wave UI")]
    public TMP_Text waveText;

    // =========================================================
    // KILL PROGRESS
    // =========================================================

    [Header("Kill Progress")]
    [Tooltip("Number of kills required to completely fill the tutorial progress bar.")]
    public int requiredKills = 5;

    [Tooltip("WaveTransition used to display the tutorial kill progress.")]
    public WaveTransition waveTransition;

    // =========================================================
    // SPAWN SETTINGS
    // =========================================================

    [Header("Spawn Settings")]
    public Transform[] spawnPoints;

    public float spawnDelay = 1f;

    // =========================================================
    // DIALOGUE
    // =========================================================

    [Header("Dialogue")]
    public CanvasGroup dialogueBox;

    public TMP_Text dialogueText;

    [TextArea(3, 8)]
    public string dialogue = "Tutorial complete.";

    public float characterWriteDelay = 0.04f;

    // =========================================================
    // DIALOGUE FADE
    // =========================================================

    [Header("Dialogue Fade")]
    public float dialogueBoxFadeDuration = 0.5f;

    public float dialogueTextFadeDuration = 0.5f;

    // =========================================================
    // SCENE FADE
    // =========================================================

    [Header("Scene Fade")]
    [Tooltip("Full-screen Image used to fade the screen to black before loading the next scene.")]
    public Image fadeImage;

    [Tooltip("How long the final black fade takes.")]
    public float fadeOutDuration = 1f;

    // =========================================================
    // NEXT SCENE
    // =========================================================

    [Header("Next Scene")]
    public bool loadSceneByBuildIndex = false;

    public int targetSceneIndex = 0;

    public string targetSceneName;

    // =========================================================
    // INTERNAL
    // =========================================================

    private readonly List<GameObject> aliveEnemies =
        new List<GameObject>();

    private int spawnedEnemyCount = 0;

    private int killCount = 0;

    private bool tutorialComplete = false;

    private bool dialogueFinishedTyping = false;

    private bool sceneLoading = false;

    // =========================================================
    // START
    // =========================================================

    private void Start()
    {
        FindPlayer();

        PrepareFadeImage();

        if (waveText != null)
        {
            waveText.text = "Tutorial";
        }

        PrepareDialogue();

        if (waveTransition == null)
        {
            waveTransition =
                FindFirstObjectByType<WaveTransition>();
        }

        // Start progress at 0/5.
        if (waveTransition != null)
        {
            waveTransition.SetWaveProgress(0f);
        }

        // IMPORTANT:
        // Do not spawn enemies immediately.
        //
        // The persistent Player may still be reconnecting
        // to this scene.
        StartCoroutine(
            StartTutorialWave()
        );
    }

    // =========================================================
    // FIND PLAYER
    // =========================================================

    private void FindPlayer()
    {
        // Use the persistent singleton first.
        if (Player.Instance != null)
        {
            player = Player.Instance;
            return;
        }

        if (player == null)
        {
            player =
                FindFirstObjectByType<Player>();
        }
    }

    // =========================================================
    // START TUTORIAL WAVE
    // =========================================================

    private IEnumerator StartTutorialWave()
    {
        // =====================================================
        // WAIT FOR PLAYER TO EXIST
        // =====================================================

        FindPlayer();

        while (player == null)
        {
            yield return null;

            FindPlayer();
        }

        // =====================================================
        // WAIT FOR PLAYER SCENE RECONNECT
        // =====================================================

        // Player.OnSceneLoaded() sets sceneResetting and
        // reconnectingToScene while reconnecting the persistent
        // player to the new scene.
        //
        // Do not spawn enemies until that process is finished.

        while (!player.IsSceneReady)
        {
            yield return null;
        }

        // =====================================================
        // ENABLE PLAYER GAMEPLAY
        // =====================================================

        if (enablePlayerOnStart)
        {
            player.EnableGameplay();

            player.movementEnabled = true;
            player.sprintEnabled = true;
            player.attacksEnabled = true;
        }

        // Give the Player one frame to finish applying
        // its scene state before enemies are created.
        yield return null;

        yield return new WaitForEndOfFrame();

        // =====================================================
        // START SPAWNING
        // =====================================================

        yield return StartCoroutine(
            TutorialWaveRoutine()
        );
    }

    // =========================================================
    // FADE IMAGE PREPARATION
    // =========================================================

    private void PrepareFadeImage()
    {
        if (fadeImage == null)
            return;

        fadeImage.gameObject.SetActive(true);

        Color color =
            fadeImage.color;

        color.a = 0f;

        fadeImage.color =
            color;

        // Put the fade above all other UI.
        fadeImage.transform.SetAsLastSibling();
    }

    // =========================================================
    // DIALOGUE PREPARATION
    // =========================================================

    private void PrepareDialogue()
    {
        if (dialogueBox != null)
        {
            dialogueBox.alpha = 0f;

            dialogueBox.interactable = false;

            dialogueBox.blocksRaycasts = false;
        }

        if (dialogueText != null)
        {
            dialogueText.alpha = 0f;

            dialogueText.text = "";
        }
    }

    // =========================================================
    // TUTORIAL WAVE
    // =========================================================

    private IEnumerator TutorialWaveRoutine()
    {
        // -----------------------------------------------------
        // SPAWN ALL TUTORIAL ENEMIES
        // -----------------------------------------------------

        foreach (EnemySpawn spawn in enemies)
        {
            if (spawn == null ||
                spawn.enemyPrefab == null ||
                spawn.amount <= 0)
            {
                continue;
            }

            for (int i = 0; i < spawn.amount; i++)
            {
                GameObject newEnemy =
                    SpawnEnemy(
                        spawn.enemyPrefab
                    );

                if (newEnemy != null)
                {
                    aliveEnemies.Add(
                        newEnemy
                    );

                    spawnedEnemyCount++;
                }

                if (spawnDelay > 0f)
                {
                    yield return new WaitForSeconds(
                        spawnDelay
                    );
                }
            }
        }

        // -----------------------------------------------------
        // WAIT FOR ALL ENEMIES TO DIE
        //
        // This uses killCount rather than waiting for the
        // enemy GameObjects to be destroyed.
        //
        // Enemy.OnEnemyDestroyed fires immediately from Die(),
        // before the death fade.
        // -----------------------------------------------------

        while (killCount < spawnedEnemyCount)
        {
            yield return null;
        }

        tutorialComplete = true;

        Debug.Log(
            "DEMO TUTORIAL WAVE: " +
            "Tutorial wave completed."
        );

        // -----------------------------------------------------
        // SHOW COMPLETION DIALOGUE
        // -----------------------------------------------------

        yield return StartCoroutine(
            ShowDialogue()
        );

        // -----------------------------------------------------
        // WAIT FOR CLICK
        // -----------------------------------------------------

        yield return StartCoroutine(
            WaitForDialogueClick()
        );
    }

    // =========================================================
    // SPAWN ENEMY
    // =========================================================

    private GameObject SpawnEnemy(
        GameObject enemyPrefab)
    {
        if (enemyPrefab == null)
            return null;

        if (spawnPoints == null ||
            spawnPoints.Length == 0)
        {
            Debug.LogWarning(
                "DEMO TUTORIAL WAVE: " +
                "No spawn points assigned."
            );

            return null;
        }

        Transform spawnPoint =
            spawnPoints[
                Random.Range(
                    0,
                    spawnPoints.Length
                )
            ];

        if (spawnPoint == null)
            return null;

        GameObject newEnemy =
            Instantiate(
                enemyPrefab,
                spawnPoint.position,
                Quaternion.identity
            );

        // =====================================================
        // ENEMY
        // =====================================================

        Enemy enemy =
            newEnemy.GetComponent<Enemy>();

        if (enemy != null)
        {
            // =================================================
            // IMPORTANT:
            // Subscribe to the death event.
            //
            // Enemy.Die() invokes this BEFORE the death fade.
            // =================================================

            enemy.OnEnemyDestroyed +=
                HandleEnemyDeath;

            enemy.waveSpawner =
                FindFirstObjectByType<WaveSpawner>();

            // Always use the persistent Player first.
            if (Player.Instance != null)
            {
                enemy.player =
                    Player.Instance;
            }
            else if (enemy.player == null)
            {
                enemy.player =
                    FindFirstObjectByType<Player>();
            }

            if (enemy.house == null)
            {
                enemy.house =
                    FindFirstObjectByType<House>();
            }

            // =================================================
            // CHOOSE TARGET AFTER PLAYER IS ASSIGNED
            // =================================================

            enemy.ChooseTarget();

            if (enemy.currentTarget != null)
            {
                enemy.FlipTowards(
                    enemy.currentTarget.position
                );
            }

            Debug.Log(
                "DEMO TUTORIAL WAVE: Enemy spawned. " +
                "Player assigned: " +
                (enemy.player != null) +
                " | Target: " +
                (
                    enemy.currentTarget != null
                        ? enemy.currentTarget.name
                        : "NULL"
                )
            );
        }

        // =====================================================
        // BOSS
        // =====================================================

        Boss1 boss =
            newEnemy.GetComponent<Boss1>();

        if (boss != null)
        {
            boss.waveSpawner =
                FindFirstObjectByType<WaveSpawner>();

            if (Player.Instance != null)
            {
                boss.player =
                    Player.Instance;
            }
            else if (boss.player == null)
            {
                boss.player =
                    FindFirstObjectByType<Player>();
            }

            if (boss.house == null)
            {
                boss.house =
                    FindFirstObjectByType<House>();
            }

            boss.ChooseTarget();

            if (boss.currentTarget != null)
            {
                boss.FlipTowards(
                    boss.currentTarget.position
                );
            }
        }

        // =====================================================
        // RANGED ENEMY
        // =====================================================

        EnemyRanged rangedEnemy =
            newEnemy.GetComponent<EnemyRanged>();

        if (rangedEnemy != null)
        {
            rangedEnemy.waveSpawner =
                FindFirstObjectByType<WaveSpawner>();

            if (Player.Instance != null)
            {
                rangedEnemy.player =
                    Player.Instance;
            }
            else if (rangedEnemy.player == null)
            {
                rangedEnemy.player =
                    FindFirstObjectByType<Player>();
            }

            if (rangedEnemy.house == null)
            {
                rangedEnemy.house =
                    FindFirstObjectByType<House>();
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

    // =========================================================
    // ENEMY DEATH EVENT
    // =========================================================


// =========================================================
// ENEMY DEATH EVENT
// =========================================================

private void HandleEnemyDeath()
    {
        if (sceneLoading || tutorialComplete)
            return;

        killCount++;

        // Each kill = progress toward the required total.
        int progressKills = Mathf.Min(
            killCount,
            Mathf.Max(1, requiredKills)
        );

        float progress = (float)progressKills /
            Mathf.Max(1, requiredKills);

        progress = Mathf.Clamp01(progress);

        if (waveTransition != null)
        {
            waveTransition.SetWaveProgress(progress);
        }

        Debug.Log(
            "DEMO TUTORIAL WAVE: Enemy died. Progress = " +
            killCount + "/" + requiredKills
        );

        // -----------------------------------------------------
        // MARK TUTORIAL COMPLETE AFTER THE FIFTH ENEMY DIES
        // -----------------------------------------------------

        if (killCount >= Mathf.Max(1, requiredKills))
        {
            PlayerPrefs.SetInt("TutorialCompleted", 1);
            PlayerPrefs.Save();

            Debug.Log(
                "TUTORIAL COMPLETED: Required enemy kills reached. " +
                "Tutorial completion saved."
            );
        }
    }


    // =========================================================
    // SHOW DIALOGUE
    // =========================================================

    private IEnumerator ShowDialogue()
    {
        dialogueFinishedTyping = false;

        // =====================================================
        // DIALOGUE BOX FADE
        // =====================================================

        if (dialogueBox != null)
        {
            dialogueBox.alpha = 0f;

            dialogueBox.interactable = false;

            dialogueBox.blocksRaycasts = false;

            float timer = 0f;

            while (
                timer <
                dialogueBoxFadeDuration
            )
            {
                timer +=
                    Time.deltaTime;

                float progress =
                    dialogueBoxFadeDuration <= 0f
                        ? 1f
                        : timer /
                          dialogueBoxFadeDuration;

                dialogueBox.alpha =
                    Mathf.Clamp01(
                        progress
                    );

                yield return null;
            }

            dialogueBox.alpha = 1f;
        }

        // =====================================================
        // DIALOGUE TEXT FADE
        // =====================================================

        if (dialogueText != null)
        {
            dialogueText.alpha = 0f;

            dialogueText.text = "";

            float timer = 0f;

            while (
                timer <
                dialogueTextFadeDuration
            )
            {
                timer +=
                    Time.deltaTime;

                float progress =
                    dialogueTextFadeDuration <= 0f
                        ? 1f
                        : timer /
                          dialogueTextFadeDuration;

                dialogueText.alpha =
                    Mathf.Clamp01(
                        progress
                    );

                yield return null;
            }

            dialogueText.alpha = 1f;
        }

        // =====================================================
        // TYPE DIALOGUE
        // =====================================================

        yield return StartCoroutine(
            TypeDialogue()
        );

        dialogueFinishedTyping = true;

        // =====================================================
        // ENABLE CLICKING ONLY AFTER TYPING
        // =====================================================

        if (dialogueBox != null)
        {
            dialogueBox.interactable = true;

            dialogueBox.blocksRaycasts = true;
        }

        Debug.Log(
            "DEMO TUTORIAL WAVE: " +
            "Dialogue finished typing. " +
            "Waiting for click."
        );
    }

    // =========================================================
    // TYPE DIALOGUE
    // =========================================================

    private IEnumerator TypeDialogue()
    {
        if (dialogueText == null)
            yield break;

        dialogueText.text = "";

        if (string.IsNullOrEmpty(dialogue))
            yield break;

        foreach (char character in dialogue)
        {
            dialogueText.text +=
                character;

            if (characterWriteDelay > 0f)
            {
                yield return new WaitForSeconds(
                    characterWriteDelay
                );
            }
            else
            {
                yield return null;
            }
        }
    }

    // =========================================================
    // WAIT FOR DIALOGUE CLICK
    // =========================================================

    private IEnumerator WaitForDialogueClick()
    {
        // Do not recognize any click until the dialogue
        // has completely finished typing.
        while (!dialogueFinishedTyping)
        {
            yield return null;
        }

        while (!sceneLoading)
        {
            if (Input.GetMouseButtonDown(0))
            {
                StartCoroutine(
                    LoadNextSceneRoutine()
                );

                yield break;
            }

            yield return null;
        }
    }

    // =========================================================
    // LOAD NEXT SCENE
    // =========================================================

    private IEnumerator LoadNextSceneRoutine()
    {
        if (sceneLoading)
            yield break;

        sceneLoading = true;

        // =====================================================
        // DISABLE PLAYER DURING TRANSITION
        // =====================================================

        FindPlayer();

        if (player != null)
        {
            player.DisableGameplay();
        }

        // =====================================================
        // DISABLE DIALOGUE CLICKING
        // =====================================================

        if (dialogueBox != null)
        {
            dialogueBox.interactable = false;

            dialogueBox.blocksRaycasts = false;
        }

        // =====================================================
        // FADE TO BLACK
        // =====================================================

        yield return StartCoroutine(
            FadeOutScreen()
        );

        // =====================================================
        // LOAD SCENE
        // =====================================================

        if (loadSceneByBuildIndex)
        {
            Debug.Log(
                "DEMO TUTORIAL WAVE: " +
                "Loading scene build index " +
                targetSceneIndex
            );

            SceneManager.LoadScene(
                targetSceneIndex
            );

            yield break;
        }

        if (string.IsNullOrEmpty(
            targetSceneName))
        {
            Debug.LogError(
                "DEMO TUTORIAL WAVE: " +
                "No target scene name assigned."
            );

            sceneLoading = false;

            yield break;
        }

        Debug.Log(
            "DEMO TUTORIAL WAVE: " +
            "Loading scene " +
            targetSceneName
        );

        SceneManager.LoadScene(
            targetSceneName
        );
    }

    // =========================================================
    // FADE OUT SCREEN
    // =========================================================

    private IEnumerator FadeOutScreen()
    {
        if (fadeImage == null)
            yield break;

        fadeImage.gameObject.SetActive(true);

        fadeImage.transform.SetAsLastSibling();

        Color color =
            fadeImage.color;

        color.a = 0f;

        fadeImage.color =
            color;

        float timer = 0f;

        float duration =
            Mathf.Max(
                0.01f,
                fadeOutDuration
            );

        while (timer < duration)
        {
            timer +=
                Time.unscaledDeltaTime;

            float progress =
                Mathf.Clamp01(
                    timer / duration
                );

            color.a =
                progress;

            fadeImage.color =
                color;

            yield return null;
        }

        color.a = 1f;

        fadeImage.color =
            color;
    }

    // =========================================================
    // CLEANUP
    // =========================================================

    private void OnDestroy()
    {
        for (int i = 0;
             i < aliveEnemies.Count;
             i++)
        {
            GameObject enemyObject =
                aliveEnemies[i];

            if (enemyObject == null)
                continue;

            Enemy enemy =
                enemyObject.GetComponent<Enemy>();

            if (enemy != null)
            {
                enemy.OnEnemyDestroyed -=
                    HandleEnemyDeath;
            }
        }
    }
}