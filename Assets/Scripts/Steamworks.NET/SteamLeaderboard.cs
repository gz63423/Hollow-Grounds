using UnityEngine;
using Steamworks;

public class SteamLeaderboard : MonoBehaviour
{
    [Header("Leaderboard Names")]
    public string netWorthLeaderboardName = "THG_NetWorth";
    public string highestWaveLeaderboardName = "THG_HighestWave";

    [Header("UI")]
    public LeaderboardRow leaderboardRowPrefab;
    public Transform netWorthContent;
    public Transform highestWaveContent;

    [Header("TESTING")]
    public bool showFakeNumberOne = true;

    public string fakePlayerName = "TheHollowedKing";

    public int fakeNetWorth = 99999999;

    public int fakeHighestWave = 9999;

    private SteamLeaderboard_t netWorthLeaderboard;
    private SteamLeaderboard_t highestWaveLeaderboard;

    private CallResult<LeaderboardFindResult_t> netWorthFindResult;
    private CallResult<LeaderboardFindResult_t> highestWaveFindResult;

    private CallResult<LeaderboardScoresDownloaded_t> netWorthDownloadResult;
    private CallResult<LeaderboardScoresDownloaded_t> highestWaveDownloadResult;

    private bool netWorthLeaderboardFound = false;
    private bool highestWaveLeaderboardFound = false;

    private void Start()
    {
        if (!SteamManager.Initialized)
        {
            Debug.LogError(
                "Steam is not initialized. Cannot load leaderboards."
            );

            return;
        }

        netWorthFindResult =
            CallResult<LeaderboardFindResult_t>.Create(
                OnNetWorthLeaderboardFound
            );

        highestWaveFindResult =
            CallResult<LeaderboardFindResult_t>.Create(
                OnHighestWaveLeaderboardFound
            );

        netWorthDownloadResult =
            CallResult<LeaderboardScoresDownloaded_t>.Create(
                OnNetWorthScoresDownloaded
            );

        highestWaveDownloadResult =
            CallResult<LeaderboardScoresDownloaded_t>.Create(
                OnHighestWaveScoresDownloaded
            );

        FindLeaderboards();
    }

    private void FindLeaderboards()
    {
        Debug.Log(
            "Finding or creating Steam leaderboards for display..."
        );

        // CHANGED:
        // FindOrCreateLeaderboard() will find the leaderboard if it
        // already exists, or create it if it does not exist.
        SteamAPICall_t netWorthCall =
            SteamUserStats.FindOrCreateLeaderboard(
                netWorthLeaderboardName,
                ELeaderboardSortMethod.k_ELeaderboardSortMethodDescending,
                ELeaderboardDisplayType.k_ELeaderboardDisplayTypeNumeric
            );

        // CHANGED:
        // Same behavior for Highest Wave.
        SteamAPICall_t highestWaveCall =
            SteamUserStats.FindOrCreateLeaderboard(
                highestWaveLeaderboardName,
                ELeaderboardSortMethod.k_ELeaderboardSortMethodDescending,
                ELeaderboardDisplayType.k_ELeaderboardDisplayTypeNumeric
            );

        netWorthFindResult.Set(
            netWorthCall
        );

        highestWaveFindResult.Set(
            highestWaveCall
        );
    }

    private void OnNetWorthLeaderboardFound(
        LeaderboardFindResult_t result,
        bool ioFailure
    )
    {
        if (
            ioFailure ||
            result.m_bLeaderboardFound == 0
        )
        {
            Debug.LogError(
                "Could not find Net Worth leaderboard."
            );

            return;
        }

        netWorthLeaderboard =
            result.m_hSteamLeaderboard;

        netWorthLeaderboardFound = true;

        Debug.Log(
            "Net Worth leaderboard found for display."
        );

        DownloadNetWorthLeaderboard();
    }

    private void OnHighestWaveLeaderboardFound(
        LeaderboardFindResult_t result,
        bool ioFailure
    )
    {
        if (
            ioFailure ||
            result.m_bLeaderboardFound == 0
        )
        {
            Debug.LogError(
                "Could not find Highest Wave leaderboard."
            );

            return;
        }

        highestWaveLeaderboard =
            result.m_hSteamLeaderboard;

        highestWaveLeaderboardFound = true;

        Debug.Log(
            "Highest Wave leaderboard found for display."
        );

        DownloadHighestWaveLeaderboard();
    }

    public void RefreshLeaderboards()
    {
        if (!SteamManager.Initialized)
        {
            Debug.LogWarning(
                "Cannot refresh leaderboards because Steam is not initialized."
            );

            return;
        }

        Debug.Log(
            "=============================================="
        );

        Debug.Log(
            "REFRESHING LEADERBOARDS IN CURRENT SCENE"
        );

        Debug.Log(
            "=============================================="
        );

        if (netWorthLeaderboardFound)
        {
            DownloadNetWorthLeaderboard();
        }
        else
        {
            Debug.LogWarning(
                "Net Worth leaderboard is not ready yet."
            );
        }

        if (highestWaveLeaderboardFound)
        {
            DownloadHighestWaveLeaderboard();
        }
        else
        {
            Debug.LogWarning(
                "Highest Wave leaderboard is not ready yet."
            );
        }
    }

