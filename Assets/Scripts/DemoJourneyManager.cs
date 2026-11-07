using System.Collections.Generic;
using System.Numerics;
using System.Security.Claims;
using Unity.VisualScripting;
using UnityEngine;

public class DemoJourneyManager : MonoBehaviour
{
    public static DemoJourneyManager Instance { get; private set; }


    // ============================================================
    // OBJECTIVE DATA
    // ============================================================

    [System.Serializable]
    public class Objective
    {
        public string id;
        public string title;
        public string description;

        public float requirement;

        public int soulstoneReward;

        public bool claimed;

        public Objective(
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


    private readonly List<Objective> objectives =
        new List<Objective>();


    // ============================================================
    // PLAYERPREFS
    // ============================================================

    private const string ClaimedPrefix =
        "DemoJourneyClaimed_";

    private const string BaselineInitializedPrefix =
        "DemoJourneyBaselineInitialized_";

    private const string BaselineIntPrefix =
        "DemoJourneyBaselineInt_";

    private const string BaselineFloatPrefix =
        "DemoJourneyBaselineFloat_";


    // ============================================================
    // DEMO JOURNEY TRACKING KEYS
    // ============================================================

    private const string ScoutsKilledKey =
        "DemoJourney_ScoutsKilled";

    private const string MoneyEarnedKey =
        "DemoJourney_MoneyEarned";

    private const string SoulSiphonActivationsKey =
        "DemoJourney_SoulSiphonActivations";

    private const string RavagersKilledKey =
        "DemoJourney_RavagersKilled";

    private const string FinaleCompletedKey =
        "DemoJourney_FinaleCompleted";


    // ============================================================
    // PERMANENT HIGHEST-VALUE KEYS
    // ============================================================
    //
    // These are separate from the live game values.
    //
    // Rebirth may lower the actual HouseLevel or armor tiers,
    // but these values NEVER decrease unless the Demo Journey
    // itself is reset.
    //

    private const string HighestHelmetTierKey =
        "DemoJourney_HighestHelmetTier";

    private const string HighestChestplateTierKey =
        "DemoJourney_HighestChestplateTier";

    private const string HighestLeggingsTierKey =
        "DemoJourney_HighestLeggingsTier";

    private const string HighestBootsTierKey =
        "DemoJourney_HighestBootsTier";

    private const string HighestHouseLevelKey =
        "DemoJourney_HighestHouseLevel";


    // ============================================================
    // DIRECT-STATE COMPLETION KEYS
    // ============================================================

    private const string BonecladCompletedKey =
        "DemoJourney_BonecladCompleted";

    private const string FortifiedCompletedKey =
        "DemoJourney_FortifiedCompleted";


    // ============================================================
    // UNITY
    // ============================================================

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;

            DontDestroyOnLoad(gameObject);

            CreateObjectives();
            LoadClaimedStates();

            // Immediately recover all historical progress
            // that was saved during previous sessions.
            SyncPermanentHighestValues();
        }
        else
        {
            Destroy(gameObject);
        }
    }


    private void Start()
    {
        // Objective #1 starts immediately.
        EnsureObjectiveBaseline(
            objectives[0].id
        );

        SyncPermanentHighestValues();

        UpdateObjectiveStates();
    }


    // ============================================================
    // CREATE OBJECTIVES
    // ============================================================

