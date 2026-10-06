using UnityEngine;

public class AttackSpeedUpgrade : MonoBehaviour
{
    [Header("Upgrade Costs")]
    public int level1Cost = 1;
    public int level2Cost = 2;
    public int level3Cost = 3;
    public int level4Cost = 4;
    public int level5Cost = 5;
    public int level6Cost = 6;
    public int level7Cost = 7;
    public int level8Cost = 8;
    public int level9Cost = 9;
    public int level10Cost = 10;

    [Header("Upgrade Settings")]
    public int maxLevel = 10;

    [Header("UI")]
    public AttackSpeedUpgradeUI upgradeUI;

    private const string AttackSpeedLevelKey = "AttackSpeedUpgradeLevel";
    private const string SoulstonesKey = "Soulstones";

    public int CurrentLevel
    {
        get
        {
            return PlayerPrefs.GetInt(AttackSpeedLevelKey, 0);
        }
    }

    private void Start()
    {
        // Make sure the saved level is valid.
        int level = Mathf.Clamp(CurrentLevel, 0, maxLevel);

        PlayerPrefs.SetInt(AttackSpeedLevelKey, level);
        PlayerPrefs.Save();

        if (upgradeUI != null)
        {
            upgradeUI.RefreshUI();
        }
    }

    public float GetCurrentAttackTime()
    {
        float attackTime =
            0.8f -
            (CurrentLevel * 0.05f);

        return Mathf.Clamp(
            attackTime,
            0.3f,
            0.8f
        );
    }

    public float GetNextAttackTime()
    {
        if (CurrentLevel >= maxLevel)
        {
            return GetCurrentAttackTime();
        }

        float nextAttackTime =
            0.8f -
            ((CurrentLevel + 1) * 0.05f);

        return Mathf.Clamp(
            nextAttackTime,
            0.3f,
            0.8f
        );
    }

    public int GetCurrentUpgradeCost()
    {
        switch (CurrentLevel)
        {
            case 0:
                return level1Cost;

            case 1:
                return level2Cost;

            case 2:
                return level3Cost;

            case 3:
                return level4Cost;

            case 4:
                return level5Cost;

            case 5:
                return level6Cost;

            case 6:
                return level7Cost;

            case 7:
                return level8Cost;

            case 8:
                return level9Cost;

            case 9:
                return level10Cost;

            default:
                return 0;
        }
    }

    public void BuyUpgrade()
    {
        if (CurrentLevel >= maxLevel)
        {
            Debug.Log("Attack Speed is already MAX.");
            return;
        }

        int cost = GetCurrentUpgradeCost();

        if (Soulstones.Instance == null)
        {
            Debug.LogWarning(
                "Soulstones.Instance is null. Cannot purchase Attack Speed upgrade."
            );

            return;
        }

        if (!Soulstones.Instance.SpendSoulstones(cost))
        {
            Debug.Log(
                "Not enough Soulstones for Attack Speed upgrade."
            );

            return;
        }

        int newLevel = CurrentLevel + 1;

        PlayerPrefs.SetInt(
            AttackSpeedLevelKey,
            newLevel
        );

        PlayerPrefs.Save();

        // ============================================================
        // ACHIEVEMENT: TOTAL UPGRADES
        // ============================================================

        AchievementStats.AddUpgradesPurchased(1);

        Debug.Log(
            "Attack Speed upgraded to Level " +
            newLevel +
            " | Attack Time: " +
            GetCurrentAttackTime().ToString("0.00") +
            " sec" +
            " | Soulstones Remaining: " +
            Soulstones.Instance.SoulstoneCount
        );

        if (upgradeUI != null)
        {
            upgradeUI.RefreshUI();
        }
    }
}