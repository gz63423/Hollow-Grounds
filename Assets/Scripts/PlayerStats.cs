using UnityEngine;
using TMPro;

public class PlayerStats : MonoBehaviour
{
    [Header("Account Level")]
    public TMP_Text accountLevelText;

    [Header("Time Played")]
    public TMP_Text timePlayedText;

    [Header("Net Worth")]
    public TMP_Text netWorthText;

    [Header("Highest Wave")]
    public TMP_Text highestWaveText;

    [Header("Enemies Killed")]
    public TMP_Text enemiesKilledText;

    [Header("Bosses Killed")]
    public TMP_Text bossesKilledText;

    [Header("Damage Dealt")]
    public TMP_Text damageDealtText;

    [Header("Player Deaths")]
    public TMP_Text playerDeathsText;

    [Header("Rebirth")]
    public TMP_Text rebirthText;


    void Update()
    {
        UpdateAccountLevel();
        UpdateTimePlayed();
        UpdateNetWorth();
        UpdateHighestWave();
        UpdateEnemiesKilled();
        UpdateBossesKilled();
        UpdateDamageDealt();
        UpdatePlayerDeaths();
        UpdateRebirth();
    }


    void UpdateAccountLevel()
    {
        if (accountLevelText == null)
            return;

        if (AccountLevel.Instance == null)
            return;

        accountLevelText.text =
            "Account Level: " +
            AccountLevel.Instance.currentLevel.ToString("N0");
    }


    void UpdateTimePlayed()
    {
        if (timePlayedText == null)
            return;

        if (TimePlayed.Instance == null)
            return;

        timePlayedText.text =
            "Time Played: " +
            TimePlayed.Instance.GetFormattedTime();
    }


    void UpdateNetWorth()
    {
        if (netWorthText == null)
            return;

        if (NetWorth.Instance == null)
            return;

        netWorthText.text =
            "Net Worth: " +
            MoneyFormatter.FormatMoney(
                NetWorth.Instance.TotalNetWorth
            );
    }


    void UpdateHighestWave()
    {
        if (highestWaveText == null)
            return;

        if (HighestWave.Instance == null)
            return;

        highestWaveText.text =
            "Highest Wave: " +
            HighestWave.Instance.HighestWaveReached.ToString("N0");
    }


    void UpdateEnemiesKilled()
    {
        if (enemiesKilledText == null)
            return;

        if (EnemiesKilled.Instance == null)
            return;

        enemiesKilledText.text =
            "Enemies Killed: " +
            EnemiesKilled.Instance.TotalEnemiesKilled.ToString("N0");
    }


    void UpdateBossesKilled()
    {
        if (bossesKilledText == null)
            return;

        if (BossesKilled.Instance == null)
            return;

        bossesKilledText.text =
            "Bosses Killed: " +
            BossesKilled.Instance.TotalBossesKilled.ToString("N0");
    }


    void UpdateDamageDealt()
    {
        if (damageDealtText == null)
            return;

        if (DamageDealt.Instance == null)
            return;

        damageDealtText.text =
            "Damage Dealt: " +
            DamageDealt.Instance.TotalDamageDealt.ToString("N0");
    }


    void UpdatePlayerDeaths()
    {
        if (playerDeathsText == null)
            return;

        if (PlayerDeaths.Instance == null)
            return;

        playerDeathsText.text =
            "Deaths: " +
            PlayerDeaths.Instance.TotalDeaths.ToString("N0");
    }


    void UpdateRebirth()
    {
        if (rebirthText == null)
            return;

        if (RebirthManager.Instance == null)
        {
            rebirthText.gameObject.SetActive(false);
            return;
        }

        int rebirth =
            RebirthManager.Instance.CurrentRebirth;

        // Hide the text before the first rebirth.
        if (rebirth < 1)
        {
            rebirthText.gameObject.SetActive(false);
            return;
        }

        // Show the text once the player has rebirthed.
        rebirthText.gameObject.SetActive(true);

        rebirthText.text =
            "Rebirth " + rebirth.ToString("N0");
    }
}

