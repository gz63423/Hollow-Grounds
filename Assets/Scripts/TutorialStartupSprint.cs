using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public partial class TutorialStartup
{
    // ============================================================
    // SPRINT REQUIREMENT
    // ============================================================

    [Header("Sprint Requirement")]

    [Tooltip("Total amount of actual sprinting required to complete the tutorial.")]
    public float sprintRequiredTime = 5f;

    [Tooltip("Text showing the player's current sprint progress.")]
    public TMP_Text sprintProgressText;

    [Tooltip("Slider used to display the player's stamina during the sprint tutorial.")]
    public Slider staminaBar;

    [Tooltip("If enabled, the stamina bar will be enabled when sprint training begins.")]
    public bool enableStaminaBar = true;

    // ============================================================
    // SPRINT TAB
    // ============================================================

    [Header("Sprint Tab")]

    [Tooltip("CanvasGroup containing the Sprint tutorial tab.")]
    public CanvasGroup sprintTab;

    [Tooltip("How long the sprint tab takes to fade in.")]
    public float sprintTabFadeDuration = 0.5f;

    [Tooltip("How long the sprint tab takes to fade out.")]
    public float sprintTabFadeOutDuration = 0.5f;

    // ============================================================
    // SPRINT STATE
    // ============================================================

    private float sprintTime;
    private bool sprintCompletionTriggered;

    // ============================================================
    // BEGIN SPRINT REQUIREMENT
    // ============================================================

    private void BeginSprintRequirement()
    {
        // --------------------------------------------------------
        // PREVENT DUPLICATE STARTS
        // --------------------------------------------------------

        if (sprintRequirementActive)
            return;

        if (sprintCompletionTriggered)
            return;

        sprintRequirementActive = true;

        sprintTime = 0f;
        sprintCompletionTriggered = false;

        // --------------------------------------------------------
        // FIND PLAYER
        // --------------------------------------------------------

        FindPlayer();

        // --------------------------------------------------------
        // ENABLE MOVEMENT + SPRINT AUTOMATICALLY
        // --------------------------------------------------------
        // This happens immediately when the final Box 3 dialogue
        // line finishes typing. No click is required.

        if (player != null)
        {
            player.movementEnabled = true;
            player.sprintEnabled = true;
        }

        // --------------------------------------------------------
        // ENABLE STAMINA BAR AUTOMATICALLY
        // --------------------------------------------------------
        // The bar is enabled at the exact same point that sprint
        // training begins.

        if (staminaBar != null)
        {
            staminaBar.gameObject.SetActive(enableStaminaBar);

            // Refresh immediately so the bar does not wait for the
            // next Update() to receive its first value.
            UpdateStaminaBar();
        }

        // --------------------------------------------------------
        // SHOW SPRINT TAB AUTOMATICALLY
        // --------------------------------------------------------

        if (sprintTab != null)
        {
            sprintTab.interactable = false;
            sprintTab.blocksRaycasts = false;

            StartCoroutine(
                FadeCanvasGroup(
                    sprintTab,
                    sprintTab.alpha,
                    1f,
                    sprintTabFadeDuration
                )
            );
        }

        // --------------------------------------------------------
        // RESET SPRINT PROGRESS
        // --------------------------------------------------------

        UpdateSprintProgressText();

        // --------------------------------------------------------
        // CLEAR DIALOGUE INPUT STATE
        // --------------------------------------------------------

        dialogueClickTimer = 0f;
    }

    // ============================================================
    // UPDATE SPRINT REQUIREMENT
    // ============================================================

    private void UpdateSprintRequirement()
    {
        FindPlayer();

        // --------------------------------------------------------
        // UPDATE STAMINA BAR
        // --------------------------------------------------------

        UpdateStaminaBar();

        // --------------------------------------------------------
        // CHECK ACTUAL SPRINTING
        // --------------------------------------------------------

        bool sprintingNow = false;

        if (player != null)
        {
            bool shiftHeld = Input.GetKey(
                KeyCode.LeftShift
            );

            Vector2 movementInput = new Vector2(
                Input.GetAxisRaw("Horizontal"),
                Input.GetAxisRaw("Vertical")
            );

            bool moving =
                movementInput.sqrMagnitude > 0.01f;

            sprintingNow =
                player.sprintEnabled &&
                shiftHeld &&
                moving;
        }

        // --------------------------------------------------------
        // ADD ACTUAL SPRINT TIME
        // --------------------------------------------------------

        if (sprintingNow)
        {
            sprintTime += Time.unscaledDeltaTime;

            sprintTime = Mathf.Clamp(
                sprintTime,
                0f,
                Mathf.Max(
                    0f,
                    sprintRequiredTime
                )
            );
        }

        // --------------------------------------------------------
        // UPDATE TEXT
        // --------------------------------------------------------

        UpdateSprintProgressText();

        // --------------------------------------------------------
        // COMPLETE
        // --------------------------------------------------------

        if (
            sprintTime >=
            Mathf.Max(
                0f,
                sprintRequiredTime
            ) &&
            !sprintCompletionTriggered
        )
        {
            sprintCompletionTriggered = true;
            sprintRequirementActive = false;

            StartCoroutine(
                FinishSprintRequirement()
            );
        }
    }

    // ============================================================
    // UPDATE SPRINT PROGRESS TEXT
    // ============================================================

    private void UpdateSprintProgressText()
    {
        if (sprintProgressText == null)
            return;

        float required = Mathf.Max(
            0f,
            sprintRequiredTime
        );

        sprintProgressText.text =
            sprintTime.ToString("0.00") +
            "s / " +
            required.ToString("0.00") +
            "s";
    }

    // ============================================================
    // UPDATE STAMINA BAR
    // ============================================================

    private void UpdateStaminaBar()
    {
        if (staminaBar == null)
            return;

        if (player == null)
            return;

        float maxStamina = Mathf.Max(
            0.001f,
            player.maxStamina
        );

        float currentStamina = Mathf.Clamp(
            player.stamina,
            0f,
            maxStamina
        );

        staminaBar.minValue = 0f;
        staminaBar.maxValue = maxStamina;
        staminaBar.value = currentStamina;
    }

    // ============================================================
    // FINISH SPRINT REQUIREMENT
    // ============================================================

    private IEnumerator FinishSprintRequirement()
    {
        // --------------------------------------------------------
        // FORCE FINAL VALUES
        // --------------------------------------------------------

        sprintTime = Mathf.Max(
            0f,
            sprintRequiredTime
        );

        UpdateSprintProgressText();

        // --------------------------------------------------------
        // KEEP SPRINT ENABLED
        // --------------------------------------------------------
        // Sprint remains enabled because it was introduced as part
        // of the tutorial and should stay available afterward.

        if (player != null)
        {
            player.sprintEnabled = true;
            player.movementEnabled = true;
        }

        // --------------------------------------------------------
        // BRIEF PAUSE
        // --------------------------------------------------------

        yield return new WaitForSecondsRealtime(
            0.25f
        );

        // --------------------------------------------------------
        // HIDE STAMINA BAR
        // --------------------------------------------------------

        if (staminaBar != null)
        {
            staminaBar.gameObject.SetActive(false);
        }

        // --------------------------------------------------------
        // FADE SPRINT TAB OUT
        // --------------------------------------------------------

        if (sprintTab != null)
        {
            sprintTab.interactable = false;
            sprintTab.blocksRaycasts = false;

            yield return StartCoroutine(
                FadeCanvasGroup(
                    sprintTab,
                    sprintTab.alpha,
                    0f,
                    sprintTabFadeOutDuration
                )
            );
        }

        // --------------------------------------------------------
        // FADE BOX 3 TEXT OUT EXPLICITLY
        // --------------------------------------------------------
        // This is intentionally separate from the CanvasGroup fade.
        // It fixes the case where dialogueText3 is not actually a
        // child of dialogueBox3 in the Canvas hierarchy.

        if (dialogueText3 != null)
        {
            yield return StartCoroutine(
                FadeText(
                    dialogueText3,
                    dialogueText3.color.a,
                    0f,
                    dialogueBox3FadeDuration
                )
            );

            dialogueText3.text = "";
        }

        // --------------------------------------------------------
        // FADE BOX 3 OUT
        // --------------------------------------------------------

        if (dialogueBox3 != null)
        {
            dialogueBox3.interactable = false;
            dialogueBox3.blocksRaycasts = false;

            yield return StartCoroutine(
                FadeDialogueBox(
                    dialogueBox3,
                    dialogueBox3.alpha,
                    0f,
                    dialogueBox3FadeDuration
                )
            );
        }

        // --------------------------------------------------------
        // START DIALOGUE BOX 4 AUTOMATICALLY
        // --------------------------------------------------------

        StartDialogueBox4();
    }
}
