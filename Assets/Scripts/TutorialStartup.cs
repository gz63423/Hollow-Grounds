using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

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

    [Tooltip("Third dialogue box UI object with a CanvasGroup component.")]
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
    // DIALOGUE COMPLETION
    // ============================================================

    [Header("Dialogue Completion")]

    [Tooltip("Called after the sprint tutorial has been completed.")]
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
    private bool isTyping;
    private bool movementRequirementActive;
    private bool sprintRequirementActive;
    private bool transitionInProgress;

    private int currentDialogueIndex;

    private Coroutine typewriterCoroutine;
    private Coroutine startupCoroutine;

    // ============================================================
    // AWAKE
    // ============================================================

    private void Awake()
    {
        FindPlayer();
        ResetTutorialState();
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

        isTyping = false;

        movementRequirementActive = false;
        sprintRequirementActive = false;

        transitionInProgress = false;

        currentDialogueIndex = 0;

        sprintTime = 0f;
        sprintCompletionTriggered = false;

        if (typewriterCoroutine != null)
        {
            StopCoroutine(typewriterCoroutine);
            typewriterCoroutine = null;
        }

        if (enableSprintCoroutine != null)
        {
            StopCoroutine(enableSprintCoroutine);
            enableSprintCoroutine = null;
        }

        FindPlayer();

        if (player != null)
        {
            player.movementEnabled = false;
            player.sprintEnabled = false;
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
        {
            return;
        }

        dialogueStarted = true;
        dialogueFinished = false;

        usingDialogueBox2 = false;
        dialogueBox2Part2 = false;
        usingDialogueBox3 = false;

        movementRequirementActive = false;
        sprintRequirementActive = false;

        transitionInProgress = false;

        currentDialogueIndex = 0;

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

        ResetChecklist();

        if (player != null)
        {
            player.movementEnabled = false;
            player.sprintEnabled = false;
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

        if (!usingDialogueBox2 && !usingDialogueBox3)
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
        else if (usingDialogueBox2 && !dialogueBox2Part2)
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
        else if (usingDialogueBox2 && dialogueBox2Part2)
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

        targetText.text = "";

        float delay = Mathf.Max(
            0.001f,
            targetCharacterDelay
        );

        for (int i = 0; i < line.Length; i++)
        {
            targetText.text = line.Substring(
                0,
                i + 1
            );

            yield return new WaitForSecondsRealtime(
                delay
            );
        }

        targetText.text = line;

        isTyping = false;
        typewriterCoroutine = null;
    }

    // ============================================================
    // UPDATE
    // ============================================================

    private void Update()
    {
        if (!dialogueStarted)
            return;

        if (dialogueFinished)
            return;

        if (transitionInProgress)
            return;

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

        if (Input.GetMouseButtonDown(0))
        {
            if (isTyping)
            {
                CompleteCurrentLine();
                return;
            }

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
        {
            dialogueText.text = "";
        }

        if (dialogueBox != null)
        {
            dialogueBox.interactable = false;
            dialogueBox.blocksRaycasts = false;
        }

        usingDialogueBox2 = true;
        dialogueBox2Part2 = false;
        usingDialogueBox3 = false;

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
            dialogueBox2.interactable = true;
            dialogueBox2.blocksRaycasts = true;

            StartCoroutine(
                FadeDialogueBox(
                    dialogueBox2,
                    0f,
                    1f,
                    dialogueBox2FadeDuration
                )
            );
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
        if (!usingDialogueBox2 && !usingDialogueBox3)
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

        if (usingDialogueBox2 && !dialogueBox2Part2)
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

        if (usingDialogueBox2 && dialogueBox2Part2)
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

        if (usingDialogueBox3)
        {
            currentDialogueIndex++;

            if (
                currentDialogueIndex >=
                dialogueLines3.Length
            )
            {
                BeginSprintRequirement();
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
        {
            dialogueText2.text = "";
        }

        if (dialogueText2Part2 != null)
        {
            dialogueText2Part2.text = "";
        }

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

        currentDialogueIndex = 0;

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
            dialogueBox3.interactable = true;
            dialogueBox3.blocksRaycasts = true;

            StartCoroutine(
                FadeDialogueBox(
                    dialogueBox3,
                    0f,
                    1f,
                    dialogueBox3FadeDuration
                )
            );
        }

        transitionInProgress = false;

        ShowDialogueLine();

        yield return StartCoroutine(
            WaitForDialogue3()
        );

        BeginSprintRequirement();
    }

    // ============================================================
    // WAIT FOR DIALOGUE 3
    // ============================================================

    private IEnumerator WaitForDialogue3()
    {
        if (
            dialogueLines3 == null ||
            dialogueLines3.Length == 0
        )
        {
            yield break;
        }

        while (
            currentDialogueIndex <
            dialogueLines3.Length - 1
        )
        {
            yield return null;
        }

        while (isTyping)
        {
            yield return null;
        }
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

        if (!usingDialogueBox2 && !usingDialogueBox3)
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
    // FINISH DIALOGUE
    // ============================================================

    private void FinishDialogue()
    {
        dialogueFinished = true;
        isTyping = false;

        movementRequirementActive = false;
        sprintRequirementActive = false;

        transitionInProgress = false;

        if (typewriterCoroutine != null)
        {
            StopCoroutine(typewriterCoroutine);
            typewriterCoroutine = null;
        }

        if (enableSprintCoroutine != null)
        {
            StopCoroutine(enableSprintCoroutine);
            enableSprintCoroutine = null;
        }

        if (player != null)
        {
            player.sprintEnabled = false;
            player.movementEnabled = true;
        }

        if (staminaBar != null)
        {
            staminaBar.gameObject.SetActive(false);
        }

        if (dialogueText2 != null)
        {
            dialogueText2.text = "";
        }

        if (dialogueText2Part2 != null)
        {
            dialogueText2Part2.text = "";
        }

        if (dialogueText3 != null)
        {
            dialogueText3.text = "";
        }

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