    private void CreateObjectives()
    {
        objectives.Clear();


        objectives.Add(
            new Objective(
                "Scoutbreaker",
                "Scoutbreaker",
                "Kill 10 Scouts.",
                10f,
                5
            )
        );


        objectives.Add(
            new Objective(
                "RisingPower",
                "Rising Power",
                "Purchase 5 upgrades.",
                5f,
                10
            )
        );


        objectives.Add(
            new Objective(
                "FirstFortune",
                "First Fortune",
                "Earn $2.50.",
                2.50f,
                20
            )
        );


        objectives.Add(
            new Objective(
                "DeepFreeze",
                "Deep Freeze",
                "Upgrade Freeze to Level 2.",
                2f,
                40
            )
        );


        objectives.Add(
            new Objective(
                "SoulHarvester",
                "Soul Harvester",
                "Activate Soul Siphon 3 times.",
                3f,
                70
            )
        );


        objectives.Add(
            new Objective(
                "RavagersEnd",
                "Ravager's End",
                "Kill 20 Ravagers.",
                20f,
                100
            )
        );


        objectives.Add(
            new Objective(
                "SecondChance",
                "Second Chance",
                "Upgrade Repair to Level 4.",
                4f,
                150
            )
        );


        // ========================================================
        // #8 — BONECLAD
        // ========================================================

        objectives.Add(
            new Objective(
                "Boneclad",
                "Boneclad",
                "Complete the Bone Armor set.",
                1f,
                200
            )
        );


        // ========================================================
        // #9 — FORTIFIED
        // ========================================================

        objectives.Add(
            new Objective(
                "Fortified",
                "Fortified",
                "Reach House Level 20.",
                20f,
                200
            )
        );


        objectives.Add(
            new Objective(
                "IntoTheHollow",
                "Into the Hollow",
                "Complete the Demo Finale.",
                1f,
                500
            )
        );
    }


    // ============================================================
    // PERMANENT HIGHEST-VALUE SYSTEM
    // ============================================================

    private void SyncPermanentHighestValues()
    {
        // --------------------------------------------------------
        // BONE ARMOR
        // --------------------------------------------------------

        int currentHelmet =
            PlayerPrefs.GetInt(
                "HelmetTier",
                0
            );

        int currentChestplate =
            PlayerPrefs.GetInt(
                "ChestplateTier",
                0
            );

        int currentLeggings =
            PlayerPrefs.GetInt(
                "LeggingsTier",
                0
            );

        int currentBoots =
            PlayerPrefs.GetInt(
                "BootsTier",
                0
            );


        int highestHelmet =
            PlayerPrefs.GetInt(
                HighestHelmetTierKey,
                0
            );

        int highestChestplate =
            PlayerPrefs.GetInt(
                HighestChestplateTierKey,
                0
            );

        int highestLeggings =
            PlayerPrefs.GetInt(
                HighestLeggingsTierKey,
                0
            );

        int highestBoots =
            PlayerPrefs.GetInt(
                HighestBootsTierKey,
                0
            );


        bool changed = false;


        if (currentHelmet > highestHelmet)
        {
            highestHelmet = currentHelmet;

            PlayerPrefs.SetInt(
                HighestHelmetTierKey,
                highestHelmet
            );

            changed = true;
        }


        if (currentChestplate > highestChestplate)
        {
            highestChestplate = currentChestplate;

            PlayerPrefs.SetInt(
                HighestChestplateTierKey,
                highestChestplate
            );

            changed = true;
        }


        if (currentLeggings > highestLeggings)
        {
            highestLeggings = currentLeggings;

            PlayerPrefs.SetInt(
                HighestLeggingsTierKey,
                highestLeggings
            );

            changed = true;
        }


        if (currentBoots > highestBoots)
        {
            highestBoots = currentBoots;

            PlayerPrefs.SetInt(
                HighestBootsTierKey,
                highestBoots
            );

            changed = true;
        }


        // --------------------------------------------------------
        // HOUSE
        // --------------------------------------------------------
        //
        // HouseLevel is internally 0-based:
        //
        // 0 = displayed Level 1
        // 1 = displayed Level 2
        // ...
        // 19 = displayed Level 20
        //

        int currentHouseLevel =
            PlayerPrefs.GetInt(
                "HouseLevel",
                0
            ) + 1;


        currentHouseLevel =
            Mathf.Max(
                currentHouseLevel,
                1
            );


        int highestHouseLevel =
            PlayerPrefs.GetInt(
                HighestHouseLevelKey,
                1
            );


        if (currentHouseLevel > highestHouseLevel)
        {
            highestHouseLevel =
                currentHouseLevel;

            PlayerPrefs.SetInt(
                HighestHouseLevelKey,
                highestHouseLevel
            );

            changed = true;
        }


        // --------------------------------------------------------
        // CHECK BONECLAD
        // --------------------------------------------------------

        if (
            highestHelmet >= 2 &&
            highestChestplate >= 2 &&
            highestLeggings >= 2 &&
            highestBoots >= 2
        )
        {
            if (
                PlayerPrefs.GetInt(
                    BonecladCompletedKey,
                    0
                ) != 1
            )
            {
                PlayerPrefs.SetInt(
                    BonecladCompletedKey,
                    1
                );

                changed = true;
            }
        }


        // --------------------------------------------------------
        // CHECK FORTIFIED
        // --------------------------------------------------------

        if (highestHouseLevel >= 20)
        {
            if (
                PlayerPrefs.GetInt(
                    FortifiedCompletedKey,
                    0
                ) != 1
            )
            {
                PlayerPrefs.SetInt(
                    FortifiedCompletedKey,
                    1
                );

                changed = true;
            }
        }


        if (changed)
        {
            PlayerPrefs.Save();
        }
    }


