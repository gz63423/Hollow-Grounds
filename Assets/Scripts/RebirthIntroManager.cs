using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class RebirthIntroManager : MonoBehaviour
{
    public static RebirthIntroManager Instance { get; private set; }


    [Header("Waves Scene")]
    public int wavesSceneIndex = 2;


    [Header("Rebirth Intro Scene")]
    public int rebirthIntroSceneIndex = 3;


    [Header("Rebirth Save Scene")]
    public int rebirthSaveSceneIndex = 12;


    [Header("Transition Settings")]
    public float initialDelay = 4f;

    public float transitionDuration = 10f;

    public float finalBlackHold = 0.05f;


    [Header("Transition SFX")]
    public AudioClip transitionSFX;

    [Range(0f, 2f)]
    public float transitionSFXVolume = 1f;


    private AudioSource transitionAudioSource;


    [Header("Pixel Grid")]
    public int columns = 32;

    public int rows = 18;

    public int pixelsPerBurst = 5;

    public int maximumPixelsPerBurst = 30;


    [Header("Pixel Colors")]
    public Color blackColor = Color.black;

    public Color redColor =
        new Color(
            0.75f,
            0f,
            0f,
            1f
        );

    [Range(0f, 1f)]
    public float redChance = 0.30f;


    [Header("Red Flash")]
    public float redFlashDuration = 0.08f;

    public float redFlashMinDelay = 0.1f;

    public float redFlashMaxDelay = 0.6f;


    [Header("Pixel Visuals")]
    public Image pixelPrefab;

    public RectTransform pixelContainer;


    [Header("Randomness")]
    public float randomBurstDelay = 0.035f;


    // ============================================================
    // PLAYER PREFS
    // ============================================================

    private const string IntroCompletedKey =
        "RebirthIntroCompleted";


    // ============================================================
    // STATE
    // ============================================================

    private bool transitionRunning;

    private bool sceneChanging;

    private Coroutine transitionCoroutine;


    // ============================================================
    // GRID
    // ============================================================

    private bool[,] filledGrid;

    private int filledPixelCount;

    private int totalPixelCount;


    // ============================================================
    // PIXELS
    // ============================================================

    private readonly List<GameObject> spawnedPixels =
        new List<GameObject>();


    // ============================================================
    // PROPERTIES
    // ============================================================

    public bool TransitionRunning =>
        transitionRunning;


    public bool IsTransitioning =>
        transitionRunning;


    public bool IntroHasPlayed
    {
        get
        {
            return PlayerPrefs.GetInt(
                IntroCompletedKey,
                0
            ) == 1;
        }
    }


    // ============================================================
    // AWAKE
    // ============================================================

    private void Awake()
    {
        if (
            Instance != null &&
            Instance != this
        )
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        DontDestroyOnLoad(gameObject);


        if (pixelContainer == null)
        {
            pixelContainer =
                GetComponent<RectTransform>();
        }


        transitionAudioSource =
            GetComponent<AudioSource>();


        if (transitionAudioSource == null)
        {
            transitionAudioSource =
                gameObject.AddComponent<AudioSource>();
        }


        transitionAudioSource.playOnAwake =
            false;

        transitionAudioSource.loop =
            false;


        SceneManager.sceneLoaded +=
            OnSceneLoaded;
    }


    // ============================================================
    // DESTROY
    // ============================================================

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -=
            OnSceneLoaded;
    }


    // ============================================================
    // SCENE LOADED
    // ============================================================

    private void OnSceneLoaded(
        Scene scene,
        LoadSceneMode mode
    )
    {
        sceneChanging = false;

        transitionRunning = false;

        transitionCoroutine = null;


        ClearPixels();

        filledPixelCount = 0;


        if (
            columns > 0 &&
            rows > 0
        )
        {
            filledGrid =
                new bool[
                    columns,
                    rows
                ];
        }


        // ========================================================
        // REBIRTH SAVE SCENE
        // ========================================================

        if (
            scene.buildIndex ==
            rebirthSaveSceneIndex
        )
        {
            // IMPORTANT:
            //
            // This scene is ONLY valid for the FIRST REBIRTH.
            //
            // If somehow loaded while the player already owns
            // Rebirth 1+, immediately ignore the intro system.

            if (
                RebirthManager.Instance != null &&
                RebirthManager.Instance.CurrentRebirth >= 1
            )
            {
                Debug.Log(
                    "Rebirth Save Scene loaded while player " +
                    "already has Rebirth 1+. " +
                    "Skipping first-rebirth transition."
                );

                SceneManager.LoadScene(
                    wavesSceneIndex
                );

                return;
            }


            if (
                MusicManager.Instance != null
            )
            {
                MusicManager.Instance.MuteMusic();
            }


            StartRebirthTransition();

            return;
        }


        // ========================================================
        // REBIRTH INTRO SCENE
        // ========================================================

        if (
            scene.buildIndex ==
            rebirthIntroSceneIndex
        )
        {
            // ====================================================
            // SAFETY CHECK
            //
            // The intro scene is ONLY valid before Rebirth 1.
            // ====================================================

            if (
                RebirthManager.Instance != null &&
                RebirthManager.Instance.CurrentRebirth >= 1
            )
            {
                Debug.Log(
                    "First rebirth intro loaded after Rebirth 1. " +
                    "Skipping intro."
                );

                SceneManager.LoadScene(
                    wavesSceneIndex
                );

                return;
            }


            // ====================================================
            // DISABLE GAMEPLAY
            // ====================================================

            if (Player.Instance != null)
            {
                Player.Instance.DisableGameplay();
            }


            // ====================================================
            // MUTE MUSIC
            // ====================================================

            if (
                MusicManager.Instance != null
            )
            {
                MusicManager.Instance.MuteMusic();
            }


            return;
        }


        // ========================================================
        // WAVES SCENE
        // ========================================================

        if (
            scene.buildIndex !=
            wavesSceneIndex
        )
        {
            return;
        }
    }


    // ============================================================
    // START REBIRTH TRANSITION
    // ============================================================

    public void StartRebirthTransition()
    {
        if (transitionRunning)
            return;

        if (sceneChanging)
            return;


        // ========================================================
        // ONLY ALLOW FIRST REBIRTH TRANSITION
        // ========================================================

        if (
            RebirthManager.Instance != null &&
            RebirthManager.Instance.CurrentRebirth >= 1
        )
        {
            Debug.Log(
                "StartRebirthTransition ignored because " +
                "player already has Rebirth 1+."
            );

            SceneManager.LoadScene(
                wavesSceneIndex
            );

            return;
        }


        columns =
            Mathf.Max(
                1,
                columns
            );

        rows =
            Mathf.Max(
                1,
                rows
            );

        transitionDuration =
            Mathf.Max(
                0.01f,
                transitionDuration
            );

        initialDelay =
            Mathf.Max(
                0f,
                initialDelay
            );


        ClearPixels();


        filledGrid =
            new bool[
                columns,
                rows
            ];


        filledPixelCount = 0;


        totalPixelCount =
            columns *
            rows;


        transitionRunning = true;

        sceneChanging = false;


        transitionCoroutine =
            StartCoroutine(
                RebirthTransitionRoutine()
            );
    }


    // ============================================================
    // MAIN TRANSITION
    // ============================================================

    private IEnumerator RebirthTransitionRoutine()
    {
        if (initialDelay > 0f)
        {
            yield return new WaitForSecondsRealtime(
                initialDelay
            );
        }


        PlayTransitionSFX();


        float sfxDuration = 0f;


        if (transitionSFX != null)
        {
            sfxDuration =
                transitionSFX.length;
        }


        float totalDuration =
            Mathf.Max(
                transitionDuration,
                sfxDuration
            );


        float elapsed = 0f;


        while (
            elapsed <
            totalDuration
        )
        {
            elapsed +=
                Time.unscaledDeltaTime;


            float progress =
                Mathf.Clamp01(
                    elapsed /
                    transitionDuration
                );


            int burstAmount =
                Mathf.RoundToInt(
                    Mathf.Lerp(
                        pixelsPerBurst,
                        maximumPixelsPerBurst,
                        progress
                    )
                );


            burstAmount =
                Mathf.Max(
                    1,
                    burstAmount
                );


            SpawnRandomBurst(
                burstAmount,
                progress
            );


            float delay =
                Mathf.Lerp(
                    randomBurstDelay,
                    0f,
                    progress
                );


            float remainingTime =
                totalDuration -
                elapsed;


            delay =
                Mathf.Min(
                    delay,
                    remainingTime
                );


            if (delay > 0f)
            {
                yield return new WaitForSecondsRealtime(
                    delay
                );
            }
            else
            {
                yield return null;
            }
        }


        ForceAllPixelsBlack();


        yield return null;


        if (finalBlackHold > 0f)
        {
            yield return new WaitForSecondsRealtime(
                finalBlackHold
            );
        }


        LoadRebirthScene();
    }


    // ============================================================
    // PLAY SFX
    // ============================================================

    private void PlayTransitionSFX()
    {
        if (
            transitionAudioSource == null ||
            transitionSFX == null
        )
        {
            return;
        }


        float volume =
            transitionSFXVolume;


        if (
            SettingsManager.Instance != null
        )
        {
            volume *=
                SettingsManager.Instance.sfxVolume;
        }


        transitionAudioSource.PlayOneShot(
            transitionSFX,
            volume
        );
    }


    // ============================================================
    // SPAWN RANDOM BURST
    // ============================================================

    private void SpawnRandomBurst(
        int amount,
        float progress
    )
    {
        if (
            filledPixelCount >=
            totalPixelCount
        )
        {
            return;
        }


        int maximumAttempts =
            Mathf.Max(
                totalPixelCount * 2,
                20
            );


        int attempts = 0;

        int spawned = 0;


        while (
            spawned < amount &&
            filledPixelCount <
                totalPixelCount &&
            attempts <
                maximumAttempts
        )
        {
            attempts++;


            int x =
                Random.Range(
                    0,
                    columns
                );


            int y =
                Random.Range(
                    0,
                    rows
                );


            if (filledGrid[x, y])
                continue;


            SpawnPixel(
                x,
                y,
                progress
            );


            spawned++;
        }
    }


    // ============================================================
    // SPAWN PIXEL
    // ============================================================

    private void SpawnPixel(
        int x,
        int y,
        float progress
    )
    {
        if (filledGrid[x, y])
            return;


        filledGrid[x, y] = true;

        filledPixelCount++;


        GameObject pixelObject;


        if (pixelPrefab != null)
        {
            pixelObject =
                Instantiate(
                    pixelPrefab.gameObject,
                    pixelContainer
                );
        }
        else
        {
            pixelObject =
                new GameObject(
                    "RebirthPixel"
                );


            pixelObject.transform.SetParent(
                pixelContainer,
                false
            );


            Image generatedImage =
                pixelObject.AddComponent<Image>();


            generatedImage.raycastTarget =
                false;
        }


        spawnedPixels.Add(
            pixelObject
        );


        RectTransform pixelRect =
            pixelObject.GetComponent<RectTransform>();


        if (pixelRect == null)
        {
            pixelRect =
                pixelObject.AddComponent<RectTransform>();
        }


        if (pixelContainer == null)
            return;


        float width =
            pixelContainer.rect.width /
            columns;


        float height =
            pixelContainer.rect.height /
            rows;


        float posX =
            (-pixelContainer.rect.width / 2f)
            +
            (width * x)
            +
            (width / 2f);


        float posY =
            (-pixelContainer.rect.height / 2f)
            +
            (height * y)
            +
            (height / 2f);


        pixelRect.anchoredPosition =
            new Vector2(
                posX,
                posY
            );


        pixelRect.SetSizeWithCurrentAnchors(
            RectTransform.Axis.Horizontal,
            width
        );


        pixelRect.SetSizeWithCurrentAnchors(
            RectTransform.Axis.Vertical,
            height
        );


        pixelRect.localRotation =
            Quaternion.identity;


        pixelRect.localScale =
            Vector3.one;


        Image image =
            pixelObject.GetComponent<Image>();


        if (image != null)
        {
            image.color =
                blackColor;


            image.raycastTarget =
                false;


            if (
                Random.value <
                redChance
            )
            {
                StartCoroutine(
                    RedFlashRoutine(
                        image
                    )
                );
            }
        }


        StartCoroutine(
            PixelAppear(
                pixelRect
            )
        );
    }


    // ============================================================
    // RED FLASH
    // ============================================================

    private IEnumerator RedFlashRoutine(
        Image image
    )
    {
        if (image == null)
            yield break;


        float delay =
            Random.Range(
                redFlashMinDelay,
                redFlashMaxDelay
            );


        yield return new WaitForSecondsRealtime(
            delay
        );


        if (image == null)
            yield break;


        image.color =
            redColor;


        yield return new WaitForSecondsRealtime(
            redFlashDuration
        );


        if (image != null)
        {
            image.color =
                blackColor;
        }
    }


    // ============================================================
    // PIXEL APPEAR
    // ============================================================

    private IEnumerator PixelAppear(
        RectTransform pixel
    )
    {
        if (pixel == null)
            yield break;


        pixel.localScale =
            Vector3.one *
            0.75f;


        float elapsed = 0f;

        float duration = 0.035f;


        while (
            elapsed <
            duration
        )
        {
            if (pixel == null)
                yield break;


            elapsed +=
                Time.unscaledDeltaTime;


            float progress =
                Mathf.Clamp01(
                    elapsed /
                    duration
                );


            float scale =
                Mathf.Lerp(
                    0.75f,
                    1f,
                    progress
                );


            pixel.localScale =
                Vector3.one *
                scale;


            yield return null;
        }


        if (pixel != null)
        {
            pixel.localScale =
                Vector3.one;
        }
    }


    // ============================================================
    // FORCE ALL PIXELS BLACK
    // ============================================================

    private void ForceAllPixelsBlack()
    {
        for (
            int i = 0;
            i < spawnedPixels.Count;
            i++
        )
        {
            GameObject pixel =
                spawnedPixels[i];


            if (pixel == null)
                continue;


            Image image =
                pixel.GetComponent<Image>();


            if (image != null)
            {
                image.color =
                    blackColor;
            }


            RectTransform rect =
                pixel.GetComponent<RectTransform>();


            if (rect != null)
            {
                rect.localRotation =
                    Quaternion.identity;

                rect.localScale =
                    Vector3.one;
            }
        }
    }


    // ============================================================
    // LOAD REBIRTH INTRO SCENE
    // ============================================================

    private void LoadRebirthScene()
    {
        if (sceneChanging)
            return;


        sceneChanging = true;

        transitionRunning = false;

        transitionCoroutine = null;


        ForceAllPixelsBlack();


        // ========================================================
        // SAFETY CHECK
        // ========================================================

        if (
            RebirthManager.Instance != null &&
            RebirthManager.Instance.CurrentRebirth >= 1
        )
        {
            Debug.Log(
                "Player already has Rebirth 1+. " +
                "Not loading first-rebirth intro."
            );


            SceneManager.LoadScene(
                wavesSceneIndex
            );


            return;
        }


        Debug.Log(
            "Loading first rebirth intro scene: " +
            rebirthIntroSceneIndex
        );


        SceneManager.LoadScene(
            rebirthIntroSceneIndex
        );
    }


    // ============================================================
    // CLEAR PIXELS
    // ============================================================

    private void ClearPixels()
    {
        for (
            int i =
                spawnedPixels.Count - 1;
            i >= 0;
            i--
        )
        {
            if (spawnedPixels[i] != null)
            {
                Destroy(
                    spawnedPixels[i]
                );
            }
        }


        spawnedPixels.Clear();
    }


    // ============================================================
    // SHOULD PLAY FIRST REBIRTH INTRO
    // ============================================================

    public bool ShouldPlayRebirthIntro()
    {
        // ========================================================
        // REBIRTH MANAGER REQUIRED
        // ========================================================

        if (
            RebirthManager.Instance == null
        )
        {
            return false;
        }


        // ========================================================
        // CRITICAL:
        //
        // THE INTRO ONLY EXISTS FOR REBIRTH 0.
        //
        // If the player has Rebirth 1+, this function MUST
        // ALWAYS return false.
        // ========================================================

        if (
            RebirthManager.Instance.CurrentRebirth != 0
        )
        {
            return false;
        }


        // ========================================================
        // INTRO ALREADY COMPLETED
        // ========================================================

        if (IntroHasPlayed)
        {
            return false;
        }


        // ========================================================
        // REQUIREMENTS
        // ========================================================

        if (
            !RebirthManager.Instance.CanRebirth()
        )
        {
            return false;
        }


        return true;
    }


    // ============================================================
    // TRY PLAY FIRST REBIRTH INTRO
    // ============================================================

    public bool TryPlayRebirthIntro()
    {
        // ========================================================
        // NEVER DO ANYTHING AFTER REBIRTH 1
        // ========================================================

        if (
            RebirthManager.Instance != null &&
            RebirthManager.Instance.CurrentRebirth >= 1
        )
        {
            Debug.Log(
                "TryPlayRebirthIntro ignored. " +
                "Player already has Rebirth 1+."
            );

            return false;
        }


        if (!ShouldPlayRebirthIntro())
            return false;


        Scene currentScene =
            SceneManager.GetActiveScene();


        if (
            currentScene.buildIndex !=
            wavesSceneIndex
        )
        {
            return false;
        }


        if (!transitionRunning)
        {
            Debug.Log(
                "First rebirth requirements met. " +
                "Loading rebirth save scene: " +
                rebirthSaveSceneIndex
            );


            SceneManager.LoadScene(
                rebirthSaveSceneIndex
            );
        }


        return true;
    }


    // ============================================================
    // MARK FIRST REBIRTH COMPLETE
    // ============================================================

    public void MarkFirstRebirthComplete()
    {
        PlayerPrefs.SetInt(
            IntroCompletedKey,
            1
        );


        PlayerPrefs.Save();


        transitionRunning = false;


        Debug.Log(
            "============================================"
        );


        Debug.Log(
            "FIRST REBIRTH INTRO COMPLETED!"
        );


        Debug.Log(
            "The first-rebirth intro will never play again."
        );


        Debug.Log(
            "============================================"
        );
    }


    // ============================================================
    // RESET FIRST REBIRTH PROGRESS
    // ============================================================

    public void ResetFirstRebirthProgress()
    {
        PlayerPrefs.DeleteKey(
            IntroCompletedKey
        );


        PlayerPrefs.Save();


        transitionRunning = false;

        sceneChanging = false;


        Debug.Log(
            "First rebirth intro progress reset."
        );
    }


    // ============================================================
    // OLD COMPATIBILITY
    // ============================================================

    public void MarkIntroComplete()
    {
        MarkFirstRebirthComplete();
    }


    // ============================================================
    // RESET TRANSITION
    // ============================================================

    public void ResetTransition()
    {
        if (
            transitionCoroutine != null
        )
        {
            StopCoroutine(
                transitionCoroutine
            );

            transitionCoroutine = null;
        }


        transitionRunning = false;

        sceneChanging = false;


        ClearPixels();


        filledPixelCount = 0;


        if (
            columns > 0 &&
            rows > 0
        )
        {
            filledGrid =
                new bool[
                    columns,
                    rows
                ];
        }
    }


    // ============================================================
    // DEBUG RESET
    // ============================================================

    [ContextMenu("Reset First Rebirth Intro")]
    private void DebugResetIntro()
    {
        ResetFirstRebirthProgress();
    }
}