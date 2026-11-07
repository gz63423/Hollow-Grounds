using System;
using UnityEngine;

public class Soulstones : MonoBehaviour
{
    public static Soulstones Instance;

    public int SoulstoneCount { get; private set; }

    public int TotalSoulstonesEarned { get; private set; }

    public Action<int> OnSoulstonesChanged;

    private const string SaveKey = "Soulstones";
    private const string TotalEarnedSaveKey = "TotalSoulstonesEarned";

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;

            DontDestroyOnLoad(gameObject);

            SoulstoneCount =
                PlayerPrefs.GetInt(
                    SaveKey,
                    0
                );

            TotalSoulstonesEarned =
                PlayerPrefs.GetInt(
                    TotalEarnedSaveKey,
                    0
                );
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddSoulstones(int amount)
    {
        if (amount <= 0)
            return;

        Debug.Log(
            "============================================================\n" +
            "SOULSTONE ADDED: +" + amount + "\n" +
            "Balance BEFORE: " + SoulstoneCount + "\n" +
            "Balance AFTER: " + (SoulstoneCount + amount) + "\n" +
            "CALL STACK:\n" +
            System.Environment.StackTrace +
            "\n============================================================"
        );

        SoulstoneCount += amount;

        TotalSoulstonesEarned += amount;


        AchievementStats.AddSoulstonesEarned(
            amount
        );

        PlayerPrefs.SetInt(
            SaveKey,
            SoulstoneCount
        );

        PlayerPrefs.SetInt(
            TotalEarnedSaveKey,
            TotalSoulstonesEarned
        );

        PlayerPrefs.Save();

        OnSoulstonesChanged?.Invoke(
            SoulstoneCount
        );
    }

    public bool SpendSoulstones(int amount)
    {
        if (amount <= 0 ||
            SoulstoneCount < amount)
        {
            return false;
        }

        SoulstoneCount -= amount;

        PlayerPrefs.SetInt(
            SaveKey,
            SoulstoneCount
        );

        PlayerPrefs.Save();

        OnSoulstonesChanged?.Invoke(
            SoulstoneCount
        );

        return true;
    }

    public bool HasEnoughSoulstones(int amount)
    {
        return SoulstoneCount >= amount;
    }

    public int GetTotalSoulstonesEarned()
    {
        return TotalSoulstonesEarned;
    }

    public void ResetSoulstones()
    {
        SoulstoneCount = 0;

        PlayerPrefs.SetInt(
            SaveKey,
            0
        );

        PlayerPrefs.Save();

        OnSoulstonesChanged?.Invoke(
            SoulstoneCount
        );
    }

    public void ResetAllSoulstoneStats()
    {
        SoulstoneCount = 0;

        TotalSoulstonesEarned = 0;

        PlayerPrefs.SetInt(
            SaveKey,
            0
        );

        PlayerPrefs.SetInt(
            TotalEarnedSaveKey,
            0
        );

        PlayerPrefs.Save();

        OnSoulstonesChanged?.Invoke(
            SoulstoneCount
        );
    }
}