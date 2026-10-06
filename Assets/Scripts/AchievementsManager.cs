using System.Collections.Generic;
using UnityEngine;

public class AchievementManager : MonoBehaviour
{
    public static AchievementManager Instance { get; private set; }

    // ============================================================
    // ACHIEVEMENT DATA
    // ============================================================

    [System.Serializable]
    public class Achievement
    {
        public string id;
        public string title;
        public string description;
        public float requirement;
        public int soulstoneReward;

        // Used by AchievementCategoryStars.cs
        public bool claimed;

        public Achievement(
            string id,
            string title,
            string description,
            float requirement,
            int soulstoneReward
        )
        {
            this.id = id;
            this.title = title;
            this.description = description;
            this.requirement = requirement;
            this.soulstoneReward = soulstoneReward;
            this.claimed = false;
        }
    }

    private readonly List<Achievement> achievements =
        new List<Achievement>();

    private readonly Dictionary<string, bool> claimed =
        new Dictionary<string, bool>();

    // ============================================================
    // UNITY
    // ============================================================

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;

            DontDestroyOnLoad(gameObject);

            CreateAchievements();
            LoadClaimedStates();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Check every achievement against the player's
        // currently saved lifetime statistics.
        //
        // This is especially important for future updates.
        // If a new achievement is added and the player has
        // already exceeded its requirement, it will immediately
        // be recognized as completed when the game starts.
        UpdateAchievementStates();
    }

    // ============================================================
    // CREATE ACHIEVEMENTS
    // ============================================================

    private void CreateAchievements()
    {
        achievements.Clear();

        // ========================================================
        // BLOODSHED
        // ========================================================

        achievements.Add(
            new Achievement(
                "Bloodshed_I",
                "Bloodshed I",
                "Kill 100 enemies.",
                1f,
                1000
            )
        );

        achievements.Add(
            new Achievement(
                "Bloodshed_II",
                "Bloodshed II",
                "Kill 500 enemies.",
                500f,
                15
            )
        );

        achievements.Add(
            new Achievement(
                "Bloodshed_III",
                "Bloodshed III",
                "Kill 2,500 enemies.",
                2500f,
                75
            )
        );

        achievements.Add(
            new Achievement(
                "Bloodshed_IV",
                "Bloodshed IV",
                "Kill 5,000 enemies.",
                5000f,
                200
            )
        );

        achievements.Add(
            new Achievement(
                "Bloodshed_V",
                "Bloodshed V",
                "Kill 10,000 enemies.",
                10000f,
                500
            )
        );

        // ========================================================
        // DEFIANCE
        // ========================================================

        achievements.Add(
            new Achievement(
                "Defiance_I",
                "Defiance I",
                "Complete Wave 3.",
                3f,
                1
            )
        );

        achievements.Add(
            new Achievement(
                "Defiance_II",
                "Defiance II",
                "Complete Wave 6.",
                6f,
                7
            )
        );

        achievements.Add(
            new Achievement(
                "Defiance_III",
                "Defiance III",
                "Complete Wave 10.",
                10f,
                25
            )
        );

        achievements.Add(
            new Achievement(
                "Defiance_IV",
                "Defiance IV",
                "Complete Wave 15.",
                15f,
                50
            )
        );

        achievements.Add(
            new Achievement(
                "Defiance_V",
                "Defiance V",
                "Complete Wave 20.",
                20f,
                100
            )
        );

        // ========================================================
        // FORTIFIED
        // ========================================================

        achievements.Add(
            new Achievement(
                "Fortified_I",
                "Fortified I",
                "Reach House Level 10.",
                10f,
                1
            )
        );

        achievements.Add(
            new Achievement(
                "Fortified_II",
                "Fortified II",
                "Reach House Level 20.",
                20f,
                7
            )
        );

        achievements.Add(
            new Achievement(
                "Fortified_III",
                "Fortified III",
                "Reach House Level 30.",
                30f,
                25
            )
        );

        achievements.Add(
            new Achievement(
                "Fortified_IV",
                "Fortified IV",
                "Reach House Level 40.",
                40f,
                50
            )
        );

        achievements.Add(
            new Achievement(
                "Fortified_V",
                "Fortified V",
                "Reach House Level 50.",
                50f,
                100
            )
        );

        // ========================================================
        // ARMORED
        // ========================================================

        achievements.Add(
            new Achievement(
                "Armored_I",
                "Armored I",
                "Max Leather Armor.",
                1f,
                1
            )
        );

        achievements.Add(
            new Achievement(
                "Armored_II",
                "Armored II",
                "Max Bone Armor.",
                2f,
                7
            )
        );

        achievements.Add(
            new Achievement(
                "Armored_III",
                "Armored III",
                "Max Copper Armor.",
                3f,
                25
            )
        );

        achievements.Add(
            new Achievement(
                "Armored_IV",
                "Armored IV",
                "Max Bronze Armor.",
                4f,
                50
            )
        );

        achievements.Add(
            new Achievement(
                "Armored_V",
                "Armored V",
                "Max Iron Armor.",
                5f,
                100
            )
        );

        // ========================================================
        // FORTUNE
        // ========================================================

        achievements.Add(
            new Achievement(
                "Fortune_I",
                "Fortune I",
                "Earn $10.",
                10f,
                2
            )
        );

        achievements.Add(
            new Achievement(
                "Fortune_II",
                "Fortune II",
                "Earn $250.",
                250f,
                15
            )
        );

        achievements.Add(
            new Achievement(
                "Fortune_III",
                "Fortune III",
                "Earn $5,000.",
                5000f,
                75
            )
        );

        achievements.Add(
            new Achievement(
                "Fortune_IV",
                "Fortune IV",
                "Earn $100,000.",
                100000f,
                200
            )
        );

        achievements.Add(
            new Achievement(
                "Fortune_V",
                "Fortune V",
                "Earn $1,000,000.",
                1000000f,
                1000
            )
        );

        // ========================================================
        // SOULBOUND
        // ========================================================

        achievements.Add(
            new Achievement(
                "Soulbound_I",
                "Soulbound I",
                "Obtain 10 Soulstones.",
                10f,
                1
            )
        );

        achievements.Add(
            new Achievement(
                "Soulbound_II",
                "Soulbound II",
                "Obtain 50 Soulstones.",
                50f,
                4
            )
        );

        achievements.Add(
            new Achievement(
                "Soulbound_III",
                "Soulbound III",
                "Obtain 250 Soulstones.",
                250f,
                10
            )
        );

        achievements.Add(
            new Achievement(
                "Soulbound_IV",
                "Soulbound IV",
                "Obtain 1,000 Soulstones.",
                1000f,
                50
            )
        );

        achievements.Add(
            new Achievement(
                "Soulbound_V",
                "Soulbound V",
                "Obtain 2,500 Soulstones.",
                2500f,
                200
            )
        );

        // ========================================================
        // ENHANCED
        // ========================================================

        achievements.Add(
            new Achievement(
                "Enhanced_I",
                "Enhanced I",
                "Purchase 25 upgrades.",
                25f,
                2
            )
        );

        achievements.Add(
            new Achievement(
                "Enhanced_II",
                "Enhanced II",
                "Purchase 100 upgrades.",
                100f,
                10
            )
        );

        achievements.Add(
            new Achievement(
                "Enhanced_III",
                "Enhanced III",
                "Purchase 200 upgrades.",
                200f,
                25
            )
        );

        achievements.Add(
            new Achievement(
                "Enhanced_IV",
                "Enhanced IV",
                "Purchase 350 upgrades.",
                350f,
                50
            )
        );

        achievements.Add(
            new Achievement(
                "Enhanced_V",
                "Enhanced V",
                "Purchase 500 upgrades.",
                500f,
                100
            )
        );

        // ========================================================
        // BRUTALITY
        // ========================================================

        achievements.Add(
            new Achievement(
                "Brutality_I",
                "Brutality I",
                "Deal 1,000 damage.",
                1000f,
                5
            )
        );

        achievements.Add(
            new Achievement(
                "Brutality_II",
                "Brutality II",
                "Deal 5,000 damage.",
                5000f,
                40
            )
        );

        achievements.Add(
            new Achievement(
                "Brutality_III",
                "Brutality III",
                "Deal 10,000 damage.",
                10000f,
                100
            )
        );

        achievements.Add(
            new Achievement(
                "Brutality_IV",
                "Brutality IV",
                "Deal 25,000 damage.",
                25000f,
                500
            )
        );

        achievements.Add(
            new Achievement(
                "Brutality_V",
                "Brutality V",
                "Deal 100,000 damage.",
                100000f,
                2000
            )
        );

        // ========================================================
        // REBORN
        // ========================================================

        achievements.Add(
            new Achievement(
                "Reborn_I",
                "Reborn I",
                "Rebirth 1 time.",
                1f,
                5
            )
        );

        achievements.Add(
            new Achievement(
                "Reborn_II",
                "Reborn II",
                "Rebirth 2 times.",
                2f,
                25
            )
        );

        achievements.Add(
            new Achievement(
                "Reborn_III",
                "Reborn III",
                "Rebirth 3 times.",
                3f,
                100
            )
        );

        achievements.Add(
            new Achievement(
                "Reborn_IV",
                "Reborn IV",
                "Rebirth 4 times.",
                4f,
                250
            )
        );

        achievements.Add(
            new Achievement(
                "Reborn_V",
                "Reborn V",
                "Rebirth 5 times.",
                5f,
                500
            )
        );

        // ========================================================
        // EXECUTIONER
        // ========================================================

        achievements.Add(
            new Achievement(
                "Executioner_I",
                "Executioner I",
                "Kill 1 boss.",
                1f,
                100
            )
        );

        achievements.Add(
            new Achievement(
                "Executioner_II",
                "Executioner II",
                "Kill 2 bosses.",
                2f,
                1000
            )
        );

        achievements.Add(
            new Achievement(
                "Executioner_III",
                "Executioner III",
                "Kill 3 bosses.",
                3f,
                2500
            )
        );

        achievements.Add(
            new Achievement(
                "Executioner_IV",
                "Executioner IV",
                "Kill 4 bosses.",
                4f,
                5000
            )
        );

        achievements.Add(
            new Achievement(
                "Executioner_V",
                "Executioner V",
                "Kill 5 bosses.",
                5f,
                10000
            )
        );
    }

    // ============================================================
    // PROGRESS
    // ============================================================

    public float GetProgress(string id)
    {
        if (string.IsNullOrEmpty(id))
            return 0f;

        // ========================================================
        // BLOODSHED
        // ========================================================

        if (id.StartsWith("Bloodshed_"))
        {
            return AchievementStats.TotalEnemiesKilled;
        }

        // ========================================================
        // DEFIANCE
        // ========================================================

        if (id.StartsWith("Defiance_"))
        {
            return AchievementStats.HighestWave;
        }

        // ========================================================
        // FORTIFIED
        // ========================================================

        if (id.StartsWith("Fortified_"))
        {
            // House starts at Level 1.
            //
            // This tracks lifetime highest house level and does
            // NOT reset when the player rebirths.

            return Mathf.Max(
                AchievementStats.HighestHouseLevel,
                1
            );
        }

        // ========================================================
        // ARMORED
        // ========================================================

        if (id.StartsWith("Armored_"))
        {
            // Lifetime highest COMPLETE armor tier.
            //
            // Actual armor resets on rebirth, but this achievement
            // progress does not.

            return AchievementStats.HighestArmorTier;
        }

        // ========================================================
        // FORTUNE
        // ========================================================

        if (id.StartsWith("Fortune_"))
        {
            // Use the actual decimal Net Worth value.

            return
                NetWorth.Instance != null
                    ? NetWorth.Instance.TotalNetWorth
                    : PlayerPrefs.GetFloat(
                        "NetWorth",
                        0f
                    );
        }

        // ========================================================
        // SOULBOUND
        // ========================================================

        if (id.StartsWith("Soulbound_"))
        {
            return AchievementStats.TotalSoulstonesEarned;
        }

        // ========================================================
        // ENHANCED
        // ========================================================

        if (id.StartsWith("Enhanced_"))
        {
            return AchievementStats.TotalUpgradesPurchased;
        }

        // ========================================================
        // BRUTALITY
        // ========================================================

        if (id.StartsWith("Brutality_"))
        {
            return AchievementStats.TotalDamageDealt;
        }

        // ========================================================
        // REBORN
        // ========================================================

        if (id.StartsWith("Reborn_"))
        {
            return AchievementStats.HighestRebirth;
        }

        // ========================================================
        // EXECUTIONER
        // ========================================================

        if (id.StartsWith("Executioner_"))
        {
            return AchievementStats.TotalBossesKilled;
        }

        return 0f;
    }

    // ============================================================
    // UPDATE ACHIEVEMENT STATES
    // ============================================================

    public void UpdateAchievementStates()
    {
        foreach (Achievement achievement in achievements)
        {
            if (achievement == null)
                continue;

            // Claimed achievements do not need to be checked again.
            if (IsClaimed(achievement.id))
                continue;

            float progress =
                GetProgress(
                    achievement.id
                );

            if (progress >= achievement.requirement)
            {
                Debug.Log(
                    "Achievement completed: " +
                    achievement.title +
                    " | Progress: " +
                    progress +
                    " / " +
                    achievement.requirement
                );
            }
        }

        // Refresh after checking ALL achievements.
        RefreshAllAchievementUI();
    }

    // ============================================================
    // GET ACHIEVEMENT
    // ============================================================

    public Achievement GetAchievement(string id)
    {
        for (int i = 0; i < achievements.Count; i++)
        {
            if (achievements[i].id == id)
                return achievements[i];
        }

        return null;
    }

    // ============================================================
    // CLAIMED
    // ============================================================

    public bool IsClaimed(string id)
    {
        if (string.IsNullOrEmpty(id))
            return false;

        return PlayerPrefs.GetInt(
            "AchievementClaimed_" + id,
            0
        ) == 1;
    }

    // ============================================================
    // CLAIM ACHIEVEMENT
    // ============================================================

    public bool ClaimAchievement(string id)
    {
        Achievement achievement =
            GetAchievement(id);

        if (achievement == null)
        {
            Debug.LogWarning(
                "Could not claim achievement. Achievement not found: " +
                id
            );

            return false;
        }

        if (IsClaimed(id))
            return false;

        float progress =
            GetProgress(id);

        if (progress < achievement.requirement)
        {
            Debug.LogWarning(
                "Achievement not completed: " +
                achievement.title
            );

            return false;
        }

        if (Soulstones.Instance == null)
        {
            Debug.LogWarning(
                "Cannot claim achievement. Soulstones.Instance is null."
            );

            return false;
        }

        Soulstones.Instance.AddSoulstones(
            achievement.soulstoneReward
        );

        PlayerPrefs.SetInt(
            "AchievementClaimed_" + id,
            1
        );

        PlayerPrefs.Save();

        // Update both the dictionary AND the achievement itself.
        claimed[id] = true;
        achievement.claimed = true;

        Debug.Log(
            "Achievement claimed: " +
            achievement.title +
            " | Reward: " +
            achievement.soulstoneReward +
            " Soulstones"
        );

        RefreshAllAchievementUI();

        return true;
    }

    // ============================================================
    // LOAD CLAIMED STATES
    // ============================================================

    private void LoadClaimedStates()
    {
        claimed.Clear();

        foreach (Achievement achievement in achievements)
        {
            bool isClaimed =
                PlayerPrefs.GetInt(
                    "AchievementClaimed_" + achievement.id,
                    0
                ) == 1;

            claimed[achievement.id] = isClaimed;

            // Important for AchievementCategoryStars.cs
            achievement.claimed = isClaimed;
        }
    }

    // ============================================================
    // RESET ALL ACHIEVEMENTS
    // ============================================================

    public void ResetAllAchievements()
    {
        foreach (Achievement achievement in achievements)
        {
            PlayerPrefs.DeleteKey(
                "AchievementClaimed_" + achievement.id
            );

            claimed[achievement.id] = false;
            achievement.claimed = false;
        }

        PlayerPrefs.Save();

        RefreshAllAchievementUI();
    }

    // ============================================================
    // REFRESH UI
    // ============================================================

    private void RefreshAllAchievementUI()
    {
        AchievementUI[] achievementUIs =
            FindObjectsByType<AchievementUI>(
                FindObjectsInactive.Include,
                FindObjectsSortMode.None
            );

        foreach (AchievementUI ui in achievementUIs)
        {
            if (ui != null)
                ui.RefreshUI();
        }

        AchievementCategoryStars[] categoryStars =
            FindObjectsByType<AchievementCategoryStars>(
                FindObjectsInactive.Include,
                FindObjectsSortMode.None
            );

        foreach (AchievementCategoryStars stars in categoryStars)
        {
            if (stars != null)
                stars.RefreshStars();
        }
    }
}