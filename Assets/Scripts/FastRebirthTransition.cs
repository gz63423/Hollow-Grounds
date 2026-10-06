using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class FastRebirthTransition : MonoBehaviour
{
    public static FastRebirthTransition Instance { get; private set; }

    // ============================================================
    // TRANSITION
    // ============================================================

    [Header("Transition")]

    [Tooltip("How long the pixelation takes to completely cover the screen.")]
    public float transitionDuration = 3.5f;

    [Tooltip("How long the screen stays completely black after the pixels finish.")]
    public float finalBlackHold = 0.05f;

    // ============================================================
    // SFX
    // ============================================================

    [Header("Transition SFX")]

    public AudioClip transitionSFX;

    [Range(0f, 2f)]
    public float transitionSFXVolume = 1f;

    private AudioSource audioSource;

    // ============================================================
    // PIXEL GRID
    // ============================================================

    [Header("Pixel Grid")]

    public int columns = 32;
    public int rows = 18;

    public int pixelsPerBurst = 15;
    public int maximumPixelsPerBurst = 50;

    // ============================================================
    // PIXEL COLORS
    // ============================================================

    [Header("Pixel Colors")]

    public Color blackColor = Color.black;

    public Color redColor = new Color(
        0.75f,
        0f,
        0f,
        1f
    );

    [Range(0f, 1f)]
    public float redChance = 0.20f;

    // ============================================================
    // RED FLASH
    // ============================================================

    [Header("Red Flash")]

    public float redFlashDuration = 0.05f;

    public float redFlashMinDelay = 0.05f;

    public float redFlashMaxDelay = 0.25f;

    // ============================================================
    // PIXEL VISUALS
    // ============================================================

    [Header("Pixel Visuals")]

    public Image pixelPrefab;

    public RectTransform pixelContainer;

    // ============================================================
    // RANDOMNESS
    // ============================================================

    [Header("Randomness")]

    public float randomBurstDelay = 0.01f;

    // ============================================================
    // STATE
    // ============================================================

    private bool transitionRunning;

    private bool sceneChanging;

    private Coroutine transitionCoroutine;

    private bool[,] filledGrid;

    private int filledPixelCount;

    private int totalPixelCount;

    private int targetSceneIndex;

    private readonly List<GameObject> spawnedPixels =
        new List<GameObject>();

    // ============================================================
    // AWAKE
    // ============================================================

    private void Awake()
    {
        if (Instance != null &&
            Instance != this)
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

        audioSource =
            GetComponent<AudioSource>();

        if (audioSource == null)
        {
            audioSource =
                gameObject.AddComponent<AudioSource>();
        }

        audioSource.playOnAwake = false;
        audioSource.loop = false;

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
        LoadSceneMode mode)
    {
        Debug.Log(
            "FastRebirthTransition: Scene loaded: " +
            scene.buildIndex
        );

        // IMPORTANT:
        // Do NOT clear the pixels before the new scene loads.
        // They stay black during the scene change.
        if (scene.buildIndex == targetSceneIndex)
        {
            StartCoroutine(
                FinishSceneTransition()
            );
        }
    }

    // ============================================================
    // START TRANSITION
    // ============================================================

    public void StartTransition(int sceneIndex)
    {
        if (transitionRunning)
            return;

        if (sceneChanging)
            return;

        targetSceneIndex =
            sceneIndex;

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
                TransitionRoutine()
            );
    }

    // ============================================================
    // MAIN TRANSITION
    // ============================================================

    private IEnumerator TransitionRoutine()
    {
        Debug.Log(
            "Fast transition started."
        );

        // ========================================================
        // PLAY SFX
        // ========================================================

        PlayTransitionSFX();

        // ========================================================
        // PIXEL TIMER
        // ========================================================

        float elapsed = 0f;

        while (
            elapsed <
            transitionDuration
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

        // ========================================================
        // MAKE ABSOLUTELY SURE EVERY PIXEL IS FILLED
        // ========================================================

        FillRemainingPixels();

        // ========================================================
        // FORCE BLACK
        // ========================================================

        ForceAllPixelsBlack();

        // Give Unity one frame to render the fully black screen.
        yield return null;

        // ========================================================
        // FINAL BLACK HOLD
        // ========================================================

        if (finalBlackHold > 0f)
        {
            yield return new WaitForSecondsRealtime(
                finalBlackHold
            );
        }

        // ========================================================
        // LOAD TARGET SCENE
        // ========================================================

        LoadTargetScene();
    }

    // ============================================================
    // PLAY SFX
    // ============================================================

    private void PlayTransitionSFX()
    {
        if (audioSource == null)
            return;

        if (transitionSFX == null)
            return;

        float volume =
            transitionSFXVolume;

        if (SettingsManager.Instance != null)
        {
            volume *=
                SettingsManager.Instance.sfxVolume;
        }

        audioSource.PlayOneShot(
            transitionSFX,
            volume
        );
    }

    // ============================================================
    // SPAWN RANDOM BURST
    // ============================================================

    private void SpawnRandomBurst(
        int amount,
        float progress)
    {
        if (filledPixelCount >=
            totalPixelCount)
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
    // FILL REMAINING PIXELS
    // ============================================================

    private void FillRemainingPixels()
    {
        for (
            int x = 0;
            x < columns;
            x++
        )
        {
            for (
                int y = 0;
                y < rows;
                y++
            )
            {
                if (filledGrid[x, y])
                    continue;

                SpawnPixel(
                    x,
                    y,
                    1f
                );
            }
        }
    }

    // ============================================================
    // SPAWN PIXEL
    // ============================================================

    private void SpawnPixel(
        int x,
        int y,
        float progress)
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
                    "FastRebirthPixel"
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
                progress < 1f &&
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
        Image image)
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
        RectTransform pixel)
    {
        if (pixel == null)
            yield break;

        pixel.localScale =
            Vector3.one *
            0.75f;

        float elapsed = 0f;

        float duration =
            0.025f;

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
    // FORCE BLACK
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

                image.enabled =
                    true;
            }

            RectTransform rect =
                pixel.GetComponent<RectTransform>();

            if (rect != null)
            {
                rect.localScale =
                    Vector3.one;

                rect.localRotation =
                    Quaternion.identity;
            }
        }
    }

    // ============================================================
    // LOAD TARGET SCENE
    // ============================================================

    private void LoadTargetScene()
    {
        if (sceneChanging)
            return;

        sceneChanging = true;

        Debug.Log(
            "Fast transition finished."
        );

        Debug.Log(
            "Loading target scene: " +
            targetSceneIndex
        );

        // IMPORTANT:
        // Do NOT clear the pixels here.
        // They must remain on screen while Unity loads.
        SceneManager.LoadScene(
            targetSceneIndex
        );
    }

    // ============================================================
    // FINISH AFTER SCENE LOAD
    // ============================================================

    private IEnumerator FinishSceneTransition()
    {
        // Wait until the new scene is actually rendered.
        yield return null;

        yield return new WaitForEndOfFrame();

        Debug.Log(
            "Target scene " +
            targetSceneIndex +
            " is now visible. Removing transition."
        );

        // NOW it is safe to remove the black overlay.
        ClearPixels();

        transitionRunning = false;

        sceneChanging = false;

        transitionCoroutine = null;
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

        filledPixelCount = 0;
    }
}