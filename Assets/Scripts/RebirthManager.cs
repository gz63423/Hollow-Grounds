using UnityEngine;

public class RebirthManager : MonoBehaviour
{
    public static RebirthManager Instance { get; private set; }


    // ============================================================
    // REBIRTH DATA 
    // ============================================================

    [Header("Current Rebirth")]
    [SerializeField] private int currentRebirth = 0;


    [Header("Maximum Rebirths")]
    public int maxRebirths = 5;


    [Header("Rebirth Multipliers")]
    public float[] rebirthMultipliers =
    {
        1f,
        2f,
        4f,
        8f,
        16f,
        32f
    };


    // ============================================================
    // REQUIREMENTS
    // ============================================================

    [Header("Armor Requirements")]
    public int[] requiredArmorTiers =
    {
        0,
        1,
        2,
        3,
        4,
        5
    };


    [Header("House Requirements")]
    public int[] requiredHouseLevels =
    {
        0,
        10,
        20,
        30,
        40,
        50
    };


    // ============================================================
    // PLAYER PREFS
    // ============================================================

    private const string RebirthKey =
        "RebirthCount";

    private const string StartAtWaveOneKey =
        "StartAtWaveOne";

    private const string CurrentRunHighestWaveKey =
        "CurrentRunHighestWave";


    // ============================================================
    // CURRENT RUN
    // ============================================================

    private bool startAtWaveOne = false;


    public bool StartAtWaveOne
    {
        get
        {
            return startAtWaveOne;
        }
    }


    // ============================================================
    // PROPERTIES
    // ============================================================

    public int CurrentRebirth
    {
        get
        {
            return currentRebirth;
        }
    }


    public float CurrentMultiplier
    {
        get
        {
            if (
                currentRebirth >= 0 &&
                currentRebirth < rebirthMultipliers.Length
            )
            {
                return rebirthMultipliers[
                    currentRebirth
                ];
            }

            return 1f;
        }
    }


    public bool HasMaxRebirth
    {
        get
        {
            return currentRebirth >= maxRebirths;
        }
    }


    // ============================================================
    // AWAKE
    // ============================================================

    private void Awake()
    {
        if (
            Instance != null &&
            Instance != this
        )
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        DontDestroyOnLoad(gameObject);

        LoadRebirth();

        // Load the saved Wave 1 reset state.
        startAtWaveOne =
            PlayerPrefs.GetInt(
                StartAtWaveOneKey,
                0
            ) == 1;
    }


    // ============================================================
    // LOAD REBIRTH
    // ============================================================

    private void LoadRebirth()
    {
        currentRebirth =
            PlayerPrefs.GetInt(
                RebirthKey,
                0
            );

        currentRebirth =
            Mathf.Clamp(
                currentRebirth,
                0,
                maxRebirths
            );
    }


    // ============================================================
    // SAVE REBIRTH
    // ============================================================

    private void SaveRebirth()
    {
        PlayerPrefs.SetInt(
            RebirthKey,
            currentRebirth
        );

        PlayerPrefs.Save();
    }


    // ============================================================
    // CAN REBIRTH
    // ============================================================

    public bool CanRebirth()
    {
        if (HasMaxRebirth)
            return false;


        int nextRebirth =
            currentRebirth + 1;


        if (
            nextRebirth >= requiredArmorTiers.Length ||
            nextRebirth >= requiredHouseLevels.Length
        )
        {
            return false;
        }


        int requiredArmorTier =
            requiredArmorTiers[
                nextRebirth
            ];


        int requiredHouseLevel =
            requiredHouseLevels[
                nextRebirth
            ];


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


        int houseLevel =
            PlayerPrefs.GetInt(
                "HouseLevel",
                0
            );


        bool armorComplete =
            helmetTier >= requiredArmorTier &&
            chestplateTier >= requiredArmorTier &&
            leggingsTier >= requiredArmorTier &&
            bootsTier >= requiredArmorTier;


        bool houseComplete =
            houseLevel >= requiredHouseLevel;


        return
            armorComplete &&
            houseComplete;
    }


    // ============================================================
    // PERFORM REBIRTH
    // ============================================================

    public bool PerformRebirth()
    {
        if (!CanRebirth())
        {
            return false;
        }


        int previousRebirth =
            currentRebirth;


        // ========================================================
        // INCREASE REBIRTH
        // ========================================================

        currentRebirth++;

        currentRebirth =
            Mathf.Clamp(
                currentRebirth,
                0,
                maxRebirths
            );


        // ========================================================
        // REBORN ACHIEVEMENT
        // ========================================================

        AchievementStats.SetHighestRebirth(
            currentRebirth
        );


        // ========================================================
        // SAVE NEW REBIRTH
        // ========================================================

        SaveRebirth();


        // ========================================================
        // RESET CURRENT RUN
        // ========================================================

        // The next wave run starts at Wave 1.
        //
        // This does NOT reset HighestWave.
        // HighestWave is the lifetime leaderboard statistic.

        startAtWaveOne = true;

        PlayerPrefs.SetInt(
            StartAtWaveOneKey,
            1
        );


        // Reset the current run's saved wave.
        PlayerPrefs.SetInt(
            CurrentRunHighestWaveKey,
            0
        );


        PlayerPrefs.Save();


        // ========================================================
        // RESET PLAYER PROGRESSION
        // ========================================================

        ResetPlayerProgress();


        // ========================================================
        // FIRST REBIRTH
        // ========================================================

        if (
            previousRebirth == 0 &&
            currentRebirth == 1
        )
        {
            if (
                RebirthIntroManager.Instance != null
            )
            {
                RebirthIntroManager.Instance
                    .MarkFirstRebirthComplete();
            }
        }


        return true;
    }