    // ============================================================
    // GET PROGRESS
    // ============================================================

    public float GetProgress(string id)
    {
        if (string.IsNullOrEmpty(id))
            return 0f;


        // Always synchronize historical values before reading
        // Boneclad or Fortified.
        if (
            id == "Boneclad" ||
            id == "Fortified"
        )
        {
            SyncPermanentHighestValues();
        }


        // ========================================================
        // #1 — SCOUTBREAKER
        // ========================================================

        if (id == "Scoutbreaker")
        {
            return GetIntProgress(
                id,
                ScoutsKilledKey
            );
        }


        // ========================================================
        // #2 — RISING POWER
        // ========================================================

        if (id == "RisingPower")
        {
            int current =
                AchievementStats.TotalUpgradesPurchased;


            int baseline =
                GetBaselineInt(id);


            return Mathf.Max(
                current - baseline,
                0
            );
        }


        // ========================================================
        // #3 — FIRST FORTUNE
        // ========================================================

        if (id == "FirstFortune")
        {
            float current =
                PlayerPrefs.GetFloat(
                    MoneyEarnedKey,
                    0f
                );

            float baseline =
                GetBaselineFloat(id);

            return Mathf.Max(
                current - baseline,
                0f
            );
        }


        // ========================================================
        // #4 — DEEP FREEZE
        // ========================================================

        if (id == "DeepFreeze")
        {
            bool unlocked =
                PlayerPrefs.GetInt(
                    "Active_Freeze_Unlocked",
                    0
                ) == 1;


            if (!unlocked)
                return 0f;


            int currentLevel =
                PlayerPrefs.GetInt(
                    "Active_Freeze_Level",
                    0
                );


            return Mathf.Clamp(
                currentLevel,
                0,
                3
            );
        }


        // ========================================================
        // #5 — SOUL HARVESTER
        // ========================================================

        if (id == "SoulHarvester")
        {
            return GetIntProgress(
                id,
                SoulSiphonActivationsKey
            );
        }


        // ========================================================
        // #6 — RAVAGER'S END
        // ========================================================

        if (id == "RavagersEnd")
        {
            return GetIntProgress(
                id,
                RavagersKilledKey
            );
        }


        // ========================================================
        // #7 — SECOND CHANCE
        // ========================================================

        if (id == "SecondChance")
        {
            bool unlocked =
                PlayerPrefs.GetInt(
                    "Active_Repair_Unlocked",
                    0
                ) == 1;


            if (!unlocked)
                return 0f;


            int currentLevel =
                PlayerPrefs.GetInt(
                    "Active_Repair_Level",
                    0
                );


            return Mathf.Clamp(
                currentLevel,
                0,
                5
            );
        }


        // ========================================================
        // #8 — BONECLAD
        // ========================================================

        if (id == "Boneclad")
        {
            // Check the permanent completion flag first.
            if (
                PlayerPrefs.GetInt(
                    BonecladCompletedKey,
                    0
                ) == 1
            )
            {
                return 1f;
            }


            // Read the HISTORICAL highest tiers,
            // not the current live tiers.
            int helmet =
                PlayerPrefs.GetInt(
                    HighestHelmetTierKey,
                    0
                );

            int chestplate =
                PlayerPrefs.GetInt(
                    HighestChestplateTierKey,
                    0
                );

            int leggings =
                PlayerPrefs.GetInt(
                    HighestLeggingsTierKey,
                    0
                );

            int boots =
                PlayerPrefs.GetInt(
                    HighestBootsTierKey,
                    0
                );


            bool complete =
                helmet >= 2 &&
                chestplate >= 2 &&
                leggings >= 2 &&
                boots >= 2;


            if (complete)
            {
                PlayerPrefs.SetInt(
                    BonecladCompletedKey,
                    1
                );

                PlayerPrefs.Save();

                return 1f;
            }


            return 0f;
        }


        // ========================================================
        // #9 — FORTIFIED
        // ========================================================

        if (id == "Fortified")
        {
            if (
                PlayerPrefs.GetInt(
                    FortifiedCompletedKey,
                    0
                ) == 1
            )
            {
                return 20f;
            }


            // Use the historical highest House Level.
            int highestHouseLevel =
                PlayerPrefs.GetInt(
                    HighestHouseLevelKey,
                    1
                );


            highestHouseLevel =
                Mathf.Clamp(
                    highestHouseLevel,
                    1,
                    20
                );


            if (highestHouseLevel >= 20)
            {
                PlayerPrefs.SetInt(
                    FortifiedCompletedKey,
                    1
                );

                PlayerPrefs.Save();

                return 20f;
            }


            return highestHouseLevel;
        }


        // ========================================================
        // #10 — INTO THE HOLLOW
        // ========================================================

        if (id == "IntoTheHollow")
        {
            return PlayerPrefs.GetInt(
                FinaleCompletedKey,
                0
            );
        }


        return 0f;
    }


