using TMPro;
using UnityEngine;

public class ArmorRarityDisplay : MonoBehaviour
{
    [Header("One TMP for each rarity")]
    public GameObject[] rarityTexts = new GameObject[5];

    // PlayerPrefs keys used by your armor scripts
    private readonly string[] armorKeys =
    {
        "HelmetTier",
        "ChestplateTier",
        "LeggingsTier",
        "BootsTier"
    };

    void Start()
    {
        UpdateRarityDisplay();
    }

    public void UpdateRarityDisplay()
    {
        int helmet = PlayerPrefs.GetInt("HelmetTier", 0);
        int chest = PlayerPrefs.GetInt("ChestplateTier", 0);
        int legs = PlayerPrefs.GetInt("LeggingsTier", 0);
        int boots = PlayerPrefs.GetInt("BootsTier", 0);

        int highestTier = Mathf.Max(helmet, chest, legs, boots);

        // Hide everything
        for (int i = 0; i < rarityTexts.Length; i++)
        {
            if (rarityTexts[i] != null)
                rarityTexts[i].SetActive(false);
        }

        // If every armor piece is max level, show the final text
        if (helmet == 5 && chest == 5 && legs == 5 && boots == 5)
        {
            if (rarityTexts.Length >= 6 && rarityTexts[5] != null)
                rarityTexts[5].SetActive(true);

            return;
        }

        // Otherwise show the highest rarity unlocked
        if (highestTier > 0 && highestTier <= 5)
        {
            if (rarityTexts[highestTier - 1] != null)
                rarityTexts[highestTier - 1].SetActive(true);
        }
    }
}