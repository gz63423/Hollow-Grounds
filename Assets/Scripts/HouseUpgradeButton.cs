using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HouseUpgradeButton : MonoBehaviour
{
    // =========================================================
    // UI
    // =========================================================

    [Header("UI")]
    public TextMeshProUGUI priceText;
    public TextMeshProUGUI currentHpText;

    public Image currentHouseImage;
    public Image nextHouseImage;


    // =========================================================
    // MAX LEVEL 50 REWARDS
    // =========================================================

    [Header("Max Level 50 Rewards")]
    public TextMeshProUGUI removeTMPText;
    public Image removeImage;
    public TextMeshProUGUI enableTMPText;


    // =========================================================
    // DEMO MAX TEXT
    // =========================================================

    [Header("Demo Max Text")]
    [Tooltip("Text shown when Demo mode reaches House Level 20.")]
    public TextMeshProUGUI demoMaxText1;

    [Tooltip("Text shown when Demo mode reaches House Level 20.")]
    public TextMeshProUGUI demoMaxText2;


    // =========================================================
    // DEMO MAX UI TOGGLES
    // =========================================================

    [Header("Demo Max UI")]
    [Tooltip("Remove the Remove TMP Text when Demo Level 20 is reached.")]
    public bool removeRemoveTMPText = false;

    [Tooltip("Remove the Remove Image when Demo Level 20 is reached.")]
    public bool removeRemoveImage = false;

    [Tooltip("Remove the Next House Image when Demo Level 20 is reached.")]
    public bool removeNextHouseImage = false;

    [Tooltip("Remove the Enable TMP Text when Demo Level 20 is reached.")]
    public bool removeEnableTMPText = false;

    [Tooltip("Remove Demo Max Text 1 when Demo Level 20 is reached.")]
    public bool removeDemoMaxText1 = false;

    [Tooltip("Remove Demo Max Text 2 when Demo Level 20 is reached.")]
    public bool removeDemoMaxText2 = false;


    // =========================================================
    // UPGRADE BAR NUMBERS
    // =========================================================

    [Header("Upgrade Bar Numbers")]
    public TextMeshProUGUI[] upgradeNumberTexts =
        new TextMeshProUGUI[10];


    // =========================================================
    // HOUSE UPGRADE SPRITES
    // =========================================================

    [Header("House Upgrade Sprites")]
    public Sprite[] houseLevelSprites =
        new Sprite[5];


    // =========================================================
    // HOUSE UPGRADE XP REWARDS
    // =========================================================

    [Header("House Upgrade XP Rewards")]
    public int level1To10XP = 10;
    public int level11To20XP = 25;
    public int level21To30XP = 50;
    public int level31To40XP = 100;
    public int level41To50XP = 250;


    // =========================================================
    // DEMO
    // =========================================================

    [Header("Demo")]
    [Tooltip("Demo mode limits the house to displayed Level 20.")]
    public bool demo = false;


    // =========================================================
    // INTERNAL
    // =========================================================

    [Header("Internal")]
    public int currentLevel;


    // =========================================================
    // CONSTANTS
    // =========================================================

    // Internal level 49 = displayed Level 50.
    private const int NORMAL_MAX_INTERNAL_LEVEL = 49;

    // Internal level 19 = displayed Level 20.
    private const int DEMO_MAX_INTERNAL_LEVEL = 19;

    // Floating-point money tolerance.
    private const float MONEY_TOLERANCE = 0.005f;


    // =========================================================
    // START
    // =========================================================

    private void Start()
    {
        currentLevel =
            PlayerPrefs.GetInt(
                "HouseLevel",
                0
            );


        currentLevel =
            Mathf.Clamp(
                currentLevel,
                0,
                NORMAL_MAX_INTERNAL_LEVEL
            );


        if (demo)
        {
            currentLevel =
                Mathf.Min(
                    currentLevel,
                    DEMO_MAX_INTERNAL_LEVEL
                );
        }


        UpdateUI();
        UpdateUpgradeBars();
        UpdateUpgradeNumbers();
        CheckMaxLevel();
    }


    // =========================================================
    // MAX LEVEL HELPERS
    // =========================================================

    private int GetMaxInternalLevel()
    {
        return demo
            ? DEMO_MAX_INTERNAL_LEVEL
            : NORMAL_MAX_INTERNAL_LEVEL;
    }


    private bool IsMaxed()
    {
        return currentLevel >= GetMaxInternalLevel();
    }


    // =========================================================
    // UPGRADE HOUSE
    // =========================================================

    public void UpgradeHouse()
    {
        if (HouseUpgradeData.Instance == null)
        {
            Debug.LogError(
                "HouseUpgradeData.Instance was not found!"
            );

            return;
        }


        if (IsMaxed())
        {
            Debug.Log(
                "House is already at MAX LEVEL!"
            );

            return;
        }


        float price =
            HouseUpgradeData.Instance.GetPrice(
                currentLevel
            );


        bool canPay = false;


        // =====================================================
        // GAMEPLAY MONEY
        // =====================================================

        if (Player.Instance != null)
        {
            float money =
                Player.Instance.Money;


            if (money >= price - MONEY_TOLERANCE)
            {
                // If the player is only short by the tolerance,
                // spend their exact remaining money instead of
                // allowing a tiny negative floating-point balance.
                float amountToSpend =
                    Mathf.Min(
                        price,
                        Mathf.Max(0f, money)
                    );


                Player.Instance.AddMoney(
                    -amountToSpend
                );


                canPay = true;
            }
        }


        // =====================================================
        // PLAYERPREFS MONEY
        // =====================================================

        else
        {
            float money =
                PlayerPrefs.GetFloat(
                    "PlayerMoney",
                    0f
                );


            if (money >= price - MONEY_TOLERANCE)
            {
                money -= price;

                // Prevent tiny negative floating-point balances.
                if (money < 0f)
                {
                    money = 0f;
                }


                PlayerPrefs.SetFloat(
                    "PlayerMoney",
                    money
                );


                PlayerPrefs.Save();


                canPay = true;
            }
        }


        // =====================================================
        // NOT ENOUGH MONEY
        // =====================================================

        if (!canPay)
        {
            Debug.Log(
                "Not enough money!"
            );

            return;
        }


        // =====================================================
        // UPGRADE
        // =====================================================

        currentLevel++;


        PlayerPrefs.SetInt(
            "HouseLevel",
            currentLevel
        );


        PlayerPrefs.Save();


        // =====================================================
        // REFRESH ACTUAL HOUSE
        // =====================================================

        RefreshActualHouse();


        // =====================================================
        // ACHIEVEMENTS
        // =====================================================

        AchievementStats.AddUpgradesPurchased(1);
        AchievementStats.UpdateHighestHouseLevel();


        // =====================================================
        // SOUND
        // =====================================================

        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlayUpgradePurchased();
        }


        // =====================================================
        // XP
        // =====================================================

        GiveHouseXP();


        // =====================================================
        // REFRESH UI
        // =====================================================

        UpdateUI();
        UpdateUpgradeBars();
        UpdateUpgradeNumbers();
        CheckMaxLevel();


        Debug.Log(
            "House upgraded to Level " +
            (currentLevel + 1)
        );
    }


    // =========================================================
    // REFRESH ACTUAL HOUSE
    // =========================================================

    private void RefreshActualHouse()
    {
        House house =
            FindFirstObjectByType<House>();


        if (house != null)
        {
            house.RefreshHouseLevel();
        }
        else
        {
            Debug.LogWarning(
                "No House component found in the scene while upgrading the house."
            );
        }
    }


    // =========================================================
    // GIVE HOUSE XP
    // =========================================================

    private void GiveHouseXP()
    {
        int xpAmount;


        if (currentLevel <= 10)
        {
            xpAmount = level1To10XP;
        }
        else if (currentLevel <= 20)
        {
            xpAmount = level11To20XP;
        }
        else if (currentLevel <= 30)
        {
            xpAmount = level21To30XP;
        }
        else if (currentLevel <= 40)
        {
            xpAmount = level31To40XP;
        }
        else
        {
            xpAmount = level41To50XP;
        }


        if (AccountLevel.Instance != null)
        {
            AccountLevel.Instance.AddXP(
                xpAmount
            );


            Debug.Log(
                "House XP Added: " +
                xpAmount
            );
        }
        else
        {
            Debug.LogWarning(
                "AccountLevel not found!"
            );
        }
    }


    // =========================================================
    // CHECK MAX LEVEL
    // =========================================================

    private void CheckMaxLevel()
    {
        bool demoMaxed =
            demo &&
            currentLevel >= DEMO_MAX_INTERNAL_LEVEL;


        if (demoMaxText1 != null)
        {
            demoMaxText1.gameObject.SetActive(
                demoMaxed &&
                !removeDemoMaxText1
            );
        }


        if (demoMaxText2 != null)
        {
            demoMaxText2.gameObject.SetActive(
                demoMaxed &&
                !removeDemoMaxText2
            );
        }


        if (demoMaxed)
        {
            if (removeTMPText != null)
            {
                removeTMPText.gameObject.SetActive(
                    !removeRemoveTMPText
                );
            }


            if (removeImage != null)
            {
                removeImage.enabled =
                    !removeRemoveImage;
            }


            if (nextHouseImage != null)
            {
                nextHouseImage.enabled =
                    !removeNextHouseImage;
            }


            if (enableTMPText != null)
            {
                enableTMPText.gameObject.SetActive(
                    !removeEnableTMPText
                );
            }


            return;
        }


        if (
            currentLevel >= 40 &&
            currentLevel < 49
        )
        {
            if (removeTMPText != null)
            {
                removeTMPText.gameObject.SetActive(false);
            }


            if (removeImage != null)
            {
                removeImage.enabled = false;
            }


            if (nextHouseImage != null)
            {
                nextHouseImage.gameObject.SetActive(false);
            }


            if (enableTMPText != null)
            {
                enableTMPText.gameObject.SetActive(false);
            }
        }

        else if (currentLevel >= 49)
        {
            if (removeTMPText != null)
            {
                removeTMPText.gameObject.SetActive(false);
            }


            if (removeImage != null)
            {
                removeImage.enabled = false;
            }


            if (nextHouseImage != null)
            {
                nextHouseImage.gameObject.SetActive(false);
            }


            if (enableTMPText != null)
            {
                enableTMPText.gameObject.SetActive(true);
            }
        }

        else
        {
            if (removeTMPText != null)
            {
                removeTMPText.gameObject.SetActive(true);
            }


            if (removeImage != null)
            {
                removeImage.enabled = true;
            }


            if (nextHouseImage != null)
            {
                nextHouseImage.gameObject.SetActive(true);
            }


            if (enableTMPText != null)
            {
                enableTMPText.gameObject.SetActive(false);
            }
        }
    }


    // =========================================================
    // UPDATE UI
    // =========================================================

    public void UpdateUI()
    {
        if (HouseUpgradeData.Instance == null)
        {
            Debug.LogError(
                "HouseUpgradeData.Instance was not found!"
            );

            return;
        }


        int level =
            Mathf.Clamp(
                currentLevel,
                0,
                NORMAL_MAX_INTERNAL_LEVEL
            );


        if (IsMaxed())
        {
            if (priceText != null)
            {
                priceText.text =
                    "MAX LEVEL";
            }
        }
        else
        {
            float price =
                HouseUpgradeData.Instance.GetPrice(
                    level
                );


            if (priceText != null)
            {
                priceText.text =
                    "Upgrade house: " +
                    MoneyFormatter.FormatMoney(
                        price
                    );
            }
        }


        int hp =
            HouseUpgradeData.Instance.GetHealth(
                level
            );


        if (currentHpText != null)
        {
            currentHpText.text =
                $"Hitpoints: {hp}";
        }


        if (
            currentHouseImage != null &&
            houseLevelSprites != null &&
            houseLevelSprites.Length > 0
        )
        {
            int currentSprite =
                Mathf.Clamp(
                    level / 10,
                    0,
                    houseLevelSprites.Length - 1
                );


            currentHouseImage.sprite =
                houseLevelSprites[currentSprite];
        }


        if (nextHouseImage != null)
        {
            if (
                currentLevel >=
                NORMAL_MAX_INTERNAL_LEVEL
            )
            {
                nextHouseImage.enabled = false;
            }
            else
            {
                if (
                    houseLevelSprites != null &&
                    houseLevelSprites.Length > 0
                )
                {
                    int nextSprite =
                        Mathf.Clamp(
                            (level + 1) / 10,
                            0,
                            houseLevelSprites.Length - 1
                        );


                    nextHouseImage.sprite =
                        houseLevelSprites[nextSprite];
                }


                nextHouseImage.enabled = true;
            }
        }
    }


    // =========================================================
    // UPDATE UPGRADE BARS
    // =========================================================

    private void UpdateUpgradeBars()
    {
        HouseUpgradeBars bars =
            FindFirstObjectByType<HouseUpgradeBars>();


        if (bars != null)
        {
            bars.UpdateBars();
        }
        else
        {
            Debug.LogWarning(
                "No HouseUpgradeBars found in scene!"
            );
        }
    }


    // =========================================================
    // UPDATE UPGRADE NUMBERS
    // =========================================================

    private void UpdateUpgradeNumbers()
    {
        int tier =
            currentLevel / 10;


        int startNumber =
            (tier * 10) + 1;


        for (
            int i = 0;
            i < upgradeNumberTexts.Length;
            i++
        )
        {
            if (upgradeNumberTexts[i] != null)
            {
                upgradeNumberTexts[i].text =
                    (startNumber + i).ToString();
            }
        }
    }


    // =========================================================
    // GET CURRENT HOUSE HP
    // =========================================================

    public int GetCurrentHouseHP()
    {
        if (HouseUpgradeData.Instance == null)
        {
            return 10;
        }


        int level =
            Mathf.Clamp(
                currentLevel,
                0,
                NORMAL_MAX_INTERNAL_LEVEL
            );


        return HouseUpgradeData.Instance.GetHealth(
            level
        );
    }
}