    // ============================================================
    // INTEGER PROGRESS
    // ============================================================

    private int GetIntProgress(
        string objectiveID,
        string sourceKey
    )
    {
        int current =
            PlayerPrefs.GetInt(
                sourceKey,
                0
            );

        int baseline =
            GetBaselineInt(objectiveID);

        return Mathf.Max(
            current - baseline,
            0
        );
    }


    // ============================================================
    // BASELINE SYSTEM
    // ============================================================

    private bool HasBaseline(string id)
    {
        return PlayerPrefs.GetInt(
            BaselineInitializedPrefix + id,
            0
        ) == 1;
    }


    private bool UsesBaseline(string id)
    {
        if (string.IsNullOrEmpty(id))
            return false;


        if (
            id == "DeepFreeze" ||
            id == "SecondChance" ||
            id == "Fortified" ||
            id == "Boneclad" ||
            id == "IntoTheHollow"
        )
        {
            return false;
        }


        return true;
    }


    private void EnsureObjectiveBaseline(string id)
    {
        if (string.IsNullOrEmpty(id))
            return;


        if (!UsesBaseline(id))
            return;


        if (HasBaseline(id))
            return;


        InitializeObjectiveBaseline(id);
    }


    private void InitializeObjectiveBaseline(string id)
    {
        if (string.IsNullOrEmpty(id))
            return;


        if (!UsesBaseline(id))
            return;


        if (id == "Scoutbreaker")
        {
            SetBaselineInt(
                id,
                PlayerPrefs.GetInt(
                    ScoutsKilledKey,
                    0
                )
            );
        }


        else if (id == "RisingPower")
        {
            SetBaselineInt(
                id,
                AchievementStats.TotalUpgradesPurchased
            );
        }


        else if (id == "FirstFortune")
        {
            SetBaselineFloat(
                id,
                PlayerPrefs.GetFloat(
                    MoneyEarnedKey,
                    0f
                )
            );
        }


        else if (id == "SoulHarvester")
        {
            SetBaselineInt(
                id,
                PlayerPrefs.GetInt(
                    SoulSiphonActivationsKey,
                    0
                )
            );
        }


        else if (id == "RavagersEnd")
        {
            SetBaselineInt(
                id,
                PlayerPrefs.GetInt(
                    RavagersKilledKey,
                    0
                )
            );
        }


        PlayerPrefs.SetInt(
            BaselineInitializedPrefix + id,
            1
        );

        PlayerPrefs.Save();


        Debug.Log(
            "Demo Journey | Baseline initialized: " +
            id
        );
    }


