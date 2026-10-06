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

    [Tooltip("How long to wait before enabling sprinting after the sprint section begins.")]
    public float sprintEnableDelay = 0.5f;

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

    private Coroutine enableSprintCoroutine;

    // ============================================================
    // BEGIN SPRINT REQUIREMENT
    // ============================================================

    private void BeginSprintRequirement()
    {
        if (sprintRequirementActive)
            return;

        sprintRequirementActive = true;

        sprintTime = 0f;
        sprintCompletionTriggered = false;

        // --------------------------------------------------------
        // RESET PROGRESS TEXT
        // --------------------------------------------------------

        UpdateSprintProgressText();

        // --------------------------------------------------------
        // ENABLE STAMINA BAR
        // --------------------------------------------------------

        if (staminaBar != null)
        {
            if (enableStaminaBar)
            {
                staminaBar.gameObject.SetActive(true);
                UpdateStaminaBar();
            }
            else
            {
                staminaBar.gameObject.SetActive(false);
            }
        }

        // --------------------------------------------------------
        // ENABLE SPRINT TAB
        // --------------------------------------------------------

        if (sprintTab != null)
        {
            sprintTab.alpha = 0f;
            sprintTab.interactable = false;
            sprintTab.blocksRaycasts = false;

            StartCoroutine(
                FadeCanvasGroup(
                    sprintTab,
                    0f,
                    1f,
                    sprintTabFadeDuration
                )
            );
        }

        // --------------------------------------------------------
        // ENABLE MOVEMENT
        // --------------------------------------------------------

        FindPlayer();

        if (player != null)
        {
            player.movementEnabled = true;
        }

        // --------------------------------------------------------
        // ENABLE SPRINT AFTER DELAY
        // --------------------------------------------------------

        if (enableSprintCoroutine != null)
        {
            StopCoroutine(enableSprintCoroutine);
        }

        enableSprintCoroutine = StartCoroutine(
            EnableSprintAfterDelay()
        );
    }

    // ============================================================
    // ENABLE SPRINT AFTER DELAY
    // ============================================================

    private IEnumerator EnableSprintAfterDelay()
    {
        float delay = Mathf.Max(
            0f,
            sprintEnableDelay
        );

        if (delay > 0f)
        {
            yield return new WaitForSecondsRealtime(
                delay
            );
        }

        FindPlayer();

        if (player != null)
        {
            player.sprintEnabled = true;
        }
        else
        {
            Debug.LogError(
                "TutorialStartup could not find the Player. " +
                "Sprint tutorial cannot enable sprinting."
            );
        }

        enableSprintCoroutine = null;
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
        // DISABLE SPRINT
        // --------------------------------------------------------

        if (player != null)
        {
            player.sprintEnabled = false;
            player.movementEnabled = true;
        }

        // --------------------------------------------------------
        // STOP SPRINT ENABLE COROUTINE
        // --------------------------------------------------------

        if (enableSprintCoroutine != null)
        {
            StopCoroutine(enableSprintCoroutine);
            enableSprintCoroutine = null;
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
        // FINISH TUTORIAL DIALOGUE
        // --------------------------------------------------------

        FinishDialogue();
    }
}