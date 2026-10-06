using UnityEngine;
using TMPro;
using System.Collections;

public class RebirthDialogue : MonoBehaviour
{
    [Header("Dialogue Text")]
    public TMP_Text dialogueText;

    [Header("Timing")]
    public float initialDelay = 2f;
    public float fadeInDuration = 2f;
    public float holdDuration = 2f;
    public float fadeOutDuration = 2f;

    [Header("Dialogue")]
    [TextArea(2, 4)]
    public string[] dialogueLines =
    {
        "They are becoming too powerful.",
        "I created the substance to extend human life.",
        "I never intended for it to change us.",
        "But every time I die...",
        "...it brings me back.",
        "And every time it does...",
        "...I feel different.",
        "Maybe death was never the end.",
        "Maybe it was the price of becoming stronger."
    };

    // ============================================================
    // SFX
    // ============================================================

    [Header("SFX")]

    [Tooltip("SFX 1 - Click")]
    public AudioClip clickSFX;

    [Tooltip("SFX 2 - Unlock")]
    public AudioClip unlockSFX;

    [Tooltip("SFX 3 - Reveal")]
    public AudioClip revealSFX;

    [Range(0f, 1f)]
    public float sfxVolume = 1f;

    // ============================================================
    // SFX TIMING
    // ============================================================

    [Header("SFX Timing")]

    [Tooltip("Click SFX")]
    public float clickTime1 = 2f;

    public float clickTime2 = 6f;

    public float clickTime3 = 10f;

    public float clickTime4 = 14f;

    [Tooltip("Unlock SFX")]
    public float unlockTime = 18f;

    [Tooltip("Reveal SFX")]
    public float revealTime = 22f;

    // ============================================================
    // AUDIO
    // ============================================================

    private AudioSource audioSource;

    // ============================================================
    // START
    // ============================================================

    private void Start()
    {
        if (dialogueText == null)
        {
            Debug.LogError(
                "RebirthDialogue: Dialogue Text is not assigned!"
            );

            return;
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

        // Start invisible.
        SetAlpha(0f);

        dialogueText.text = "";

        StartCoroutine(
            PlayDialogue()
        );

        StartCoroutine(
            PlaySFXTimeline()
        );
    }

    // ============================================================
    // DIALOGUE
    // ============================================================

    private IEnumerator PlayDialogue()
    {
        // Initial blank
        yield return new WaitForSeconds(
            initialDelay
        );

        // Play each line
        foreach (string line in dialogueLines)
        {
            dialogueText.text = line;

            // Fade IN
            yield return StartCoroutine(
                FadeText(
                    0f,
                    1f,
                    fadeInDuration
                )
            );

            // Hold
            yield return new WaitForSeconds(
                holdDuration
            );

            // Fade OUT
            yield return StartCoroutine(
                FadeText(
                    1f,
                    0f,
                    fadeOutDuration
                )
            );

            // Clear
            dialogueText.text = "";
        }

        // Finished
        SetAlpha(0f);
    }

    // ============================================================
    // SFX TIMELINE
    // ============================================================

    private IEnumerator PlaySFXTimeline()
    {
        float elapsed = 0f;

        bool click1Played = false;
        bool click2Played = false;
        bool click3Played = false;
        bool click4Played = false;

        bool unlockPlayed = false;
        bool revealPlayed = false;

        while (
            !click1Played ||
            !click2Played ||
            !click3Played ||
            !click4Played ||
            !unlockPlayed ||
            !revealPlayed
        )
        {
            elapsed += Time.deltaTime;

            // ====================================================
            // CLICK 1 - 2 SECONDS
            // ====================================================

            if (!click1Played &&
                elapsed >= clickTime1)
            {
                PlaySFX(clickSFX);

                click1Played = true;
            }

            // ====================================================
            // CLICK 2 - 6 SECONDS
            // ====================================================

            if (!click2Played &&
                elapsed >= clickTime2)
            {
                PlaySFX(clickSFX);

                click2Played = true;
            }

            // ====================================================
            // CLICK 3 - 10 SECONDS
            // ====================================================

            if (!click3Played &&
                elapsed >= clickTime3)
            {
                PlaySFX(clickSFX);

                click3Played = true;
            }

            // ====================================================
            // CLICK 4 - 14 SECONDS
            // ====================================================

            if (!click4Played &&
                elapsed >= clickTime4)
            {
                PlaySFX(clickSFX);

                click4Played = true;
            }

            // ====================================================
            // UNLOCK - 18 SECONDS
            // ====================================================

            if (!unlockPlayed &&
                elapsed >= unlockTime)
            {
                PlaySFX(unlockSFX);

                unlockPlayed = true;
            }

            // ====================================================
            // REVEAL - 22 SECONDS
            // ====================================================

            if (!revealPlayed &&
                elapsed >= revealTime)
            {
                PlaySFX(revealSFX);

                revealPlayed = true;
            }

            yield return null;
        }
    }

    // ============================================================
    // PLAY SFX
    // ============================================================

    private void PlaySFX(
        AudioClip clip
    )
    {
        if (clip == null)
            return;

        if (audioSource == null)
            return;

        float volume =
            sfxVolume;

        // Respect global SFX settings.
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
    // FADE TEXT
    // ============================================================

    private IEnumerator FadeText(
        float startAlpha,
        float endAlpha,
        float duration
    )
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed +=
                Time.deltaTime;

            float progress =
                Mathf.Clamp01(
                    elapsed / duration
                );

            float alpha =
                Mathf.Lerp(
                    startAlpha,
                    endAlpha,
                    progress
                );

            SetAlpha(alpha);

            yield return null;
        }

        SetAlpha(
            endAlpha
        );
    }

    // ============================================================
    // SET ALPHA
    // ============================================================

    private void SetAlpha(
        float alpha
    )
    {
        if (dialogueText == null)
            return;

        Color color =
            dialogueText.color;

        dialogueText.color =
            new Color(
                color.r,
                color.g,
                color.b,
                alpha
            );
    }
}