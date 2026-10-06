using UnityEngine;
using TMPro;
using System;

public class LeaderboardRefreshTimer : MonoBehaviour
{
    [Header("Leaderboard Timer Text")]
    public TMP_Text netWorthTimerText;
    public TMP_Text highestWaveTimerText;

    private const long RefreshIntervalSeconds = 180;

    private long lastProcessedCycle = -1;

    private void Start()
    {
        InitializeTimerCycle();
        UpdateTimer();
    }

    private void Update()
    {
        UpdateTimer();
        CheckForRefresh();
    }

    private void InitializeTimerCycle()
    {
        long currentUnixTime =
            DateTimeOffset.UtcNow.ToUnixTimeSeconds();

        long currentCycle =
            currentUnixTime / RefreshIntervalSeconds;

        lastProcessedCycle =
            currentCycle;
    }

    private void UpdateTimer()
    {
        long currentUnixTime =
            DateTimeOffset.UtcNow.ToUnixTimeSeconds();

        long secondsIntoCurrentCycle =
            currentUnixTime % RefreshIntervalSeconds;

        long secondsRemaining =
            RefreshIntervalSeconds -
            secondsIntoCurrentCycle;

        int minutes =
            (int)(secondsRemaining / 60);

        int seconds =
            (int)(secondsRemaining % 60);

        string timerText =
            "Refreshes in: " +
            minutes.ToString("0") +
            ":" +
            seconds.ToString("00");

        if (netWorthTimerText != null)
        {
            netWorthTimerText.text =
                timerText;
        }

        if (highestWaveTimerText != null)
        {
            highestWaveTimerText.text =
                timerText;
        }
    }

    private void CheckForRefresh()
    {
        long currentUnixTime =
            DateTimeOffset.UtcNow.ToUnixTimeSeconds();

        long currentCycle =
            currentUnixTime / RefreshIntervalSeconds;

        if (currentCycle <= lastProcessedCycle)
        {
            return;
        }

        lastProcessedCycle =
            currentCycle;

        RefreshLeaderboards();
    }

    private void RefreshLeaderboards()
    {
        Debug.Log(
            "=============================================="
        );

        Debug.Log(
            "2-MINUTE TIMER REACHED"
        );

        Debug.Log(
            "Refreshing leaderboard display..."
        );

        Debug.Log(
            "=============================================="
        );

        SteamLeaderboard leaderboard =
            FindFirstObjectByType<SteamLeaderboard>();

        if (leaderboard != null)
        {
            leaderboard.RefreshLeaderboards();

            Debug.Log(
                "Leaderboard display refresh requested."
            );
        }
        else
        {
            Debug.LogWarning(
                "SteamLeaderboard was not found in the current scene."
            );
        }
    }
}