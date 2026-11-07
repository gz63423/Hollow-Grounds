using UnityEngine;
using System.Collections;

public partial class TutorialStartup
{
    // ============================================================
    // WASD CHECKLIST IMAGES
    // ============================================================

    [Header("WASD Checklist Images")]

    [Tooltip("Image displaying the W checklist state.")]
    public UnityEngine.UI.Image wImage;

    [Tooltip("Image displaying the A checklist state.")]
    public UnityEngine.UI.Image aImage;

    [Tooltip("Image displaying the S checklist state.")]
    public UnityEngine.UI.Image sImage;

    [Tooltip("Image displaying the D checklist state.")]
    public UnityEngine.UI.Image dImage;

    // ============================================================
    // CHECKLIST SPRITES
    // ============================================================

    [Header("Checklist Sprites")]

    [Tooltip("Sprite displayed when a key has not been completed.")]
    public Sprite checklistUncompletedSprite;

    [Tooltip("Sprite displayed when a key has been completed.")]
    public Sprite checklistCompletedSprite;

    // ============================================================
    // MOVEMENT REQUIREMENT
    // ============================================================

    [Header("Movement Requirement")]

    [Tooltip("How long each WASD key must be continuously held.")]
    public float keyHoldRequirement = 1f;

    [Tooltip("Player whose movementEnabled setting will be enabled for the tutorial.")]
    public Player player;

    [Tooltip("How long to wait before enabling player movement when the WASD tutorial begins.")]
    public float movementEnableDelay = 1f;

    // ============================================================
    // WASD STATE
    // ============================================================

    private float wHoldTime;
    private float aHoldTime;
    private float sHoldTime;
    private float dHoldTime;

    private bool wCompleted;
    private bool aCompleted;
    private bool sCompleted;
    private bool dCompleted;

    private bool movementCompletionTriggered;

    // ============================================================
    // MOVEMENT CHECKLIST
    // ============================================================

    private void UpdateMovementChecklist()
    {
        // --------------------------------------------------------
        // W
        // --------------------------------------------------------

        if (!wCompleted)
        {
            if (Input.GetKey(KeyCode.W))
            {
                wHoldTime += Time.unscaledDeltaTime;

                if (wHoldTime >= keyHoldRequirement)
                {
                    wCompleted = true;
                    wHoldTime = keyHoldRequirement;

                    SetChecklistSprite(
                        wImage,
                        true
                    );
                }
            }
            else
            {
                wHoldTime = 0f;
            }
        }

        // --------------------------------------------------------
        // A
        // --------------------------------------------------------

        if (!aCompleted)
        {
            if (Input.GetKey(KeyCode.A))
            {
                aHoldTime += Time.unscaledDeltaTime;

                if (aHoldTime >= keyHoldRequirement)
                {
                    aCompleted = true;
                    aHoldTime = keyHoldRequirement;

                    SetChecklistSprite(
                        aImage,
                        true
                    );
                }
            }
            else
            {
                aHoldTime = 0f;
            }
        }

        // --------------------------------------------------------
        // S
        // --------------------------------------------------------

        if (!sCompleted)
        {
            if (Input.GetKey(KeyCode.S))
            {
                sHoldTime += Time.unscaledDeltaTime;

                if (sHoldTime >= keyHoldRequirement)
                {
                    sCompleted = true;
                    sHoldTime = keyHoldRequirement;

                    SetChecklistSprite(
                        sImage,
                        true
                    );
                }
            }
            else
            {
                sHoldTime = 0f;
            }
        }

        // --------------------------------------------------------
        // D
        // --------------------------------------------------------

        if (!dCompleted)
        {
            if (Input.GetKey(KeyCode.D))
            {
                dHoldTime += Time.unscaledDeltaTime;

                if (dHoldTime >= keyHoldRequirement)
                {
                    dCompleted = true;
                    dHoldTime = keyHoldTimeRequirementSafe();

                    SetChecklistSprite(
                        dImage,
                        true
                    );
                }
            }
            else
            {
                dHoldTime = 0f;
            }
        }

        // --------------------------------------------------------
        // ALL FOUR COMPLETE
        // --------------------------------------------------------

        if (
            wCompleted &&
            aCompleted &&
            sCompleted &&
            dCompleted &&
            !movementCompletionTriggered
        )
        {
            movementCompletionTriggered = true;
            movementRequirementActive = false;

            StartCoroutine(
                FinishMovementRequirement()
            );
        }
    }

    // ============================================================
    // SAFE KEY HOLD VALUE
    // ============================================================

