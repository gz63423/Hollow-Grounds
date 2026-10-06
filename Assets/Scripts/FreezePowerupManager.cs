using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FreezePowerupManager : MonoBehaviour
{
    // =========================================================
    // POWERUP INFORMATION
    // =========================================================

    [Header("Powerup Information")]

    [Tooltip("Unique name for this powerup.")]
    public string powerupName = "Freeze";

    [Tooltip("Maximum level INCLUDING the unlock level.")]
    public int maxLevel = 10;


    // =========================================================
    // POWERUP VALUES
    // =========================================================

    [Header("Powerup Values")]

    public float lockedValue = 0f;

    public float unlockValue = 0.5f;

    public float maxValue = 5f;


    // =========================================================
    // PRICES
    // =========================================================

    [Header("Upgrade Prices")]

    public float[] upgradePrices = new float[10];


    // =========================================================
    // UNLOCK UI
    // =========================================================

    [Header("Unlock UI")]

    public Button unlockButton;

    public TMP_Text unlockButtonText;

    public TMP_Text unlockText;

    public Image soulstoneImage1;

    public TMP_Text unlockPriceText;


    // =========================================================
    // UPGRADE UI
    // =========================================================

    [Header("Upgrade UI")]

    public Button upgradeButton;

    public TMP_Text upgradeButtonText;

    public TMP_Text upgradeText;

    public Image soulstoneImage2;

    public TMP_Text upgradePriceText;


    // =========================================================
    // INFO UI
    // =========================================================

    [Header("Info UI")]

    public TMP_Text upgradeInfoText;

    public TMP_Text infoMaxedText;


    // =========================================================
    // IMAGES
    // =========================================================

    [Header("Images")]

    public Image image1;

    public Image image2;

    public Sprite image1Locked;

    public Sprite image2Locked;

    public Sprite image1Unlocked;

    public Sprite image2Unlocked;


    // =========================================================
    // VALUE / LEVEL
    // =========================================================

    [Header("Value / Level")]

    public TMP_Text valueText;

    public TMP_Text levelText;


    // =========================================================
    // SAVE
    // =========================================================

    [Header("Save")]

    public string saveKey = "Active_Freeze";


    // =========================================================
    // INTERNAL
    // =========================================================

    private bool isUnlocked;

    private int currentLevel;


    public bool IsUnlocked => isUnlocked;

    public int CurrentLevel => currentLevel;


    // =========================================================
    // UNITY
    // =========================================================

    private void Awake()
    {
        EnsureSaveKey();

        LoadPowerup();
    }


    private void OnEnable()
    {
        EnsureSaveKey();

        LoadPowerup();

        UpdateUI();

        RefreshActiveUI();
    }


    private void Start()
    {
        LoadPowerup();

        UpdateUI();

        RefreshActiveUI();
    }


    // =========================================================
    // SAVE KEY
    // =========================================================

    private void EnsureSaveKey()
    {
        if (string.IsNullOrWhiteSpace(saveKey))
        {
            saveKey =
                "Active_" +
                powerupName.Replace(" ", "");
        }
    }


    // =========================================================
    // LOAD
    // =========================================================

    public void LoadPowerup()
    {
        EnsureSaveKey();

        isUnlocked =
            PlayerPrefs.GetInt(
                saveKey + "_Unlocked",
                0
            ) == 1;


        currentLevel =
            PlayerPrefs.GetInt(
                saveKey + "_Level",
                0
            );


        currentLevel =
            Mathf.Clamp(
                currentLevel,
                0,
                maxLevel
            );


        if (!isUnlocked)
        {
            currentLevel = 0;
        }
    }


    public void ReloadPowerup()
    {
        LoadPowerup();

        UpdateUI();

        RefreshActiveUI();
    }


    // =========================================================
    // UNLOCK
    // =========================================================

    public void UnlockPowerup()
    {
        if (isUnlocked)
            return;


        float price = GetUnlockPrice();


        if (!TrySpendSoulstones(price))
            return;


        // COUNT SUCCESSFUL PURCHASE
        AchievementStats.AddUpgradesPurchased(1);


        isUnlocked = true;

        currentLevel = 1;


        SavePowerup();

        UpdateUI();

        RefreshActiveUI();
    }


    // =========================================================
    // UPGRADE
    // =========================================================

    public void UpgradePowerup()
    {
        if (!isUnlocked)
            return;


        if (currentLevel >= maxLevel)
            return;


        float price =
            GetUpgradePrice();


        if (!TrySpendSoulstones(price))
            return;


        // COUNT SUCCESSFUL PURCHASE
        AchievementStats.AddUpgradesPurchased(1);


        currentLevel++;


        currentLevel =
            Mathf.Clamp(
                currentLevel,
                1,
                maxLevel
            );


        SavePowerup();

        UpdateUI();

        RefreshActiveUI();
    }


    // =========================================================
    // SOULSTONES
    // =========================================================

    private bool TrySpendSoulstones(float amount)
    {
        if (amount <= 0f)
            return true;


        if (Soulstones.Instance == null)
            return false;


        int cost =
            Mathf.RoundToInt(amount);


        if (!Soulstones.Instance.HasEnoughSoulstones(cost))
            return false;


        return Soulstones.Instance.SpendSoulstones(cost);
    }


    // =========================================================
    // SAVE
    // =========================================================

    public void SavePowerup()
    {
        PlayerPrefs.SetInt(
            saveKey + "_Unlocked",
            isUnlocked ? 1 : 0
        );


        PlayerPrefs.SetInt(
            saveKey + "_Level",
            currentLevel
        );


        PlayerPrefs.Save();
    }


    // =========================================================
    // RESET
    // =========================================================

    public void ResetPowerup()
    {
        isUnlocked = false;

        currentLevel = 0;


        PlayerPrefs.DeleteKey(
            saveKey + "_Unlocked"
        );


        PlayerPrefs.DeleteKey(
            saveKey + "_Level"
        );


        PlayerPrefs.Save();


        // -----------------------------------------------------
        // ACTUALLY UNEQUIP FREEZE
        // -----------------------------------------------------

        if (ActivePowerupManager.Instance != null)
        {
            ActivePowerupManager.Instance.UnequipPowerup(
                ActivePowerupManager.FreezeIndex
            );
        }


        // -----------------------------------------------------
        // REFRESH FREEZE UI
        // -----------------------------------------------------

        UpdateUI();


        // -----------------------------------------------------
        // REFRESH ACTIVE SLOT UI
        // -----------------------------------------------------

        if (ActivePowerupManager.Instance != null)
        {
            ActivePowerupManager.Instance.RefreshAllActiveUI();
        }
    }


    // =========================================================
    // VALUE
    // =========================================================

    public float GetCurrentValue()
    {
        if (!isUnlocked || currentLevel <= 0)
            return lockedValue;


        if (currentLevel >= maxLevel)
            return maxValue;


        if (maxLevel <= 1)
            return maxValue;


        float progress =
            (float)(currentLevel - 1) /
            (maxLevel - 1);


        return Mathf.Lerp(
            unlockValue,
            maxValue,
            progress
        );
    }


    // =========================================================
    // PRICES
    // =========================================================

    private float GetUnlockPrice()
    {
        if (upgradePrices == null ||
            upgradePrices.Length == 0)
        {
            return 0f;
        }


        return upgradePrices[0];
    }


    private float GetUpgradePrice()
    {
        if (upgradePrices == null ||
            upgradePrices.Length == 0)
        {
            return 0f;
        }


        int index =
            Mathf.Clamp(
                currentLevel,
                0,
                upgradePrices.Length - 1
            );


        return upgradePrices[index];
    }


    // =========================================================
    // UI
    // =========================================================

    public void UpdateUI()
    {
        bool maxed =
            isUnlocked &&
            currentLevel >= maxLevel;


        // =====================================================
        // LOCKED
        // =====================================================

        if (!isUnlocked)
        {
            if (unlockButton != null)
                unlockButton.gameObject.SetActive(true);


            if (unlockButtonText != null)
                unlockButtonText.gameObject.SetActive(true);


            if (unlockText != null)
                unlockText.gameObject.SetActive(true);


            if (soulstoneImage1 != null)
                soulstoneImage1.gameObject.SetActive(true);


            if (unlockPriceText != null)
            {
                unlockPriceText.gameObject.SetActive(true);

                unlockPriceText.text =
                    FormatPrice(GetUnlockPrice());
            }


            if (upgradeButton != null)
                upgradeButton.gameObject.SetActive(false);


            if (upgradeButtonText != null)
                upgradeButtonText.gameObject.SetActive(false);


            if (upgradeText != null)
                upgradeText.gameObject.SetActive(false);


            if (soulstoneImage2 != null)
                soulstoneImage2.gameObject.SetActive(false);


            if (upgradePriceText != null)
                upgradePriceText.gameObject.SetActive(false);


            if (upgradeInfoText != null)
                upgradeInfoText.gameObject.SetActive(true);


            if (infoMaxedText != null)
                infoMaxedText.gameObject.SetActive(false);


            SetLockedImages();

            UpdateValueText();

            UpdateLevelText();

            return;
        }


        // =====================================================
        // UNLOCKED
        // =====================================================

        if (unlockButton != null)
            unlockButton.gameObject.SetActive(false);


        if (unlockButtonText != null)
            unlockButtonText.gameObject.SetActive(false);


        if (unlockText != null)
            unlockText.gameObject.SetActive(false);


        if (soulstoneImage1 != null)
            soulstoneImage1.gameObject.SetActive(false);


        if (unlockPriceText != null)
            unlockPriceText.gameObject.SetActive(false);


        SetUnlockedImages();


        // =====================================================
        // MAXED
        // =====================================================

        if (maxed)
        {
            if (upgradeButton != null)
                upgradeButton.gameObject.SetActive(false);


            if (upgradeButtonText != null)
                upgradeButtonText.gameObject.SetActive(false);


            if (upgradeText != null)
                upgradeText.gameObject.SetActive(false);


            if (soulstoneImage2 != null)
                soulstoneImage2.gameObject.SetActive(false);


            if (upgradePriceText != null)
                upgradePriceText.gameObject.SetActive(false);


            if (upgradeInfoText != null)
                upgradeInfoText.gameObject.SetActive(false);


            if (infoMaxedText != null)
                infoMaxedText.gameObject.SetActive(true);
        }


        // =====================================================
        // UPGRADE AVAILABLE
        // =====================================================

        else
        {
            if (upgradeButton != null)
                upgradeButton.gameObject.SetActive(true);


            if (upgradeButtonText != null)
                upgradeButtonText.gameObject.SetActive(true);


            if (upgradeText != null)
                upgradeText.gameObject.SetActive(true);


            if (soulstoneImage2 != null)
                soulstoneImage2.gameObject.SetActive(true);


            if (upgradePriceText != null)
            {
                upgradePriceText.gameObject.SetActive(true);

                upgradePriceText.text =
                    FormatPrice(GetUpgradePrice());
            }


            if (upgradeInfoText != null)
                upgradeInfoText.gameObject.SetActive(true);


            if (infoMaxedText != null)
                infoMaxedText.gameObject.SetActive(false);
        }


        UpdateValueText();

        UpdateLevelText();
    }


    // =========================================================
    // VALUE TEXT
    // =========================================================

    private void UpdateValueText()
    {
        if (valueText == null)
            return;


        // -----------------------------------------------------
        // LOCKED
        // -----------------------------------------------------

        if (!isUnlocked)
        {
            valueText.text =
                $"{lockedValue:0.0} seconds → <color=#06FF00>{unlockValue:0.0} seconds</color>";

            return;
        }


        // -----------------------------------------------------
        // CURRENT VALUE
        // -----------------------------------------------------

        float current =
            GetCurrentValue();


        // -----------------------------------------------------
        // MAX LEVEL
        // -----------------------------------------------------

        if (currentLevel >= maxLevel)
        {
            valueText.text =
                $"{current:0.0} seconds";

            return;
        }


        // -----------------------------------------------------
        // NEXT VALUE
        // -----------------------------------------------------

        int nextLevel =
            currentLevel + 1;


        float progress =
            (float)(nextLevel - 1) /
            (maxLevel - 1);


        float next =
            Mathf.Lerp(
                unlockValue,
                maxValue,
                progress
            );


        next =
            Mathf.Clamp(
                next,
                unlockValue,
                maxValue
            );


        // -----------------------------------------------------
        // DISPLAY
        // -----------------------------------------------------

        valueText.text =
            $"{current:0.0} seconds → <color=#06FF00>{next:0.0} seconds</color>";
    }


    // =========================================================
    // LEVEL TEXT
    // =========================================================

    private void UpdateLevelText()
    {
        if (levelText == null)
            return;


        if (!isUnlocked)
        {
            levelText.text =
                "Not owned";

            return;
        }


        levelText.text =
            $"Level {currentLevel} / {maxLevel}";
    }


    // =========================================================
    // IMAGE STATES
    // =========================================================

    private void SetLockedImages()
    {
        if (image1 != null)
            image1.sprite = image1Locked;


        if (image2 != null)
            image2.sprite = image2Locked;
    }


    private void SetUnlockedImages()
    {
        if (image1 != null)
            image1.sprite = image1Unlocked;


        if (image2 != null)
            image2.sprite = image2Unlocked;
    }


    // =========================================================
    // PRICE FORMAT
    // =========================================================

    private string FormatPrice(float price)
    {
        return price.ToString("0.##");
    }


    // =========================================================
    // ACTIVE UI
    // =========================================================

    private void RefreshActiveUI()
    {
        if (ActivePowerupManager.Instance != null)
        {
            ActivePowerupManager.Instance.RefreshAllActiveUI();
        }
    }
}