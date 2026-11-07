using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LeggingsBuyButton : MonoBehaviour
{
    [Header("Demo")]
    [Tooltip("Demo mode limits the leggings to Tier 2 (Leather/Bone).")]
    public bool demo = false;


    [Header("Tier Prices")]
    public float tier1Price = 10f;
    public float tier2Price = 25f;
    public float tier3Price = 50f;
    public float tier4Price = 100f;
    public float tier5Price = 200f;


    [Header("Tier HP Bonuses")]
    public float tier1HP = 10f;
    public float tier2HP = 20f;
    public float tier3HP = 40f;
    public float tier4HP = 80f;
    public float tier5HP = 150f;


    [Header("Leggings Images")]
    public Image currentLeggingsImage;


    public Sprite[] leggingsTierSprites = new Sprite[6];


    [Header("UI")]
    public TextMeshProUGUI priceText;
    public TextMeshProUGUI hpBoostText;


    [Header("XP Rewards")]
    public int tier1XP = 10;
    public int tier2XP = 20;
    public int tier3XP = 40;
    public int tier4XP = 80;
    public int tier5XP = 150;


    private const float MONEY_TOLERANCE = 0.005f;

    private int currentTier;
    private LeggingsUpgradeBars leggingsBars;


    void Start()
    {
        currentTier =
            PlayerPrefs.GetInt(
                "LeggingsTier",
                0
            );


        if (demo)
        {
            currentTier = Mathf.Min(currentTier, 2);
        }


        leggingsBars =
            FindFirstObjectByType<LeggingsUpgradeBars>();


        UpdateUI();
        UpdateLeggingsImage();


        if (leggingsBars != null)
        {
            leggingsBars.UpdateBars();
        }
    }


    public void BuyLeggings()
    {
        if (IsMaxed())
        {
            Debug.Log("Leggings already max level!");
            return;
        }


        float price =
            GetPrice(currentTier);


        bool canPay = false;


        if (Player.Instance != null)
        {
            float money =
                Player.Instance.Money;


            if (money >= price - MONEY_TOLERANCE)
            {
                float amountToSpend =
                    Mathf.Min(
                        price,
                        Mathf.Max(0f, money)
                    );


                canPay =
                    Player.Instance.TrySpendMoney(
                        amountToSpend
                    );
            }
        }



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


                if (money < 0f)
                {
                    money = 0f;
                }


                PlayerPrefs.SetFloat(
                    "PlayerMoney",
                    money
                );


                canPay = true;
            }
        }


        if (!canPay)
        {
            Debug.Log("Not enough money!");
            return;
        }


        currentTier++;


        PlayerPrefs.SetInt(
            "LeggingsTier",
            currentTier
        );


        PlayerPrefs.SetFloat(
            "LeggingsHPBonus",
            GetHP(currentTier)
        );


        PlayerPrefs.Save();


        AchievementStats.AddUpgradesPurchased(1);
        AchievementStats.UpdateHighestArmorTier();


        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlayUpgradePurchased();
        }


        GiveArmorXP();


        ArmorRarityDisplay display =
            FindFirstObjectByType<ArmorRarityDisplay>();


        if (display != null)
        {
            display.UpdateRarityDisplay();
        }


        if (Player.Instance != null)
        {
            Player.Instance.LoadArmorStats();
        }


        LeggingsAnimationsManager manager =
            FindFirstObjectByType<LeggingsAnimationsManager>();


        if (manager != null)
        {
            manager.ResetAnimator();
        }


        UpdateUI();
        UpdateLeggingsImage();


        if (leggingsBars != null)
        {
            leggingsBars.UpdateBars();
        }


        Debug.Log(
            "Leggings upgraded to Tier " +
            currentTier +
            " +" +
            GetHP(currentTier).ToString("0.##") +
            " HP"
        );
    }


    bool IsMaxed()
    {
        return demo
            ? currentTier >= 2
            : currentTier >= 5;
    }


    float GetPrice(int tier)
    {
        return tier switch
        {
            0 => tier1Price,
            1 => tier2Price,
            2 => tier3Price,
            3 => tier4Price,
            4 => tier5Price,
            _ => 0f
        };
    }


    float GetHP(int tier)
    {
        return tier switch
        {
            1 => tier1HP,
            2 => tier2HP,
            3 => tier3HP,
            4 => tier4HP,
            5 => tier5HP,
            _ => 0f
        };
    }


    public void UpdateUI()
    {
        if (priceText != null)
        {
            if (IsMaxed())
            {
                priceText.text = "MAX LEVEL";
            }
            else
            {
                priceText.text =
                    "Upgrade Leggings: " +
                    MoneyFormatter.FormatMoney(
                        GetPrice(currentTier)
                    );
            }
        }


        if (hpBoostText != null)
        {
            hpBoostText.text =
                "+" +
                GetHP(currentTier).ToString("0.##") +
                " HP";
        }
    }


    void GiveArmorXP()
    {
        int xpAmount = 0;


        switch (currentTier)
        {
            case 1:
                xpAmount = tier1XP;
                break;

            case 2:
                xpAmount = tier2XP;
                break;

            case 3:
                xpAmount = tier3XP;
                break;

            case 4:
                xpAmount = tier4XP;
                break;

            case 5:
                xpAmount = tier5XP;
                break;
        }


        if (AccountLevel.Instance != null)
        {
            AccountLevel.Instance.AddXP(xpAmount);

            Debug.Log(
                "Armor XP Added: " +
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


    public void UpdateLeggingsImage()
    {
        if (currentLeggingsImage == null)
            return;


        int spriteIndex =
            Mathf.Clamp(
                currentTier,
                0,
                leggingsTierSprites.Length - 1
            );


        currentLeggingsImage.enabled = true;


        currentLeggingsImage.sprite =
            leggingsTierSprites[spriteIndex];
    }
}