    // ============================================================
    // CONSUME WAVE 1 RESET
    // ============================================================

    public bool ConsumeStartAtWaveOne()
    {
        if (!startAtWaveOne)
            return false;


        startAtWaveOne = false;


        PlayerPrefs.SetInt(
            StartAtWaveOneKey,
            0
        );

        PlayerPrefs.Save();


        return true;
    }


    // ============================================================
    // RESET PLAYER PROGRESS
    // ============================================================

    private void ResetPlayerProgress()
    {
        // ========================================================
        // MONEY
        // ========================================================

        PlayerPrefs.SetFloat(
            "PlayerMoney",
            0f
        );


        // ========================================================
        // ARMOR TIERS
        // ========================================================

        PlayerPrefs.SetInt(
            "HelmetTier",
            0
        );

        PlayerPrefs.SetInt(
            "ChestplateTier",
            0
        );

        PlayerPrefs.SetInt(
            "LeggingsTier",
            0
        );

        PlayerPrefs.SetInt(
            "BootsTier",
            0
        );


        // ========================================================
        // ARMOR HP BONUSES
        // ========================================================

        PlayerPrefs.SetFloat(
            "HelmetHPBonus",
            0f
        );

        PlayerPrefs.SetFloat(
            "ChestplateHPBonus",
            0f
        );

        PlayerPrefs.SetFloat(
            "LeggingsHPBonus",
            0f
        );

        PlayerPrefs.SetFloat(
            "BootsHPBonus",
            0f
        );


        // ========================================================
        // HOUSE
        // ========================================================

        PlayerPrefs.SetInt(
            "HouseLevel",
            0
        );


        // ========================================================
        // SAVE EVERYTHING
        // ========================================================

        PlayerPrefs.Save();
    }


    // ============================================================
    // ARMOR REQUIREMENT
    // ============================================================

    public bool HasRequiredArmor()
    {
        if (HasMaxRebirth)
            return true;


        int nextRebirth =
            currentRebirth + 1;


        if (
            nextRebirth >=
            requiredArmorTiers.Length
        )
        {
            return false;
        }


        int requiredTier =
            requiredArmorTiers[
                nextRebirth
            ];


        return
            PlayerPrefs.GetInt(
                "HelmetTier",
                0
            ) >= requiredTier &&

            PlayerPrefs.GetInt(
                "ChestplateTier",
                0
            ) >= requiredTier &&

            PlayerPrefs.GetInt(
                "LeggingsTier",
                0
            ) >= requiredTier &&

            PlayerPrefs.GetInt(
                "BootsTier",
                0
            ) >= requiredTier;
    }


    // ============================================================
    // HOUSE REQUIREMENT
    // ============================================================

    public bool HasRequiredHouse()
    {
        if (HasMaxRebirth)
            return true;


        int nextRebirth =
            currentRebirth + 1;


        if (
            nextRebirth >=
            requiredHouseLevels.Length
        )
        {
            return false;
        }


        int requiredLevel =
            requiredHouseLevels[
                nextRebirth
            ];


        return
            PlayerPrefs.GetInt(
                "HouseLevel",
                0
            ) >= requiredLevel;
    }


    // ============================================================
    // NEXT REBIRTH
    // ============================================================

    public int GetNextRebirthNumber()
    {
        return currentRebirth + 1;
    }


    public int GetRequiredArmorTier()
    {
        if (HasMaxRebirth)
            return 0;


        int nextRebirth =
            currentRebirth + 1;


        if (
            nextRebirth >=
            requiredArmorTiers.Length
        )
        {
            return 0;
        }


        return requiredArmorTiers[
            nextRebirth
        ];
    }


    public int GetRequiredHouseLevel()
    {
        if (HasMaxRebirth)
            return 0;


        int nextRebirth =
            currentRebirth + 1;


        if (
            nextRebirth >=
            requiredHouseLevels.Length
        )
        {
            return 0;
        }


        return requiredHouseLevels[
            nextRebirth
        ];
    }


    public float GetNextMultiplier()
    {
        if (HasMaxRebirth)
            return CurrentMultiplier;


        int nextRebirth =
            currentRebirth + 1;


        if (
            nextRebirth >= 0 &&
            nextRebirth < rebirthMultipliers.Length
        )
        {
            return rebirthMultipliers[
                nextRebirth
            ];
        }


        return 1f;
    }


    // ============================================================
    // RESET REBIRTH PROGRESS
    // ============================================================

    public void ResetRebirthProgress()
    {
        currentRebirth = 0;


        PlayerPrefs.SetInt(
            RebirthKey,
            0
        );


        // Reset player progression.
        ResetPlayerProgress();


        // Reset current run wave behavior.
        startAtWaveOne = false;


        PlayerPrefs.SetInt(
            StartAtWaveOneKey,
            0
        );


        // Reset current run wave.
        PlayerPrefs.SetInt(
            CurrentRunHighestWaveKey,
            0
        );


        // Reset first rebirth intro.
        if (
            RebirthIntroManager.Instance != null
        )
        {
            RebirthIntroManager.Instance
                .ResetFirstRebirthProgress();
        }


        PlayerPrefs.Save();
    }
}