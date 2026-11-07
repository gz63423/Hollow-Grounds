using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public partial class TutorialStartup : MonoBehaviour
{
    // ============================================================
    // BLACK FADE
    // ============================================================

    [Header("Black Fade")]

    [Tooltip("Full-screen black UI Image used to hide the scene at startup.")]
    public UnityEngine.UI.Image blackImage;

    [Tooltip("How long the black screen takes to completely fade away.")]
    public float fadeDuration = 2f;

    // ============================================================
    // MUSIC
    // ============================================================

    [Header("Music")]

    [Tooltip("How long the music remains completely silent before fading in.")]
    public float musicDelay = 1f;

    [Tooltip("How long the music takes to fade from 0 to the normal volume.")]
    public float musicFadeDuration = 4f;

    // ============================================================
    // DIALOGUE BOX 1
    // ============================================================

    [Header("Dialogue Box 1")]

    [Tooltip("The first dialogue box UI object with a CanvasGroup component.")]
    public CanvasGroup dialogueBox;

    [Tooltip("How long after startup before the first dialogue box appears.")]
    public float dialogueDelay = 2f;

    [Tooltip("How long the first dialogue box takes to fade in/out.")]
    public float dialogueFadeDuration = 0.5f;

    // ============================================================
    // DIALOGUE TEXT 1
    // ============================================================

    [Header("Dialogue Text 1")]

    [Tooltip("Text component used to display the first dialogue.")]
    public TMP_Text dialogueText;

    [Tooltip("Dialogue lines shown in order in Box 1.")]
    [TextArea(2, 5)]
    public string[] dialogueLines;

    // ============================================================
    // TYPEWRITER 1
    // ============================================================

    [Header("Typewriter 1")]

    [Tooltip("Time between each character appearing in Box 1.")]
    public float characterDelay = 0.03f;

    // ============================================================
    // DIALOGUE CLICK DELAY
    // ============================================================

    [Header("Dialogue Click Delay")]

    [Tooltip("Minimum time between dialogue actions.")]
    public float dialogueClickDelay = 0.5f;

    private float dialogueClickTimer;

    // ============================================================
    // QUEUED DIALOGUE CLICKS
    // ============================================================

    [Header("Queued Dialogue Clicks")]

    [Tooltip("Maximum number of clicks that can be stored while the dialogue is busy.")]
    public int maximumQueuedDialogueClicks = 20;

    private int queuedDialogueClicks;

    // ============================================================
    // DIALOGUE BOX 2
    // ============================================================

    [Header("Dialogue Box 2")]

    [Tooltip("The second dialogue box UI object with a CanvasGroup component.")]
    public CanvasGroup dialogueBox2;

    [Tooltip("How long Box 2 waits after Box 1 is hidden before appearing.")]
    public float dialogueBox2Delay = 0f;

    [Tooltip("How long the second dialogue box takes to fade in/out.")]
    public float dialogueBox2FadeDuration = 0.5f;

    // ============================================================
    // DIALOGUE 2 - PART 1
    // ============================================================

    [Header("Dialogue 2 - Part 1")]

    [Tooltip("Text component used for Dialogue 2 Part 1.")]
    public TMP_Text dialogueText2;

    [Tooltip("Dialogue lines shown before the WASD requirement.")]
    [TextArea(2, 5)]
    public string[] dialogueLines2Part1;

    [Tooltip("Time between each character appearing in Part 1.")]
    public float characterDelay2Part1 = 0.03f;

    // ============================================================
    // DIALOGUE 2 - PART 2
    // ============================================================

    [Header("Dialogue 2 - Part 2")]

    [Tooltip("Text component used for Dialogue 2 Part 2.")]
    public TMP_Text dialogueText2Part2;

    [Tooltip("Dialogue lines shown after all four WASD keys are completed.")]
    [TextArea(2, 5)]
    public string[] dialogueLines2Part2;

    [Tooltip("Time between each character appearing in Part 2.")]
    public float characterDelay2Part2 = 0.03f;

    // ============================================================
    // DIALOGUE 2 TAB / CHECKLIST
    // ============================================================

    [Header("Dialogue 2 Tab / WASD Checklist")]

    [Tooltip("CanvasGroup containing the Dialogue 2 tab and WASD checklist.")]
    public CanvasGroup dialogue2Tab;

    [Tooltip("How long the tab/checklist takes to fade in.")]
    public float dialogue2TabFadeDuration = 0.5f;

    [Tooltip("How long the tab/checklist takes to fade out.")]
    public float dialogue2TabFadeOutDuration = 0.5f;

    // ============================================================
    // SPRINT DIALOGUE BOX 3
    // ============================================================

    [Header("Sprint Dialogue Box 3")]

    [Tooltip("Third dialogue box UI object with CanvasGroup component.")]
    public CanvasGroup dialogueBox3;

    [Tooltip("How long Box 3 takes to fade in/out.")]
    public float dialogueBox3FadeDuration = 0.5f;

    [Tooltip("How long to wait before showing Box 3.")]
    public float dialogueBox3Delay = 0f;

    // ============================================================
    // SPRINT DIALOGUE TEXT
    // ============================================================

    [Header("Sprint Dialogue Text")]

    [Tooltip("Text component used for the sprint tutorial dialogue.")]
    public TMP_Text dialogueText3;

    [Tooltip("Dialogue lines shown before the sprint requirement begins.")]
    [TextArea(2, 5)]
    public string[] dialogueLines3;

    [Tooltip("Time between each character appearing in Box 3.")]
    public float characterDelay3 = 0.03f;

    // ============================================================
    // DIALOGUE BOX 4
    // ============================================================

    [Header("Dialogue Box 4")]

    [Tooltip("Fourth dialogue box UI object with CanvasGroup component.")]
    public CanvasGroup dialogueBox4;

    [Tooltip("How long Box 4 takes to fade in/out.")]
    public float dialogueBox4FadeDuration = 0.5f;

    [Tooltip("How long to wait before showing Box 4.")]
    public float dialogueBox4Delay = 0f;

    // ============================================================
    // DIALOGUE 4 - PART 1
    // ============================================================

    [Header("Dialogue 4 - Part 1")]

    [Tooltip("Text component used for Dialogue 4 Part 1.")]
    public TMP_Text dialogueText4;

    [Tooltip("Dialogue lines shown in Dialogue 4 Part 1.")]
    [TextArea(2, 5)]
    public string[] dialogueLines4;

    [Tooltip("Time between each character appearing in Dialogue 4 Part 1.")]
    public float characterDelay4 = 0.03f;

    // ============================================================
    // DIALOGUE 4 - PART 2
    // ============================================================

    [Header("Dialogue 4 - Part 2")]

    [Tooltip("Text component used for Dialogue 4 Part 2.")]
    public TMP_Text dialogueText4Part2;

    [Tooltip("Dialogue lines shown in Dialogue 4 Part 2.")]
    [TextArea(2, 5)]
    public string[] dialogueLines4Part2;

    [Tooltip("Time between each character appearing in Dialogue 4 Part 2.")]
    public float characterDelay4Part2 = 0.03f;

    // ============================================================
    // DIALOGUE 4 TAB / CHECKLIST
    // ============================================================

    [Header("Dialogue 4 Tab / Checklist")]

    [Tooltip("CanvasGroup containing the Dialogue 4 Part 2 tab/checklist.")]
    public CanvasGroup dialogue4Tab;

    [Tooltip("How long the Dialogue 4 tab/checklist takes to fade in.")]
    public float dialogue4TabFadeDuration = 0.5f;

    [Tooltip("How long the Dialogue 4 tab/checklist takes to fade out.")]
    public float dialogue4TabFadeOutDuration = 0.5f;

    // ============================================================
    // DIALOGUE COMPLETION
    // ============================================================

    [Header("Dialogue Completion")]

    [Tooltip("Called after the tutorial dialogue has been completed.")]
    public UnityEvent onDialogueFinished;

    // ============================================================
    // STATE
    // ============================================================

    private bool startupStarted;
    private bool dialogueStarted;
    private bool dialogueFinished;

    private bool usingDialogueBox2;
    private bool dialogueBox2Part2;

    private bool usingDialogueBox3;

    private bool usingDialogueBox4;
    private bool dialogueBox4Part2;

    private bool isTyping;
    private bool movementRequirementActive;
    private bool sprintRequirementActive;
    private bool attackRequirementActive;
    private bool transitionInProgress;

    private int currentDialogueIndex;

    private Coroutine typewriterCoroutine;
    private Coroutine startupCoroutine;

    // ============================================================
    // PLAYER ATTACK EVENT
    // ============================================================

    private void SubscribeToPlayerAttack()
    {
        FindPlayer();

        if (player != null)
        {
            player.OnPlayerAttack -= HandlePlayerAttack;
            player.OnPlayerAttack += HandlePlayerAttack;
        }
    }

    private void UnsubscribeFromPlayerAttack()
    {
        if (player != null)
        {
            player.OnPlayerAttack -= HandlePlayerAttack;
        }
    }

    private void HandlePlayerAttack()
    {
        if (!attackRequirementActive)
            return;

        if (attackCompletionTriggered)
            return;

        attackCount++;

        attackCount = Mathf.Clamp(
            attackCount,
            0,
            Mathf.Max(0, attackRequiredCount)
        );

        UpdateAttackProgressText();

        if (attackCount >= Mathf.Max(0, attackRequiredCount))
        {
            attackCompletionTriggered = true;
            attackRequirementActive = false;

            // Stop the tutorial from accepting anything
            // while the final box is being prepared.
            ClearQueuedDialogueClicks();

            StartCoroutine(
                FinishAttackRequirement()
            );
        }
    }

    // ============================================================
    // AWAKE
    // ============================================================

    private void Awake()
    {
        FindPlayer();
        SubscribeToPlayerAttack();
        ResetTutorialState();
    }

    // ============================================================
    // DESTROY
    // ============================================================

    private void OnDestroy()
    {
        UnsubscribeFromPlayerAttack();
    }

    // ============================================================
    // START
    // ============================================================

    private void Start()
    {
        StartTutorialStartup();
    }

    // ============================================================
    // FIND PLAYER
    // ============================================================

    private void FindPlayer()
    {
        if (player != null)
            return;

        player = Player.Instance;

        if (player == null)
        {
            player = FindFirstObjectByType<Player>();
        }
    }

    // ============================================================
    // RESET QUEUED CLICKS
    // ============================================================

    private void ClearQueuedDialogueClicks()
    {
        queuedDialogueClicks = 0;
    }

    // ============================================================
    // QUEUE DIALOGUE CLICK
    // ============================================================

    private void QueueDialogueClick()
    {
        if (queuedDialogueClicks >= maximumQueuedDialogueClicks)
            return;

        queuedDialogueClicks++;
    }

    // ============================================================
    // RESET TUTORIAL STATE
    // ============================================================

    private void ResetTutorialState()
    {
        startupStarted = false;
        dialogueStarted = false;
        dialogueFinished = false;

        usingDialogueBox2 = false;
        dialogueBox2Part2 = false;

        usingDialogueBox3 = false;

        usingDialogueBox4 = false;
        dialogueBox4Part2 = false;

        isTyping = false;

        movementRequirementActive = false;
        sprintRequirementActive = false;
        attackRequirementActive = false;

        transitionInProgress = false;

        currentDialogueIndex = 0;

        dialogueClickTimer = 0f;

        ClearQueuedDialogueClicks();

        attackCount = 0;
        attackCompletionTriggered = false;

        UpdateAttackProgressText();

        sprintTime = 0f;
        sprintCompletionTriggered = false;

        if (typewriterCoroutine != null)
        {
            StopCoroutine(typewriterCoroutine);
            typewriterCoroutine = null;
        }

        FindPlayer();

        if (player != null)
        {
            player.movementEnabled = false;
            player.sprintEnabled = false;
            player.attacksEnabled = false;
        }

        if (blackImage != null)
        {
            Color color = blackImage.color;
            color.a = 1f;
            blackImage.color = color;

            blackImage.raycastTarget = false;
        }

        if (dialogueBox != null)
        {
            dialogueBox.alpha = 0f;
            dialogueBox.interactable = false;
            dialogueBox.blocksRaycasts = false;
        }

        if (dialogueText != null)
        {
            dialogueText.text = "";

            Color color = dialogueText.color;
            color.a = 1f;
            dialogueText.color = color;
        }

        if (dialogueBox2 != null)
        {
            dialogueBox2.alpha = 0f;
            dialogueBox2.interactable = false;
            dialogueBox2.blocksRaycasts = false;
        }

        if (dialogueText2 != null)
        {
            dialogueText2.text = "";

            Color color = dialogueText2.color;
            color.a = 1f;
            dialogueText2.color = color;
        }

        if (dialogueText2Part2 != null)
        {
            dialogueText2Part2.text = "";

            Color color = dialogueText2Part2.color;
            color.a = 0f;
            dialogueText2Part2.color = color;
        }

        if (dialogue2Tab != null)
        {
            dialogue2Tab.alpha = 0f;
            dialogue2Tab.interactable = false;
            dialogue2Tab.blocksRaycasts = false;
        }

        if (dialogueBox3 != null)
        {
            dialogueBox3.alpha = 0f;
            dialogueBox3.interactable = false;
            dialogueBox3.blocksRaycasts = false;
        }

        if (dialogueText3 != null)
        {
            dialogueText3.text = "";

            Color color = dialogueText3.color;
            color.a = 1f;
            dialogueText3.color = color;
        }

        if (dialogueBox4 != null)
        {
            dialogueBox4.alpha = 0f;
            dialogueBox4.interactable = false;
            dialogueBox4.blocksRaycasts = false;
        }

        if (dialogueText4 != null)
        {
            dialogueText4.text = "";

            Color color = dialogueText4.color;
            color.a = 1f;
            dialogueText4.color = color;
        }

        if (dialogueText4Part2 != null)
        {
            dialogueText4Part2.text = "";

            Color color = dialogueText4Part2.color;
            color.a = 0f;
            dialogueText4Part2.color = color;
        }

        if (dialogue4Tab != null)
        {
            dialogue4Tab.alpha = 0f;
            dialogue4Tab.interactable = false;
            dialogue4Tab.blocksRaycasts = false;
        }

        UpdateSprintProgressText();

        if (staminaBar != null)
        {
            staminaBar.gameObject.SetActive(false);
        }

        if (sprintTab != null)
        {
            sprintTab.alpha = 0f;
            sprintTab.interactable = false;
            sprintTab.blocksRaycasts = false;
        }

        ResetChecklist();

        // --------------------------------------------------------
        // RESET FINAL BOX
        // --------------------------------------------------------

        ResetFinalBox();
    }

    // ============================================================
    // START TUTORIAL STARTUP
    // ============================================================

    public void StartTutorialStartup()
    {
        if (startupStarted)
            return;

        ResetTutorialState();

        startupStarted = true;

        startupCoroutine = StartCoroutine(
            StartupRoutine()
        );
    }

    // ============================================================
    // STARTUP ROUTINE
    // ============================================================

    private IEnumerator StartupRoutine()
    {
        MusicManager musicManager = MusicManager.Instance;

        if (musicManager != null)
        {
            musicManager.StartTutorialMusicFade(
                musicDelay,
                musicFadeDuration
            );
        }

        float duration = Mathf.Max(0.01f, fadeDuration);
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

            if (blackImage != null)
            {
                Color color = blackImage.color;

                color.a = Mathf.Lerp(
                    1f,
                    0f,
                    smoothProgress
                );

                blackImage.color = color;
            }

            yield return null;
        }

        if (blackImage != null)
        {
            Color color = blackImage.color;
            color.a = 0f;
            blackImage.color = color;
        }

        if (dialogueDelay > 0f)
        {
            yield return new WaitForSecondsRealtime(
                dialogueDelay
            );
        }

        StartDialogue();

        startupCoroutine = null;
    }

    // ============================================================
    // START DIALOGUE
    // ============================================================

    public void StartDialogue()
    {
        if (dialogueStarted)
            return;

        if (dialogueLines == null || dialogueLines.Length == 0)
            return;

        dialogueStarted = true;
        dialogueFinished = false;

        usingDialogueBox2 = false;
        dialogueBox2Part2 = false;

        usingDialogueBox3 = false;

        usingDialogueBox4 = false;
        dialogueBox4Part2 = false;

        movementRequirementActive = false;
        sprintRequirementActive = false;
        attackRequirementActive = false;

        transitionInProgress = false;

        currentDialogueIndex = 0;

        dialogueClickTimer = 0f;
        ClearQueuedDialogueClicks();

        if (dialogueText != null)
        {
            dialogueText.text = "";

            Color color = dialogueText.color;
            color.a = 1f;
            dialogueText.color = color;
        }

        if (dialogueText2 != null)
        {
            dialogueText2.text = "";

            Color color = dialogueText2.color;
            color.a = 1f;
            dialogueText2.color = color;
        }

        if (dialogueText2Part2 != null)
        {
            dialogueText2Part2.text = "";

            Color color = dialogueText2Part2.color;
            color.a = 0f;
            dialogueText2Part2.color = color;
        }

        if (dialogueText3 != null)
        {
            dialogueText3.text = "";

            Color color = dialogueText3.color;
            color.a = 1f;
            dialogueText3.color = color;
        }

        if (dialogueText4 != null)
        {
            dialogueText4.text = "";

            Color color = dialogueText4.color;
            color.a = 1f;
            dialogueText4.color = color;
        }

        if (dialogueText4Part2 != null)
        {
            dialogueText4Part2.text = "";

            Color color = dialogueText4Part2.color;
            color.a = 0f;
            dialogueText4Part2.color = color;
        }

        ResetChecklist();
        ResetFinalBox();

        if (dialogue4Tab != null)
        {
            dialogue4Tab.alpha = 0f;
            dialogue4Tab.interactable = false;
            dialogue4Tab.blocksRaycasts = false;
        }

        if (player != null)
        {
            player.movementEnabled = false;
            player.sprintEnabled = false;
            player.attacksEnabled = false;
        }

        if (dialogueBox != null)
        {
            dialogueBox.interactable = true;
            dialogueBox.blocksRaycasts = true;

            StartCoroutine(
                FadeDialogueBox(
                    dialogueBox,
                    0f,
                    1f,
                    dialogueFadeDuration
                )
            );
        }

        ShowDialogueLine();
    }

    // ============================================================
    // SHOW CURRENT DIALOGUE LINE
    // ============================================================

    private void ShowDialogueLine()
    {
        TMP_Text targetText = null;
        string targetLine = "";

        if (
            !usingDialogueBox2 &&
            !usingDialogueBox3 &&
            !usingDialogueBox4
        )
        {
            targetText = dialogueText;

            if (
                dialogueLines != null &&
                currentDialogueIndex >= 0 &&
                currentDialogueIndex < dialogueLines.Length
            )
            {
                targetLine = dialogueLines[
                    currentDialogueIndex
                ];
            }
        }
        else if (
            usingDialogueBox2 &&
            !dialogueBox2Part2
        )
        {
            targetText = dialogueText2;

            if (
                dialogueLines2Part1 != null &&
                currentDialogueIndex >= 0 &&
                currentDialogueIndex < dialogueLines2Part1.Length
            )
            {
                targetLine = dialogueLines2Part1[
                    currentDialogueIndex
                ];
            }
        }
        else if (
            usingDialogueBox2 &&
            dialogueBox2Part2
        )
        {
            targetText = dialogueText2Part2;

            if (
                dialogueLines2Part2 != null &&
                currentDialogueIndex >= 0 &&
                currentDialogueIndex < dialogueLines2Part2.Length
            )
            {
                targetLine = dialogueLines2Part2[
                    currentDialogueIndex
                ];
            }
        }
        else if (usingDialogueBox3)
        {
            targetText = dialogueText3;

            if (
                dialogueLines3 != null &&
                currentDialogueIndex >= 0 &&
                currentDialogueIndex < dialogueLines3.Length
            )
            {
                targetLine = dialogueLines3[
                    currentDialogueIndex
                ];
            }
        }
        else if (
            usingDialogueBox4 &&
            !dialogueBox4Part2
        )
        {
            targetText = dialogueText4;

            if (
                dialogueLines4 != null &&
                currentDialogueIndex >= 0 &&
                currentDialogueIndex < dialogueLines4.Length
            )
            {
                targetLine = dialogueLines4[
                    currentDialogueIndex
                ];
            }
        }
        else if (
            usingDialogueBox4 &&
            dialogueBox4Part2
        )
        {
            targetText = dialogueText4Part2;

            if (
                dialogueLines4Part2 != null &&
                currentDialogueIndex >= 0 &&
                currentDialogueIndex < dialogueLines4Part2.Length
            )
            {
                targetLine = dialogueLines4Part2[
                    currentDialogueIndex
                ];
            }
        }

        if (targetText == null)
            return;

        if (typewriterCoroutine != null)
        {
            StopCoroutine(typewriterCoroutine);
            typewriterCoroutine = null;
        }

        float targetCharacterDelay = characterDelay;

        if (usingDialogueBox2)
        {
            targetCharacterDelay = dialogueBox2Part2
                ? characterDelay2Part2
                : characterDelay2Part1;
        }
        else if (usingDialogueBox3)
        {
            targetCharacterDelay = characterDelay3;
        }
        else if (usingDialogueBox4)
        {
            targetCharacterDelay = dialogueBox4Part2
                ? characterDelay4Part2
                : characterDelay4;
        }

        typewriterCoroutine = StartCoroutine(
            TypeDialogue(
                targetText,
                targetLine,
                targetCharacterDelay
            )
        );
    }

    // ============================================================
    // TYPE DIALOGUE
    // ============================================================

    private IEnumerator TypeDialogue(
        TMP_Text targetText,
        string line,
        float targetCharacterDelay)
    {
        isTyping = true;

        if (targetText != null)
            targetText.text = "";

        float delay = Mathf.Max(
            0.001f,
            targetCharacterDelay
        );

        for (int i = 0; i < line.Length; i++)
        {
            if (targetText != null)
                targetText.text = line.Substring(0, i + 1);

            yield return new WaitForSecondsRealtime(delay);
        }

        if (targetText != null)
            targetText.text = line;

        isTyping = false;
        typewriterCoroutine = null;

        // Box 3's final line automatically starts sprint training.
        if (
            usingDialogueBox3 &&
            dialogueLines3 != null &&
            currentDialogueIndex >= 0 &&
            currentDialogueIndex == dialogueLines3.Length - 1 &&
            !sprintRequirementActive &&
            !sprintCompletionTriggered
        )
        {
            BeginSprintRequirement();
        }
    }

    // ============================================================
    // UPDATE
    // ============================================================

    private void Update()
    {
        if (!dialogueStarted || dialogueFinished)
            return;

        if (transitionInProgress)
            return;

        // --------------------------------------------------------
        // FINAL BOX
        // --------------------------------------------------------

        if (finalBoxActive)
        {
            UpdateFinalBox();
            return;
        }

        // --------------------------------------------------------
        // NEVER ACCEPT CLICKS WHILE TYPING
        // --------------------------------------------------------

        if (isTyping)
            return;

        // --------------------------------------------------------
        // REQUIREMENTS
        // --------------------------------------------------------

        if (movementRequirementActive)
        {
            UpdateMovementChecklist();
            return;
        }

        if (sprintRequirementActive)
        {
            UpdateSprintRequirement();
            return;
        }

        if (attackRequirementActive)
        {
            return;
        }

        // --------------------------------------------------------
        // NORMAL DIALOGUE CLICK
        // --------------------------------------------------------

        if (Input.GetMouseButtonDown(0))
        {
            dialogueClickTimer = Mathf.Max(
                0f,
                dialogueClickDelay
            );

            AdvanceToNextDialogue();
        }
    }

    // ============================================================
    // START DIALOGUE BOX 2
    // ============================================================

    private void StartDialogueBox2()
    {
        if (transitionInProgress)
            return;

        transitionInProgress = true;

        if (typewriterCoroutine != null)
        {
            StopCoroutine(typewriterCoroutine);
            typewriterCoroutine = null;
        }

        isTyping = false;

        if (dialogueText != null)
            dialogueText.text = "";

        if (dialogueBox != null)
        {
            dialogueBox.interactable = false;
            dialogueBox.blocksRaycasts = false;
        }

        usingDialogueBox2 = true;
        dialogueBox2Part2 = false;
        usingDialogueBox3 = false;
        usingDialogueBox4 = false;
        dialogueBox4Part2 = false;

        currentDialogueIndex = 0;

        if (dialogueText2 != null)
        {
            dialogueText2.text = "";

            Color color = dialogueText2.color;
            color.a = 1f;
            dialogueText2.color = color;
        }

        if (dialogueText2Part2 != null)
        {
            dialogueText2Part2.text = "";

            Color color = dialogueText2Part2.color;
            color.a = 0f;
            dialogueText2Part2.color = color;
        }

        ResetChecklist();

        StartCoroutine(
            TransitionToDialogueBox2()
        );
    }

    // ============================================================
    // TRANSITION TO DIALOGUE BOX 2
    // ============================================================

    private IEnumerator TransitionToDialogueBox2()
    {
        if (dialogueBox != null)
        {
            yield return StartCoroutine(
                FadeDialogueBox(
                    dialogueBox,
                    dialogueBox.alpha,
                    0f,
                    dialogueFadeDuration
                )
            );
        }

        if (dialogueBox2Delay > 0f)
        {
            yield return new WaitForSecondsRealtime(
                dialogueBox2Delay
            );
        }

        if (dialogueBox2 != null)
        {
            dialogueBox2.interactable = false;
            dialogueBox2.blocksRaycasts = false;

            yield return StartCoroutine(
                FadeDialogueBox(
                    dialogueBox2,
                    0f,
                    1f,
                    dialogueBox2FadeDuration
                )
            );

            dialogueBox2.interactable = true;
            dialogueBox2.blocksRaycasts = true;
        }

        if (dialogue2Tab != null)
        {
            dialogue2Tab.alpha = 0f;
            dialogue2Tab.interactable = false;
            dialogue2Tab.blocksRaycasts = false;

            StartCoroutine(
                FadeCanvasGroup(
                    dialogue2Tab,
                    0f,
                    1f,
                    dialogue2TabFadeDuration
                )
            );
        }

        transitionInProgress = false;

        ShowDialogueLine();

        yield return StartCoroutine(
            WaitForPart1Dialogue()
        );

        BeginMovementRequirement();
    }

    // ============================================================
    // WAIT FOR PART 1 DIALOGUE
    // ============================================================

    private IEnumerator WaitForPart1Dialogue()
    {
        while (isTyping)
        {
            yield return null;
        }

        if (
            dialogueLines2Part1 != null &&
            dialogueLines2Part1.Length > 0
        )
        {
            while (
                currentDialogueIndex <
                dialogueLines2Part1.Length - 1
            )
            {
                yield return null;
            }

            while (isTyping)
            {
                yield return null;
            }
        }
    }

    // ============================================================
    // START DIALOGUE 2 PART 2
    // ============================================================

    private IEnumerator StartDialogue2Part2()
    {
        transitionInProgress = true;

        dialogueClickTimer = 0f;

        if (typewriterCoroutine != null)
        {
            StopCoroutine(typewriterCoroutine);
            typewriterCoroutine = null;
        }

        isTyping = false;

        if (dialogueText2 != null)
        {
            yield return StartCoroutine(
                FadeText(
                    dialogueText2,
                    1f,
                    0f,
                    dialogue2TabFadeOutDuration
                )
            );

            dialogueText2.text = "";
        }

        dialogueBox2Part2 = true;
        currentDialogueIndex = 0;

        if (dialogueText2Part2 != null)
        {
            Color color = dialogueText2Part2.color;
            color.a = 0f;
            dialogueText2Part2.color = color;

            ShowDialogueLine();

            yield return StartCoroutine(
                FadeText(
                    dialogueText2Part2,
                    0f,
                    1f,
                    dialogue2TabFadeDuration
                )
            );
        }
        else
        {
            ShowDialogueLine();
        }

        transitionInProgress = false;
    }

    // ============================================================
    // ADVANCE TO NEXT DIALOGUE
    // ============================================================

    private void AdvanceToNextDialogue()
    {
        // --------------------------------------------------------
        // DIALOGUE BOX 1
        // --------------------------------------------------------

        if (
            !usingDialogueBox2 &&
            !usingDialogueBox3 &&
            !usingDialogueBox4
        )
        {
            currentDialogueIndex++;

            if (
                currentDialogueIndex >=
                dialogueLines.Length
            )
            {
                StartDialogueBox2();
                return;
            }

            ShowDialogueLine();
            return;
        }

        // --------------------------------------------------------
        // DIALOGUE BOX 2 PART 1
        // --------------------------------------------------------

        if (
            usingDialogueBox2 &&
            !dialogueBox2Part2
        )
        {
            currentDialogueIndex++;

            if (
                currentDialogueIndex >=
                dialogueLines2Part1.Length
            )
            {
                BeginMovementRequirement();
                return;
            }

            ShowDialogueLine();
            return;
        }

        // --------------------------------------------------------
        // DIALOGUE BOX 2 PART 2
        // --------------------------------------------------------

        if (
            usingDialogueBox2 &&
            dialogueBox2Part2
        )
        {
            currentDialogueIndex++;

            if (
                currentDialogueIndex >=
                dialogueLines2Part2.Length
            )
            {
                StartDialogueBox3();
                return;
            }

            ShowDialogueLine();
            return;
        }

        // --------------------------------------------------------
        // DIALOGUE BOX 3
        // --------------------------------------------------------

        if (usingDialogueBox3)
        {
            currentDialogueIndex++;

            if (
                dialogueLines3 == null ||
                currentDialogueIndex >= dialogueLines3.Length
            )
            {
                BeginSprintRequirement();
                return;
            }

            ShowDialogueLine();
            return;
        }

        // --------------------------------------------------------
        // DIALOGUE BOX 4 PART 1
        // --------------------------------------------------------

        if (
            usingDialogueBox4 &&
            !dialogueBox4Part2
        )
        {
            currentDialogueIndex++;

            if (
                dialogueLines4 != null &&
                currentDialogueIndex >= dialogueLines4.Length
            )
            {
                StartCoroutine(
                    StartDialogue4Part2()
                );

                return;
            }

            ShowDialogueLine();
            return;
        }

        // --------------------------------------------------------
        // DIALOGUE BOX 4 PART 2
        // --------------------------------------------------------

        if (
            usingDialogueBox4 &&
            dialogueBox4Part2
        )
        {
            currentDialogueIndex++;

            if (
                dialogueLines4Part2 != null &&
                currentDialogueIndex >= dialogueLines4Part2.Length
            )
            {
                BeginAttackRequirement();
                return;
            }

            ShowDialogueLine();
        }
    }

    // ============================================================
    // START DIALOGUE BOX 3
    // ============================================================

    private void StartDialogueBox3()
    {
        if (transitionInProgress)
            return;

        transitionInProgress = true;

        if (typewriterCoroutine != null)
        {
            StopCoroutine(typewriterCoroutine);
            typewriterCoroutine = null;
        }

        isTyping = false;

        if (dialogueText2 != null)
            dialogueText2.text = "";

        if (dialogueText2Part2 != null)
            dialogueText2Part2.text = "";

        if (dialogueBox2 != null)
        {
            dialogueBox2.interactable = false;
            dialogueBox2.blocksRaycasts = false;
        }

        if (dialogue2Tab != null)
        {
            dialogue2Tab.interactable = false;
            dialogue2Tab.blocksRaycasts = false;

            StartCoroutine(
                FadeCanvasGroup(
                    dialogue2Tab,
                    dialogue2Tab.alpha,
                    0f,
                    dialogue2TabFadeOutDuration
                )
            );
        }

        usingDialogueBox2 = false;
        dialogueBox2Part2 = false;

        usingDialogueBox3 = true;

        usingDialogueBox4 = false;
        dialogueBox4Part2 = false;

        currentDialogueIndex = 0;

        dialogueClickTimer = 0f;

        if (dialogueText3 != null)
        {
            dialogueText3.text = "";

            Color color = dialogueText3.color;
            color.a = 1f;
            dialogueText3.color = color;
        }

        sprintTime = 0f;
        sprintCompletionTriggered = false;

        UpdateSprintProgressText();

        StartCoroutine(
            TransitionToDialogueBox3()
        );
    }

    // ============================================================
    // TRANSITION TO DIALOGUE BOX 3
    // ============================================================

    private IEnumerator TransitionToDialogueBox3()
    {
        if (dialogueBox2 != null)
        {
            yield return StartCoroutine(
                FadeDialogueBox(
                    dialogueBox2,
                    dialogueBox2.alpha,
                    0f,
                    dialogueBox2FadeDuration
                )
            );
        }

        if (dialogueBox3Delay > 0f)
        {
            yield return new WaitForSecondsRealtime(
                dialogueBox3Delay
            );
        }

        if (dialogueBox3 != null)
        {
            dialogueBox3.interactable = false;
            dialogueBox3.blocksRaycasts = false;

            yield return StartCoroutine(
                FadeDialogueBox(
                    dialogueBox3,
                    0f,
                    1f,
                    dialogueBox3FadeDuration
                )
            );

            dialogueBox3.interactable = true;
            dialogueBox3.blocksRaycasts = true;
        }

        transitionInProgress = false;
        dialogueClickTimer = 0f;

        ShowDialogueLine();
    }

    // ============================================================
    // START DIALOGUE BOX 4
    // ============================================================

    private void StartDialogueBox4()
    {
        if (transitionInProgress)
            return;

        transitionInProgress = true;

        if (typewriterCoroutine != null)
        {
            StopCoroutine(typewriterCoroutine);
            typewriterCoroutine = null;
        }

        isTyping = false;

        sprintRequirementActive = false;

        if (dialogueText3 != null)
        {
            Color color = dialogueText3.color;
            color.a = 0f;
            dialogueText3.color = color;
            dialogueText3.text = "";
        }

        if (dialogueBox3 != null)
        {
            dialogueBox3.interactable = false;
            dialogueBox3.blocksRaycasts = false;
        }

        if (sprintTab != null)
        {
            sprintTab.interactable = false;
            sprintTab.blocksRaycasts = false;

            StartCoroutine(
                FadeCanvasGroup(
                    sprintTab,
                    sprintTab.alpha,
                    0f,
                    sprintTabFadeOutDuration
                )
            );
        }

        if (staminaBar != null)
        {
            staminaBar.gameObject.SetActive(false);
        }

        usingDialogueBox3 = false;

        usingDialogueBox4 = true;
        dialogueBox4Part2 = false;

        currentDialogueIndex = 0;

        if (dialogueText4 != null)
        {
            dialogueText4.text = "";

            Color color = dialogueText4.color;
            color.a = 1f;
            dialogueText4.color = color;
        }

        if (dialogueText4Part2 != null)
        {
            dialogueText4Part2.text = "";

            Color color = dialogueText4Part2.color;
            color.a = 0f;
            dialogueText4Part2.color = color;
        }

        if (dialogue4Tab != null)
        {
            dialogue4Tab.alpha = 0f;
            dialogue4Tab.interactable = false;
            dialogue4Tab.blocksRaycasts = false;
        }

        StartCoroutine(
            TransitionToDialogueBox4()
        );
    }

    // ============================================================
    // TRANSITION TO DIALOGUE BOX 4
    // ============================================================

    private IEnumerator TransitionToDialogueBox4()
    {
        if (dialogueBox3 != null)
        {
            yield return StartCoroutine(
                FadeDialogueBox(
                    dialogueBox3,
                    dialogueBox3.alpha,
                    0f,
                    dialogueBox3FadeDuration
                )
            );
        }

        if (dialogueBox4Delay > 0f)
        {
            yield return new WaitForSecondsRealtime(
                dialogueBox4Delay
            );
        }

        if (dialogueBox4 != null)
        {
            dialogueBox4.interactable = false;
            dialogueBox4.blocksRaycasts = false;

            yield return StartCoroutine(
                FadeDialogueBox(
                    dialogueBox4,
                    0f,
                    1f,
                    dialogueBox4FadeDuration
                )
            );

            dialogueBox4.interactable = true;
            dialogueBox4.blocksRaycasts = true;
        }

        transitionInProgress = false;

        dialogueClickTimer = 0f;

        ShowDialogueLine();
    }

    // ============================================================
    // START DIALOGUE 4 PART 2
    // ============================================================

    private IEnumerator StartDialogue4Part2()
    {
        transitionInProgress = true;

        if (typewriterCoroutine != null)
        {
            StopCoroutine(typewriterCoroutine);
            typewriterCoroutine = null;
        }

        isTyping = false;

        if (dialogueText4 != null)
        {
            yield return StartCoroutine(
                FadeText(
                    dialogueText4,
                    1f,
                    0f,
                    dialogueBox4FadeDuration
                )
            );

            dialogueText4.text = "";
        }

        dialogueBox4Part2 = true;
        currentDialogueIndex = 0;

        if (dialogueText4Part2 != null)
        {
            Color color = dialogueText4Part2.color;
            color.a = 0f;
            dialogueText4Part2.color = color;

            ShowDialogueLine();

            yield return StartCoroutine(
                FadeText(
                    dialogueText4Part2,
                    0f,
                    1f,
                    dialogueBox4FadeDuration
                )
            );
        }
        else
        {
            ShowDialogueLine();
        }

        if (dialogue4Tab != null)
        {
            dialogue4Tab.alpha = 0f;
            dialogue4Tab.interactable = true;
            dialogue4Tab.blocksRaycasts = true;

            yield return StartCoroutine(
                FadeCanvasGroup(
                    dialogue4Tab,
                    0f,
                    1f,
                    dialogue4TabFadeDuration
                )
            );
        }

        // ========================================================
        // IMPORTANT:
        // ATTACKS ARE ENABLED AS DIALOGUE 4 PART 2 LOADS.
        // ========================================================

        FindPlayer();

        if (player != null)
        {
            player.movementEnabled = true;
            player.sprintEnabled = true;
            player.attacksEnabled = true;
        }

        // Start the attack requirement here so the player can
        // attack as soon as Part 2 is loaded.
        BeginAttackRequirement();

        transitionInProgress = false;
    }

    // ============================================================
    // COMPLETE CURRENT LINE
    // ============================================================

    private void CompleteCurrentLine()
    {
        if (!isTyping)
            return;

        if (typewriterCoroutine != null)
        {
            StopCoroutine(typewriterCoroutine);
            typewriterCoroutine = null;
        }

        TMP_Text targetText = null;
        string[] targetLines = null;

        if (
            !usingDialogueBox2 &&
            !usingDialogueBox3 &&
            !usingDialogueBox4
        )
        {
            targetText = dialogueText;
            targetLines = dialogueLines;
        }
        else if (
            usingDialogueBox2 &&
            !dialogueBox2Part2
        )
        {
            targetText = dialogueText2;
            targetLines = dialogueLines2Part1;
        }
        else if (
            usingDialogueBox2 &&
            dialogueBox2Part2
        )
        {
            targetText = dialogueText2Part2;
            targetLines = dialogueLines2Part2;
        }
        else if (usingDialogueBox3)
        {
            targetText = dialogueText3;
            targetLines = dialogueLines3;
        }
        else if (
            usingDialogueBox4 &&
            !dialogueBox4Part2
        )
        {
            targetText = dialogueText4;
            targetLines = dialogueLines4;
        }
        else if (
            usingDialogueBox4 &&
            dialogueBox4Part2
        )
        {
            targetText = dialogueText4Part2;
            targetLines = dialogueLines4Part2;
        }

        if (
            targetText != null &&
            targetLines != null &&
            currentDialogueIndex >= 0 &&
            currentDialogueIndex < targetLines.Length
        )
        {
            targetText.text =
                targetLines[currentDialogueIndex];
        }

        isTyping = false;
    }

    // ============================================================
    // FINISH ATTACK REQUIREMENT
    // ============================================================

    private IEnumerator FinishAttackRequirement()
    {
        // --------------------------------------------------------
        // Disable attack input immediately after 5 attacks.
        // --------------------------------------------------------

        FindPlayer();

        if (player != null)
        {
            player.attacksEnabled = false;
        }

        attackRequirementActive = false;

        ClearQueuedDialogueClicks();

        // Give the player a tiny pause after the fifth attack.
        yield return new WaitForSecondsRealtime(0.5f);

        if (dialogueFinished)
            yield break;

        // --------------------------------------------------------
        // Hide Dialogue 4.
        // --------------------------------------------------------

        if (dialogue4Tab != null)
        {
            dialogue4Tab.interactable = false;
            dialogue4Tab.blocksRaycasts = false;

            StartCoroutine(
                FadeCanvasGroup(
                    dialogue4Tab,
                    dialogue4Tab.alpha,
                    0f,
                    dialogue4TabFadeOutDuration
                )
            );
        }

        if (dialogueBox4 != null)
        {
            dialogueBox4.interactable = false;
            dialogueBox4.blocksRaycasts = false;

            yield return StartCoroutine(
                FadeDialogueBox(
                    dialogueBox4,
                    dialogueBox4.alpha,
                    0f,
                    dialogueBox4FadeDuration
                )
            );
        }

        if (dialogueText4 != null)
            dialogueText4.text = "";

        if (dialogueText4Part2 != null)
            dialogueText4Part2.text = "";

        // --------------------------------------------------------
        // Show final box.
        // --------------------------------------------------------

        StartFinalBox();
    }

    // ============================================================
    // FINISH DIALOGUE
    // ============================================================

    private void FinishDialogue()
    {
        if (
            usingDialogueBox3 &&
            !usingDialogueBox4 &&
            !dialogueFinished
        )
        {
            ClearQueuedDialogueClicks();

            StartDialogueBox4();
            return;
        }

        dialogueFinished = true;
        isTyping = false;

        movementRequirementActive = false;
        sprintRequirementActive = false;
        attackRequirementActive = false;

        transitionInProgress = false;

        ClearQueuedDialogueClicks();

        dialogueClickTimer = 0f;

        if (typewriterCoroutine != null)
        {
            StopCoroutine(typewriterCoroutine);
            typewriterCoroutine = null;
        }

        if (player != null)
        {
            player.sprintEnabled = true;
            player.movementEnabled = true;
            player.attacksEnabled = true;
        }

        if (staminaBar != null)
        {
            staminaBar.gameObject.SetActive(false);
        }

        if (dialogueText2 != null)
            dialogueText2.text = "";

        if (dialogueText2Part2 != null)
            dialogueText2Part2.text = "";

        if (dialogueText3 != null)
            dialogueText3.text = "";

        if (dialogueText4 != null)
            dialogueText4.text = "";

        if (dialogueText4Part2 != null)
            dialogueText4Part2.text = "";

        if (dialogueBox2 != null)
        {
            dialogueBox2.interactable = false;
            dialogueBox2.blocksRaycasts = false;

            StartCoroutine(
                FadeDialogueBox(
                    dialogueBox2,
                    dialogueBox2.alpha,
                    0f,
                    dialogueBox2FadeDuration
                )
            );
        }

        if (dialogueBox3 != null)
        {
            dialogueBox3.interactable = false;
            dialogueBox3.blocksRaycasts = false;

            StartCoroutine(
                FadeDialogueBox(
                    dialogueBox3,
                    dialogueBox3.alpha,
                    0f,
                    dialogueBox3FadeDuration
                )
            );
        }

        if (dialogueBox4 != null)
        {
            dialogueBox4.interactable = false;
            dialogueBox4.blocksRaycasts = false;

            StartCoroutine(
                FadeDialogueBox(
                    dialogueBox4,
                    dialogueBox4.alpha,
                    0f,
                    dialogueBox4FadeDuration
                )
            );
        }

        if (dialogue2Tab != null)
        {
            dialogue2Tab.interactable = false;
            dialogue2Tab.blocksRaycasts = false;

            StartCoroutine(
                FadeCanvasGroup(
                    dialogue2Tab,
                    dialogue2Tab.alpha,
                    0f,
                    dialogue2TabFadeOutDuration
                )
            );
        }

        if (dialogue4Tab != null)
        {
            dialogue4Tab.interactable = false;
            dialogue4Tab.blocksRaycasts = false;

            StartCoroutine(
                FadeCanvasGroup(
                    dialogue4Tab,
                    dialogue4Tab.alpha,
                    0f,
                    dialogue4TabFadeOutDuration
                )
            );
        }

        if (sprintTab != null)
        {
            sprintTab.interactable = false;
            sprintTab.blocksRaycasts = false;

            StartCoroutine(
                FadeCanvasGroup(
                    sprintTab,
                    sprintTab.alpha,
                    0f,
                    sprintTabFadeOutDuration
                )
            );
        }

        if (onDialogueFinished != null)
        {
            onDialogueFinished.Invoke();
        }
    }

    // ============================================================
    // FADE DIALOGUE BOX
    // ============================================================

    private IEnumerator FadeDialogueBox(
        CanvasGroup targetBox,
        float startAlpha,
        float targetAlpha,
        float duration)
    {
        if (targetBox == null)
            yield break;

        duration = Mathf.Max(
            0.01f,
            duration
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

            targetBox.alpha = Mathf.Lerp(
                startAlpha,
                targetAlpha,
                smoothProgress
            );

            yield return null;
        }

        targetBox.alpha = targetAlpha;
    }

    // ============================================================
    // FADE CANVAS GROUP
    // ============================================================

    private IEnumerator FadeCanvasGroup(
        CanvasGroup targetGroup,
        float startAlpha,
        float targetAlpha,
        float duration)
    {
        if (targetGroup == null)
            yield break;

        duration = Mathf.Max(
            0.01f,
            duration
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

            targetGroup.alpha = Mathf.Lerp(
                startAlpha,
                targetAlpha,
                smoothProgress
            );

            yield return null;
        }

        targetGroup.alpha = targetAlpha;
    }

    // ============================================================
    // FADE TEXT
    // ============================================================

    private IEnumerator FadeText(
        TMP_Text targetText,
        float startAlpha,
        float targetAlpha,
        float duration)
    {
        if (targetText == null)
            yield break;

        duration = Mathf.Max(
            0.01f,
            duration
        );

        float elapsed = 0f;
        Color color = targetText.color;

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
                startAlpha,
                targetAlpha,
                smoothProgress
            );

            targetText.color = color;

            yield return null;
        }

        color.a = targetAlpha;
        targetText.color = color;
    }
}