    private float keyHoldTimeRequirementSafe()
    {
        return Mathf.Max(
            0f,
            keyHoldRequirement
        );
    }

    // ============================================================
    // FINISH MOVEMENT REQUIREMENT
    // ============================================================

    // ============================================================
    // FINISH MOVEMENT REQUIREMENT
    // ============================================================

    private IEnumerator FinishMovementRequirement()
    {
        // IMPORTANT:
        // Lock all dialogue input immediately.
        //
        // Without this, there is a small window after the final
        // WASD key is completed where a mouse click can still be
        // processed. That click can restart the WASD requirement
        // and reset the checklist back to X's.
        transitionInProgress = true;

        // Make absolutely sure all four checklist items remain checked.
        wCompleted = true;
        aCompleted = true;
        sCompleted = true;
        dCompleted = true;

        SetChecklistSprite(wImage, true);
        SetChecklistSprite(aImage, true);
        SetChecklistSprite(sImage, true);
        SetChecklistSprite(dImage, true);

        // Enable movement.
        if (player != null)
        {
            player.movementEnabled = true;
        }

        // Brief pause so the player can see all four completed checks.
        // Dialogue input remains completely locked during this time.
        yield return new WaitForSecondsRealtime(0.25f);

        // Fade the checklist out.
        if (dialogue2Tab != null)
        {
            yield return StartCoroutine(
                FadeCanvasGroup(
                    dialogue2Tab,
                    dialogue2Tab.alpha,
                    0f,
                    dialogue2TabFadeOutDuration
                )
            );
        }

        // Move to Dialogue 2 Part 2.
        //
        // StartDialogue2Part2() keeps transitionInProgress true
        // while it changes the dialogue UI and starts the typewriter.
        yield return StartCoroutine(
            StartDialogue2Part2()
        );
    }

    // ============================================================
    // BEGIN MOVEMENT REQUIREMENT
    // ============================================================

    private void BeginMovementRequirement()
    {
        if (movementRequirementActive)
            return;

        movementRequirementActive = true;
        movementCompletionTriggered = false;

        // --------------------------------------------------------
        // RESET HOLD TIMERS
        // --------------------------------------------------------

        wHoldTime = 0f;
        aHoldTime = 0f;
        sHoldTime = 0f;
        dHoldTime = 0f;

        // --------------------------------------------------------
        // RESET COMPLETION STATES
        // --------------------------------------------------------

        wCompleted = false;
        aCompleted = false;
        sCompleted = false;
        dCompleted = false;

        // --------------------------------------------------------
        // RESET VISUALS
        // --------------------------------------------------------

        SetChecklistSprite(
            wImage,
            false
        );

        SetChecklistSprite(
            aImage,
            false
        );

        SetChecklistSprite(
            sImage,
            false
        );

        SetChecklistSprite(
            dImage,
            false
        );

        // --------------------------------------------------------
        // ENABLE MOVEMENT AFTER DELAY
        // --------------------------------------------------------

        StartCoroutine(
            EnableMovementAfterDelay()
        );
    }

    // ============================================================
    // ENABLE MOVEMENT AFTER DELAY
    // ============================================================

    private IEnumerator EnableMovementAfterDelay()
    {
        float delay = Mathf.Max(
            0f,
            movementEnableDelay
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
            player.movementEnabled = true;
        }
        else
        {
            Debug.LogError(
                "TutorialStartup could not find the Player. " +
                "WASD tutorial movement cannot be enabled."
            );
        }
    }

    // ============================================================
    // RESET CHECKLIST
    // ============================================================

    private void ResetChecklist()
    {
        wHoldTime = 0f;
        aHoldTime = 0f;
        sHoldTime = 0f;
        dHoldTime = 0f;

        wCompleted = false;
        aCompleted = false;
        sCompleted = false;
        dCompleted = false;

        movementCompletionTriggered = false;

        SetChecklistSprite(
            wImage,
            false
        );

        SetChecklistSprite(
            aImage,
            false
        );

        SetChecklistSprite(
            sImage,
            false
        );

        SetChecklistSprite(
            dImage,
            false
        );
    }

    // ============================================================
    // SET CHECKLIST SPRITE
    // ============================================================

    private void SetChecklistSprite(
        UnityEngine.UI.Image targetImage,
        bool completed)
    {
        if (targetImage == null)
            return;

        targetImage.sprite =
            completed
                ? checklistCompletedSprite
                : checklistUncompletedSprite;

        targetImage.enabled = true;
    }
}