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


    private bool houseDestroyed = false;


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


    private void Start()
    {
        LoadHouseLevel();
    }


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


        health =
            maxHealth;


        UpdateHouseSprite(level);


        Debug.Log(
            $"Loaded House Level: {level + 1} | " +
            $"HP: {maxHealth}"
        );
    }


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


        health =
            maxHealth;


        UpdateHouseSprite(level);


        Debug.Log(
            $"House refreshed to Level {level + 1} | " +
            $"HP: {health}/{maxHealth}"
        );
    }



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



    public void TakeDamage(int amount)
    {
        if (houseDestroyed)
            return;


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



        if (health <= 0)
        {
            houseDestroyed = true;


            Debug.Log(
                "House destroyed!"
            );



            if (PlayerDeaths.Instance != null)
            {
                PlayerDeaths.Instance.AddDeath();
            }




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

                    return;
                }
            }



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



    public void Heal(int amount)
    {
        if (houseDestroyed)
            return;


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


    public int GetHealth()
    {
        return health;
    }


    public int GetMaxHealth()
    {
        return maxHealth;
    }
}