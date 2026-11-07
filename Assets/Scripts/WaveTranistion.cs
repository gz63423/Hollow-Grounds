using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class WaveTransition : MonoBehaviour
{
    [Header("References")]
    public TextMeshProUGUI waveText;
    public Image background;
    public Image uiBackground;
    public Slider progressBar;
    public Image progressFill;
    public WaveSpawner waveSpawner;

    [Header("Player")]
    [Tooltip("Player that should be re-enabled when the tutorial wave scene begins.")]
    public Player player;

    [Tooltip("If true, movement/sprinting/attacking will be enabled when this scene starts.")]
    public bool enablePlayerOnStart = true;

    [Header("Transition Timing")]
    public float fadeInDuration = 1f;
    public float holdDuration = 1f;
    public float moveOutDuration = 1f;

    [Header("Text Animation")]
    public float textMoveDistance = 250f;

    [Range(0.1f, 1f)]
    public float finalTextScale = 0.45f;

    [Header("Main Background")]
    [Range(0f, 1f)]
    public float maxBackgroundAlpha = 200f / 255f;

    [Header("UI Background")]
    [Range(0f, 1f)]
    public float uiBackgroundAlpha = 119f / 255f;

    [Header("Progress Bar")]
    public float progressBarFadeDuration = 0.35f;

    [Header("Tutorial Kill Progress")]
    [Tooltip("Number of kills required to completely fill the progress bar.")]
    public int tutorialKillsRequired = 5;

    private int tutorialKills = 0;

    private RectTransform textRect;

    private Vector2 startPosition;
    private Vector2 finalPosition;

    private Color originalTextColor;
    private Color originalBackgroundColor;
    private Color originalUIBackgroundColor;

    private bool transitionFinished;


    // ==========================================
    // START
    // ==========================================

    private void Start()
    {
        FindPlayer();

        if (waveSpawner == null)
        {
            waveSpawner =
                FindFirstObjectByType<WaveSpawner>();
        }

        // --------------------------------------------------
        // The previous TutorialStartup may have disabled
        // the persistent Player before changing scenes.
        //
        // The tutorial-wave scene takes control of the
        // player here.
        // --------------------------------------------------

        EnablePlayerForWave();


        // ==========================================
        // REFERENCES
        // ==========================================

        if (waveText == null)
        {
            Debug.LogError(
                "WaveTransition: Wave Text reference is missing."
            );

            return;
        }

        if (background == null)
        {
            Debug.LogError(
                "WaveTransition: Background Image reference is missing."
            );

            return;
        }

        if (uiBackground == null)
        {
            Debug.LogError(
                "WaveTransition: UI Background Image reference is missing."
            );

            return;
        }


        textRect =
            waveText.GetComponent<RectTransform>();

        startPosition =
            textRect.anchoredPosition;

        finalPosition =
            startPosition +
            Vector2.up *
            textMoveDistance;

        originalTextColor =
            waveText.color;

        originalBackgroundColor =
            background.color;

        originalUIBackgroundColor =
            uiBackground.color;


        // ==========================================
        // INITIAL STATE
        // ==========================================

        SetBackgroundAlpha(0f);

        SetUIBackgroundAlpha(0f);

        SetTextAlpha(0f);


        // ==========================================
        // INITIALIZE PROGRESS BAR
        // ==========================================

        if (progressBar != null)
        {
            progressBar.gameObject.SetActive(true);

            progressBar.minValue = 0f;
            progressBar.maxValue = 1f;
            progressBar.value = 0f;

            SetProgressBarAlpha(0f);
        }


        // Hide Fill until progress exists.

        if (progressFill != null)
        {
            progressFill.gameObject.SetActive(false);
        }


        // Reset tutorial kill count.

        tutorialKills = 0;


        textRect.anchoredPosition =
            startPosition;

        waveText.transform.localScale =
            Vector3.one;


        // ==========================================
        // SET CORRECT WAVE TEXT
        // ==========================================

        UpdateWaveText();


        StartCoroutine(
            PlayTransition()
        );
    }


    // ==========================================
    // FIND PLAYER
    // ==========================================

    private void FindPlayer()
    {
        if (player != null)
            return;

        player =
            Player.Instance;

        if (player == null)
        {
            player =
                FindFirstObjectByType<Player>();
        }
    }


    // ==========================================
    // ENABLE PLAYER FOR WAVE
    // ==========================================

    private void EnablePlayerForWave()
    {
        if (!enablePlayerOnStart)
            return;

        FindPlayer();

        if (player == null)
        {
            Debug.LogWarning(
                "WaveTransition: Could not find Player. " +
                "Player movement was not automatically enabled."
            );

            return;
        }

        player.movementEnabled = true;
        player.sprintEnabled = true;
        player.attacksEnabled = true;
    }


    // ==========================================
    // GET CORRECT WAVE TEXT
    // ==========================================

    private void UpdateWaveText()
    {
        if (waveText == null)
            return;


        if (waveSpawner == null)
        {
            waveText.text =
                "Wave 1";

            return;
        }


        // ==========================================
        // ENDLESS MODE
        // ==========================================

        if (waveSpawner.IsEndlessMode)
        {
            waveText.text =
                "Endless Mode";

            return;
        }


        // ==========================================
        // CURRENT WAVE
        // ==========================================

        int currentWave =
            waveSpawner.CurrentWaveNumber;


        // ==========================================
        // THE FINALE
        // ==========================================

        if (
            waveSpawner.betaVersion &&
            currentWave ==
            waveSpawner.betaMaxWave
        )
        {
            waveText.text =
                "The Finale";

            return;
        }


        // ==========================================
        // NORMAL WAVE
        // ==========================================

        waveText.text =
            "Wave " +
            currentWave;
    }


    // ==========================================
    // TRANSITION
    // ==========================================

    private IEnumerator PlayTransition()
    {
        // ==========================================
        // PHASE 1
        // MAIN BACKGROUND + TEXT FADE IN
        // ==========================================

        float timer = 0f;

        while (timer < fadeInDuration)
        {
            timer +=
                Time.unscaledDeltaTime;

            float t =
                Mathf.Clamp01(
                    timer /
                    Mathf.Max(
                        0.01f,
                        fadeInDuration
                    )
                );


            SetBackgroundAlpha(
                Mathf.Lerp(
                    0f,
                    maxBackgroundAlpha,
                    t
                )
            );

            SetTextAlpha(t);

            SetUIBackgroundAlpha(0f);


            if (progressBar != null)
            {
                SetProgressBarAlpha(0f);
            }

            yield return null;
        }


        SetBackgroundAlpha(
            maxBackgroundAlpha
        );

        SetTextAlpha(1f);

        SetUIBackgroundAlpha(0f);


        if (progressBar != null)
        {
            SetProgressBarAlpha(0f);
        }


        if (progressFill != null)
        {
            progressFill.gameObject.SetActive(false);
        }


        // ==========================================
        // PHASE 2
        // HOLD
        // ==========================================

        if (holdDuration > 0f)
        {
            yield return new WaitForSecondsRealtime(
                holdDuration
            );
        }


        // ==========================================
        // PHASE 3
        // MOVE UP + SHRINK
        // MAIN BACKGROUND FADES OUT
        // ==========================================

        timer = 0f;

        while (timer < moveOutDuration)
        {
            timer +=
                Time.unscaledDeltaTime;

            float t =
                Mathf.Clamp01(
                    timer /
                    Mathf.Max(
                        0.01f,
                        moveOutDuration
                    )
                );


            // Move upward.

            textRect.anchoredPosition =
                Vector2.Lerp(
                    startPosition,
                    finalPosition,
                    t
                );


            // Shrink.

            float scale =
                Mathf.Lerp(
                    1f,
                    finalTextScale,
                    t
                );

            waveText.transform.localScale =
                Vector3.one *
                scale;


            // Fade main background out.

            SetBackgroundAlpha(
                Mathf.Lerp(
                    maxBackgroundAlpha,
                    0f,
                    t
                )
            );

            SetUIBackgroundAlpha(0f);


            if (progressBar != null)
            {
                SetProgressBarAlpha(0f);
            }

            yield return null;
        }


        // ==========================================
        // TEXT ANIMATION FINISHED
        // ==========================================

        SetBackgroundAlpha(0f);

        SetTextAlpha(1f);

        textRect.anchoredPosition =
            finalPosition;

        waveText.transform.localScale =
            Vector3.one *
            finalTextScale;


        // ==========================================
        // UI BACKGROUND APPEARS
        // ==========================================

        yield return StartCoroutine(
            FadeInUI()
        );


        // ==========================================
        // PROGRESS BAR APPEARS
        // ==========================================

        if (progressBar != null)
        {
            progressBar.gameObject.SetActive(true);

            yield return StartCoroutine(
                FadeInProgressBar()
            );
        }


        // ==========================================
        // ENDLESS MODE
        // ==========================================

        if (
            waveSpawner != null &&
            waveSpawner.IsEndlessMode
        )
        {
            SetWaveProgress(1f);
        }


        transitionFinished = true;
    }


    // ==========================================
    // FADE IN UI
    // ==========================================

    private IEnumerator FadeInUI()
    {
        float duration =
            Mathf.Max(
                0.01f,
                progressBarFadeDuration
            );

        float timer = 0f;


        while (timer < duration)
        {
            timer +=
                Time.unscaledDeltaTime;

            float t =
                Mathf.Clamp01(
                    timer /
                    duration
                );


            SetUIBackgroundAlpha(
                Mathf.Lerp(
                    0f,
                    uiBackgroundAlpha,
                    t
                )
            );

            yield return null;
        }


        SetUIBackgroundAlpha(
            uiBackgroundAlpha
        );
    }


    // ==========================================
    // FADE IN PROGRESS BAR
    // ==========================================

    private IEnumerator FadeInProgressBar()
    {
        if (progressBar == null)
            yield break;


        progressBar.gameObject.SetActive(true);


        float duration =
            Mathf.Max(
                0.01f,
                progressBarFadeDuration
            );

        float timer = 0f;


        while (timer < duration)
        {
            timer +=
                Time.unscaledDeltaTime;

            float t =
                Mathf.Clamp01(
                    timer /
                    duration
                );


            SetProgressBarAlpha(t);

            yield return null;
        }


        SetProgressBarAlpha(1f);
    }


    // ==========================================
    // REGISTER TUTORIAL KILL
    // ==========================================

    public void RegisterTutorialKill()
    {
        // Make sure the required amount is valid.

        int required =
            Mathf.Max(
                1,
                tutorialKillsRequired
            );


        // Do not allow the count to go above 5.

        if (tutorialKills >= required)
            return;


        // Add exactly one kill.

        tutorialKills++;


        // Convert the kill count into 0-1 progress.

        float progress =
            (float)tutorialKills /
            required;


        // Update the actual Slider.

        SetWaveProgress(progress);


        Debug.Log(
            "Tutorial Kill Progress: " +
            tutorialKills +
            "/" +
            required +
            " (" +
            Mathf.RoundToInt(progress * 100f) +
            "%)"
        );
    }


    // ==========================================
    // RESET TUTORIAL KILL PROGRESS
    // ==========================================

    public void ResetTutorialKillProgress()
    {
        tutorialKills = 0;

        SetWaveProgress(0f);
    }


    // ==========================================
    // GET TUTORIAL KILL COUNT
    // ==========================================

    public int GetTutorialKillCount()
    {
        return tutorialKills;
    }


    // ==========================================
    // WAVE PROGRESS
    // ==========================================

    public void SetWaveProgress(float progress)
    {
        if (progressBar == null)
            return;


        // Make sure the Slider itself is visible.

        if (!progressBar.gameObject.activeSelf)
        {
            progressBar.gameObject.SetActive(true);
        }


        // Endless Mode is always 100%.

        if (
            waveSpawner != null &&
            waveSpawner.IsEndlessMode
        )
        {
            progress = 1f;
        }
        else
        {
            progress =
                Mathf.Clamp01(progress);
        }


        progressBar.value =
            progress;


        // Hide Fill at 0%.
        // Show Fill as soon as progress exists.

        if (progressFill != null)
        {
            progressFill.gameObject.SetActive(
                progress > 0f
            );
        }
    }


    // ==========================================
    // HIDE PROGRESS BAR
    // ==========================================

    public void HideProgressBar()
    {
        if (progressBar != null)
        {
            progressBar.gameObject.SetActive(false);
        }
    }


    // ==========================================
    // RESET WAVE PROGRESS
    // ==========================================

    public void ResetWaveProgress()
    {
        if (progressBar == null)
            return;


        // Never reset Endless Mode back to 0%.

        if (
            waveSpawner != null &&
            waveSpawner.IsEndlessMode
        )
        {
            progressBar.gameObject.SetActive(true);

            progressBar.value = 1f;


            if (progressFill != null)
            {
                progressFill.gameObject.SetActive(true);
            }

            return;
        }


        progressBar.gameObject.SetActive(true);

        progressBar.value = 0f;


        if (progressFill != null)
        {
            progressFill.gameObject.SetActive(false);
        }
    }


    // ==========================================
    // MAIN BACKGROUND
    // ==========================================

    private void SetBackgroundAlpha(float alpha)
    {
        if (background == null)
            return;


        Color color =
            originalBackgroundColor;

        color.a =
            alpha;

        background.color =
            color;
    }


    // ==========================================
    // UI BACKGROUND
    // ==========================================

    private void SetUIBackgroundAlpha(float alpha)
    {
        if (uiBackground == null)
            return;


        Color color =
            originalUIBackgroundColor;

        color.a =
            alpha;

        uiBackground.color =
            color;
    }


    // ==========================================
    // WAVE TEXT
    // ==========================================

    private void SetTextAlpha(float alpha)
    {
        if (waveText == null)
            return;


        Color color =
            originalTextColor;

        color.a =
            alpha;

        waveText.color =
            color;
    }


    // ==========================================
    // PROGRESS BAR
    // ==========================================

    private void SetProgressBarAlpha(float alpha)
    {
        if (progressBar == null)
            return;


        Image[] images =
            progressBar.GetComponentsInChildren<Image>(
                true
            );


        foreach (Image image in images)
        {
            // Don't alter Fill alpha here.
            // Its visibility is controlled separately.

            if (
                progressFill != null &&
                image == progressFill
            )
            {
                continue;
            }


            Color color =
                image.color;

            color.a =
                alpha;

            image.color =
                color;
        }
    }
}