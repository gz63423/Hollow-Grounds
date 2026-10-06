using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DemoJourneyUI : MonoBehaviour
{
    // ============================================================
    // DEMO JOURNEY
    // ============================================================

    [Header("Demo Journey")]
    [Range(1, 10)]
    [Tooltip("Which Demo Journey objective this box represents.")]
    public int objectiveNumber = 1;

    [Tooltip("Must exactly match an ID in DemoJourneyManager.")]
    public string objectiveID;


    // ============================================================
    // TEXT
    // ============================================================

    [Header("Text")]
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI descriptionText;
    public TextMeshProUGUI progressText;


    // ============================================================
    // LOCKED TEXT
    // ============================================================

    [Header("Locked Text")]
    [Tooltip("Shown when this objective is locked.")]
    public TextMeshProUGUI lockedText1;

    [Tooltip("Shown when this objective is locked.")]
    public TextMeshProUGUI lockedText2;


    // ============================================================
    // PROGRESS BAR
    // ============================================================

    [Header("Progress Bar")]
    public Slider progressSlider;


    // ============================================================
    // REWARD
    // ============================================================

    [Header("Reward")]
    public GameObject soulstoneIcon;
    public TextMeshProUGUI soulstoneAmountText;
    public Button claimButton;


    // ============================================================
    // CLAIM BUTTON TEXT
    // ============================================================

    [Header("Claim Button Text")]
    [Tooltip("Text inside the Claim button. It disappears only after the objective is claimed.")]
    public GameObject claimButtonText;


    // ============================================================
    // BUTTON COLORS
    // ============================================================

    private readonly Color startingButtonColor =
        new Color32(0x5B, 0x5B, 0x5B, 0xFF);

    private readonly Color completedButtonColor =
        new Color32(0x00, 0xFF, 0x08, 0xFF);


    // Exact alpha values matching AchievementUI.

    private const byte NormalAlpha = 24;
    private const byte HighlightedAlpha = 58;
    private const byte PressedAlpha = 92;
    private const byte SelectedAlpha = 24;
    private const byte DisabledAlpha = 24;


    // ============================================================
    // START
    // ============================================================

    private void Start()
    {
        DisableTextRaycasts();


        // ========================================================
        // CLAIM BUTTON LISTENER
        // ========================================================

        if (claimButton != null)
        {
            claimButton.onClick.RemoveListener(Claim);
            claimButton.onClick.AddListener(Claim);
        }


        RefreshUI();
    }


    // ============================================================
    // DESTROY
    // ============================================================

    private void OnDestroy()
    {
        if (claimButton != null)
        {
            claimButton.onClick.RemoveListener(Claim);
        }
    }


    // ============================================================
    // DISABLE TEXT RAYCASTS
    // ============================================================

    private void DisableTextRaycasts()
    {
        TextMeshProUGUI[] texts =
            GetComponentsInChildren<TextMeshProUGUI>(true);

        foreach (TextMeshProUGUI text in texts)
        {
            if (text != null)
                text.raycastTarget = false;
        }
    }


    // ============================================================
    // REFRESH UI
    // ============================================================

    public void RefreshUI()
    {
        if (DemoJourneyManager.Instance == null)
            return;


        if (string.IsNullOrEmpty(objectiveID))
        {
            Debug.LogWarning(
                "DemoJourneyUI is missing an Objective ID on: " +
                gameObject.name
            );

            return;
        }


        DemoJourneyManager.Objective objective =
            DemoJourneyManager.Instance.GetObjective(
                objectiveID
            );


        if (objective == null)
        {
            Debug.LogWarning(
                "DemoJourneyUI could not find Demo Journey objective with ID: " +
                objectiveID
            );

            return;
        }


        // ========================================================
        // CHECK STATES
        // ========================================================

        bool unlocked =
            DemoJourneyManager.Instance.IsUnlocked(
                objectiveID
            );


        bool completed =
            DemoJourneyManager.Instance.IsCompleted(
                objectiveID
            );


        bool claimed =
            DemoJourneyManager.Instance.IsClaimed(
                objectiveID
            );


        // ========================================================
        // LOCKED
        // ========================================================

        if (!unlocked)
        {
            SetLockedState();
            return;
        }


        // ========================================================
        // UNLOCKED
        // ========================================================

        SetUnlockedState();


        // ========================================================
        // TITLE
        // ========================================================

        if (titleText != null)
            titleText.text = objective.title;


        // ========================================================
        // DESCRIPTION
        // ========================================================

        if (descriptionText != null)
            descriptionText.text = objective.description;


        // ========================================================
        // PROGRESS
        // ========================================================

        float progress =
            DemoJourneyManager.Instance.GetProgress(
                objectiveID
            );


        float requirement =
            objective.requirement;


        progress = Mathf.Clamp(
            progress,
            0f,
            requirement
        );


        // ========================================================
        // PROGRESS TEXT
        // ========================================================

        if (progressText != null)
        {
            progressText.text =
                FormatProgress(
                    progress,
                    requirement
                );
        }


        // ========================================================
        // PROGRESS SLIDER
        // ========================================================

        if (progressSlider != null)
        {
            progressSlider.minValue = 0f;

            progressSlider.maxValue =
                requirement;

            progressSlider.value =
                progress;

            // Player cannot drag the slider.
            progressSlider.interactable = false;
        }


        // ========================================================
        // CLAIM BUTTON COLORS
        // ========================================================

        if (claimButton != null)
        {
            SetButtonColors(completed);
        }


        // ========================================================
        // CLAIMED
        // ========================================================

        if (claimed)
        {
            // Hide Soulstone icon.
            if (soulstoneIcon != null)
                soulstoneIcon.SetActive(false);


            // Hide Soulstone amount.
            if (soulstoneAmountText != null)
                soulstoneAmountText.gameObject.SetActive(false);


            // Hide Claim button.
            if (claimButton != null)
            {
                claimButton.interactable = false;
                claimButton.gameObject.SetActive(false);
            }


            // Hide Claim button text.
            if (claimButtonText != null)
                claimButtonText.SetActive(false);


            return;
        }


        // ========================================================
        // NOT CLAIMED — REWARD
        // ========================================================

        if (soulstoneIcon != null)
            soulstoneIcon.SetActive(true);


        if (soulstoneAmountText != null)
        {
            soulstoneAmountText.gameObject.SetActive(true);

            soulstoneAmountText.text =
                objective.soulstoneReward.ToString();
        }


        // ========================================================
        // CLAIM BUTTON
        // ========================================================

        if (claimButton != null)
        {
            claimButton.gameObject.SetActive(true);

            // Incomplete = disabled / grey.
            // Completed = enabled / green.
            claimButton.interactable = completed;
        }


        // ========================================================
        // CLAIM BUTTON TEXT
        // ========================================================

        if (claimButtonText != null)
        {
            // Stays visible until actually claimed.
            claimButtonText.SetActive(true);
        }
    }


    // ============================================================
    // LOCKED STATE
    // ============================================================

    private void SetLockedState()
    {
        // --------------------------------------------------------
        // LOCKED TEXT
        // --------------------------------------------------------

        if (lockedText1 != null)
            lockedText1.gameObject.SetActive(true);

        if (lockedText2 != null)
            lockedText2.gameObject.SetActive(true);


        // --------------------------------------------------------
        // NORMAL CONTENT OFF
        // --------------------------------------------------------

        if (titleText != null)
            titleText.gameObject.SetActive(false);

        if (descriptionText != null)
            descriptionText.gameObject.SetActive(false);

        if (progressText != null)
            progressText.gameObject.SetActive(false);

        if (progressSlider != null)
            progressSlider.gameObject.SetActive(false);

        if (soulstoneIcon != null)
            soulstoneIcon.SetActive(false);

        if (soulstoneAmountText != null)
            soulstoneAmountText.gameObject.SetActive(false);

        if (claimButton != null)
        {
            claimButton.interactable = false;
            claimButton.gameObject.SetActive(false);
        }

        if (claimButtonText != null)
            claimButtonText.SetActive(false);
    }


    // ============================================================
    // UNLOCKED STATE
    // ============================================================

    private void SetUnlockedState()
    {
        // --------------------------------------------------------
        // LOCKED TEXT OFF
        // --------------------------------------------------------

        if (lockedText1 != null)
            lockedText1.gameObject.SetActive(false);

        if (lockedText2 != null)
            lockedText2.gameObject.SetActive(false);


        // --------------------------------------------------------
        // NORMAL CONTENT ON
        // --------------------------------------------------------

        if (titleText != null)
            titleText.gameObject.SetActive(true);

        if (descriptionText != null)
            descriptionText.gameObject.SetActive(true);

        if (progressText != null)
            progressText.gameObject.SetActive(true);

        if (progressSlider != null)
            progressSlider.gameObject.SetActive(true);
    }


    // ============================================================
    // SET BUTTON COLORS
    // ============================================================

    private void SetButtonColors(bool completed)
    {
        if (claimButton == null)
            return;


        ColorBlock colors =
            claimButton.colors;


        Color baseColor =
            completed
                ? completedButtonColor
                : startingButtonColor;


        colors.normalColor =
            new Color(
                baseColor.r,
                baseColor.g,
                baseColor.b,
                NormalAlpha / 255f
            );


        colors.highlightedColor =
            new Color(
                baseColor.r,
                baseColor.g,
                baseColor.b,
                HighlightedAlpha / 255f
            );


        colors.pressedColor =
            new Color(
                baseColor.r,
                baseColor.g,
                baseColor.b,
                PressedAlpha / 255f
            );


        colors.selectedColor =
            new Color(
                baseColor.r,
                baseColor.g,
                baseColor.b,
                SelectedAlpha / 255f
            );


        colors.disabledColor =
            new Color(
                baseColor.r,
                baseColor.g,
                baseColor.b,
                DisabledAlpha / 255f
            );


        // Only modify the Button ColorBlock.
        // The Button Image remains white.
        claimButton.colors = colors;
    }


    // ============================================================
    // CLAIM
    // ============================================================

    public void Claim()
    {
        if (DemoJourneyManager.Instance == null)
            return;


        if (string.IsNullOrEmpty(objectiveID))
            return;


        // Manager performs all validation:
        //
        // - Exists
        // - Unlocked
        // - Completed
        // - Not already claimed
        // - Soulstones available

        bool claimed =
            DemoJourneyManager.Instance.ClaimObjective(
                objectiveID
            );


        // Only refresh if the claim actually succeeded.
        if (claimed)
        {
            RefreshUI();
        }
    }


    // ============================================================
    // PROGRESS FORMATTING
    // ============================================================

    private string FormatProgress(
        float progress,
        float requirement
    )
    {
        // Money objective.
        if (objectiveID == "FirstFortune")
        {
            return FormatCurrency(progress) +
                   " / " +
                   FormatCurrency(requirement);
        }


        // Normal integer objective.
        return Mathf.FloorToInt(progress) +
               " / " +
               Mathf.FloorToInt(requirement);
    }


    // ============================================================
    // CURRENCY FORMATTING
    // ============================================================

    private string FormatCurrency(float amount)
    {
        if (amount >= 1000000f)
        {
            return "$" +
                   (amount / 1000000f).ToString("0.00") +
                   "m";
        }


        if (amount >= 1000f)
        {
            return "$" +
                   (amount / 1000f).ToString("0.00") +
                   "k";
        }


        return "$" +
               amount.ToString("0.00");
    }
}