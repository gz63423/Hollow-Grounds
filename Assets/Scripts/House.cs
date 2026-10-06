using UnityEngine;
using UnityEngine.SceneManagement;

public class House : MonoBehaviour
{
    [Header("House Stats")]
    public int maxHealth = 10;
    public int health;


    [Header("Upgrade Data")]
    public Sprite[] houseLevelSprites =
        new Sprite[5];


    [Header("References")]
    public SpriteRenderer spriteRenderer;
    public Collider2D houseCollider;


    // ============================================================
    // STATE
    // ============================================================

    private bool houseDestroyed = false;


    // ============================================================
    // AWAKE
    // ============================================================

    private void Awake()
    {
        if (spriteRenderer == null)
        {
            spriteRenderer =
                GetComponent<SpriteRenderer>();
        }


        if (houseCollider == null)
        {
            houseCollider =
                GetComponent<Collider2D>();
        }
    }


    // ============================================================
    // START
    // ============================================================

    private void Start()
    {
        LoadHouseLevel();
    }


    // ============================================================
    // LOAD HOUSE LEVEL
    // ============================================================

    private void LoadHouseLevel()
    {
        int level =
            PlayerPrefs.GetInt(
                "HouseLevel",
                0
            );


        level =
            Mathf.Clamp(
                level,
                0,
                49
            );


        // ========================================================
        // GET MANUAL HP
        // ========================================================

        if (HouseUpgradeData.Instance != null)
        {
            maxHealth =
                HouseUpgradeData.Instance.GetHealth(
                    level
                );
        }
        else
        {
            Debug.LogError(
                "HouseUpgradeData.Instance was not found!"
            );


            maxHealth = 10;
        }


        // ========================================================
        // SET CURRENT HEALTH
        // ========================================================

        health =
            maxHealth;


        // ========================================================
        // UPDATE HOUSE SPRITE
        // ========================================================

        UpdateHouseSprite(level);


        Debug.Log(
            $"Loaded House Level: {level + 1} | " +
            $"HP: {maxHealth}"
        );
    }


    // ============================================================
    // REFRESH HOUSE AFTER UPGRADE
    // ============================================================

    public void RefreshHouseLevel()
    {
        if (houseDestroyed)
            return;


        int level =
            PlayerPrefs.GetInt(
                "HouseLevel",
                0
            );


        level =
            Mathf.Clamp(
                level,
                0,
                49
            );


        // ========================================================
        // GET NEW MAX HP
        // ========================================================

        if (HouseUpgradeData.Instance != null)
        {
            maxHealth =
                HouseUpgradeData.Instance.GetHealth(
                    level
                );
        }
        else
        {
            Debug.LogError(
                "HouseUpgradeData.Instance was not found!"
            );

            maxHealth = 10;
        }


        // ========================================================
        // REFRESH CURRENT HEALTH
        // ========================================================
        //
        // Upgrading the house fully repairs it to its new
        // maximum health.
        //
        // ========================================================

        health =
            maxHealth;


        // ========================================================
        // REFRESH SPRITE
        // ========================================================

        UpdateHouseSprite(level);


        Debug.Log(
            $"House refreshed to Level {level + 1} | " +
            $"HP: {health}/{maxHealth}"
        );
    }


    // ============================================================
    // UPDATE HOUSE SPRITE
    // ============================================================

    private void UpdateHouseSprite(int level)
    {
        if (
            houseLevelSprites != null &&
            houseLevelSprites.Length > 0 &&
            spriteRenderer != null
        )
        {
            int spriteIndex =
                Mathf.Clamp(
                    level / 10,
                    0,
                    houseLevelSprites.Length - 1
                );


            spriteRenderer.sprite =
                houseLevelSprites[spriteIndex];
        }
    }


    // ============================================================
    // TAKE DAMAGE
    // ============================================================

    public void TakeDamage(int amount)
    {
        // House has already been destroyed.
        if (houseDestroyed)
            return;


        // Ignore invalid damage.
        if (amount <= 0)
            return;


        health -= amount;


        health =
            Mathf.Max(
                health,
                0
            );


        Debug.Log(
            $"House took {amount} damage! " +
            $"Remaining: {health}/{maxHealth}"
        );


        // ========================================================
        // HOUSE DESTROYED
        // ========================================================

        if (health <= 0)
        {
            houseDestroyed = true;


            Debug.Log(
                "House destroyed!"
            );


            // ====================================================
            // ADD DEATH STAT
            // ====================================================

            if (PlayerDeaths.Instance != null)
            {
                PlayerDeaths.Instance.AddDeath();
            }


            // ====================================================
            // FIRST REBIRTH INTRO CHECK
            // ====================================================

            if (RebirthIntroManager.Instance != null)
            {
                bool introTriggered =
                    RebirthIntroManager.Instance
                        .TryPlayRebirthIntro();


                if (introTriggered)
                {
                    Debug.Log(
                        "Rebirth 1 requirements met. " +
                        "Skipping normal scene reset and starting rebirth transition."
                    );


                    // Do NOT continue into the normal
                    // scene reload.
                    return;
                }
            }


            // ====================================================
            // NORMAL HOUSE DEATH
            // ====================================================

            Debug.Log(
                "Rebirth intro not triggered. " +
                "Reloading current scene normally."
            );


            SceneManager.LoadScene(
                SceneManager.GetActiveScene()
                    .buildIndex
            );
        }
    }


    // ============================================================
    // HEAL
    // ============================================================

    public void Heal(int amount)
    {
        // House has already been destroyed.
        if (houseDestroyed)
            return;


        // Ignore invalid healing amounts.
        if (amount <= 0)
            return;


        int oldHealth =
            health;


        health += amount;


        health =
            Mathf.Clamp(
                health,
                0,
                maxHealth
            );


        int actualHealing =
            health -
            oldHealth;


        Debug.Log(
            $"House healed for {actualHealing} HP! " +
            $"Health: {health}/{maxHealth}"
        );
    }


    // ============================================================
    // GET HEALTH
    // ============================================================

    public int GetHealth()
    {
        return health;
    }


    // ============================================================
    // GET MAX HEALTH
    // ============================================================

    public int GetMaxHealth()
    {
        return maxHealth;
    }
}