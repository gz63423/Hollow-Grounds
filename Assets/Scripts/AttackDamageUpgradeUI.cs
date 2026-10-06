using TMPro;
using UnityEngine;

public class AttackDamageUpgradeUI : MonoBehaviour
{
    [Header("Upgrade")]
    public AttackDamageUpgrade upgrade;

    [Header("UI")]
    public TMP_Text costText;
    public TMP_Text damageText;

    [Header("Colors")]
    public string nextDamageColor = "#06FF00";

    private void Start()
    {
        RefreshUI();
    }

    private void Update()
    {
        RefreshUI();
    }

    public void RefreshUI()
    {
        if (upgrade == null)
        {
            return;
        }

        int currentLevel = upgrade.CurrentLevel;

        // MAX LEVEL
        if (currentLevel >= upgrade.maxLevel)
        {
            if (costText != null)
            {
                costText.text = "MAX";
            }

            if (damageText != null)
            {
                int maxDamage =
                    Mathf.RoundToInt(
                        upgrade.GetCurrentDamage()
                    );

                damageText.text =
                    maxDamage.ToString();
            }

            return;
        }

        // COST
        int cost = upgrade.GetCurrentUpgradeCost();

        if (costText != null)
        {
            costText.text = cost.ToString();
        }

        // CURRENT AND NEXT DAMAGE
        int currentDamage =
            Mathf.RoundToInt(
                upgrade.GetCurrentDamage()
            );

        int nextDamage =
            Mathf.RoundToInt(
                upgrade.GetNextDamage()
            );

        if (damageText != null)
        {
            damageText.text =
                currentDamage.ToString() +
                "  ->  " +
                "<color=" +
                nextDamageColor +
                ">" +
                nextDamage.ToString() +
                "</color>";
        }
    }
}