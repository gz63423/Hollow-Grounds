using TMPro;
using UnityEngine;

public class AttackSpeedUpgradeUI : MonoBehaviour
{
    [Header("Upgrade")]
    public AttackSpeedUpgrade upgrade;

    [Header("UI")]
    public TMP_Text costText;
    public TMP_Text damageText;

    [Header("Colors")]
    public string nextSpeedColor = "#06FF00";

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

        //Maxed
        if (currentLevel >= upgrade.maxLevel)
        {
            if (costText != null)
            {
                costText.text = "MAX";
            }

            if (damageText != null)
            {
                float currentSpeed = upgrade.GetCurrentAttackTime();

                damageText.text =
                    currentSpeed.ToString("0.00") +
                    " sec";
            }

            return;
        }


        int cost = upgrade.GetCurrentUpgradeCost();

        if (costText != null)
        {
            costText.text = cost.ToString();
        }

        float currentAttackTime = upgrade.GetCurrentAttackTime();
        float nextAttackTime = upgrade.GetNextAttackTime();

        if (damageText != null)
        {
            damageText.text =
                currentAttackTime.ToString("0.00") +
                " sec  ->  " +
                "<color=" + nextSpeedColor + ">" +
                nextAttackTime.ToString("0.00") +
                " sec" +
                "</color>";
        }
    }
}