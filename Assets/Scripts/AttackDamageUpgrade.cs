using UnityEngine;

public class AttackDamageUpgrade : MonoBehaviour
{
    [Header("Upgrade Costs - Levels 1 to 99")]
    [SerializeField]
    private int[] upgradeCosts =
    {
        2, 8, 25, 50, 85, 140, 200, 275, 360, 460,
        575, 700, 850, 1025, 1225, 1450, 1700, 2000, 2350, 2750,
        3200, 3700, 4250, 4850, 5500, 6200, 7000, 7900, 8900, 10000,
        11200, 12500, 14000, 15600, 17400, 19300, 21400, 23700, 26200, 29000,
        32000, 35300, 38900, 42900, 47300, 52100, 57400, 63200, 69600, 76600,
        84300, 92800, 102000, 112000, 123000, 135000, 148000, 162000, 178000, 196000,
        216000, 238000, 262000, 288000, 317000, 349000, 384000, 422000, 464000, 510000,
        561000, 617000, 679000, 747000, 822000, 904000, 994000, 1090000, 1200000, 1320000,
        1450000, 1600000, 1760000, 1940000, 2130000, 2350000, 2590000, 2850000, 3140000, 3460000,
        3810000, 4200000, 4620000, 5080000, 5590000, 6150000, 6770000, 7450000, 8200000
    };

    [Header("Upgrade Settings")]
    public int maxLevel = 99;

    [Header("UI")]
    public AttackDamageUpgradeUI upgradeUI;

    private const string AttackDamageLevelKey = "AttackDamageUpgradeLevel";

    public int CurrentLevel
    {
        get
        {
            return PlayerPrefs.GetInt(
                AttackDamageLevelKey,
                0
            );
        }
    }

    private void Start()
    {
        maxLevel = Mathf.Clamp(
            maxLevel,
            1,
            upgradeCosts.Length
        );


        int level = Mathf.Clamp(
            CurrentLevel,
            0,
            maxLevel
        );

        PlayerPrefs.SetInt(
            AttackDamageLevelKey,
            level
        );

        PlayerPrefs.Save();

        if (upgradeUI != null)
        {
            upgradeUI.RefreshUI();
        }
    }



    public float GetCurrentDamage()
    {
        float damage =
            1f +
            CurrentLevel;

        return damage;
    }



    public float GetNextDamage()
    {
        if (CurrentLevel >= maxLevel)
        {
            return GetCurrentDamage();
        }

        float nextDamage =
            1f +
            (CurrentLevel + 1);

        return nextDamage;
    }


    public int GetCurrentUpgradeCost()
    {
        if (CurrentLevel >= maxLevel)
        {
            return 0;
        }

        if (CurrentLevel < 0 ||
            CurrentLevel >= upgradeCosts.Length)
        {
            return 0;
        }

        return upgradeCosts[CurrentLevel];
    }


    //Buy


    public void BuyUpgrade()
    {
        if (CurrentLevel >= maxLevel)
        {
            Debug.Log(
                "Attack Damage is already MAX."
            );

            return;
        }

        int cost = GetCurrentUpgradeCost();

        if (cost <= 0)
        {
            Debug.LogWarning(
                "Attack Damage upgrade has an invalid cost."
            );

            return;
        }

        if (Soulstones.Instance == null)
        {
            Debug.LogWarning(
                "Soulstones.Instance is null. " +
                "Cannot purchase Attack Damage upgrade."
            );

            return;
        }

        if (!Soulstones.Instance.SpendSoulstones(cost))
        {
            Debug.Log(
                "Not enough Soulstones for Attack Damage upgrade."
            );

            return;
        }

        int newLevel =
            CurrentLevel + 1;

        PlayerPrefs.SetInt(
            AttackDamageLevelKey,
            newLevel
        );

        PlayerPrefs.Save();

        //Stats

        AchievementStats.AddUpgradesPurchased(1);

        Debug.Log(
            "Attack Damage upgraded to Level " +
            newLevel +
            " | Damage: " +
            GetCurrentDamage().ToString("0") +
            " | Cost: " +
            cost +
            " Soulstones" +
            " | Soulstones Remaining: " +
            Soulstones.Instance.SoulstoneCount
        );

        if (upgradeUI != null)
        {
            upgradeUI.RefreshUI();
        }
    }
}
