using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Steamworks;

public class SteamProfileDisplay : MonoBehaviour
{
    [Header("UI")]
    public TMP_Text steamNameText;
    public RawImage profilePicture;
    public TMP_Text netWorthRankText;
    public TMP_Text highestWaveRankText;

    [Header("Leaderboard Names")]
    public string netWorthLeaderboardName = "THG_NetWorth";
    public string highestWaveLeaderboardName = "THG_HighestWave";

    private SteamLeaderboard_t netWorthLeaderboard;
    private SteamLeaderboard_t highestWaveLeaderboard;

    private CallResult<LeaderboardFindResult_t> netWorthFindResult;
    private CallResult<LeaderboardFindResult_t> highestWaveFindResult;

    private CallResult<LeaderboardScoresDownloaded_t> rankDownloadResult;

    private Texture2D avatarTexture;

    private bool netWorthLeaderboardFound = false;
    private bool highestWaveLeaderboardFound = false;

    private bool netWorthRankDownloaded = false;

    private void Start()
    {
        if (!SteamManager.Initialized)
        {
            Debug.LogError(
                "Steam is not initialized."
            );

            HideRankText(
                netWorthRankText
            );

            HideRankText(
                highestWaveRankText
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

        rankDownloadResult =
            CallResult<LeaderboardScoresDownloaded_t>.Create(
                OnRankScoresDownloaded
            );

        HideRankText(
            netWorthRankText
        );

        HideRankText(
            highestWaveRankText
        );

        DisplaySteamName();

        DisplaySteamAvatar();

        FindLeaderboards();
    }

    private void DisplaySteamName()
    {
        if (steamNameText == null)
        {
            return;
        }

        string steamName =
            SteamFriends.GetPersonaName();

        if (string.IsNullOrEmpty(steamName))
        {
            steamName = "Unknown Player";
        }

        steamNameText.text =
            steamName;
    }

    private void DisplaySteamAvatar()
    {
        if (profilePicture == null)
        {
            return;
        }

        CSteamID steamID =
            SteamUser.GetSteamID();

        int avatarHandle =
            SteamFriends.GetLargeFriendAvatar(
                steamID
            );

        if (avatarHandle == -1)
        {
            // Steam is still loading the avatar
            Invoke(
                nameof(DisplaySteamAvatar),
                0.5f
            );

            return;
        }

        if (avatarHandle == 0)
        {
            Debug.LogWarning(
                "No Steam profile picture available."
            );

            return;
        }

        uint width;
        uint height;

        bool success =
            SteamUtils.GetImageSize(
                avatarHandle,
                out width,
                out height
            );

        if (!success ||
            width == 0 ||
            height == 0)
        {
            Debug.LogWarning(
                "Could not get Steam avatar image size."
            );

            return;
        }

        byte[] imageData =
            new byte[width * height * 4];

        bool imageSuccess =
            SteamUtils.GetImageRGBA(
                avatarHandle,
                imageData,
                imageData.Length
            );

        if (!imageSuccess)
        {
            Debug.LogWarning(
                "Could not get Steam avatar image data."
            );

            return;
        }

        byte[] flippedImageData =
            new byte[imageData.Length];

        int rowSize =
            (int)width * 4;

        for (int y = 0; y < height; y++)
        {
            int sourceRow =
                y * rowSize;

            int destinationRow =
                ((int)height - 1 - y) * rowSize;

            System.Array.Copy(
                imageData,
                sourceRow,
                flippedImageData,
                destinationRow,
                rowSize
            );
        }

        avatarTexture =
            new Texture2D(
                (int)width,
                (int)height,
                TextureFormat.RGBA32,
                false
            );

        avatarTexture.LoadRawTextureData(
            flippedImageData
        );

        avatarTexture.Apply();

        profilePicture.texture =
            avatarTexture;
    }

    private void FindLeaderboards()
    {
        Debug.Log(
            "Finding Steam leaderboards for profile page..."
        );

        SteamAPICall_t netWorthCall =
            SteamUserStats.FindLeaderboard(
                netWorthLeaderboardName
            );

        SteamAPICall_t highestWaveCall =
            SteamUserStats.FindLeaderboard(
                highestWaveLeaderboardName
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

            HideRankText(
                netWorthRankText
            );

            TryStartRankDownloads();

            return;
        }

        netWorthLeaderboard =
            result.m_hSteamLeaderboard;

        netWorthLeaderboardFound =
            true;

        Debug.Log(
            "Net Worth leaderboard found."
        );

        TryStartRankDownloads();
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

            HideRankText(
                highestWaveRankText
            );

            TryStartRankDownloads();

            return;
        }

        highestWaveLeaderboard =
            result.m_hSteamLeaderboard;

        highestWaveLeaderboardFound =
            true;

        Debug.Log(
            "Highest Wave leaderboard found."
        );

        TryStartRankDownloads();
    }

    private void TryStartRankDownloads()
    {
        if (
            !netWorthRankDownloaded &&
            netWorthLeaderboardFound
        )
        {
            DownloadNetWorthRank();

            return;
        }

        if (
            netWorthRankDownloaded &&
            highestWaveLeaderboardFound
        )
        {
            DownloadHighestWaveRank();
        }
    }

    private void DownloadNetWorthRank()
    {
        Debug.Log(
            "Requesting player's exact Net Worth leaderboard entry..."
        );

        CSteamID[] users =
        {
            SteamUser.GetSteamID()
        };

        SteamAPICall_t call =
            SteamUserStats.DownloadLeaderboardEntriesForUsers(
                netWorthLeaderboard,
                users,
                users.Length
            );

        rankDownloadResult.Set(
            call
        );
    }

    private void DownloadHighestWaveRank()
    {
        Debug.Log(
            "Requesting player's exact Highest Wave leaderboard entry..."
        );

        CSteamID[] users =
        {
            SteamUser.GetSteamID()
        };

        SteamAPICall_t call =
            SteamUserStats.DownloadLeaderboardEntriesForUsers(
                highestWaveLeaderboard,
                users,
                users.Length
            );

        rankDownloadResult.Set(
            call
        );
    }

    private void OnRankScoresDownloaded(
        LeaderboardScoresDownloaded_t result,
        bool ioFailure
    )
    {
        if (ioFailure)
        {
            Debug.LogError(
                "Steam failed to download the player's leaderboard entry."
            );

            if (!netWorthRankDownloaded)
            {
                Debug.Log(
                    "Net Worth rank request failed."
                );

                netWorthRankDownloaded = true;

                TryStartRankDownloads();
            }
            else
            {
                Debug.Log(
                    "Highest Wave rank request failed."
                );
            }

            return;
        }

        if (result.m_cEntryCount <= 0)
        {
            HandleNoLeaderboardEntry(
                result.m_hSteamLeaderboard
            );

            return;
        }

        LeaderboardEntry_t entry;

        bool success =
            SteamUserStats.GetDownloadedLeaderboardEntry(
                result.m_hSteamLeaderboardEntries,
                0,
                out entry,
                null,
                0
            );

        if (!success)
        {
            Debug.LogError(
                "Steam returned a leaderboard entry, but it could not be read."
            );

            HandleNoLeaderboardEntry(
                result.m_hSteamLeaderboard
            );

            return;
        }

        CSteamID playerSteamID =
            SteamUser.GetSteamID();

        if (entry.m_steamIDUser != playerSteamID)
        {
            Debug.LogError(
                "Steam returned a leaderboard entry for the wrong user."
            );

            HandleNoLeaderboardEntry(
                result.m_hSteamLeaderboard
            );

            return;
        }

        int playerRank =
            entry.m_nGlobalRank;

        Debug.Log(
            "=============================================="
        );

        Debug.Log(
            "PLAYER LEADERBOARD RANK CONFIRMED"
        );

        Debug.Log(
            "Steam ID: " +
            playerSteamID
        );

        Debug.Log(
            "Rank: #" +
            playerRank
        );

        Debug.Log(
            "Score: " +
            entry.m_nScore
        );

        Debug.Log(
            "=============================================="
        );

        if (
            result.m_hSteamLeaderboard ==
            netWorthLeaderboard
        )
        {
            ShowRankText(
                netWorthRankText
            );

            netWorthRankText.text =
                "World #" +
                playerRank +
                " For Net Worth";

            Debug.Log(
                "PLAYER NET WORTH RANK: #" +
                playerRank
            );

            netWorthRankDownloaded =
                true;

            TryStartRankDownloads();

            return;
        }

        if (
            result.m_hSteamLeaderboard ==
            highestWaveLeaderboard
        )
        {
            ShowRankText(
                highestWaveRankText
            );

            highestWaveRankText.text =
                "World #" +
                playerRank +
                " For Highest Wave";

            Debug.Log(
                "PLAYER HIGHEST WAVE RANK: #" +
                playerRank
            );

            return;
        }
    }

    private void HandleNoLeaderboardEntry(
        SteamLeaderboard_t leaderboard
    )
    {
        if (
            leaderboard ==
            netWorthLeaderboard
        )
        {
            Debug.Log(
                "Player does not have a Net Worth leaderboard entry yet."
            );

            HideRankText(
                netWorthRankText
            );

            netWorthRankDownloaded =
                true;

            TryStartRankDownloads();

            return;
        }

        if (
            leaderboard ==
            highestWaveLeaderboard
        )
        {
            Debug.Log(
                "Player does not have a Highest Wave leaderboard entry yet."
            );

            HideRankText(
                highestWaveRankText
            );

            return;
        }
    }

    private void ShowRankText(
        TMP_Text text
    )
    {
        if (text == null)
        {
            return;
        }

        text.gameObject.SetActive(
            true
        );
    }

    private void HideRankText(
        TMP_Text text
    )
    {
        if (text == null)
        {
            return;
        }

        text.gameObject.SetActive(
            false
        );
    }

    private void OnDestroy()
    {
        if (avatarTexture != null)
        {
            Destroy(
                avatarTexture
            );
        }
    }
}