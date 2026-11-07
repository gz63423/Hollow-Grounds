using UnityEngine;

public static class AchievementStats
{
    public static int TotalEnemiesKilled
    {
        get => PlayerPrefs.GetInt(
            "Achievement_TotalEnemiesKilled",
            0
        );
    }

    public static void AddEnemiesKilled(int amount = 1)
    {
        if (amount <= 0)
            return;

        int current =
            TotalEnemiesKilled;

        PlayerPrefs.SetInt(
            "Achievement_TotalEnemiesKilled",
            current + amount
        );

        PlayerPrefs.Save();
    }




    public static int HighestWave
    {
        get => PlayerPrefs.GetInt(
            "Achievement_HighestWave",
            0
        );
    }

    public static void SetHighestWave(int wave)
    {
        if (wave <= HighestWave)
            return;

        PlayerPrefs.SetInt(
            "Achievement_HighestWave",
            wave
        );

        PlayerPrefs.Save();
    }




    public static int TotalMoneyEarned
    {
        get => PlayerPrefs.GetInt(
            "Achievement_TotalMoneyEarned",
            0
        );
    }

    public static void AddMoneyEarned(float amount)
    {
        int current =
            TotalMoneyEarned;

        int amountToAdd =
            Mathf.FloorToInt(amount);

        if (amountToAdd <= 0)
            return;

        PlayerPrefs.SetInt(
            "Achievement_TotalMoneyEarned",
            current + amountToAdd
        );

        PlayerPrefs.Save();
    }




    public static int TotalSoulstonesEarned
    {
        get => PlayerPrefs.GetInt(
            "Achievement_TotalSoulstonesEarned",
            0
        );
    }

    public static void AddSoulstonesEarned(int amount)
    {
        if (amount <= 0)
            return;

        int current =
            TotalSoulstonesEarned;

        PlayerPrefs.SetInt(
            "Achievement_TotalSoulstonesEarned",
            current + amount
        );

        PlayerPrefs.Save();
    }




    public static int TotalUpgradesPurchased
    {
        get => PlayerPrefs.GetInt(
            "Achievement_TotalUpgradesPurchased",
            0
        );
    }

    public static void AddUpgradesPurchased(int amount = 1)
    {
        if (amount <= 0)
            return;

        int current =
            TotalUpgradesPurchased;

        PlayerPrefs.SetInt(
            "Achievement_TotalUpgradesPurchased",
            current + amount
        );

        PlayerPrefs.Save();
    }




    public static long TotalDamageDealt
    {
        get
        {
            string savedValue =
                PlayerPrefs.GetString(
                    "Achievement_TotalDamageDealt",
                    "0"
                );

            if (long.TryParse(
                savedValue,
                out long result))
            {
                return result;
            }

            return 0;
        }
    }

    public static void AddDamageDealt(float amount)
    {
        if (amount <= 0)
            return;

        long current =
            TotalDamageDealt;

        long amountToAdd =
            Mathf.FloorToInt(amount);

        if (amountToAdd <= 0)
            return;

        PlayerPrefs.SetString(
            "Achievement_TotalDamageDealt",
            (current + amountToAdd).ToString()
        );

        PlayerPrefs.Save();
    }




    public static int HighestRebirth
    {
        get => PlayerPrefs.GetInt(
            "Achievement_HighestRebirth",
            0
        );
    }

    public static void SetHighestRebirth(int rebirth)
    {
        if (rebirth <= HighestRebirth)
            return;

        PlayerPrefs.SetInt(
            "Achievement_HighestRebirth",
            rebirth
        );

        PlayerPrefs.Save();
    }



    public static int TotalBossesKilled
    {
        get => PlayerPrefs.GetInt(
            "Achievement_TotalBossesKilled",
            0
        );
    }

    public static void AddBossKilled(int amount = 1)
    {
        if (amount <= 0)
            return;

        int current =
            TotalBossesKilled;

        PlayerPrefs.SetInt(
            "Achievement_TotalBossesKilled",
            current + amount
        );

        PlayerPrefs.Save();
    }




    public static int HighestHouseLevel
    {
        get
        {
            int level =
                PlayerPrefs.GetInt(
                    "Achievement_HighestHouseLevel",
                    1
                );

            // House starts at Level 1

            return Mathf.Max(
                level,
                1
            );
        }
    }

    public static void SetHighestHouseLevel(int level)
    {
        if (level <= HighestHouseLevel)
            return;

        PlayerPrefs.SetInt(
            "Achievement_HighestHouseLevel",
            level
        );

        PlayerPrefs.Save();
    }

    public static void UpdateHighestHouseLevel()
    {
        int currentHouseLevel =
            PlayerPrefs.GetInt(
                "HouseLevel",
                0
            ) + 1;

        SetHighestHouseLevel(
            currentHouseLevel
        );
    }




    public static int HighestArmorTier
    {
        get => PlayerPrefs.GetInt(
            "Achievement_HighestArmorTier",
            0
        );
    }

    public static void SetHighestArmorTier(int tier)
    {
        if (tier <= HighestArmorTier)
            return;

        PlayerPrefs.SetInt(
            "Achievement_HighestArmorTier",
            tier
        );

        PlayerPrefs.Save();
    }

    public static void UpdateHighestArmorTier()
    {
        int helmetTier =
            PlayerPrefs.GetInt(
                "HelmetTier",
                0
            );

        int chestplateTier =
            PlayerPrefs.GetInt(
                "ChestplateTier",
                0
            );

        int leggingsTier =
            PlayerPrefs.GetInt(
                "LeggingsTier",
                0
            );

        int bootsTier =
            PlayerPrefs.GetInt(
                "BootsTier",
                0
            );

        // The completed armor tier is determined by the lowest of the four armor pieces.

        int completeArmorTier =
            Mathf.Min(
                helmetTier,
                chestplateTier,
                leggingsTier,
                bootsTier
            );

        SetHighestArmorTier(
            completeArmorTier
        );
    }


    //RESET


    public static void ResetAllAchievementStats()
    {
        PlayerPrefs.DeleteKey(
            "Achievement_TotalEnemiesKilled"
        );

        PlayerPrefs.DeleteKey(
            "Achievement_HighestWave"
        );

        PlayerPrefs.DeleteKey(
            "Achievement_TotalMoneyEarned"
        );

        PlayerPrefs.DeleteKey(
            "Achievement_TotalSoulstonesEarned"
        );

        PlayerPrefs.DeleteKey(
            "Achievement_TotalUpgradesPurchased"
        );

        PlayerPrefs.DeleteKey(
            "Achievement_TotalDamageDealt"
        );

        PlayerPrefs.DeleteKey(
            "Achievement_HighestRebirth"
        );

        PlayerPrefs.DeleteKey(
            "Achievement_TotalBossesKilled"
        );

        PlayerPrefs.DeleteKey(
            "Achievement_HighestHouseLevel"
        );

        PlayerPrefs.DeleteKey(
            "Achievement_HighestArmorTier"
        );

        PlayerPrefs.Save();
    }
}