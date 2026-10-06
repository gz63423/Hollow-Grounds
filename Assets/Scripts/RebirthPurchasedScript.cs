using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class RebirthPurchasedScript : MonoBehaviour
{
    // ============================================================
    // SFX
    // ============================================================

    [Header("SFX")]

    public AudioClip sfx1;
    public AudioClip sfx2;

    [Range(0f, 2f)]
    public float sfxVolume = 1f;

    [Header("SFX Timing")]

    public float sfx1Time = 0f;
    public float sfx2Time = 0f;
    public float sfx3Time = 0f;
    public float sfx4Time = 0f;

    [Header("SFX Event Selection")]

    public SFXChoice sfx1Choice = SFXChoice.SFX1;
    public SFXChoice sfx2Choice = SFXChoice.SFX1;
    public SFXChoice sfx3Choice = SFXChoice.SFX2;
    public SFXChoice sfx4Choice = SFXChoice.SFX1;

    // ============================================================
    // TEXT
    // ============================================================

    [Header("Text")]

    public TMP_Text text1;
    public TMP_Text text2;
    public TMP_Text text3;
    public TMP_Text text4;

    // ============================================================
    // TEXT TIMING
    // ============================================================

    [Header("Text Timing")]

    [Tooltip("Time when Text 1 begins fading in.")]
    public float text1Time = 0f;

    [Tooltip("Time when Text 2 begins fading in.")]
    public float text2Time = 0f;

    [Tooltip("Time when Text 3 begins fading in.")]
    public float text3Time = 0f;

    [Tooltip("Time when Text 4 begins fading in.")]
    public float text4Time = 0f;

    // ============================================================
    // TEXT FADE
    // ============================================================

    [Header("Text Fade In")]

    public float text1FadeDuration = 1f;

    public float text2FadeDuration = 1f;

    public float text3FadeDuration = 1f;

    [Tooltip("How long Text 4 takes to fade in.")]
    public float text4FadeDuration = 1f;

    // ============================================================
    // TEXT CONTENT
    // ============================================================

    [Header("Text Content")]

    [TextArea(2, 5)]
    public string text1Content =
        "Rebirth 1 reached.";

    [TextArea(2, 5)]
    public string text2Content =
        "Your earnings multiplier has increased.";

    [TextArea(2, 5)]
    public string text3Content =
        "You are becoming stronger.";

    [TextArea(2, 5)]
    public string text4Content =
        "Click to continue";

    // ============================================================
    // MAIN MENU
    // ============================================================

    [Header("Main Menu")]

    [Tooltip("Build Index of the Main Menu scene.")]
    public int mainMenuSceneIndex = 0;

    // ============================================================
    // TEXT BEHAVIOR
    // ============================================================

    [Header("Text Behavior")]

    public bool hideTextAtStart = true;

    public bool enableTextObjects = true;

    // ============================================================
    // AUDIO
    // ============================================================

    private AudioSource audioSource;

    // ============================================================
    // STATE
    // ============================================================

    private bool text4FullyVisible = false;

    private bool loadingMainMenu = false;

    // ============================================================
    // ENUM
    // ============================================================

    public enum SFXChoice
    {
        SFX1,
        SFX2
    }

    // ============================================================
    // START
    // ============================================================

    private void Start()
    {
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
        // HIDE TEXT
        // --------------------------------------------------------

        if (hideTextAtStart)
        {
            HideText(text1);
            HideText(text2);
            HideText(text3);
            HideText(text4);
        }

        text4FullyVisible = false;

        loadingMainMenu = false;

        StartCoroutine(
            SceneSequence()
        );
    }

    // ============================================================
    // UPDATE
    // ============================================================

    private void Update()
    {
        if (loadingMainMenu)
            return;

        // IMPORTANT:
        // Clicking does absolutely nothing until
        // Text 4 has completely finished fading in.
        if (!text4FullyVisible)
            return;

        // Left mouse click
        if (Input.GetMouseButtonDown(0))
        {
            GoToMainMenu();
            return;
        }

        // Touch support
        if (Input.touchCount > 0)
        {
            if (
                Input.GetTouch(0).phase ==
                TouchPhase.Began
            )
            {
                GoToMainMenu();
            }
        }
    }

    // ============================================================
    // MAIN SEQUENCE
    // ============================================================

    private IEnumerator SceneSequence()
    {
        float elapsed = 0f;

        bool sfx1Played = false;
        bool sfx2Played = false;
        bool sfx3Played = false;
        bool sfx4Played = false;

        bool text1Started = false;
        bool text2Started = false;
        bool text3Started = false;
        bool text4Started = false;

        while (true)
        {
            elapsed +=
                Time.unscaledDeltaTime;

            // ====================================================
            // SFX 1
            // ====================================================

            if (!sfx1Played &&
                elapsed >= sfx1Time)
            {
                PlaySFX(
                    sfx1Choice
                );

                sfx1Played = true;
            }

            // ====================================================
            // SFX 2
            // ====================================================

            if (!sfx2Played &&
                elapsed >= sfx2Time)
            {
                PlaySFX(
                    sfx2Choice
                );

                sfx2Played = true;
            }

            // ====================================================
            // SFX 3
            // ====================================================

            if (!sfx3Played &&
                elapsed >= sfx3Time)
            {
                PlaySFX(
                    sfx3Choice
                );

                sfx3Played = true;
            }

            // ====================================================
            // SFX 4
            // ====================================================

            if (!sfx4Played &&
                elapsed >= sfx4Time)
            {
                PlaySFX(
                    sfx4Choice
                );

                sfx4Played = true;
            }

            // ====================================================
            // TEXT 1
            // ====================================================

            if (!text1Started &&
                elapsed >= text1Time)
            {
                StartTextFade(
                    text1,
                    text1Content,
                    text1FadeDuration,
                    false
                );

                text1Started = true;
            }

            // ====================================================
            // TEXT 2
            // ====================================================

            if (!text2Started &&
                elapsed >= text2Time)
            {
                StartTextFade(
                    text2,
                    text2Content,
                    text2FadeDuration,
                    false
                );

                text2Started = true;
            }

            // ====================================================
            // TEXT 3
            // ====================================================

            if (!text3Started &&
                elapsed >= text3Time)
            {
                StartTextFade(
                    text3,
                    text3Content,
                    text3FadeDuration,
                    false
                );

                text3Started = true;
            }

            // ====================================================
            // TEXT 4
            // ====================================================

            if (!text4Started &&
                elapsed >= text4Time)
            {
                StartTextFade(
                    text4,
                    text4Content,
                    text4FadeDuration,
                    true
                );

                text4Started = true;
            }

            // ====================================================
            // FINISHED STARTING EVERYTHING
            // ====================================================

            if (
                sfx1Played &&
                sfx2Played &&
                sfx3Played &&
                sfx4Played &&
                text1Started &&
                text2Started &&
                text3Started &&
                text4Started
            )
            {
                break;
            }

            yield return null;
        }
    }

    // ============================================================
    // START TEXT FADE
    // ============================================================

    private void StartTextFade(
        TMP_Text text,
        string content,
        float fadeDuration,
        bool isFinalText
    )
    {
        if (text == null)
            return;

        StartCoroutine(
            FadeText(
                text,
                content,
                fadeDuration,
                isFinalText
            )
        );
    }

    // ============================================================
    // FADE TEXT
    // ============================================================

    private IEnumerator FadeText(
        TMP_Text text,
        string content,
        float duration,
        bool isFinalText
    )
    {
        if (text == null)
            yield break;

        text.text =
            content;

        if (enableTextObjects)
        {
            text.gameObject.SetActive(
                true
            );
        }

        Color color =
            text.color;

        text.color =
            new Color(
                color.r,
                color.g,
                color.b,
                0f
            );

        // --------------------------------------------------------
        // INSTANT
        // --------------------------------------------------------

        if (duration <= 0f)
        {
            text.color =
                new Color(
                    color.r,
                    color.g,
                    color.b,
                    1f
                );

            if (isFinalText)
            {
                text4FullyVisible = true;
            }

            yield break;
        }

        // --------------------------------------------------------
        // FADE IN
        // --------------------------------------------------------

        float elapsed = 0f;

        while (
            elapsed <
            duration
        )
        {
            if (text == null)
                yield break;

            elapsed +=
                Time.unscaledDeltaTime;

            float progress =
                Mathf.Clamp01(
                    elapsed /
                    duration
                );

            float alpha =
                Mathf.Lerp(
                    0f,
                    1f,
                    progress
                );

            text.color =
                new Color(
                    color.r,
                    color.g,
                    color.b,
                    alpha
                );

            yield return null;
        }

        // --------------------------------------------------------
        // FULLY VISIBLE
        // --------------------------------------------------------

        if (text != null)
        {
            text.color =
                new Color(
                    color.r,
                    color.g,
                    color.b,
                    1f
                );
        }

        // IMPORTANT:
        // This is only set AFTER the fade has completely finished.
        if (isFinalText)
        {
            text4FullyVisible = true;

            Debug.Log(
                "Rebirth Scene 13: " +
                "Click to continue is fully visible."
            );
        }
    }

    // ============================================================
    // PLAY SFX
    // ============================================================

    private void PlaySFX(
        SFXChoice choice
    )
    {
        if (audioSource == null)
            return;

        AudioClip clip = null;

        if (choice == SFXChoice.SFX1)
        {
            clip = sfx1;
        }
        else if (choice == SFXChoice.SFX2)
        {
            clip = sfx2;
        }

        if (clip == null)
        {
            Debug.LogWarning(
                "RebirthScene13Manager: " +
                "SFX clip is not assigned."
            );

            return;
        }

        float volume =
            sfxVolume;

        if (SettingsManager.Instance != null)
        {
            volume *=
                SettingsManager.Instance.sfxVolume;
        }

        audioSource.PlayOneShot(
            clip,
            volume
        );
    }

    // ============================================================
    // GO TO MAIN MENU
    // ============================================================

    private void GoToMainMenu()
    {
        if (loadingMainMenu)
            return;

        loadingMainMenu = true;

        Debug.Log(
            "Rebirth Scene 13: " +
            "Returning to Main Menu."
        );

        SceneManager.LoadScene(
            mainMenuSceneIndex
        );
    }

    // ============================================================
    // HIDE TEXT
    // ============================================================

    private void HideText(
        TMP_Text text
    )
    {
        if (text == null)
            return;

        Color color =
            text.color;

        text.color =
            new Color(
                color.r,
                color.g,
                color.b,
                0f
            );

        if (enableTextObjects)
        {
            text.gameObject.SetActive(
                false
            );
        }
    }
}