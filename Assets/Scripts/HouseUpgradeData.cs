using UnityEngine;

public class HouseUpgradeData : MonoBehaviour
{
    public static HouseUpgradeData Instance { get; private set; }

    // ============================================================
    // HOUSE LEVEL DATA
    // ============================================================

    [System.Serializable]
    public class HouseLevel
    {
        [Header("Upgrade Price")]
        [Min(0f)]
        public float upgradePrice = 0.25f;

        [Header("Maximum Health")]
        [Min(1)]
        public int maxHealth = 10;
    }


    [Header("MANUAL HOUSE LEVEL DATA")]
    [Tooltip(
        "Element 0 = House Level 1\n" +
        "Element 1 = House Level 2\n" +
        "...\n" +
        "Element 49 = House Level 50"
    )]
    public HouseLevel[] levels = new HouseLevel[50];


    // ============================================================
    // AWAKE
    // ============================================================

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        DontDestroyOnLoad(gameObject);

        InitializeLevels();
    }


    // ============================================================
    // INITIALIZE LEVELS
    // ============================================================

    private void InitializeLevels()
    {
        if (levels == null || levels.Length != 50)
        {
            HouseLevel[] oldLevels = levels;

            levels = new HouseLevel[50];

            for (int i = 0; i < levels.Length; i++)
            {
                if (oldLevels != null &&
                    i < oldLevels.Length &&
                    oldLevels[i] != null)
                {
                    levels[i] = oldLevels[i];
                }
                else
                {
                    levels[i] = new HouseLevel();
                }
            }
        }

        for (int i = 0; i < levels.Length; i++)
        {
            if (levels[i] == null)
            {
                levels[i] = new HouseLevel();
            }
        }
    }


    // ============================================================
    // GET HEALTH
    // ============================================================

    public int GetHealth(int level)
    {
        level = Mathf.Clamp(level, 0, 49);

        return levels[level].maxHealth;
    }


    // ============================================================
    // GET PRICE
    // ============================================================

    public float GetPrice(int level)
    {
        level = Mathf.Clamp(level, 0, 49);

        return levels[level].upgradePrice;
    }
}