    private void SetBaselineInt(
        string id,
        int value
    )
    {
        PlayerPrefs.SetInt(
            BaselineIntPrefix + id,
            value
        );
    }


    private void SetBaselineFloat(
        string id,
        float value
    )
    {
        PlayerPrefs.SetFloat(
            BaselineFloatPrefix + id,
            value
        );
    }


    private int GetBaselineInt(string id)
    {
        return PlayerPrefs.GetInt(
            BaselineIntPrefix + id,
            0
        );
    }


    private float GetBaselineFloat(string id)
    {
        return PlayerPrefs.GetFloat(
            BaselineFloatPrefix + id,
            0f
        );
    }


    // ============================================================
    // EVENT TRACKING
    // ============================================================

    public void AddScoutKill()
    {
        int current =
            PlayerPrefs.GetInt(
                ScoutsKilledKey,
                0
            );

        current++;


        PlayerPrefs.SetInt(
            ScoutsKilledKey,
            current
        );

        PlayerPrefs.Save();


        Debug.Log(
            "Demo Journey | Scout killed: " +
            GetProgress("Scoutbreaker") +
            " / 10"
        );


        UpdateObjectiveStates();
    }


    public void AddRavagerKill()
    {
        int current =
            PlayerPrefs.GetInt(
                RavagersKilledKey,
                0
            );

        current++;


        PlayerPrefs.SetInt(
            RavagersKilledKey,
            current
        );

        PlayerPrefs.Save();


        Debug.Log(
            "Demo Journey | Ravager killed: " +
            GetProgress("RavagersEnd") +
            " / 20"
        );


        UpdateObjectiveStates();
    }


    public void AddMoneyEarned(float amount)
    {
        if (amount <= 0f)
            return;


        float current =
            PlayerPrefs.GetFloat(
                MoneyEarnedKey,
                0f
            );

        current += amount;


        PlayerPrefs.SetFloat(
            MoneyEarnedKey,
            current
        );

        PlayerPrefs.Save();


        Debug.Log(
            "Demo Journey | Money earned: $" +
            GetProgress("FirstFortune").ToString("0.00") +
            " / $2.50"
        );


        UpdateObjectiveStates();
    }


    public void AddSoulSiphonActivation()
    {
        int current =
            PlayerPrefs.GetInt(
                SoulSiphonActivationsKey,
                0
            );

        current++;


        PlayerPrefs.SetInt(
            SoulSiphonActivationsKey,
            current
        );

        PlayerPrefs.Save();


        Debug.Log(
            "Demo Journey | Soul Siphon activation: " +
            GetProgress("SoulHarvester") +
            " / 5"
        );


        UpdateObjectiveStates();
    }


    public void CompleteDemoFinale()
    {
        PlayerPrefs.SetInt(
            FinaleCompletedKey,
            1
        );

        PlayerPrefs.Save();


        Debug.Log(
            "Demo Journey | Demo Finale completed!"
        );


        UpdateObjectiveStates();
    }


    // ============================================================
    // GET OBJECTIVE
    // ============================================================

    public Objective GetObjective(string id)
    {
        if (string.IsNullOrEmpty(id))
            return null;


        for (int i = 0; i < objectives.Count; i++)
        {
            if (objectives[i].id == id)
                return objectives[i];
        }


        return null;
    }


    // ============================================================
    // GET OBJECTIVE INDEX
    // ============================================================

    private int GetObjectiveIndex(string id)
    {
        if (string.IsNullOrEmpty(id))
            return -1;


        for (int i = 0; i < objectives.Count; i++)
        {
            if (objectives[i].id == id)
                return i;
        }


        return -1;
    }


    // ============================================================
    // CLAIMED
    // ============================================================

    public bool IsClaimed(string id)
    {
        if (string.IsNullOrEmpty(id))
            return false;


        return PlayerPrefs.GetInt(
            ClaimedPrefix + id,
            0
        ) == 1;
    }


    // ============================================================
    // UNLOCKED
    // ============================================================

    public bool IsUnlocked(string id)
    {
        int index =
            GetObjectiveIndex(id);


        if (index < 0)
            return false;


        // Objective #1 is always unlocked.
        if (index == 0)
            return true;


        Objective previous =
            objectives[index - 1];


        return IsClaimed(previous.id);
    }


