using UnityEngine;
using TMPro;

public class MainMenuDisplays : MonoBehaviour
{
    [Header("House UI")]
    public TextMeshProUGUI houseHPText;


    [Header("Player UI")]
    public TextMeshProUGUI playerHPText;


    [Header("House Sprite")]
    public SpriteRenderer currentHouseSpriteRenderer;


    [Header("House Sprites")]
    public Sprite[] houseLevelSprites = new Sprite[5];


    // ============================================================
    // START
    // ============================================================

    void Start()
    {
        // ========================================================
        // RESTORE MUSIC
        // ========================================================
        //
        // Music remains muted during the rebirth sequence.
        // It is restored only when the Main Menu loads.
        //

        if (MusicManager.Instance != null)
        {
            MusicManager.Instance.RestoreMusic();
        }


        // ========================================================
        // UPDATE HOUSE DISPLAY
        // ========================================================

        UpdateHouseDisplay();
    }


    // ============================================================
    // UPDATE HOUSE DISPLAY
    // ============================================================

    public void UpdateHouseDisplay()
    {
        //------------------------
        // HOUSE
        //------------------------

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


        int hp = 10;


        // Get the manually configured HP.
        if (HouseUpgradeData.Instance != null)
        {
            hp =
                HouseUpgradeData.Instance.GetHealth(
                    level
                );
        }
        else
        {
            Debug.LogWarning(
                "HouseUpgradeData.Instance was not found. " +
                "Using default house HP of 10."
            );
        }


        if (houseHPText != null)
        {
            houseHPText.text =
                $"{hp} HP";
        }


        // ========================================================
        // HOUSE SPRITE
        // ========================================================

        if (
            currentHouseSpriteRenderer != null &&
            houseLevelSprites != null &&
            houseLevelSprites.Length > 0
        )
        {
            int spriteIndex =
                Mathf.Clamp(
                    level / 10,
                    0,
                    houseLevelSprites.Length - 1
                );


            currentHouseSpriteRenderer.sprite =
                houseLevelSprites[spriteIndex];
        }


        //------------------------
        // PLAYER HP
        //------------------------

        float playerHP =
            2f +
            PlayerPrefs.GetFloat(
                "HelmetHPBonus",
                0f
            ) +
            PlayerPrefs.GetFloat(
                "ChestplateHPBonus",
                0f
            ) +
            PlayerPrefs.GetFloat(
                "LeggingsHPBonus",
                0f
            ) +
            PlayerPrefs.GetFloat(
                "BootsHPBonus",
                0f
            );


        if (playerHPText != null)
        {
            playerHPText.text =
                $"{playerHP} HP";
        }


        // ========================================================
        // DEBUG
        // ========================================================

        Debug.Log(
            $"House Level: {level + 1} | " +
            $"House HP: {hp} | " +
            $"Player HP: {playerHP}"
        );
    }
}