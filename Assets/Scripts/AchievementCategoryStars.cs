using UnityEngine;
using UnityEngine.UI;

public class AchievementCategoryStars : MonoBehaviour
{
    [Header("Category")]
    [Tooltip("Example: Bloodshed, Defiance, Fortified, etc.")]
    public string categoryName;

    [Header("Stars - Left to Right")]
    [Tooltip("Assign the 5 stars from left to right.")]
    public Image[] stars = new Image[5];

    private readonly Color unclaimedColor =
        new Color32(0x37, 0x37, 0x37, 0xFF);

    private readonly Color claimedColor =
        new Color32(0xF8, 0xFF, 0x00, 0xFF);


    private void Start()
    {
        RefreshStars();
    }


    public void RefreshStars()
    {
        if (AchievementManager.Instance == null)
            return;

        if (stars == null || stars.Length == 0)
            return;

        string[] achievementIDs =
            GetCategoryIDs();

        int claimedCount = 0;

        foreach (string id in achievementIDs)
        {
            AchievementManager.Achievement achievement =
                AchievementManager.Instance.GetAchievement(id);

            if (achievement != null &&
                achievement.claimed)
            {
                claimedCount++;
            }
        }


        // Stars light from LEFT to RIGHT
        for (int i = 0; i < stars.Length; i++)
        {
            if (stars[i] == null)
                continue;

            stars[i].color =
                i < claimedCount
                    ? claimedColor
                    : unclaimedColor;
        }
    }


    private string[] GetCategoryIDs()
    {
        switch (categoryName)
        {
            case "Bloodshed":
                return new string[]
                {
                    "Bloodshed_I",
                    "Bloodshed_II",
                    "Bloodshed_III",
                    "Bloodshed_IV",
                    "Bloodshed_V"
                };

            case "Defiance":
                return new string[]
                {
                    "Defiance_I",
                    "Defiance_II",
                    "Defiance_III",
                    "Defiance_IV",
                    "Defiance_V"
                };

            case "Fortified":
                return new string[]
                {
                    "Fortified_I",
                    "Fortified_II",
                    "Fortified_III",
                    "Fortified_IV",
                    "Fortified_V"
                };

            case "Armored":
                return new string[]
                {
                    "Armored_I",
                    "Armored_II",
                    "Armored_III",
                    "Armored_IV",
                    "Armored_V"
                };

            case "Fortune":
                return new string[]
                {
                    "Fortune_I",
                    "Fortune_II",
                    "Fortune_III",
                    "Fortune_IV",
                    "Fortune_V"
                };

            case "Soulbound":
                return new string[]
                {
                    "Soulbound_I",
                    "Soulbound_II",
                    "Soulbound_III",
                    "Soulbound_IV",
                    "Soulbound_V"
                };

            case "Enhanced":
                return new string[]
                {
                    "Enhanced_I",
                    "Enhanced_II",
                    "Enhanced_III",
                    "Enhanced_IV",
                    "Enhanced_V"
                };

            case "Brutality":
                return new string[]
                {
                    "Brutality_I",
                    "Brutality_II",
                    "Brutality_III",
                    "Brutality_IV",
                    "Brutality_V"
                };

            case "Reborn":
                return new string[]
                {
                    "Reborn_I",
                    "Reborn_II",
                    "Reborn_III",
                    "Reborn_IV",
                    "Reborn_V"
                };

            case "Executioner":
                return new string[]
                {
                    "Executioner_I",
                    "Executioner_II",
                    "Executioner_III",
                    "Executioner_IV",
                    "Executioner_V"
                };

            default:
                Debug.LogWarning(
                    "AchievementCategoryStars: " +
                    "Unknown category: " +
                    categoryName
                );

                return new string[0];
        }
    }
}