    // ============================================================
    // COMPLETED
    // ============================================================

    public bool IsCompleted(string id)
    {
        Objective objective =
            GetObjective(id);


        if (objective == null)
            return false;


        float progress =
            GetProgress(id);


        if (id == "FirstFortune")
        {
            return progress + 0.001f >=
                   objective.requirement;
        }


        return progress >=
               objective.requirement;
    }


    // ============================================================
    // UPDATE OBJECTIVE STATES
    // ============================================================

    public void UpdateObjectiveStates()
    {
        // --------------------------------------------------------
        // FIRST:
        // Recover the highest House/Bone values.
        //
        // This is safe to call repeatedly.
        // It only ever increases the saved values.
        // --------------------------------------------------------

        SyncPermanentHighestValues();


        // --------------------------------------------------------
        // PERMANENT DIRECT-STATE OBJECTIVES
        // --------------------------------------------------------

        GetProgress("Boneclad");
        GetProgress("Fortified");


        // --------------------------------------------------------
        // ONLY INITIALIZE THE CURRENT UNLOCKED OBJECTIVE.
        //
        // This prevents future baseline objectives from
        // inheriting old progress.
        // --------------------------------------------------------

        for (int i = 0; i < objectives.Count; i++)
        {
            Objective objective =
                objectives[i];

            if (objective == null)
                continue;


            if (IsClaimed(objective.id))
                continue;


            if (!IsUnlocked(objective.id))
                continue;


            EnsureObjectiveBaseline(
                objective.id
            );


            // Only the first unlocked unclaimed objective
            // tracks normally.
            break;
        }


        // --------------------------------------------------------
        // #10 TRACKS IN THE BACKGROUND.
        // --------------------------------------------------------

        Objective finale =
            GetObjective("IntoTheHollow");

        if (finale != null)
        {
            // FinaleCompletedKey is always read directly.
        }


        // --------------------------------------------------------
        // COMPLETION DEBUG
        // --------------------------------------------------------

        foreach (Objective objective in objectives)
        {
            if (objective == null)
                continue;


            if (IsClaimed(objective.id))
                continue;


            if (!IsUnlocked(objective.id))
                continue;


            float progress =
                GetProgress(objective.id);


            if (IsCompleted(objective.id))
            {
                Debug.Log(
                    "Demo Journey objective completed: " +
                    objective.title +
                    " | Progress: " +
                    progress +
                    " / " +
                    objective.requirement
                );
            }
        }


        RefreshAllUI();
    }


    // ============================================================
    // CLAIM OBJECTIVE
    // ============================================================

    public bool ClaimObjective(string id)
    {
        Objective objective =
            GetObjective(id);


        if (objective == null)
        {
            Debug.LogWarning(
                "Could not claim Demo Journey objective. " +
                "Objective not found: " +
                id
            );

            return false;
        }


        if (IsClaimed(id))
            return false;


        if (!IsUnlocked(id))
        {
            Debug.LogWarning(
                "Demo Journey objective is locked: " +
                objective.title
            );

            return false;
        }


        // Make sure historical House/Bone state is current.
        SyncPermanentHighestValues();


        // Make sure its baseline exists if it uses one.
        EnsureObjectiveBaseline(id);


        if (!IsCompleted(id))
        {
            Debug.LogWarning(
                "Demo Journey objective is not completed: " +
                objective.title +
                " | Progress: " +
                GetProgress(id) +
                " / " +
                objective.requirement
            );

            return false;
        }


        if (Soulstones.Instance == null)
        {
            Debug.LogWarning(
                "Cannot claim Demo Journey objective. " +
                "Soulstones.Instance is null."
            );

            return false;
        }


        // ========================================================
        // REWARD
        // ========================================================

        Soulstones.Instance.AddSoulstones(
            objective.soulstoneReward
        );


        // ========================================================
        // CLAIM
        // ========================================================

        PlayerPrefs.SetInt(
            ClaimedPrefix + id,
            1
        );


        objective.claimed = true;


        // ========================================================
        // SAVE
        // ========================================================

        PlayerPrefs.Save();


        Debug.Log(
            "Demo Journey objective claimed: " +
            objective.title +
            " | Reward: " +
            objective.soulstoneReward +
            " Soulstones"
        );


        // ========================================================
        // INITIALIZE NEXT OBJECTIVE
        // ========================================================

        int currentIndex =
            GetObjectiveIndex(id);

        int nextIndex =
            currentIndex + 1;


        if (
            nextIndex >= 0 &&
            nextIndex < objectives.Count
        )
        {
            Objective nextObjective =
                objectives[nextIndex];


            if (
                nextObjective != null &&
                UsesBaseline(nextObjective.id)
            )
            {
                DeleteBaseline(
                    nextObjective.id
                );


                InitializeObjectiveBaseline(
                    nextObjective.id
                );
            }
        }


        RefreshAllUI();


        return true;
    }


