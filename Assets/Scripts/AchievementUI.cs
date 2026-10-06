using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AchievementUI : MonoBehaviour
{
    [Header("Achievement ID")]
    [Tooltip("Must exactly match an ID in AchievementManager.")]
    public string achievementID;


    [Header("Text")]
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI descriptionText;
    public TextMeshProUGUI progressText;


    [Header("Progress Bar")]
    public Slider progressSlider;


    [Header("Reward")]
    public GameObject soulstoneIcon;
    public TextMeshProUGUI soulstoneAmountText;
    public Button claimButton;


    [Header("Claim Button Text")]
    [Tooltip("Text inside the Claim button. It disappears only after the achievement is claimed.")]
    public GameObject claimButtonText;


    [Header("Optional Completed Object")]
    public GameObject completedObject;


    // ============================================================
    // BUTTON COLORS
    // ============================================================

    private readonly Color startingButtonColor =
        new Color32(0x5B, 0x5B, 0x5B, 0xFF);

    private readonly Color completedButtonColor =
        new Color32(0x00, 0xFF, 0x08, 0xFF);


    // Exact alpha values for the Button ColorBlock.
    private const byte NormalAlpha = 24;
    private const byte HighlightedAlpha = 58;
    private const byte PressedAlpha = 92;
    private const byte SelectedAlpha = 24;
    private const byte DisabledAlpha = 24;


    private void Start()
    {
        DisableTextRaycasts();
        RefreshUI();
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
            text.raycastTarget = false;
        }
    }


    // ============================================================
    // REFRESH UI
    // ============================================================

    public void RefreshUI()
    {
        if (AchievementManager.Instance == null)
            return;

        if (string.IsNullOrEmpty(achievementID))
            return;


        AchievementManager.Achievement achievement =
            AchievementManager.Instance.GetAchievement(
                achievementID
            );


        if (achievement == null)
        {
            Debug.LogWarning(
                "AchievementUI could not find achievement with ID: " +
                achievementID
            );

            return;
        }


        // ============================================================
        // TEXT
        // ============================================================

        if (titleText != null)
            titleText.text = achievement.title;


        if (descriptionText != null)
            descriptionText.text = achievement.description;


        // ============================================================
        // PROGRESS
        // ============================================================

        float progress =
            AchievementManager.Instance.GetProgress(
                achievementID
            );

        float requirement =
            achievement.requirement;


        progress = Mathf.Clamp(
            progress,
            0f,
            requirement
        );


        if (progressText != null)
        {
            bool isFortune =
                achievementID.StartsWith("Fortune_");

            if (isFortune)
            {
                progressText.text =
                    FormatCurrency(progress) +
                    " / " +
                    FormatCurrency(requirement);
            }
            else
            {
                progressText.text =
                    FormatNumber(
                        Mathf.FloorToInt(progress)
                    ) +
                    " / " +
                    FormatNumber(
                        Mathf.FloorToInt(requirement)
                    );
            }
        }


        // ============================================================
        // PROGRESS SLIDER
        // ============================================================

        if (progressSlider != null)
        {
            progressSlider.minValue = 0f;
            progressSlider.maxValue = requirement;
            progressSlider.value = progress;

            // Player cannot drag the achievement slider.
            progressSlider.interactable = false;
        }


        // ============================================================
        // STATES
        // ============================================================

        bool completed =
            progress >= requirement;

        bool claimed =
            AchievementManager.Instance.IsClaimed(
                achievementID
            );


        // ============================================================
        // BUTTON COLORS
        // ============================================================

        if (claimButton != null)
        {
            SetButtonColors(completed);
        }


        // ============================================================
        // COMPLETED OBJECT
        // ============================================================

        if (completedObject != null)
            completedObject.SetActive(completed);


        // ============================================================
        // CLAIMED
        // ============================================================

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
                claimButton.gameObject.SetActive(false);


            // Hide Claim button text too.
            if (claimButtonText != null)
                claimButtonText.SetActive(false);

            return;
        }


        // ============================================================
        // NOT CLAIMED
        // ============================================================

        if (soulstoneIcon != null)
            soulstoneIcon.SetActive(true);


        if (soulstoneAmountText != null)
        {
            soulstoneAmountText.gameObject.SetActive(true);

            soulstoneAmountText.text =
                achievement.soulstoneReward.ToString();
        }


        if (claimButton != null)
        {
            claimButton.gameObject.SetActive(true);

            // Incomplete = disabled.
            // Completed = enabled.
            claimButton.interactable = completed;
        }


        // ============================================================
        // CLAIM BUTTON TEXT
        // ============================================================

        if (claimButtonText != null)
        {
            // The text stays visible both before and after completion.
            //
            // It only disappears AFTER the player actually claims
            // the achievement.
            claimButtonText.SetActive(true);
        }
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


        colors.normalColor = new Color(
            baseColor.r,
            baseColor.g,
            baseColor.b,
            NormalAlpha / 255f
        );

        colors.highlightedColor = new Color(
            baseColor.r,
            baseColor.g,
            baseColor.b,
            HighlightedAlpha / 255f
        );

        colors.pressedColor = new Color(
            baseColor.r,
            baseColor.g,
            baseColor.b,
            PressedAlpha / 255f
        );

        colors.selectedColor = new Color(
            baseColor.r,
            baseColor.g,
            baseColor.b,
            SelectedAlpha / 255f
        );

        colors.disabledColor = new Color(
            baseColor.r,
            baseColor.g,
            baseColor.b,
            DisabledAlpha / 255f
        );


        // Only change the Button ColorBlock.
        // The Button Image stays white.
        claimButton.colors = colors;
    }


    // ============================================================
    // CLAIM
    // ============================================================

    public void Claim()
    {
        if (AchievementManager.Instance == null)
            return;

        if (string.IsNullOrEmpty(achievementID))
            return;


        AchievementManager.Instance.ClaimAchievement(
            achievementID
        );


        // Refresh after claiming so the button,
        // Soulstone icon, amount, and text disappear.
        RefreshUI();
    }


    // ============================================================
    // NUMBER FORMATTING
    // ============================================================

    private string FormatNumber(int number)
    {
        if (number >= 1000000)
            return (number / 1000000f).ToString("0.00") + "m";


        if (number >= 1000)
            return (number / 1000f).ToString("0.00") + "k";


        return number.ToString();
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