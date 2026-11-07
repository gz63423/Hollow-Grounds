using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public partial class TutorialStartup
{
    // ============================================================
    // ATTACK REQUIREMENT
    // ============================================================

    [Header("Attack Requirement")]

    [Tooltip("Number of actual player attacks required to complete the attack tutorial.")]
    public int attackRequiredCount = 5;

    [Tooltip("Text showing the player's current attack progress.")]
    public TMP_Text attackProgressText;

    // ============================================================
    // FINAL BOX UI
    // ============================================================

    [Header("Final Box UI")]

    [Tooltip("CanvasGroup for the final tutorial box.")]
    public CanvasGroup lastBoxUI;

    [Tooltip("Text displayed inside the final tutorial box.")]
    public TMP_Text lastText;

    [Tooltip("How long the final box takes to fade in.")]
    public float lastBoxFadeDuration = 0.5f;

    [Tooltip("Time between each character appearing in the final text.")]
    public float lastTextCharacterDelay = 0.03f;

    [Tooltip("Final text shown after all five attacks are completed.")]
    [TextArea(2, 5)]
    public string lastTextContent;

    // ============================================================
    // FINAL SCENE TRANSITION
    // ============================================================

    [Header("Final Scene Transition")]

    [Tooltip("Full-screen image used to fade the screen to black. If empty, Black Image is used.")]
    public Image sceneFadeImage;

    [Tooltip("How long the final screen takes to fade to black.")]
    public float sceneFadeDuration = 1f;

    [Tooltip("Scene name to load after the final fade. If empty, the build index is used.")]
    public string sceneName;

    [Tooltip("Scene Build Index to load if Scene Name is empty.")]
    public int sceneBuildIndex = -1;

    // ============================================================
    // ATTACK STATE
    // ============================================================

    private int attackCount;
    private bool attackCompletionTriggered;

    // ============================================================
    // FINAL BOX STATE
    // ============================================================

    private bool finalBoxActive;
    private bool finalTextTyping;
    private bool finalTextFinished;
    private bool finalSceneTransitionStarted;

    private Coroutine finalTextCoroutine;
    private Coroutine finalBoxFadeCoroutine;
    private Coroutine finalSceneFadeCoroutine;

    // ============================================================
    // BEGIN ATTACK REQUIREMENT
    // ============================================================

    private void BeginAttackRequirement()
    {
        if (attackRequirementActive)
            return;

        attackRequirementActive = true;

        attackCount = 0;
        attackCompletionTriggered = false;

        finalBoxActive = false;
        finalTextTyping = false;
        finalTextFinished = false;
        finalSceneTransitionStarted = false;

        FindPlayer();

        if (player != null)
        {
            player.movementEnabled = true;
            player.sprintEnabled = true;

            // This is intentionally enabled here too.
            // StartDialogue4Part2 also enables it when Part 2 loads.
            player.attacksEnabled = true;
        }

        UpdateAttackProgressText();
    }

    // ============================================================
    // UPDATE ATTACK REQUIREMENT
    // ============================================================

    private void UpdateAttackRequirement()
    {
        // Intentionally empty.
        //
        // Attack progression is handled by Player.OnPlayerAttack.
        // This prevents mouse clicks from being counted as attacks.
    }

    // ============================================================
    // UPDATE ATTACK PROGRESS TEXT
    // ============================================================

    private void UpdateAttackProgressText()
    {
        if (attackProgressText == null)
            return;

        int required = Mathf.Max(
            0,
            attackRequiredCount
        );

        attackProgressText.text =
            attackCount +
            " / " +
            required;
    }

    // ============================================================
    // RESET FINAL BOX
    // ============================================================

    private void ResetFinalBox()
    {
        finalBoxActive = false;
        finalTextTyping = false;
        finalTextFinished = false;
        finalSceneTransitionStarted = false;

        if (finalTextCoroutine != null)
        {
            StopCoroutine(finalTextCoroutine);
            finalTextCoroutine = null;
        }

        if (finalBoxFadeCoroutine != null)
        {
            StopCoroutine(finalBoxFadeCoroutine);
            finalBoxFadeCoroutine = null;
        }

        if (finalSceneFadeCoroutine != null)
        {
            StopCoroutine(finalSceneFadeCoroutine);
            finalSceneFadeCoroutine = null;
        }

        if (lastBoxUI != null)
        {
            lastBoxUI.alpha = 0f;
            lastBoxUI.interactable = false;
            lastBoxUI.blocksRaycasts = false;
        }

        if (lastText != null)
        {
            lastText.text = "";

            Color color = lastText.color;
            color.a = 1f;
            lastText.color = color;
        }
    }

    // ============================================================
    // START FINAL BOX
    // ============================================================

    private void StartFinalBox()
    {
        if (finalBoxActive)
            return;

        finalBoxActive = true;
        finalTextTyping = false;
        finalTextFinished = false;
        finalSceneTransitionStarted = false;

        transitionInProgress = true;

        ClearQueuedDialogueClicks();

        if (player != null)
        {
            player.attacksEnabled = false;
        }

        if (lastText != null)
        {
            lastText.text = "";

            Color color = lastText.color;
            color.a = 1f;
            lastText.color = color;
        }

        if (lastBoxUI == null)
        {
            Debug.LogWarning(
                "TutorialStartup: Last Box UI is not assigned."
            );

            transitionInProgress = false;

            // If no final box exists, finish the tutorial instead.
            FinishDialogue();

            return;
        }

        lastBoxUI.alpha = 0f;
        lastBoxUI.interactable = false;
        lastBoxUI.blocksRaycasts = false;

        StartCoroutine(
            ShowFinalBoxRoutine()
        );
    }

    // ============================================================
    // SHOW FINAL BOX
    // ============================================================

    private IEnumerator ShowFinalBoxRoutine()
    {
        if (lastBoxUI != null)
        {
            yield return StartCoroutine(
                FadeCanvasGroup(
                    lastBoxUI,
                    0f,
                    1f,
                    lastBoxFadeDuration
                )
            );

            lastBoxUI.interactable = true;
            lastBoxUI.blocksRaycasts = true;
        }

        transitionInProgress = false;

        // --------------------------------------------------------
        // Start writing the final text.
        // --------------------------------------------------------

        if (lastText != null)
        {
            finalTextCoroutine = StartCoroutine(
                TypeFinalText()
            );
        }
        else
        {
            finalTextFinished = true;
            finalTextTyping = false;
        }
    }

    // ============================================================
    // TYPE FINAL TEXT
    // ============================================================

    private IEnumerator TypeFinalText()
    {
        finalTextTyping = true;
        finalTextFinished = false;

        if (lastText != null)
            lastText.text = "";

        string line = lastTextContent ?? "";

        float delay = Mathf.Max(
            0.001f,
            lastTextCharacterDelay
        );

        for (int i = 0; i < line.Length; i++)
        {
            if (lastText != null)
            {
                lastText.text = line.Substring(
                    0,
                    i + 1
                );
            }

            yield return new WaitForSecondsRealtime(
                delay
            );
        }

        if (lastText != null)
            lastText.text = line;

        finalTextTyping = false;
        finalTextFinished = true;
        finalTextCoroutine = null;
    }

    // ============================================================
    // UPDATE FINAL BOX
    // ============================================================

    private void UpdateFinalBox()
    {
        // --------------------------------------------------------
        // Do absolutely nothing while the final text is typing.
        // --------------------------------------------------------

        if (finalTextTyping)
            return;

        // --------------------------------------------------------
        // Only a fully written final message can accept a click.
        // --------------------------------------------------------

        if (!finalTextFinished)
            return;

        if (finalSceneTransitionStarted)
            return;

        if (Input.GetMouseButtonDown(0))
        {
            StartFinalSceneTransition();
        }
    }

    // ============================================================
    // START FINAL SCENE TRANSITION
    // ============================================================

    private void StartFinalSceneTransition()
    {
        if (finalSceneTransitionStarted)
            return;

        if (!finalTextFinished)
            return;

        finalSceneTransitionStarted = true;
        transitionInProgress = true;

        ClearQueuedDialogueClicks();

        if (lastBoxUI != null)
        {
            lastBoxUI.interactable = false;
            lastBoxUI.blocksRaycasts = false;
        }

        FindPlayer();

        if (player != null)
        {
            player.attacksEnabled = false;
            player.movementEnabled = false;
            player.sprintEnabled = false;
        }

        finalSceneFadeCoroutine = StartCoroutine(
            FinalSceneFadeRoutine()
        );
    }

    // ============================================================
    // FINAL SCENE FADE ROUTINE
    // ============================================================

    private IEnumerator FinalSceneFadeRoutine()
    {
        Image fadeImage = sceneFadeImage;

        if (fadeImage == null)
            fadeImage = blackImage;

        // --------------------------------------------------------
        // If there is a fade image, fade it to black.
        // --------------------------------------------------------

        if (fadeImage != null)
        {
            fadeImage.raycastTarget = true;

            Color color = fadeImage.color;
            color.a = 0f;
            fadeImage.color = color;

            float duration = Mathf.Max(
                0.01f,
                sceneFadeDuration
            );

            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;

                float progress = Mathf.Clamp01(
                    elapsed / duration
                );

                float smoothProgress = Mathf.SmoothStep(
                    0f,
                    1f,
                    progress
                );

                color.a = Mathf.Lerp(
                    0f,
                    1f,
                    smoothProgress
                );

                fadeImage.color = color;

                yield return null;
            }

            color.a = 1f;
            fadeImage.color = color;
        }

        // --------------------------------------------------------
        // Load by scene name if one is supplied.
        // Otherwise use build index.
        // --------------------------------------------------------

        if (!string.IsNullOrWhiteSpace(sceneName))
        {
            SceneManager.LoadScene(
                sceneName
            );

            yield break;
        }

        if (sceneBuildIndex >= 0)
        {
            SceneManager.LoadScene(
                sceneBuildIndex
            );

            yield break;
        }

        Debug.LogError(
            "TutorialStartup: No scene name or valid scene build index was assigned."
        );

        finalSceneTransitionStarted = false;
        transitionInProgress = false;

        if (lastBoxUI != null)
        {
            lastBoxUI.interactable = true;
            lastBoxUI.blocksRaycasts = true;
        }
    }
}