    // ============================================================
    // DELETE BASELINE
    // ============================================================

    private void DeleteBaseline(string id)
    {
        PlayerPrefs.DeleteKey(
            BaselineInitializedPrefix + id
        );

        PlayerPrefs.DeleteKey(
            BaselineIntPrefix + id
        );

        PlayerPrefs.DeleteKey(
            BaselineFloatPrefix + id
        );
    }


    // ============================================================
    // LOAD CLAIMED STATES
    // ============================================================

    private void LoadClaimedStates()
    {
        foreach (Objective objective in objectives)
        {
            if (objective == null)
                continue;


            objective.claimed =
                IsClaimed(objective.id);
        }
    }


    // ============================================================
    // REFRESH ALL UI
    // ============================================================

    public void RefreshAllUI()
    {
        DemoJourneyUI[] journeyUIs =
            FindObjectsByType<DemoJourneyUI>(
                FindObjectsInactive.Include,
                FindObjectsSortMode.None
            );


        foreach (DemoJourneyUI ui in journeyUIs)
        {
            if (ui != null)
                ui.RefreshUI();
        }
    }


    // ============================================================
    // RESET DEMO JOURNEY
    // ============================================================

    public void ResetDemoJourney()
    {
        // --------------------------------------------------------
        // RESET CLAIMS + BASELINES
        // --------------------------------------------------------

        foreach (Objective objective in objectives)
        {
            if (objective == null)
                continue;


            PlayerPrefs.DeleteKey(
                ClaimedPrefix + objective.id
            );


            DeleteBaseline(
                objective.id
            );


            objective.claimed = false;
        }


        // --------------------------------------------------------
        // RESET PERMANENT DIRECT-STATE COMPLETION
        // --------------------------------------------------------

        PlayerPrefs.DeleteKey(
            BonecladCompletedKey
        );

        PlayerPrefs.DeleteKey(
            FortifiedCompletedKey
        );


        // --------------------------------------------------------
        // RESET HISTORICAL HIGHEST VALUES
        // --------------------------------------------------------

        PlayerPrefs.DeleteKey(
            HighestHelmetTierKey
        );

        PlayerPrefs.DeleteKey(
            HighestChestplateTierKey
        );

        PlayerPrefs.DeleteKey(
            HighestLeggingsTierKey
        );

        PlayerPrefs.DeleteKey(
            HighestBootsTierKey
        );

        PlayerPrefs.DeleteKey(
            HighestHouseLevelKey
        );


        // --------------------------------------------------------
        // RESET TRACKING
        // --------------------------------------------------------

        PlayerPrefs.DeleteKey(
            ScoutsKilledKey
        );


        PlayerPrefs.DeleteKey(
            MoneyEarnedKey
        );


        PlayerPrefs.DeleteKey(
            SoulSiphonActivationsKey
        );


        PlayerPrefs.DeleteKey(
            RavagersKilledKey
        );


        PlayerPrefs.DeleteKey(
            FinaleCompletedKey
        );


        PlayerPrefs.Save();


        // --------------------------------------------------------
        // OBJECTIVE #1 STARTS FROM ZERO
        // --------------------------------------------------------

        InitializeObjectiveBaseline(
            "Scoutbreaker"
        );


        RefreshAllUI();
    }
}
