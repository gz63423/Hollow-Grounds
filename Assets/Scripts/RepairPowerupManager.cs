using NUnit.Framework;
using System.Net.Sockets;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RepairPowerupManager : MonoBehaviour
{
    // =========================================================
    // POWERUP INFORMATION
    // =========================================================

    [Header("Powerup Information")]

    [Tooltip("Unique name for this powerup.")]
    public string powerupName = "Repair";

    [Tooltip("Powerup index. Repair = 2.")]
    public int powerupIndex = ActivePowerupManager.RepairIndex;

    [Tooltip("Maximum level INCLUDING the unlock level.")]
    public int maxLevel = 10;


    // =========================================================
    // REBIRTH REQUIREMENT
    // =========================================================

    [Header("Rebirth Requirement")]

    [Tooltip("Minimum rebirths required to unlock Repair.")]
    public int requiredRebirths = 1;

    // Must match RebirthManager.
    private const string RebirthSaveKey = "RebirthCount";


    // =========================================================
    // REBIRTH REQUIREMENT UI
    // =========================================================

    [Header("Rebirth Requirement UI")]

    [Tooltip("Text shown instead of the normal UI while the rebirth requirement has not been met.")]
    public TMP_Text rebirthRequirementText;

    [Tooltip("Message displayed while the required rebirths have not been reached.")]
    public string rebirthRequirementMessage = "Requires 1 Rebirth";


    // =========================================================
    // POWERUP VALUES
    // =========================================================

    [Header("Repair Values")]

    [Tooltip("Repair amount for each level. Element 0 = Level 1, Element 9 = Level 10.")]
    public float[] repairValues = new float[10]
    {
        3f,
        6f,
        9f,
        12f,
        15f,
        18f,
        21f,
        24f,
        27f,
        30f
    };

    [Tooltip("Value shown while locked.")]
    public float lockedValue = 3f;


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
    // MISC UI
    // =========================================================

    [Header("Misc UI")]

    [Tooltip("Normal UI text. Hidden while the rebirth requirement is not met.")]
    public TMP_Text miscText1;

    [Tooltip("Normal UI text. Hidden while the rebirth requirement is not met.")]
    public TMP_Text miscText2;

    [Tooltip("Normal UI text. Hidden while the rebirth requirement is not met.")]
    public TMP_Text miscText3;

    [Tooltip("Normal UI text. Hidden while the rebirth requirement is not met.")]
    public TMP_Text miscText4;


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

    public string saveKey = "Active_Repair";


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
        EnsurePowerupIndex();

        EnsureSaveKey();

        LoadPowerup();
    }


    private void OnEnable()
    {
        EnsurePowerupIndex();

        EnsureSaveKey();

        LoadPowerup();

        UpdateUI();

        RefreshActiveUI();
    }


    private void Start()
    {
        EnsurePowerupIndex();

        LoadPowerup();

        UpdateUI();

        RefreshActiveUI();
    }


    // =========================================================
    // POWERUP INDEX
    // =========================================================

    private void EnsurePowerupIndex()
    {
        powerupIndex =
            ActivePowerupManager.RepairIndex;
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
    // REBIRTH REQUIREMENT
    // =========================================================

    public int GetCurrentRebirths()
    {
        return PlayerPrefs.GetInt(
            RebirthSaveKey,
            0
        );
    }


    public bool HasRequiredRebirths()
    {
        return GetCurrentRebirths() >= requiredRebirths;
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


        // If not unlocked, level must always be 0.
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


        // =====================================================
        // REBIRTH REQUIREMENT
        // =====================================================

        if (!HasRequiredRebirths())
        {
            Debug.Log(
                $"[RepairPowerupManager] Repair requires " +
                $"at least {requiredRebirths} rebirth."
            );

            UpdateUI();

            return;
        }


        float price =
            GetUnlockPrice();


        if (!TrySpendSoulstones(price))
            return;


        // COUNT SUCCESSFUL PURCHASE
        AchievementStats.AddUpgradesPurchased(1);


        // =====================================================
        // UNLOCK
        // =====================================================

        isUnlocked = true;

        currentLevel = 1;


        // =====================================================
        // SAVE
        // =====================================================

        SavePowerup();


        // =====================================================
        // UPDATE UI
        // =====================================================

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


        if (!Soulstones.Instance.HasEnoughSoulstones(
                Mathf.RoundToInt(amount)))
        {
            return false;
        }


        return Soulstones.Instance.SpendSoulstones(
            Mathf.RoundToInt(amount)
        );
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


        if (ActivePowerupManager.Instance != null)
        {
            ActivePowerupManager.Instance.UnequipPowerup(
                ActivePowerupManager.RepairIndex
            );
        }


        UpdateUI();

        RefreshActiveUI();
    }


    // =========================================================
    // CURRENT REPAIR VALUE
    // =========================================================

    public float GetCurrentValue()
    {
        if (!isUnlocked || currentLevel <= 0)
        {
            return lockedValue;
        }


        if (repairValues == null ||
            repairValues.Length == 0)
        {
            return 0f;
        }


        int index =
            Mathf.Clamp(
                currentLevel - 1,
                0,
                repairValues.Length - 1
            );


        return repairValues[index];
    }


    // =========================================================
    // REPAIR VALUE
    // =========================================================

    public float GetRepairAmount()
    {
        return GetCurrentValue();
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


        bool rebirthRequirementMet =
            HasRequiredRebirths();


        // =====================================================
        // REBIRTH REQUIREMENT NOT MET
        // =====================================================

        if (!isUnlocked && !rebirthRequirementMet)
        {
            SetRebirthRequirementUI();

            return;
        }


        // =====================================================
        // NORMAL UI
        // =====================================================

        HideRebirthRequirementText();

        SetMiscTextUI(true);


        // =====================================================
        // NOT UNLOCKED
        // =====================================================

        if (!isUnlocked)
        {
            if (unlockButton != null)
            {
                unlockButton.gameObject.SetActive(true);

                unlockButton.interactable =
                    rebirthRequirementMet;
            }


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
        // UNLOCKED BUT NOT MAXED
        // =====================================================

        else
        {
            if (upgradeButton != null)
                upgradeButton.gameObject.SetActive(true);


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
    // REBIRTH REQUIREMENT UI STATE
    // =========================================================

    private void SetRebirthRequirementUI()
    {
        // =====================================================
        // HIDE ALL NORMAL UI
        // =====================================================

        if (unlockButton != null)
        {
            unlockButton.gameObject.SetActive(false);
            unlockButton.interactable = false;
        }


        if (unlockButtonText != null)
            unlockButtonText.gameObject.SetActive(false);


        if (unlockText != null)
            unlockText.gameObject.SetActive(false);


        if (soulstoneImage1 != null)
            soulstoneImage1.gameObject.SetActive(false);


        if (unlockPriceText != null)
            unlockPriceText.gameObject.SetActive(false);


        if (upgradeButton != null)
        {
            upgradeButton.gameObject.SetActive(false);
            upgradeButton.interactable = false;
        }


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
            infoMaxedText.gameObject.SetActive(false);


        if (valueText != null)
            valueText.gameObject.SetActive(false);


        // =====================================================
        // MISC UI
        // =====================================================

        SetMiscTextUI(false);


        // =====================================================
        // KEEP LEVEL TEXT
        // =====================================================

        UpdateLevelText();


        if (levelText != null)
            levelText.gameObject.SetActive(true);


        // =====================================================
        // KEEP BOTH IMAGES
        // =====================================================

        SetLockedImages();


        if (image1 != null)
            image1.gameObject.SetActive(true);


        if (image2 != null)
            image2.gameObject.SetActive(true);


        // =====================================================
        // SHOW REBIRTH REQUIREMENT MESSAGE
        // =====================================================

        if (rebirthRequirementText != null)
        {
            rebirthRequirementText.gameObject.SetActive(true);

            rebirthRequirementText.text =
                rebirthRequirementMessage;
        }
    }


    // =========================================================
    // HIDE REBIRTH REQUIREMENT TEXT
    // =========================================================

    private void HideRebirthRequirementText()
    {
        if (rebirthRequirementText != null)
            rebirthRequirementText.gameObject.SetActive(false);
    }


    // =========================================================
    // MISC UI
    // =========================================================

    private void SetMiscTextUI(bool active)
    {
        if (miscText1 != null)
            miscText1.gameObject.SetActive(active);


        if (miscText2 != null)
            miscText2.gameObject.SetActive(active);


        if (miscText3 != null)
            miscText3.gameObject.SetActive(active);


        if (miscText4 != null)
            miscText4.gameObject.SetActive(active);
    }


    // =========================================================
    // UNLOCK UI STATE
    // =========================================================

    private void SetUnlockUI()
    {
        if (unlockButton != null)
        {
            unlockButton.gameObject.SetActive(true);

            unlockButton.interactable =
                HasRequiredRebirths();
        }


        if (unlockText != null)
            unlockText.gameObject.SetActive(true);


        if (soulstoneImage1 != null)
            soulstoneImage1.gameObject.SetActive(true);


        if (unlockPriceText != null)
            unlockPriceText.gameObject.SetActive(true);


        if (upgradeButton != null)
            upgradeButton.gameObject.SetActive(false);


        if (upgradeText != null)
            upgradeText.gameObject.SetActive(false);


        if (soulstoneImage2 != null)
            soulstoneImage2.gameObject.SetActive(false);


        if (upgradePriceText != null)
            upgradePriceText.gameObject.SetActive(false);


        if (upgradeInfoText != null)
            upgradeInfoText.gameObject.SetActive(false);


        if (infoMaxedText != null)
            infoMaxedText.gameObject.SetActive(false);


        if (unlockPriceText != null)
        {
            unlockPriceText.text =
                FormatPrice(
                    GetUnlockPrice()
                );
        }


        SetLockedImages();
    }


    // =========================================================
    // UPGRADE UI STATE
    // =========================================================

    private void SetUpgradeUI()
    {
        if (unlockButton != null)
            unlockButton.gameObject.SetActive(false);


        if (unlockText != null)
            unlockText.gameObject.SetActive(false);


        if (soulstoneImage1 != null)
            soulstoneImage1.gameObject.SetActive(false);


        if (unlockPriceText != null)
            unlockPriceText.gameObject.SetActive(false);


        if (upgradeButton != null)
            upgradeButton.gameObject.SetActive(true);


        if (upgradeText != null)
            upgradeText.gameObject.SetActive(true);


        if (soulstoneImage2 != null)
            soulstoneImage2.gameObject.SetActive(true);


        if (upgradePriceText != null)
            upgradePriceText.gameObject.SetActive(true);


        SetUnlockedImages();


        if (upgradePriceText != null)
        {
            upgradePriceText.text =
                FormatPrice(
                    GetUpgradePrice()
                );
        }
    }


    // =========================================================
    // UPGRADE AVAILABLE UI STATE
    // =========================================================

    private void SetUpgradeableUI()
    {
        if (upgradeButton != null)
            upgradeButton.gameObject.SetActive(true);


        if (upgradeInfoText != null)
            upgradeInfoText.gameObject.SetActive(true);


        if (infoMaxedText != null)
            infoMaxedText.gameObject.SetActive(false);


        if (upgradePriceText != null)
        {
            upgradePriceText.gameObject.SetActive(true);

            upgradePriceText.text =
                FormatPrice(
                    GetUpgradePrice()
                );
        }


        if (soulstoneImage2 != null)
            soulstoneImage2.gameObject.SetActive(true);
    }


    // =========================================================
    // MAXED UI
    // =========================================================

    private void SetMaxedUI()
    {
        if (upgradeButton != null)
            upgradeButton.gameObject.SetActive(false);


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


    // =========================================================
    // VALUE TEXT
    // =========================================================

    private void UpdateValueText()
    {
        if (valueText == null)
            return;


        // =====================================================
        // LOCKED
        // =====================================================

        if (!isUnlocked)
        {
            float firstValue =
                GetFirstRepairValue();


            valueText.text =
                $"0 HP -> <color=#06FF00>{firstValue:0.0} HP</color>";

            return;
        }


        // =====================================================
        // CURRENT VALUE
        // =====================================================

        float current =
            GetCurrentValue();


        // =====================================================
        // MAXED
        // =====================================================

        if (currentLevel >= maxLevel)
        {
            valueText.text =
                $"{current:0.0} HP";

            return;
        }


        // =====================================================
        // NEXT VALUE
        // =====================================================

        int nextLevel =
            currentLevel + 1;


        float next =
            GetValueForLevel(
                nextLevel
            );


        valueText.text =
            $"{current:0.0} HP -> <color=#06FF00>{next:0.0} HP</color>";
    }


    // =========================================================
    // GET FIRST REPAIR VALUE
    // =========================================================

    private float GetFirstRepairValue()
    {
        if (repairValues == null ||
            repairValues.Length == 0)
        {
            return 0f;
        }


        return repairValues[0];
    }


    // =========================================================
    // GET VALUE FOR LEVEL
    // =========================================================

    private float GetValueForLevel(int level)
    {
        if (repairValues == null ||
            repairValues.Length == 0)
        {
            return 0f;
        }


        int index =
            Mathf.Clamp(
                level - 1,
                0,
                repairValues.Length - 1
            );


        return repairValues[index];
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