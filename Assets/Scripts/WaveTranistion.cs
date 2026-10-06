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

    private RectTransform textRect;

    private Vector2 startPosition;
    private Vector2 finalPosition;

    private Color originalTextColor;
    private Color originalBackgroundColor;
    private Color originalUIBackgroundColor;


    // ==========================================
    // START
    // ==========================================

    private void Start()
    {
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

        // Automatically find WaveSpawner if
        // it wasn't assigned in the Inspector.

        if (waveSpawner == null)
        {
            waveSpawner =
                FindFirstObjectByType<WaveSpawner>();
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

        if (progressBar != null)
        {
            progressBar.value = 0f;
            SetProgressBarAlpha(0f);
        }

        // Hide the actual Fill until
        // the first enemy is killed.

        if (progressFill != null)
        {
            progressFill.gameObject.SetActive(false);
        }

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
    // GET CORRECT WAVE TEXT
    // ==========================================

    private void UpdateWaveText()
    {
        if (waveText == null)
            return;

        if (waveSpawner == null)
        {
            waveText.text = "Wave 1";
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
            timer += Time.deltaTime;

            float t =
                Mathf.Clamp01(
                    timer /
                    fadeInDuration
                );

            SetBackgroundAlpha(
                Mathf.Lerp(
                    0f,
                    maxBackgroundAlpha,
                    t
                )
            );

            SetTextAlpha(t);

            // UI background remains completely hidden.
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

        // Make absolutely sure Fill is hidden.
        if (progressFill != null)
        {
            progressFill.gameObject.SetActive(false);
        }


        // ==========================================
        // PHASE 2
        // HOLD
        // ==========================================

        yield return new WaitForSeconds(
            holdDuration
        );


        // ==========================================
        // PHASE 3
        // MOVE UP + SHRINK
        // MAIN BACKGROUND FADES OUT
        // ==========================================

        timer = 0f;

        while (timer < moveOutDuration)
        {
            timer += Time.deltaTime;

            float t =
                Mathf.Clamp01(
                    timer /
                    moveOutDuration
                );


            // Move straight upward.
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
                Vector3.one * scale;


            // Fade main background out.
            SetBackgroundAlpha(
                Mathf.Lerp(
                    maxBackgroundAlpha,
                    0f,
                    t
                )
            );


            // UI background remains completely hidden.
            SetUIBackgroundAlpha(0f);


            // Progress bar remains completely hidden.
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
            yield return StartCoroutine(
                FadeInProgressBar()
            );
        }


        // ==========================================
        // ENDLESS MODE
        // ==========================================

        // Make absolutely sure Endless Mode
        // remains at 100%.

        if (
            waveSpawner != null &&
            waveSpawner.IsEndlessMode
        )
        {
            SetWaveProgress(1f);
        }
    }


    // ==========================================
    // FADE IN UI
    // ==========================================

    private IEnumerator FadeInUI()
    {
        float timer = 0f;

        while (timer < progressBarFadeDuration)
        {
            timer += Time.deltaTime;

            float t =
                Mathf.Clamp01(
                    timer /
                    progressBarFadeDuration
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
        float timer = 0f;

        while (timer < progressBarFadeDuration)
        {
            timer += Time.deltaTime;

            float t =
                Mathf.Clamp01(
                    timer /
                    progressBarFadeDuration
                );

            SetProgressBarAlpha(t);

            yield return null;
        }

        SetProgressBarAlpha(1f);
    }


    // ==========================================
    // WAVE PROGRESS
    // ==========================================

    public void SetWaveProgress(float progress)
    {
        if (progressBar == null)
            return;


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
        // Show it as soon as progress exists.

        if (progressFill != null)
        {
            progressFill.gameObject.SetActive(
                progress > 0f
            );
        }
    }

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
            progressBar.value = 1f;

            if (progressFill != null)
            {
                progressFill.gameObject.SetActive(true);
            }

            return;
        }


        progressBar.value = 0f;


        // Hide Fill again for the new wave.

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

        color.a = alpha;

        background.color = color;
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

        color.a = alpha;

        uiBackground.color = color;
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

        color.a = alpha;

        waveText.color = color;
    }


    // ==========================================
    // PROGRESS BAR
    // ==========================================

    private void SetProgressBarAlpha(float alpha)
    {
        if (progressBar == null)
            return;


        // Get every Image underneath the Slider.
        // This controls the Background, Fill,
        // Outline, and any other Image components.

        Image[] images =
            progressBar.GetComponentsInChildren<Image>(
                true
            );


        foreach (Image image in images)
        {
            // Don't change the Fill's alpha here
            // while it is disabled.

            if (
                progressFill != null &&
                image == progressFill
            )
            {
                continue;
            }


            Color color =
                image.color;

            color.a = alpha;

            image.color = color;
        }
    }
}

