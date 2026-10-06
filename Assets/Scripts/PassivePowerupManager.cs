using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PassivePowerupManager : MonoBehaviour
{
    [Header("Powerup Information")]
    [Tooltip("Unique name for this powerup. Used only for identification.")]
    public string powerupName;

    [Tooltip("Maximum level INCLUDING the unlock level.")]
    public int maxLevel = 10;

    [Tooltip("Unique index for this passive powerup.")]
    public int passiveIndex = 0;


    [Header("Powerup Values")]
    [Tooltip("Value while locked.")]
    public float lockedValue = 0f;

    [Tooltip("Value immediately after unlocking.")]
    public float unlockValue = 2.5f;

    [Tooltip("Value at max level.")]
    public float maxValue = 25f;


    [Header("Upgrade Prices")]
    [Tooltip(
        "Element 0 = Unlock price.\n" +
        "Element 1 = Upgrade from Level 1 to 2.\n" +
        "Element 2 = Upgrade from Level 2 to 3.\n" +
        "...\n" +
        "Element 9 = Upgrade from Level 9 to 10."
    )]
    public int[] upgradePrices = new int[10];


    [Header("Unlock UI")]
    public GameObject unlockButton;
    public GameObject unlockButtonText;

    [Tooltip("Non-button text such as 'Purchase for:' shown while locked.")]
    public GameObject unlockText;

    [Tooltip("Soulstone image shown before the powerup is unlocked.")]
    public GameObject soulstoneImage1;

    [Tooltip("Price shown before the powerup is unlocked.")]
    public TMP_Text unlockPriceText;


    [Header("Upgrade UI")]
    public GameObject upgradeButton;
    public GameObject upgradeButtonText;

    [Tooltip("Non-button text such as 'Purchase for:' shown while upgrading.")]
    public GameObject upgradeText;

    [Tooltip("Soulstone image shown when upgrading.")]
    public GameObject soulstoneImage2;

    [Tooltip("Price shown when upgrading.")]
    public TMP_Text upgradePriceText;


    [Header("Info UI")]
    [Tooltip("Information text shown while the powerup can still be upgraded.")]
    public GameObject upgradeInfoText;

    [Tooltip("Information text shown when the powerup reaches max level.")]
    public GameObject infoMaxedText;


    [Header("Equip UI")]
    [Tooltip("Shown when the powerup is unlocked and NOT equipped.")]
    public GameObject equipButton;

    [Tooltip("Text inside the Equip button.")]
    public GameObject equipButtonText;

    [Tooltip("Shown when the powerup IS equipped.")]
    public GameObject unequipButton;

    [Tooltip("Text inside the Unequip button.")]
    public GameObject unequipButtonText;


    [Header("Powerup Images")]
    [Tooltip("GameObject containing the first Image component.")]
    public GameObject image1;

    [Tooltip("GameObject containing the second Image component.")]
    public GameObject image2;


    [Header("Locked Images")]
    public Sprite image1Locked;
    public Sprite image2Locked;


    [Header("Unlocked Images")]
    public Sprite image1Unlocked;
    public Sprite image2Unlocked;


    [Header("Value Display")]
    public TMP_Text valueText;


    [Header("Level Display")]
    public TMP_Text levelText;


    [Header("Save Settings")]
    [Tooltip(
        "Unique save key for this powerup.\n" +
        "Example: Passive_SoulSiphon\n" +
        "If left blank, it will automatically use Powerup Name."
    )]
    public string saveKey;


    private bool isUnlocked;
    private bool isEquipped;
    private int currentLevel;


    private const string EquippedPassiveIndexKey =
        "EquippedPassiveIndex";


    // =========================================================
    // START
    // =========================================================

    private void Start()
    {
        LoadPowerup();
        UpdateUI();
    }


    // =========================================================
    // ON ENABLE
    // =========================================================

    private void OnEnable()
    {
        LoadPowerup();
        UpdateUI();
    }


    // =========================================================
    // GET SAVE KEY
    // =========================================================

    private string GetSaveKey()
    {
        // -----------------------------------------------------
        // Explicit save key always takes priority.
        // -----------------------------------------------------

        if (!string.IsNullOrEmpty(saveKey))
            return saveKey;


        // -----------------------------------------------------
        // Automatically generate the passive save key.
        // Example:
        // Soul Siphon -> Passive_SoulSiphon
        // -----------------------------------------------------

        if (!string.IsNullOrEmpty(powerupName))
        {
            return "Passive_" +
                   powerupName.Replace(" ", "");
        }


        return "";
    }


    // =========================================================
    // LOAD POWERUP
    // =========================================================

    private void LoadPowerup()
    {
        string key = GetSaveKey();


        if (string.IsNullOrEmpty(key))
        {
            Debug.LogError(
                gameObject.name +
                " cannot load because it has no Save Key " +
                "and no Powerup Name."
            );

            isUnlocked = false;
            isEquipped = false;
            currentLevel = 0;

            return;
        }


        // -----------------------------------------------------
        // Store generated key in inspector/runtime variable.
        // -----------------------------------------------------

        if (string.IsNullOrEmpty(saveKey))
        {
            saveKey = key;
        }


        // -----------------------------------------------------
        // LOAD UNLOCKED STATE
        // -----------------------------------------------------

        isUnlocked =
            PlayerPrefs.GetInt(
                key + "_Unlocked",
                0
            ) == 1;


        // -----------------------------------------------------
        // LOAD LEVEL
        // -----------------------------------------------------

        currentLevel =
            PlayerPrefs.GetInt(
                key + "_Level",
                0
            );

        currentLevel =
            Mathf.Clamp(
                currentLevel,
                0,
                maxLevel
            );


        // -----------------------------------------------------
        // LOAD CURRENT EQUIPPED PASSIVE
        // -----------------------------------------------------

        int equippedIndex =
            PlayerPrefs.GetInt(
                EquippedPassiveIndexKey,
                -1
            );


        // -----------------------------------------------------
        // OLD SAVE FALLBACK
        // -----------------------------------------------------

        bool oldSavedEquipped =
            PlayerPrefs.GetInt(
                key + "_Equipped",
                0
            ) == 1;


        if (equippedIndex != -1)
        {
            isEquipped =
                isUnlocked &&
                equippedIndex == passiveIndex;
        }
        else
        {
            isEquipped =
                isUnlocked &&
                oldSavedEquipped;


            // -------------------------------------------------
            // Migrate old equipped save to the shared index.
            // -------------------------------------------------

            if (isEquipped)
            {
                PlayerPrefs.SetInt(
                    EquippedPassiveIndexKey,
                    passiveIndex
                );

                PlayerPrefs.Save();
            }
        }


        // -----------------------------------------------------
        // LOCKED POWERUPS CANNOT BE EQUIPPED.
        // -----------------------------------------------------

        if (!isUnlocked)
        {
            currentLevel = 0;
            isEquipped = false;
        }


        Debug.Log(
            "PASSIVE LOADED: " +
            powerupName +
            " | Save Key: " +
            key +
            " | Unlocked: " +
            isUnlocked +
            " | Level: " +
            currentLevel +
            " | Equipped: " +
            isEquipped +
            " | Index: " +
            passiveIndex
        );
    }


    // =========================================================
    // RELOAD POWERUP
    // =========================================================

    public void ReloadPowerup()
    {
        LoadPowerup();
        UpdateUI();
    }


    // =========================================================
    // UNLOCK POWERUP
    // =========================================================

    public void UnlockPowerup()
    {
        if (isUnlocked)
            return;


        if (upgradePrices == null ||
            upgradePrices.Length < 1)
        {
            Debug.LogError(
                powerupName +
                " does not have an unlock price assigned."
            );

            return;
        }


        int unlockPrice =
            upgradePrices[0];


        if (Soulstones.Instance == null)
        {
            Debug.LogError(
                "Soulstones.Instance was not found."
            );

            return;
        }


        if (!Soulstones.Instance.HasEnoughSoulstones(
            unlockPrice))
        {
            return;
        }


        bool spent =
            Soulstones.Instance.SpendSoulstones(
                unlockPrice
            );


        if (!spent)
            return;


        // -----------------------------------------------------
        // COUNT SUCCESSFUL PURCHASE
        // -----------------------------------------------------

        if (UpgradesPurchased.Instance != null)
        {
            AchievementStats.AddUpgradesPurchased(1);
        }


        // -----------------------------------------------------
        // UNLOCK
        // -----------------------------------------------------

        isUnlocked = true;
        currentLevel = 1;
        isEquipped = false;


        SavePowerup();
        UpdateUI();


        Debug.Log(
            "PASSIVE UNLOCKED: " +
            powerupName +
            " | Level: " +
            currentLevel
        );
    }


    // =========================================================
    // UPGRADE POWERUP
    // =========================================================

    public void UpgradePowerup()
    {
        if (!isUnlocked)
            return;


        if (currentLevel >= maxLevel)
            return;


        int priceIndex =
            currentLevel;


        if (upgradePrices == null ||
            priceIndex < 0 ||
            priceIndex >= upgradePrices.Length)
        {
            Debug.LogError(
                powerupName +
                " is missing the upgrade price for level " +
                (currentLevel + 1)
            );

            return;
        }


        int upgradePrice =
            upgradePrices[priceIndex];


        if (Soulstones.Instance == null)
        {
            Debug.LogError(
                "Soulstones.Instance was not found."
            );

            return;
        }


        if (!Soulstones.Instance.HasEnoughSoulstones(
            upgradePrice))
        {
            return;
        }


        bool spent =
            Soulstones.Instance.SpendSoulstones(
                upgradePrice
            );


        if (!spent)
            return;


        // -----------------------------------------------------
        // COUNT SUCCESSFUL PURCHASE
        // -----------------------------------------------------

        if (UpgradesPurchased.Instance != null)
        {
            AchievementStats.AddUpgradesPurchased(1);
        }


        currentLevel++;


        currentLevel =
            Mathf.Clamp(
                currentLevel,
                1,
                maxLevel
            );


        SavePowerup();
        UpdateUI();
    }


    // =========================================================
    // EQUIP POWERUP
    // =========================================================

    public void EquipPowerup()
    {
        if (!isUnlocked)
            return;


        // -----------------------------------------------------
        // Already equipped.
        // -----------------------------------------------------

        if (IsPassiveEquipped(passiveIndex))
        {
            isEquipped = true;
            UpdateUI();
            return;
        }


        // -----------------------------------------------------
        // Get currently equipped passive.
        // -----------------------------------------------------

        int previousEquippedIndex =
            GetEquippedPassiveIndex();


        // -----------------------------------------------------
        // Clear previous passive's old equipped save.
        // -----------------------------------------------------

        if (previousEquippedIndex != -1 &&
            previousEquippedIndex != passiveIndex)
        {
            string previousSaveKey = "";


            // Soul Siphon
            if (previousEquippedIndex == 0)
            {
                previousSaveKey =
                    "Passive_SoulSiphon";
            }


            if (!string.IsNullOrEmpty(previousSaveKey))
            {
                PlayerPrefs.SetInt(
                    previousSaveKey + "_Equipped",
                    0
                );
            }
        }


        // -----------------------------------------------------
        // Equip this passive.
        // -----------------------------------------------------

        isEquipped = true;


        string key =
            GetSaveKey();


        // Shared equipped passive index.
        PlayerPrefs.SetInt(
            EquippedPassiveIndexKey,
            passiveIndex
        );


        // Save this passive's data.
        PlayerPrefs.SetInt(
            key + "_Unlocked",
            1
        );


        PlayerPrefs.SetInt(
            key + "_Level",
            currentLevel
        );


        PlayerPrefs.SetInt(
            key + "_Equipped",
            1
        );


        PlayerPrefs.Save();


        Debug.Log(
            "PASSIVE EQUIPPED AND SAVED: " +
            powerupName +
            " | Index: " +
            passiveIndex +
            " | Equipped Index Saved: " +
            GetEquippedPassiveIndex()
        );


        UpdateUI();
    }


    // =========================================================
    // UNEQUIP POWERUP
    // =========================================================

    public void UnequipPowerup()
    {
        if (!isEquipped &&
            !IsPassiveEquipped(passiveIndex))
        {
            return;
        }


        isEquipped = false;


        string key =
            GetSaveKey();


        // -----------------------------------------------------
        // Clear this passive's equipped save.
        // -----------------------------------------------------

        PlayerPrefs.SetInt(
            key + "_Equipped",
            0
        );


        // -----------------------------------------------------
        // Clear shared equipped index if this is the
        // currently equipped passive.
        // -----------------------------------------------------

        int equippedIndex =
            GetEquippedPassiveIndex();


        if (equippedIndex == passiveIndex)
        {
            PlayerPrefs.SetInt(
                EquippedPassiveIndexKey,
                -1
            );
        }


        PlayerPrefs.Save();


        Debug.Log(
            "PASSIVE UNEQUIPPED AND SAVED: " +
            powerupName +
            " | Equipped Index: " +
            GetEquippedPassiveIndex()
        );


        UpdateUI();
    }


    // =========================================================
    // SAVE POWERUP
    // =========================================================

    private void SavePowerup()
    {
        string key =
            GetSaveKey();


        if (string.IsNullOrEmpty(key))
        {
            Debug.LogError(
                gameObject.name +
                " cannot save because there is no Save Key " +
                "or Powerup Name."
            );

            return;
        }


        if (string.IsNullOrEmpty(saveKey))
        {
            saveKey = key;
        }


        PlayerPrefs.SetInt(
            key + "_Unlocked",
            isUnlocked ? 1 : 0
        );


        PlayerPrefs.SetInt(
            key + "_Level",
            currentLevel
        );


        PlayerPrefs.SetInt(
            key + "_Equipped",
            isEquipped ? 1 : 0
        );


        if (isEquipped)
        {
            PlayerPrefs.SetInt(
                EquippedPassiveIndexKey,
                passiveIndex
            );
        }
        else
        {
            int equippedIndex =
                PlayerPrefs.GetInt(
                    EquippedPassiveIndexKey,
                    -1
                );


            if (equippedIndex == passiveIndex)
            {
                PlayerPrefs.SetInt(
                    EquippedPassiveIndexKey,
                    -1
                );
            }
        }


        PlayerPrefs.Save();


        Debug.Log(
            "PASSIVE SAVED: " +
            powerupName +
            " | Save Key: " +
            key +
            " | Unlocked: " +
            isUnlocked +
            " | Level: " +
            currentLevel +
            " | Equipped: " +
            isEquipped
        );
    }


    // =========================================================
    // UPDATE UI
    // =========================================================

    private void UpdateUI()
    {
        // -----------------------------------------------------
        // LOCKED
        // -----------------------------------------------------

        if (!isUnlocked)
        {
            if (unlockButton != null)
                unlockButton.SetActive(true);

            if (unlockButtonText != null)
                unlockButtonText.SetActive(true);

            if (unlockText != null)
                unlockText.SetActive(true);

            if (soulstoneImage1 != null)
                soulstoneImage1.SetActive(true);

            if (unlockPriceText != null)
                unlockPriceText.gameObject.SetActive(true);


            if (upgradeButton != null)
                upgradeButton.SetActive(false);

            if (upgradeButtonText != null)
                upgradeButtonText.SetActive(false);

            if (upgradeText != null)
                upgradeText.SetActive(false);

            if (soulstoneImage2 != null)
                soulstoneImage2.SetActive(false);

            if (upgradePriceText != null)
                upgradePriceText.gameObject.SetActive(false);


            if (upgradeInfoText != null)
                upgradeInfoText.SetActive(true);

            if (infoMaxedText != null)
                infoMaxedText.SetActive(false);


            SetEquipUI(false);
            SetLockedImages();

            UpdateValueText();
            UpdateUnlockPriceText();

            return;
        }


        // -----------------------------------------------------
        // UNLOCKED
        // -----------------------------------------------------

        if (unlockButton != null)
            unlockButton.SetActive(false);

        if (unlockButtonText != null)
            unlockButtonText.SetActive(false);

        if (unlockText != null)
            unlockText.SetActive(false);

        if (soulstoneImage1 != null)
            soulstoneImage1.SetActive(false);

        if (unlockPriceText != null)
            unlockPriceText.gameObject.SetActive(false);


        UpdateEquipUI();


        // -----------------------------------------------------
        // MAX LEVEL
        // -----------------------------------------------------

        if (currentLevel >= maxLevel)
        {
            if (upgradeButton != null)
                upgradeButton.SetActive(false);

            if (upgradeButtonText != null)
                upgradeButtonText.SetActive(false);

            if (upgradeText != null)
                upgradeText.SetActive(false);

            if (soulstoneImage2 != null)
                soulstoneImage2.SetActive(false);

            if (upgradePriceText != null)
                upgradePriceText.gameObject.SetActive(false);

            if (upgradeInfoText != null)
                upgradeInfoText.SetActive(false);

            if (infoMaxedText != null)
                infoMaxedText.SetActive(true);


            SetUnlockedImages();

            UpdateValueText();

            return;
        }


        // -----------------------------------------------------
        // CAN UPGRADE
        // -----------------------------------------------------

        if (upgradeButton != null)
            upgradeButton.SetActive(true);

        if (upgradeButtonText != null)
            upgradeButtonText.SetActive(true);

        if (upgradeText != null)
            upgradeText.SetActive(true);

        if (soulstoneImage2 != null)
            soulstoneImage2.SetActive(true);

        if (upgradePriceText != null)
            upgradePriceText.gameObject.SetActive(true);

        if (upgradeInfoText != null)
            upgradeInfoText.SetActive(true);

        if (infoMaxedText != null)
            infoMaxedText.SetActive(false);


        SetUnlockedImages();

        UpdateValueText();
        UpdateUpgradePriceText();
    }


    // =========================================================
    // UPDATE EQUIP UI
    // =========================================================

    private void UpdateEquipUI()
    {
        // Always use the saved equipped index.
        isEquipped =
            IsPassiveEquipped(passiveIndex);


        if (isEquipped)
        {
            SetEquipUI(false);


            if (unequipButton != null)
                unequipButton.SetActive(true);

            if (unequipButtonText != null)
                unequipButtonText.SetActive(true);
        }
        else
        {
            if (equipButton != null)
                equipButton.SetActive(true);

            if (equipButtonText != null)
                equipButtonText.SetActive(true);

            if (unequipButton != null)
                unequipButton.SetActive(false);

            if (unequipButtonText != null)
                unequipButtonText.SetActive(false);
        }
    }


    // =========================================================
    // SET EQUIP UI
    // =========================================================

    private void SetEquipUI(bool enabled)
    {
        if (equipButton != null)
            equipButton.SetActive(enabled);

        if (equipButtonText != null)
            equipButtonText.SetActive(enabled);

        if (unequipButton != null)
            unequipButton.SetActive(enabled);

        if (unequipButtonText != null)
            unequipButtonText.SetActive(enabled);
    }


    // =========================================================
    // GET CURRENT VALUE
    // =========================================================

    public float GetCurrentValue()
    {
        if (!isUnlocked)
            return lockedValue;


        if (currentLevel <= 1)
            return unlockValue;


        if (currentLevel >= maxLevel)
            return maxValue;


        float progress =
            (currentLevel - 1f) /
            (maxLevel - 1f);


        return Mathf.Lerp(
            unlockValue,
            maxValue,
            progress
        );
    }


    // =========================================================
    // RESET POWERUP
    // =========================================================

    public void ResetPowerup()
    {
        isUnlocked = false;
        isEquipped = false;
        currentLevel = 0;


        string key =
            GetSaveKey();


        if (!string.IsNullOrEmpty(key))
        {
            // -------------------------------------------------
            // Reset this powerup's saved state.
            // -------------------------------------------------

            PlayerPrefs.SetInt(
                key + "_Unlocked",
                0
            );


            PlayerPrefs.SetInt(
                key + "_Level",
                0
            );


            PlayerPrefs.SetInt(
                key + "_Equipped",
                0
            );


            // -------------------------------------------------
            // Clear shared equipped index if this passive
            // was equipped.
            // -------------------------------------------------

            int equippedIndex =
                PlayerPrefs.GetInt(
                    EquippedPassiveIndexKey,
                    -1
                );


            if (equippedIndex == passiveIndex)
            {
                PlayerPrefs.SetInt(
                    EquippedPassiveIndexKey,
                    -1
                );
            }


            PlayerPrefs.Save();


            Debug.Log(
                "POWERUP RESET: " +
                powerupName +
                " | Save Key: " +
                key
            );
        }


        UpdateUI();
    }


    // =========================================================
    // UPDATE VALUE TEXT
    // =========================================================

    private void UpdateValueText()
    {
        if (valueText != null)
        {
            if (!isUnlocked)
            {
                valueText.text =
                    lockedValue.ToString("0.0") +
                    "% -> <color=#06FF00>" +
                    unlockValue.ToString("0.0") +
                    "%</color>";
            }
            else if (currentLevel >= maxLevel)
            {
                valueText.text =
                    maxValue.ToString("0.0") +
                    "%";
            }
            else
            {
                float currentValue =
                    GetCurrentValue();


                int nextLevel =
                    currentLevel + 1;


                float nextValue;


                if (nextLevel >= maxLevel)
                {
                    nextValue = maxValue;
                }
                else
                {
                    float progress =
                        (nextLevel - 1f) /
                        (maxLevel - 1f);


                    nextValue =
                        Mathf.Lerp(
                            unlockValue,
                            maxValue,
                            progress
                        );
                }


                valueText.text =
                    currentValue.ToString("0.0") +
                    "% -> <color=#06FF00>" +
                    nextValue.ToString("0.0") +
                    "%</color>";
            }
        }


        if (levelText != null)
        {
            if (!isUnlocked)
            {
                levelText.text = "Not owned";
            }
            else
            {
                levelText.text =
                    "Level " +
                    currentLevel +
                    " / " +
                    maxLevel;
            }
        }
    }


    // =========================================================
    // UPDATE UNLOCK PRICE
    // =========================================================

    private void UpdateUnlockPriceText()
    {
        if (unlockPriceText == null)
            return;


        if (upgradePrices == null ||
            upgradePrices.Length == 0)
        {
            unlockPriceText.text = "";
            return;
        }


        unlockPriceText.text =
            upgradePrices[0].ToString();
    }


    // =========================================================
    // UPDATE UPGRADE PRICE
    // =========================================================

    private void UpdateUpgradePriceText()
    {
        if (upgradePriceText == null)
            return;


        if (!isUnlocked ||
            currentLevel >= maxLevel)
        {
            upgradePriceText.text = "";
            return;
        }


        if (upgradePrices == null)
        {
            upgradePriceText.text = "";
            return;
        }


        int priceIndex =
            currentLevel;


        if (priceIndex < 0 ||
            priceIndex >= upgradePrices.Length)
        {
            upgradePriceText.text = "";
            return;
        }


        upgradePriceText.text =
            upgradePrices[priceIndex].ToString();
    }


    // =========================================================
    // SET LOCKED IMAGES
    // =========================================================

    private void SetLockedImages()
    {
        if (image1 != null)
        {
            Image image =
                image1.GetComponent<Image>();


            if (image != null)
            {
                image.sprite =
                    image1Locked;

                image.enabled = true;
            }
        }


        if (image2 != null)
        {
            Image image =
                image2.GetComponent<Image>();


            if (image != null)
            {
                image.sprite =
                    image2Locked;

                image.enabled = true;
            }
        }
    }


    // =========================================================
    // SET UNLOCKED IMAGES
    // =========================================================

    private void SetUnlockedImages()
    {
        if (image1 != null)
        {
            Image image =
                image1.GetComponent<Image>();


            if (image != null)
            {
                image.sprite =
                    image1Unlocked;

                image.enabled = true;
            }
        }


        if (image2 != null)
        {
            Image image =
                image2.GetComponent<Image>();


            if (image != null)
            {
                image.sprite =
                    image2Unlocked;

                image.enabled = true;
            }
        }
    }


    // =========================================================
    // GET EQUIPPED PASSIVE INDEX
    // =========================================================

    public static int GetEquippedPassiveIndex()
    {
        return PlayerPrefs.GetInt(
            EquippedPassiveIndexKey,
            -1
        );
    }


    // =========================================================
    // CHECK IF PASSIVE IS EQUIPPED
    // =========================================================

    public static bool IsPassiveEquipped(
        int passiveIndex)
    {
        return GetEquippedPassiveIndex() ==
               passiveIndex;
    }


    // =========================================================
    // PUBLIC GETTERS
    // =========================================================

    public bool IsUnlocked()
    {
        return isUnlocked;
    }


    public bool IsEquipped()
    {
        return IsPassiveEquipped(
            passiveIndex
        );
    }


    public int GetCurrentLevel()
    {
        return currentLevel;
    }


    public int GetMaxLevel()
    {
        return maxLevel;
    }


    public float GetCurrentPowerupValue()
    {
        return GetCurrentValue();
    }


    public int GetPassiveIndex()
    {
        return passiveIndex;
    }
}