    private void DownloadNetWorthLeaderboard()
    {
        if (!netWorthLeaderboardFound)
        {
            Debug.LogWarning(
                "Net Worth leaderboard is not ready."
            );

            return;
        }

        Debug.Log(
            "Downloading top 100 Net Worth entries..."
        );

        SteamAPICall_t call =
            SteamUserStats.DownloadLeaderboardEntries(
                netWorthLeaderboard,

                ELeaderboardDataRequest
                    .k_ELeaderboardDataRequestGlobal,

                1,

                100
            );

        netWorthDownloadResult.Set(
            call
        );
    }

    private void DownloadHighestWaveLeaderboard()
    {
        if (!highestWaveLeaderboardFound)
        {
            Debug.LogWarning(
                "Highest Wave leaderboard is not ready."
            );

            return;
        }

        Debug.Log(
            "Downloading top 100 Highest Wave entries..."
        );

        SteamAPICall_t call =
            SteamUserStats.DownloadLeaderboardEntries(
                highestWaveLeaderboard,

                ELeaderboardDataRequest
                    .k_ELeaderboardDataRequestGlobal,

                1,

                100
            );

        highestWaveDownloadResult.Set(
            call
        );
    }

    private void OnNetWorthScoresDownloaded(
        LeaderboardScoresDownloaded_t result,
        bool ioFailure
    )
    {
        if (ioFailure)
        {
            Debug.LogError(
                "Failed to download Net Worth leaderboard."
            );

            return;
        }

        Debug.Log(
            "Downloaded " +
            result.m_cEntryCount +
            " Net Worth entries."
        );

        DisplayLeaderboard(
            result,
            netWorthContent,
            "Net Worth",
            true
        );
    }

    private void OnHighestWaveScoresDownloaded(
        LeaderboardScoresDownloaded_t result,
        bool ioFailure
    )
    {
        if (ioFailure)
        {
            Debug.LogError(
                "Failed to download Highest Wave leaderboard."
            );

            return;
        }

        Debug.Log(
            "Downloaded " +
            result.m_cEntryCount +
            " Highest Wave entries."
        );

        DisplayLeaderboard(
            result,
            highestWaveContent,
            "Highest Wave",
            false
        );
    }

    private void DisplayLeaderboard(
        LeaderboardScoresDownloaded_t result,
        Transform content,
        string leaderboardType,
        bool isCurrency
    )
    {
        if (content == null)
        {
            Debug.LogError(
                leaderboardType +
                " Content is not assigned in SteamLeaderboard."
            );

            return;
        }

        if (leaderboardRowPrefab == null)
        {
            Debug.LogError(
                "Leaderboard Row Prefab is NOT assigned in SteamLeaderboard!"
            );

            return;
        }

        // Remove old leaderboard rows.
        for (
            int i = content.childCount - 1;
            i >= 0;
            i--
        )
        {
            Destroy(
                content.GetChild(i).gameObject
            );
        }

        // =========================================================
        // FAKE #1 TEST ENTRY
        // =========================================================

        if (showFakeNumberOne)
        {
            LeaderboardRow fakeRow =
                Instantiate(
                    leaderboardRowPrefab,
                    content
                );

            int fakeScore;

            if (isCurrency)
            {
                fakeScore =
                    fakeNetWorth;
            }
            else
            {
                fakeScore =
                    fakeHighestWave;
            }

            fakeRow.SetData(
                1,
                fakePlayerName,
                null,
                fakeScore,
                isCurrency,
                CSteamID.Nil
            );

            Debug.Log(
                "FAKE " +
                leaderboardType +
                " #1 ADDED: " +
                fakePlayerName +
                " - " +
                fakeScore
            );
        }

        // =========================================================
        // REAL STEAM ENTRIES
        // =========================================================

        for (
            int i = 0;
            i < result.m_cEntryCount;
            i++
        )
        {
            LeaderboardEntry_t entry;

            bool success =
                SteamUserStats.GetDownloadedLeaderboardEntry(
                    result.m_hSteamLeaderboardEntries,
                    i,
                    out entry,
                    null,
                    0
                );

            if (!success)
            {
                Debug.LogWarning(
                    "Could not get " +
                    leaderboardType +
                    " leaderboard entry at index " +
                    i
                );

                continue;
            }

            LeaderboardRow row =
                Instantiate(
                    leaderboardRowPrefab,
                    content
                );

            string playerName =
                SteamFriends.GetFriendPersonaName(
                    entry.m_steamIDUser
                );

            if (string.IsNullOrEmpty(playerName))
            {
                playerName = "Unknown Player";
            }

            int displayedRank =
                entry.m_nGlobalRank;

            // If the fake player is enabled,
            // visually shift every real player down by 1.
            if (showFakeNumberOne)
            {
                displayedRank++;
            }

            row.SetData(
                displayedRank,
                playerName,
                null,
                entry.m_nScore,
                isCurrency,
                entry.m_steamIDUser
            );

            Debug.Log(
                leaderboardType +
                " Rank " +
                displayedRank +
                ": " +
                playerName +
                " - " +
                entry.m_nScore
            );
        }
    }
}