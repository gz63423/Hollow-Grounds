using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class GameStartup : MonoBehaviour
{
    public static GameStartup Instance { get; private set; }


    // ============================================================
    // STARTUP TIMER
    // ============================================================

    [Header("Startup Timer")]

    [Tooltip("How long GameStartup waits before allowing the logo and Begin text to fade in.")]
    public float startupDelay = 1f;


    // ============================================================
    // LOGO
    // ============================================================

    [Header("Startup Logo")]

    public Image startupLogo;

    [Tooltip("How long the logo takes to fade in.")]
    public float logoFadeInDuration = 0.75f;

    [Tooltip("How much the logo subtly grows/shrinks while idle.")]
    public float logoIdleScaleAmount = 0.04f;

    [Tooltip("Duration of one complete idle scale cycle.")]
    public float logoIdleScaleDuration = 2f;

    [Tooltip("How long the logo takes to fade out after clicking.")]
    public float logoFadeOutDuration = 1f;

    [Tooltip("Final logo scale during the click transition.")]
    public float logoExpandedScale = 1.5f;


    // ============================================================
    // BEGIN TEXT
    // ============================================================

    [Header("Begin Text")]

    public TMP_Text beginText;

    [Tooltip("How long the Begin text takes to fade in.")]
    public float beginTextFadeInDuration = 0.75f;

    [Tooltip("Whether the Begin text should continuously flash while idle.")]
    public bool flashBeginText = true;

    [Tooltip("Minimum alpha of the flashing Begin text.")]
    [Range(0f, 1f)]
    public float beginTextMinimumAlpha = 0.35f;

    [Tooltip("Duration of one complete Begin text flash cycle.")]
    public float beginTextFlashDuration = 1.5f;

    [Tooltip("How long the Begin text takes to fade out after clicking.")]
    public float beginTextFadeOutDuration = 0.5f;


    // ============================================================
    // CENTER UI LIGHT
    // ============================================================

    [Header("Center UI Light")]

    [Tooltip("Center UI Light that will brighten when Begin is clicked.")]
    public CenterUILight centerUILight;

    [Tooltip("How long the center light takes to reach its normal intensity after clicking.")]
    public float centerLightFadeInDuration = 1f;


    // ============================================================
    // CLICK SETTINGS
    // ============================================================

    [Header("Click Settings")]

    [Tooltip("Allows clicking anywhere with the left mouse button to begin.")]
    public bool clickAnywhereToBegin = true;


    // ============================================================
    // STARTUP SFX
    // ============================================================

    [Header("Startup SFX")]

    [Tooltip("Sound played once when the GameStartup scene begins.")]
    public AudioClip startupSFX;

    [Range(0f, 2f)]
    public float startupSFXVolume = 1f;

    [Tooltip("Delay before the startup SFX plays.")]
    public float startupSFXDelay = 0.1f;


    // ============================================================
    // TRANSITION SFX
    // ============================================================

    [Header("Transition SFX")]

    [Tooltip("Sound played immediately when Begin is clicked.")]
    public AudioClip transitionSFX;

    [Range(0f, 2f)]
    public float transitionSFXVolume = 1f;

    private AudioSource audioSource;


    // ============================================================
    // PIXEL TRANSITION
    // ============================================================

    [Header("Pixel Transition")]

    [Tooltip("How long the pixelation takes to completely cover the screen.")]
    public float transitionDuration = 3.5f;

    [Tooltip("How long the screen stays completely black after the pixels finish.")]
    public float finalBlackHold = 0.05f;


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

    public float randomBurstDelay = 0.01f;


    // ============================================================
    // SCENE LOADING
    // ============================================================

    [Header("Scene Loading")]

    public string targetSceneName = "Main Menu";

    public int targetSceneBuildIndex = 2;


    // ============================================================
    // STATE
    // ============================================================

    private bool startupRunning;

    private bool startupReady;

    private bool clickedToBegin;

    private bool sceneChanging;


    // ============================================================
    // COROUTINES
    // ============================================================

    private Coroutine startupCoroutine;

    private Coroutine logoIdleCoroutine;

    private Coroutine beginTextFlashCoroutine;

    private Coroutine pixelTransitionCoroutine;

    private Coroutine centerLightFadeCoroutine;


    // ============================================================
    // PIXEL GRID STATE
    // ============================================================

    private bool[,] filledGrid;

    private int filledPixelCount;

    private int totalPixelCount;


    // ============================================================
    // SPAWNED PIXELS
    // ============================================================

    private readonly List<GameObject> spawnedPixels =
        new List<GameObject>();


    // ============================================================
    // AWAKE
    // ============================================================

    private void Awake()
    {
        // --------------------------------------------------------
        // SINGLETON
        // --------------------------------------------------------

        if (Instance != null &&
            Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        DontDestroyOnLoad(gameObject);


        // --------------------------------------------------------
        // PIXEL CONTAINER
        // --------------------------------------------------------

        if (pixelContainer == null)
        {
            pixelContainer =
                GetComponent<RectTransform>();
        }


        // --------------------------------------------------------
        // AUDIO SOURCE
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
        // LOGO INITIAL STATE
        // --------------------------------------------------------

        if (startupLogo != null)
        {
            Color color =
                startupLogo.color;

            color.a = 0f;

            startupLogo.color =
                color;

            startupLogo.rectTransform.localScale =
                Vector3.one;
        }


        // --------------------------------------------------------
        // BEGIN TEXT INITIAL STATE
        // --------------------------------------------------------

        if (beginText != null)
        {
            Color color =
                beginText.color;

            color.a = 0f;

            beginText.color =
                color;
        }


        // --------------------------------------------------------
        // CENTER LIGHT INITIAL STATE
        // --------------------------------------------------------

        if (centerUILight != null)
        {
            Color lightColor =
                centerUILight.lightColor;

            lightColor.a = 1f;

            centerUILight.lightColor =
                lightColor;
        }


        // --------------------------------------------------------
        // INITIAL STATE
        // --------------------------------------------------------

        startupRunning = false;

        startupReady = false;

        clickedToBegin = false;

        sceneChanging = false;


        // --------------------------------------------------------
        // SCENE EVENT
        // --------------------------------------------------------

        SceneManager.sceneLoaded -=
            OnSceneLoaded;

        SceneManager.sceneLoaded +=
            OnSceneLoaded;
    }


    // ============================================================
    // ON DESTROY
    // ============================================================

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -=
            OnSceneLoaded;
    }


    // ============================================================
    // START
    // ============================================================

    private void Start()
    {
        // --------------------------------------------------------
        // STARTUP SFX
        // --------------------------------------------------------

        StartCoroutine(
            StartupSFXRoutine()
        );


        // --------------------------------------------------------
        // START STARTUP
        // --------------------------------------------------------

        StartStartup();
    }


    // ============================================================
    // UPDATE
    // ============================================================

    private void Update()
    {
        if (!startupRunning)
            return;


        // --------------------------------------------------------
        // BLOCK INPUT UNTIL FULLY READY
        // --------------------------------------------------------

        if (!startupReady)
            return;

        if (clickedToBegin)
            return;

        if (sceneChanging)
            return;

        if (!clickAnywhereToBegin)
            return;


        if (Input.GetMouseButtonDown(0))
        {
            BeginGame();
        }
    }


    // ============================================================
    // SCENE LOADED
    // ============================================================

    private void OnSceneLoaded(
        Scene scene,
        LoadSceneMode mode
    )
    {
        Debug.Log(
            "GameStartup: Scene loaded: " +
            scene.buildIndex
        );


        if (scene.buildIndex ==
            targetSceneBuildIndex)
        {
            if (clickedToBegin)
            {
                StartCoroutine(
                    FinishSceneTransition()
                );
            }
        }
    }


    // ============================================================
    // START STARTUP
    // ============================================================

    public void StartStartup()
    {
        if (startupRunning)
            return;


        startupRunning = true;

        startupReady = false;

        clickedToBegin = false;

        sceneChanging = false;


        // --------------------------------------------------------
        // STOP OLD COROUTINES
        // --------------------------------------------------------

        if (startupCoroutine != null)
        {
            StopCoroutine(
                startupCoroutine
            );
        }


        if (logoIdleCoroutine != null)
        {
            StopCoroutine(
                logoIdleCoroutine
            );

            logoIdleCoroutine = null;
        }


        if (beginTextFlashCoroutine != null)
        {
            StopCoroutine(
                beginTextFlashCoroutine
            );

            beginTextFlashCoroutine = null;
        }


        if (centerLightFadeCoroutine != null)
        {
            StopCoroutine(
                centerLightFadeCoroutine
            );

            centerLightFadeCoroutine = null;
        }





        // --------------------------------------------------------
        // START STARTUP ROUTINE
        // --------------------------------------------------------

        startupCoroutine =
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
        // STARTUP DELAY
        // --------------------------------------------------------

        float delay =
            Mathf.Max(
                0f,
                startupDelay
            );


        if (delay > 0f)
        {
            yield return new WaitForSecondsRealtime(
                delay
            );
        }


        // --------------------------------------------------------
        // LOGO FADE IN
        // --------------------------------------------------------

        yield return StartCoroutine(
            LogoFadeInRoutine()
        );


        // --------------------------------------------------------
        // BEGIN TEXT FADE IN
        // --------------------------------------------------------

        yield return StartCoroutine(
            BeginTextFadeInRoutine()
        );


        // --------------------------------------------------------
        // LOGO IDLE
        // --------------------------------------------------------

        if (startupLogo != null)
        {
            logoIdleCoroutine =
                StartCoroutine(
                    LogoIdleRoutine()
                );
        }


        // --------------------------------------------------------
        // BEGIN TEXT FLASH
        // --------------------------------------------------------

        if (flashBeginText &&
            beginText != null)
        {
            beginTextFlashCoroutine =
                StartCoroutine(
                    BeginTextFlashRoutine()
                );
        }


        // --------------------------------------------------------
        // FULLY READY
        // --------------------------------------------------------

        startupReady = true;


        Debug.Log(
            "GameStartup: Startup fully ready. Input enabled."
        );


        startupCoroutine = null;
    }


    // ============================================================
    // LOGO FADE IN
    // ============================================================

    private IEnumerator LogoFadeInRoutine()
    {
        if (startupLogo == null)
            yield break;


        float duration =
            Mathf.Max(
                0.01f,
                logoFadeInDuration
            );


        Color color =
            startupLogo.color;

        color.a = 0f;

        startupLogo.color =
            color;


        float elapsed = 0f;


        while (elapsed < duration)
        {
            if (startupLogo == null)
                yield break;


            elapsed +=
                Time.unscaledDeltaTime;


            float progress =
                Mathf.Clamp01(
                    elapsed /
                    duration
                );


            float smoothProgress =
                Mathf.SmoothStep(
                    0f,
                    1f,
                    progress
                );


            color =
                startupLogo.color;

            color.a =
                Mathf.Lerp(
                    0f,
                    1f,
                    smoothProgress
                );

            startupLogo.color =
                color;


            yield return null;
        }


        color =
            startupLogo.color;

        color.a = 1f;

        startupLogo.color =
            color;
    }


    // ============================================================
    // LOGO IDLE
    // ============================================================

    private IEnumerator LogoIdleRoutine()
    {
        if (startupLogo == null)
            yield break;


        float duration =
            Mathf.Max(
                0.01f,
                logoIdleScaleDuration
            );


        while (!clickedToBegin)
        {
            if (startupLogo == null)
                yield break;


            float time =
                Time.unscaledTime /
                duration;


            float wave =
                Mathf.Sin(
                    time *
                    Mathf.PI *
                    2f
                );


            float scale =
                1f +
                wave *
                logoIdleScaleAmount;


            startupLogo.rectTransform.localScale =
                Vector3.one *
                scale;


            yield return null;
        }
    }


    // ============================================================
    // BEGIN TEXT FADE IN
    // ============================================================

    private IEnumerator BeginTextFadeInRoutine()
    {
        if (beginText == null)
            yield break;


        float duration =
            Mathf.Max(
                0.01f,
                beginTextFadeInDuration
            );


        Color color =
            beginText.color;

        color.a = 0f;

        beginText.color =
            color;


        float elapsed = 0f;


        while (elapsed < duration)
        {
            if (beginText == null)
                yield break;


            elapsed +=
                Time.unscaledDeltaTime;


            float progress =
                Mathf.Clamp01(
                    elapsed /
                    duration
                );


            float smoothProgress =
                Mathf.SmoothStep(
                    0f,
                    1f,
                    progress
                );


            color =
                beginText.color;

            color.a =
                Mathf.Lerp(
                    0f,
                    1f,
                    smoothProgress
                );

            beginText.color =
                color;


            yield return null;
        }


        color =
            beginText.color;

        color.a = 1f;

        beginText.color =
            color;
    }


    // ============================================================
    // BEGIN TEXT FLASH
    // ============================================================

    private IEnumerator BeginTextFlashRoutine()
    {
        if (beginText == null)
            yield break;


        float duration =
            Mathf.Max(
                0.01f,
                beginTextFlashDuration
            );


        while (!clickedToBegin)
        {
            if (beginText == null)
                yield break;


            float time =
                Time.unscaledTime /
                duration;


            float wave =
                (Mathf.Sin(
                    time *
                    Mathf.PI *
                    2f
                ) + 1f) *
                0.5f;


            Color color =
                beginText.color;

            color.a =
                Mathf.Lerp(
                    beginTextMinimumAlpha,
                    1f,
                    wave
                );

            beginText.color =
                color;


            yield return null;
        }
    }


    // ============================================================
    // BEGIN GAME
    // ============================================================

    public void BeginGame()
    {
        if (!startupRunning)
            return;

        if (!startupReady)
            return;

        if (clickedToBegin)
            return;

        if (sceneChanging)
            return;


        clickedToBegin = true;


        // --------------------------------------------------------
        // STOP IDLE ANIMATIONS
        // --------------------------------------------------------

        if (logoIdleCoroutine != null)
        {
            StopCoroutine(
                logoIdleCoroutine
            );

            logoIdleCoroutine = null;
        }


        if (beginTextFlashCoroutine != null)
        {
            StopCoroutine(
                beginTextFlashCoroutine
            );

            beginTextFlashCoroutine = null;
        }


        // --------------------------------------------------------
        // START CENTER LIGHT FADE
        // --------------------------------------------------------

        if (centerUILight != null)
        {
            if (centerLightFadeCoroutine != null)
            {
                StopCoroutine(
                    centerLightFadeCoroutine
                );
            }

            centerLightFadeCoroutine =
                StartCoroutine(
                    CenterLightFadeRoutine()
                );
        }


        // --------------------------------------------------------
        // START CLICK TRANSITION
        // --------------------------------------------------------

        StartCoroutine(
            ClickSequenceRoutine()
        );
    }


    // ============================================================
    // CENTER LIGHT FADE
    // ============================================================

    private IEnumerator CenterLightFadeRoutine()
    {
        if (centerUILight == null)
            yield break;

        centerUILight.ActivateClickedBoost();

        centerLightFadeCoroutine = null;

        yield break;
    }





    // ============================================================
    // CLICK SEQUENCE
    // ============================================================

    private IEnumerator ClickSequenceRoutine()
    {
        // ========================================================
        // INITIALIZE PIXEL GRID
        // ========================================================

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


        if (pixelContainer == null)
        {
            Debug.LogError(
                "GameStartup: Pixel Container is missing."
            );
        }
        else
        {
            pixelContainer.gameObject.SetActive(true);


            filledGrid =
                new bool[
                    columns,
                    rows
                ];


            filledPixelCount = 0;


            totalPixelCount =
                columns *
                rows;


            Debug.Log(
                "GameStartup: Pixel container found. " +
                "Size = " +
                pixelContainer.rect.width +
                " x " +
                pixelContainer.rect.height
            );


            pixelTransitionCoroutine =
                StartCoroutine(
                    PixelTransitionRoutine()
                );
        }


        // ========================================================
        // LOGO FADE OUT IMMEDIATELY
        // ========================================================

        StartCoroutine(
            LogoFadeOutRoutine()
        );


        // ========================================================
        // BEGIN TEXT FADE OUT IMMEDIATELY
        // ========================================================

        StartCoroutine(
            BeginTextFadeOutRoutine()
        );


        yield break;
    }


    // ============================================================
    // PIXEL TRANSITION
    // ============================================================

    private IEnumerator PixelTransitionRoutine()
    {
        Debug.Log(
            "GameStartup: Pixel transition started."
        );


        // ========================================================
        // PLAY TRANSITION SFX
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
        // FILL EVERYTHING
        // ========================================================

        FillRemainingPixels();


        // ========================================================
        // FORCE BLACK
        // ========================================================

        ForceAllPixelsBlack();


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
    // SPAWN RANDOM BURST
    // ============================================================

    private void SpawnRandomBurst(
        int amount,
        float progress
    )
    {
        if (filledGrid == null)
            return;


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
        if (filledGrid == null)
            return;


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
        float progress
    )
    {
        if (pixelContainer == null)
            return;


        if (filledGrid == null)
            return;


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


            pixelObject.SetActive(true);
        }
        else
        {
            pixelObject =
                new GameObject(
                    "GameStartupPixel"
                );


            pixelObject.transform.SetParent(
                pixelContainer,
                false
            );


            Image generatedImage =
                pixelObject.AddComponent<Image>();


            generatedImage.raycastTarget =
                false;


            pixelObject.SetActive(true);
        }


        spawnedPixels.Add(
            pixelObject
        );


        // ========================================================
        // RECT TRANSFORM
        // ========================================================

        RectTransform pixelRect =
            pixelObject.GetComponent<RectTransform>();


        if (pixelRect == null)
        {
            pixelRect =
                pixelObject.AddComponent<RectTransform>();
        }


        pixelRect.SetParent(
            pixelContainer,
            false
        );


        pixelRect.SetAsLastSibling();


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


        // ========================================================
        // IMAGE
        // ========================================================

        Image image =
            pixelObject.GetComponent<Image>();


        if (image != null)
        {
            image.enabled = true;


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


        // ========================================================
        // PIXEL APPEAR
        // ========================================================

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


            pixel.SetActive(true);


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

                rect.SetAsLastSibling();
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
            "GameStartup: Pixel transition finished."
        );


        Debug.Log(
            "GameStartup: Loading target scene: " +
            targetSceneBuildIndex
        );


        SceneManager.LoadScene(
            targetSceneBuildIndex
        );
    }


    // ============================================================
    // FINISH AFTER SCENE LOAD
    // ============================================================

    private IEnumerator FinishSceneTransition()
    {
        yield return null;

        yield return new WaitForEndOfFrame();


        Debug.Log(
            "GameStartup: Target scene is now visible. Removing transition."
        );


        ClearPixels();


        startupRunning = false;

        startupReady = false;

        sceneChanging = false;

        pixelTransitionCoroutine = null;
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


        filledGrid = null;


        filledPixelCount = 0;


        totalPixelCount = 0;
    }


    // ============================================================
    // STARTUP SFX ROUTINE
    // ============================================================

    private IEnumerator StartupSFXRoutine()
    {
        float delay =
            Mathf.Max(
                0f,
                startupSFXDelay
            );


        if (delay > 0f)
        {
            yield return new WaitForSecondsRealtime(
                delay
            );
        }


        PlayStartupSFX();
    }


    // ============================================================
    // PLAY STARTUP SFX
    // ============================================================

    private void PlayStartupSFX()
    {
        if (audioSource == null)
            return;


        if (startupSFX == null)
            return;


        float volume =
            startupSFXVolume;


        if (SettingsManager.Instance != null)
        {
            volume *=
                SettingsManager.Instance.sfxVolume;
        }


        audioSource.PlayOneShot(
            startupSFX,
            volume
        );
    }


    // ============================================================
    // PLAY TRANSITION SFX
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
    // LOGO FADE OUT
    // ============================================================

    private IEnumerator LogoFadeOutRoutine()
    {
        if (startupLogo == null)
            yield break;


        float duration =
            Mathf.Max(
                0.01f,
                logoFadeOutDuration
            );


        Vector3 startScale =
            startupLogo.rectTransform.localScale;


        Vector3 targetScale =
            Vector3.one *
            logoExpandedScale;


        Color color =
            startupLogo.color;


        float startAlpha =
            color.a;


        float elapsed = 0f;


        while (elapsed < duration)
        {
            if (startupLogo == null)
                yield break;


            elapsed +=
                Time.unscaledDeltaTime;


            float progress =
                Mathf.Clamp01(
                    elapsed /
                    duration
                );


            float smoothProgress =
                Mathf.SmoothStep(
                    0f,
                    1f,
                    progress
                );


            startupLogo.rectTransform.localScale =
                Vector3.Lerp(
                    startScale,
                    targetScale,
                    smoothProgress
                );


            color =
                startupLogo.color;


            color.a =
                Mathf.Lerp(
                    startAlpha,
                    0f,
                    smoothProgress
                );


            startupLogo.color =
                color;


            yield return null;
        }


        color =
            startupLogo.color;


        color.a = 0f;

        startupLogo.color =
            color;


        startupLogo.rectTransform.localScale =
            targetScale;
    }


    // ============================================================
    // BEGIN TEXT FADE OUT
    // ============================================================

    private IEnumerator BeginTextFadeOutRoutine()
    {
        if (beginText == null)
            yield break;


        float duration =
            Mathf.Max(
                0.01f,
                beginTextFadeOutDuration
            );


        Color color =
            beginText.color;


        float startAlpha =
            color.a;


        float elapsed = 0f;


        while (elapsed < duration)
        {
            if (beginText == null)
                yield break;


            elapsed +=
                Time.unscaledDeltaTime;


            float progress =
                Mathf.Clamp01(
                    elapsed /
                    duration
                );


            float smoothProgress =
                Mathf.SmoothStep(
                    0f,
                    1f,
                    progress
                );


            color =
                beginText.color;


            color.a =
                Mathf.Lerp(
                    startAlpha,
                    0f,
                    smoothProgress
                );


            beginText.color =
                color;


            yield return null;
        }


        color =
            beginText.color;


        color.a = 0f;

        beginText.color =
            color;
    }


}
