using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class HollowGroundsStartup : MonoBehaviour
{
    // ============================================================
    // SCENE TRANSITION
    // ============================================================

    [Header("Scene Transition")]

    [Tooltip("Build Index of the scene to load.")]
    public int sceneIndex = 1;

    [Tooltip("How long after startup begins before the scene switches.")]
    public float sceneSwitchTime = 3.5f;


    // ============================================================
    // STARTUP DELAY
    // ============================================================

    [Header("Startup Delay")]

    public float startupDelay = 0f;


    // ============================================================
    // STARTUP IMAGE
    // ============================================================

    [Header("Startup Image")]

    public Image startupImage;

    public float zoomAmount = 1.08f;

    public float zoomDuration = 8f;


    // ============================================================
    // TRANSITION SFX
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

    [Min(1)]
    public int columns = 32;

    [Min(1)]
    public int rows = 18;

    [Min(1)]
    public int pixelsPerBurst = 15;

    [Min(1)]
    public int maximumPixelsPerBurst = 50;


    // ============================================================
    // PIXEL COLORS
    // ============================================================

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

    [Tooltip("Delay between pixel bursts.")]
    public float randomBurstDelay = 0.01f;


    // ============================================================
    // STATE
    // ============================================================

    private bool[,] filledGrid;

    private int filledPixelCount;

    private int totalPixelCount;

    private readonly List<GameObject> spawnedPixels =
        new List<GameObject>();

    private bool startupRunning = false;

    private bool sceneLoading = false;


    // ============================================================
    // AWAKE
    // ============================================================

    private void Awake()
    {
        columns = Mathf.Max(1, columns);
        rows = Mathf.Max(1, rows);

        pixelsPerBurst =
            Mathf.Max(1, pixelsPerBurst);

        maximumPixelsPerBurst =
            Mathf.Max(
                pixelsPerBurst,
                maximumPixelsPerBurst
            );

        sceneSwitchTime =
            Mathf.Max(
                0.01f,
                sceneSwitchTime
            );

        zoomDuration =
            Mathf.Max(
                0.01f,
                zoomDuration
            );

        redFlashDuration =
            Mathf.Max(
                0f,
                redFlashDuration
            );

        redFlashMinDelay =
            Mathf.Max(
                0f,
                redFlashMinDelay
            );

        redFlashMaxDelay =
            Mathf.Max(
                redFlashMinDelay,
                redFlashMaxDelay
            );

        randomBurstDelay =
            Mathf.Max(
                0f,
                randomBurstDelay
            );


        // --------------------------------------------------------
        // PIXEL CONTAINER
        // --------------------------------------------------------

        if (pixelContainer == null)
        {
            pixelContainer =
                GetComponent<RectTransform>();
        }


        // --------------------------------------------------------
        // AUDIO
        // --------------------------------------------------------

        audioSource =
            GetComponent<AudioSource>();

        if (audioSource == null)
        {
            audioSource =
                gameObject.AddComponent<AudioSource>();
        }

        audioSource.playOnAwake = false;
        audioSource.loop = false;


        // --------------------------------------------------------
        // LOGO STARTS INVISIBLE
        // --------------------------------------------------------

        if (startupImage != null)
        {
            Color imageColor =
                startupImage.color;

            imageColor.a = 0f;

            startupImage.color =
                imageColor;

            startupImage.enabled = true;

            startupImage.rectTransform.localScale =
                Vector3.one;
        }
    }


    // ============================================================
    // START
    // ============================================================

    private void Start()
    {
        if (startupRunning)
            return;

        startupRunning = true;

        StartCoroutine(
            StartupRoutine()
        );
    }


    // ============================================================
    // STARTUP ROUTINE
    // ============================================================

    private IEnumerator StartupRoutine()
    {
        // --------------------------------------------------------
        // OPTIONAL STARTUP DELAY
        // --------------------------------------------------------

        if (startupDelay > 0f)
        {
            yield return new WaitForSecondsRealtime(
                startupDelay
            );
        }


        // --------------------------------------------------------
        // SETUP PIXELS
        // --------------------------------------------------------

        ClearPixels();

        filledGrid =
            new bool[
                columns,
                rows
            ];

        filledPixelCount = 0;

        totalPixelCount =
            columns * rows;


        // --------------------------------------------------------
        // SOUND
        // --------------------------------------------------------

        PlayTransitionSFX();


        // --------------------------------------------------------
        // START ZOOM
        // --------------------------------------------------------

        if (startupImage != null)
        {
            StartCoroutine(
                ZoomImageRoutine()
            );
        }


        // --------------------------------------------------------
        // REVEAL
        // --------------------------------------------------------

        yield return StartCoroutine(
            RevealImageRoutine()
        );


        // --------------------------------------------------------
        // IMPORTANT:
        // THE SCENE IS NOT LOADED HERE.
        //
        // The scene switch is controlled by sceneSwitchTime.
        // --------------------------------------------------------

        yield break;
    }


    // ============================================================
    // IMAGE ZOOM
    // ============================================================

    private IEnumerator ZoomImageRoutine()
    {
        if (startupImage == null)
            yield break;

        RectTransform imageRect =
            startupImage.rectTransform;

        imageRect.localScale =
            Vector3.one;

        float elapsed = 0f;

        float duration =
            Mathf.Max(
                0.01f,
                zoomDuration
            );

        while (elapsed < duration)
        {
            elapsed +=
                Time.unscaledDeltaTime;

            float progress =
                Mathf.Clamp01(
                    elapsed / duration
                );

            float easedProgress =
                1f -
                Mathf.Pow(
                    1f - progress,
                    3f
                );

            float scale =
                Mathf.Lerp(
                    1f,
                    zoomAmount,
                    easedProgress
                );

            imageRect.localScale =
                Vector3.one * scale;

            yield return null;
        }

        imageRect.localScale =
            Vector3.one * zoomAmount;
    }


    // ============================================================
    // REVEAL IMAGE
    // ============================================================

    private IEnumerator RevealImageRoutine()
    {
        float elapsed = 0f;

        float duration =
            Mathf.Max(
                0.01f,
                sceneSwitchTime
            );

        while (elapsed < duration)
        {
            elapsed +=
                Time.unscaledDeltaTime;

            float progress =
                Mathf.Clamp01(
                    elapsed / duration
                );


            // ----------------------------------------------------
            // LOGO FADE
            // ----------------------------------------------------

            if (startupImage != null)
            {
                Color imageColor =
                    startupImage.color;

                imageColor.a =
                    progress;

                startupImage.color =
                    imageColor;
            }


            // ----------------------------------------------------
            // PIXELS
            // ----------------------------------------------------

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


            // ----------------------------------------------------
            // DELAY
            // ----------------------------------------------------

            if (randomBurstDelay > 0f)
            {
                yield return new WaitForSecondsRealtime(
                    randomBurstDelay
                );
            }
            else
            {
                yield return null;
            }
        }


        // --------------------------------------------------------
        // FULLY VISIBLE
        // --------------------------------------------------------

        if (startupImage != null)
        {
            Color imageColor =
                startupImage.color;

            imageColor.a = 1f;

            startupImage.color =
                imageColor;
        }


        // --------------------------------------------------------
        // MAKE SURE ALL PIXELS ARE FILLED
        // --------------------------------------------------------

        FillRemainingPixels();
    }


    // ============================================================
    // RANDOM BURST
    // ============================================================

    private void SpawnRandomBurst(
        int amount,
        float progress)
    {
        if (filledGrid == null)
            return;

        if (filledPixelCount >= totalPixelCount)
            return;

        int maximumAttempts =
            Mathf.Max(
                totalPixelCount * 2,
                20
            );

        int attempts = 0;

        int spawned = 0;

        while (
            spawned < amount &&
            filledPixelCount < totalPixelCount &&
            attempts < maximumAttempts
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
        if (filledGrid == null)
            return;

        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < columns; x++)
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


        // --------------------------------------------------------
        // USE PREFAB
        // --------------------------------------------------------

        if (pixelPrefab != null)
        {
            pixelObject =
                Instantiate(
                    pixelPrefab.gameObject,
                    pixelContainer
                );
        }


        // --------------------------------------------------------
        // GENERATE IMAGE
        // --------------------------------------------------------

        else
        {
            pixelObject =
                new GameObject(
                    "HollowGroundsStartupPixel"
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


        // --------------------------------------------------------
        // PIXEL SIZE
        // --------------------------------------------------------

        float width =
            pixelContainer.rect.width /
            columns;

        float height =
            pixelContainer.rect.height /
            rows;


        // --------------------------------------------------------
        // PIXEL POSITION
        // --------------------------------------------------------

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


        // --------------------------------------------------------
        // COLOR
        // --------------------------------------------------------

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
                Random.value < redChance
            )
            {
                StartCoroutine(
                    RedFlashRoutine(
                        image
                    )
                );
            }
        }


        // --------------------------------------------------------
        // SMALL APPEAR ANIMATION
        // --------------------------------------------------------

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

        if (delay > 0f)
        {
            yield return new WaitForSecondsRealtime(
                delay
            );
        }

        if (image == null)
            yield break;

        image.color =
            redColor;


        if (redFlashDuration > 0f)
        {
            yield return new WaitForSecondsRealtime(
                redFlashDuration
            );
        }


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
            Vector3.one * 0.75f;

        float elapsed = 0f;

        const float duration = 0.025f;

        while (elapsed < duration)
        {
            if (pixel == null)
                yield break;

            elapsed +=
                Time.unscaledDeltaTime;

            float progress =
                Mathf.Clamp01(
                    elapsed / duration
                );

            float scale =
                Mathf.Lerp(
                    0.75f,
                    1f,
                    progress
                );

            pixel.localScale =
                Vector3.one * scale;

            yield return null;
        }

        if (pixel != null)
        {
            pixel.localScale =
                Vector3.one;
        }
    }


    // ============================================================
    // SCENE SWITCH
    // ============================================================

    private IEnumerator SceneSwitchRoutine()
    {
        if (sceneLoading)
            yield break;

        sceneLoading = true;

        // Wait exactly the amount of time selected
        // in the Inspector.
        if (sceneSwitchTime > 0f)
        {
            yield return new WaitForSecondsRealtime(
                sceneSwitchTime
            );
        }

        LoadTargetScene();
    }


    // ============================================================
    // LOAD TARGET SCENE
    // ============================================================

    private void LoadTargetScene()
    {
        if (sceneLoading == false)
            sceneLoading = true;

        if (
            sceneIndex < 0 ||
            sceneIndex >= SceneManager.sceneCountInBuildSettings
        )
        {
            Debug.LogError(
                "HollowGroundsStartup: Invalid scene index " +
                sceneIndex +
                ". Check your Build Settings."
            );

            return;
        }

        SceneManager.LoadScene(
            sceneIndex
        );
    }


    // ============================================================
    // SFX
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


    // ============================================================
    // DESTROY
    // ============================================================

    private void OnDestroy()
    {
        ClearPixels();
    }
}