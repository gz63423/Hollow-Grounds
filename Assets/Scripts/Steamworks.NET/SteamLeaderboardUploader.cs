using Steamworks;
using System;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;

public class SteamLeaderboardUploader : MonoBehaviour
{
    public static SteamLeaderboardUploader Instance;

    [Header("Leaderboard Names")]
    public string netWorthLeaderboardName = "THG_NetWorth";
    public string highestWaveLeaderboardName = "THG_HighestWave";

    private SteamLeaderboard_t netWorthLeaderboard;
    private SteamLeaderboard_t highestWaveLeaderboard;

    private CallResult<LeaderboardFindResult_t> netWorthFindResult;
    private CallResult<LeaderboardFindResult_t> highestWaveFindResult;

    private CallResult<LeaderboardScoreUploaded_t> netWorthUploadResult;
    private CallResult<LeaderboardScoreUploaded_t> highestWaveUploadResult;

    private bool netWorthLeaderboardFound = false;
    private bool highestWaveLeaderboardFound = false;

    private bool localValuesLoaded = false;

    private bool uploadingScores = false;

    private bool netWorthUploadPending = false;
    private bool highestWaveUploadPending = false;

    private float pendingNetWorth = 0f;
    private int pendingHighestWave = 0;

    private const long UploadIntervalSeconds = 180;

    private long lastProcessedCycle = -1;

    private bool startupUploadPending = false;


    private void Awake()
    {
        // =========================================================
        // FORCE ENABLE
        // =========================================================

        enabled = true;

        if (!gameObject.activeSelf)
        {
            gameObject.SetActive(true);
        }

        Debug.Log(
            "!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!"
        );

        Debug.Log(
            "STEAM LEADERBOARD UPLOADER AWAKE"
        );

        Debug.Log(
            "!!!!!!!! NEW BUILD TEST 123456 !!!!!!!!"
        );

        Debug.Log(
            "Uploader GameObject: " +
            gameObject.name
        );

        Debug.Log(
            "Uploader enabled AFTER FORCE: " +
            enabled
        );

        Debug.Log(
            "GameObject active AFTER FORCE: " +
            gameObject.activeInHierarchy
        );

        Debug.Log(
            "!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!"
        );


        if (
            Instance != null &&
            Instance != this
        )
        {
            Debug.LogWarning(
                "STEAM LEADERBOARD UPLOADER: Duplicate instance detected. Destroying this uploader component only."
            );

            // Do NOT destroy the GameObject because this component
            // is attached to the SteamManager GameObject.
            Destroy(this);

            return;
        }


        Instance = this;

        DontDestroyOnLoad(gameObject);


        Debug.Log(
            "STEAM LEADERBOARD UPLOADER: Instance created successfully."
        );
    }


    private void OnEnable()
    {
        Debug.Log(
            "=============================================="
        );

        Debug.Log(
            "STEAM LEADERBOARD UPLOADER OnEnable() CALLED"
        );

        Debug.Log(
            "Uploader GameObject: " +
            gameObject.name
        );

        Debug.Log(
            "Uploader enabled: " +
            enabled
        );

        Debug.Log(
            "GameObject active: " +
            gameObject.activeInHierarchy
        );

        Debug.Log(
            "=============================================="
        );
    }


    private void Start()
    {
        // =========================================================
        // FORCE ENABLE
        // =========================================================

        enabled = true;

        Debug.Log(
            "!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!"
        );

        Debug.Log(
            "STEAM LEADERBOARD UPLOADER START() CALLED"
        );

        Debug.Log(
            "UPLOADER BUILD TEST: VERSION 123456"
        );

        Debug.Log(
            "Uploader enabled at START: " +
            enabled
        );

        Debug.Log(
            "GameObject active at START: " +
            gameObject.activeInHierarchy
        );

        Debug.Log(
            "SteamManager.Initialized = " +
            SteamManager.Initialized
        );

        Debug.Log(
            "!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!"
        );


        if (!SteamManager.Initialized)
        {
            Debug.LogError(
                "STEAM LEADERBOARD UPLOADER: Steam is not initialized."
            );

            return;
        }


        // =========================================================
        // STEAM ACCOUNT DIAGNOSTIC
        // =========================================================

        CSteamID steamID =
            SteamUser.GetSteamID();

        Debug.Log(
            "=============================================="
        );

        Debug.Log(
            "STEAM ACCOUNT DIAGNOSTIC"
        );

        Debug.Log(
            "Steam ID: " +
            steamID
        );

        Debug.Log(
            "Steam Persona Name: " +
            SteamFriends.GetPersonaName()
        );

        Debug.Log(
            "Steam Logged On: " +
            SteamUser.BLoggedOn()
        );

        Debug.Log(
            "=============================================="
        );


        netWorthFindResult =
            CallResult<LeaderboardFindResult_t>.Create(
                OnNetWorthLeaderboardFound
            );

        highestWaveFindResult =
            CallResult<LeaderboardFindResult_t>.Create(
                OnHighestWaveLeaderboardFound
            );

        netWorthUploadResult =
            CallResult<LeaderboardScoreUploaded_t>.Create(
                OnNetWorthScoreUploaded
            );

        highestWaveUploadResult =
            CallResult<LeaderboardScoreUploaded_t>.Create(
                OnHighestWaveScoreUploaded
            );


        long currentUnixTime =
            DateTimeOffset.UtcNow.ToUnixTimeSeconds();

        long currentCycle =
            currentUnixTime /
            UploadIntervalSeconds;

        long secondsIntoCycle =
            currentUnixTime %
            UploadIntervalSeconds;


        if (secondsIntoCycle == 0)
        {
            lastProcessedCycle =
                currentCycle - 1;
        }
        else
        {
            lastProcessedCycle =
                currentCycle;
        }


        Debug.Log(
            "=============================================="
        );

        Debug.Log(
            "STEAM LEADERBOARD UPLOADER STARTED"
        );

        Debug.Log(
            "UPLOADER TEST VERSION: DIAGNOSTIC LOGGING ENABLED"
        );

        Debug.Log(
            "BUILD TEST VERSION: 123456"
        );

        Debug.Log(
            "Global upload interval: 3 minutes"
        );

        Debug.Log(
            "Current global cycle: " +
            currentCycle
        );

        Debug.Log(
            "Current cycle position: " +
            secondsIntoCycle +
            " seconds"
        );

        Debug.Log(
            "Startup leaderboard upload ENABLED."
        );

        Debug.Log(
            "Net Worth leaderboard: " +
            netWorthLeaderboardName
        );

        Debug.Log(
            "Highest Wave leaderboard: " +
            highestWaveLeaderboardName
        );

        Debug.Log(
            "=============================================="
        );


        FindLeaderboards();
    }


    private void Update()
    {
        // =========================================================
        // FORCE ENABLE
        // =========================================================

        if (!enabled)
        {
            Debug.LogWarning(
                "STEAM LEADERBOARD UPLOADER WAS DISABLED AT RUNTIME. FORCING ENABLE."
            );

            enabled = true;
        }


        if (!gameObject.activeSelf)
        {
            Debug.LogWarning(
                "STEAM LEADERBOARD UPLOADER GAMEOBJECT WAS DISABLED AT RUNTIME. FORCING ENABLE."
            );

            gameObject.SetActive(true);
        }


        if (!SteamManager.Initialized)
        {
            return;
        }


        if (!localValuesLoaded)
        {
            TryLoadLocalValues();
        }


        if (!localValuesLoaded)
        {
            return;
        }


        if (
            startupUploadPending &&
            !uploadingScores
        )
        {
            TryUploadStartupValues();

            return;
        }


        if (uploadingScores)
        {
            return;
        }


        CheckGlobalUploadCycle();
    }


    private void TryLoadLocalValues()
    {
        if (NetWorth.Instance == null)
        {
            Debug.LogWarning(
                "STEAM LEADERBOARD UPLOADER: NetWorth.Instance not found yet."
            );

            return;
        }


        if (HighestWave.Instance == null)
        {
            Debug.LogWarning(
                "STEAM LEADERBOARD UPLOADER: HighestWave.Instance not found yet."
            );

            return;
        }


        pendingNetWorth =
            NetWorth.Instance.TotalNetWorth;

        pendingHighestWave =
            HighestWave.Instance.HighestWaveReached;


        localValuesLoaded = true;

        startupUploadPending = true;


        Debug.Log(
            "=============================================="
        );

        Debug.Log(
            "STEAM LOCAL VALUES READY"
        );

        Debug.Log(
            "Net Worth: $" +
            pendingNetWorth.ToString("N2")
        );

        Debug.Log(
            "Highest Wave: " +
            pendingHighestWave
        );

        Debug.Log(
            "Startup upload is pending."
        );

        Debug.Log(
            "=============================================="
        );
    }


    private void FindLeaderboards()
    {
        Debug.Log(
            "=============================================="
        );

        Debug.Log(
            "SEARCHING FOR STEAM LEADERBOARDS"
        );

        Debug.Log(
            "Net Worth: " +
            netWorthLeaderboardName
        );

        Debug.Log(
            "Highest Wave: " +
            highestWaveLeaderboardName
        );

        Debug.Log(
            "=============================================="
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
                "=============================================="
            );

            Debug.LogError(
                "STEAM: COULD NOT FIND NET WORTH LEADERBOARD"
            );

            Debug.LogError(
                "Leaderboard: " +
                netWorthLeaderboardName
            );

            Debug.LogError(
                "IO Failure: " +
                ioFailure
            );

            Debug.LogError(
                "=============================================="
            );

            return;
        }


        netWorthLeaderboard =
            result.m_hSteamLeaderboard;

        netWorthLeaderboardFound = true;


        Debug.Log(
            "STEAM: Net Worth leaderboard FOUND."
        );


        TryUploadStartupValues();
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
                "=============================================="
            );

            Debug.LogError(
                "STEAM: COULD NOT FIND HIGHEST WAVE LEADERBOARD"
            );

            Debug.LogError(
                "Leaderboard: " +
                highestWaveLeaderboardName
            );

            Debug.LogError(
                "IO Failure: " +
                ioFailure
            );

            Debug.LogError(
                "=============================================="
            );

            return;
        }


        highestWaveLeaderboard =
            result.m_hSteamLeaderboard;

        highestWaveLeaderboardFound = true;


        Debug.Log(
            "STEAM: Highest Wave leaderboard FOUND."
        );


        TryUploadStartupValues();
    }


    private void TryUploadStartupValues()
    {
        if (!startupUploadPending)
        {
            return;
        }


        if (!localValuesLoaded)
        {
            return;
        }


        if (!netWorthLeaderboardFound)
        {
            return;
        }


        if (!highestWaveLeaderboardFound)
        {
            return;
        }


        if (uploadingScores)
        {
            return;
        }


        startupUploadPending = false;

        netWorthUploadPending = true;
        highestWaveUploadPending = true;


        Debug.Log(
            "=============================================="
        );

        Debug.Log(
            "STARTUP LEADERBOARD UPLOAD"
        );

        Debug.Log(
            "Uploading current Net Worth and Highest Wave..."
        );

        Debug.Log(
            "Net Worth: $" +
            pendingNetWorth.ToString("N2")
        );

        Debug.Log(
            "Highest Wave: " +
            pendingHighestWave
        );

        Debug.Log(
            "=============================================="
        );


        TryUploadPendingValues();
    }


    public void OnNetWorthChanged(
        float newValue
    )
    {
        if (newValue < 0f)
        {
            newValue = 0f;
        }


        pendingNetWorth = newValue;

        netWorthUploadPending = true;


        Debug.Log(
            "STEAM LEADERBOARD: Net Worth changed to $" +
            newValue.ToString("N2")
        );
    }


    public void OnHighestWaveChanged(
        int newValue
    )
    {
        if (newValue < 0)
        {
            newValue = 0;
        }


        pendingHighestWave = newValue;

        highestWaveUploadPending = true;


        Debug.Log(
            "STEAM LEADERBOARD: Highest Wave changed to " +
            newValue
        );
    }


    private void CheckGlobalUploadCycle()
    {
        long currentUnixTime =
            DateTimeOffset.UtcNow.ToUnixTimeSeconds();


        long currentCycle =
            currentUnixTime /
            UploadIntervalSeconds;


        if (currentCycle <= lastProcessedCycle)
        {
            return;
        }


        if (!netWorthLeaderboardFound)
        {
            return;
        }


        if (!highestWaveLeaderboardFound)
        {
            return;
        }


        lastProcessedCycle =
            currentCycle;


        if (NetWorth.Instance != null)
        {
            pendingNetWorth =
                NetWorth.Instance.TotalNetWorth;
        }


        if (HighestWave.Instance != null)
        {
            pendingHighestWave =
                HighestWave.Instance.HighestWaveReached;
        }


        netWorthUploadPending = true;
        highestWaveUploadPending = true;


        Debug.Log(
            "=============================================="
        );

        Debug.Log(
            "GLOBAL 3-MINUTE LEADERBOARD CYCLE"
        );

        Debug.Log(
            "Cycle: " +
            currentCycle
        );

        Debug.Log(
            "Net Worth: $" +
            pendingNetWorth.ToString("N2")
        );

        Debug.Log(
            "Highest Wave: " +
            pendingHighestWave
        );

        Debug.Log(
            "Starting leaderboard uploads..."
        );

        Debug.Log(
            "=============================================="
        );


        TryUploadPendingValues();
    }


    private void TryUploadPendingValues()
    {
        if (uploadingScores)
        {
            return;
        }


        if (!SteamManager.Initialized)
        {
            return;
        }


        if (!localValuesLoaded)
        {
            return;
        }


        if (
            netWorthUploadPending &&
            netWorthLeaderboardFound
        )
        {
            UploadNetWorth();

            return;
        }


        if (
            highestWaveUploadPending &&
            highestWaveLeaderboardFound
        )
        {
            UploadHighestWave();

            return;
        }
    }


    private void UploadNetWorth()
    {
        if (uploadingScores)
        {
            return;
        }


        if (!netWorthLeaderboardFound)
        {
            return;
        }


        uploadingScores = true;

        netWorthUploadPending = false;


        float actualNetWorth =
            pendingNetWorth;


        int netWorthCents =
            Mathf.RoundToInt(
                actualNetWorth * 100f
            );


        Debug.Log(
            "----------------------------------------------"
        );

        Debug.Log(
            "UPLOADING NET WORTH"
        );

        Debug.Log(
            "CURRENT STEAM USER BEFORE UPLOAD: " +
            SteamFriends.GetPersonaName()
        );

        Debug.Log(
            "CURRENT STEAM ID BEFORE UPLOAD: " +
            SteamUser.GetSteamID()
        );

        Debug.Log(
            "Steam Logged On: " +
            SteamUser.BLoggedOn()
        );

        Debug.Log(
            "Actual value: $" +
            actualNetWorth.ToString("N2")
        );

        Debug.Log(
            "Steam score: " +
            netWorthCents
        );

        Debug.Log(
            "Leaderboard: " +
            netWorthLeaderboardName
        );

        Debug.Log(
            "----------------------------------------------"
        );


        SteamAPICall_t call =
            SteamUserStats.UploadLeaderboardScore(
                netWorthLeaderboard,
                ELeaderboardUploadScoreMethod
                    .k_ELeaderboardUploadScoreMethodForceUpdate,
                netWorthCents,
                null,
                0
            );


        netWorthUploadResult.Set(
            call
        );
    }


    private void OnNetWorthScoreUploaded(
        LeaderboardScoreUploaded_t result,
        bool ioFailure
    )
    {
        if (
            ioFailure ||
            result.m_bSuccess == 0
        )
        {
            Debug.LogError(
                "=============================================="
            );

            Debug.LogError(
                "NET WORTH UPLOAD FAILED"
            );

            Debug.LogError(
                "Steam IO Failure: " +
                ioFailure
            );

            Debug.LogError(
                "Steam Success: " +
                result.m_bSuccess
            );

            Debug.LogError(
                "=============================================="
            );


            netWorthUploadPending = true;

            uploadingScores = false;

            return;
        }


        Debug.Log(
            "=============================================="
        );

        Debug.Log(
            "NET WORTH UPLOAD SUCCESSFUL!"
        );

        Debug.Log(
            "Steam Score: " +
            result.m_nScore
        );

        Debug.Log(
            "Score Changed: " +
            result.m_bScoreChanged
        );

        Debug.Log(
            "New Global Rank: " +
            result.m_nGlobalRankNew
        );

        Debug.Log(
            "=============================================="
        );


        uploadingScores = false;


        if (
            highestWaveUploadPending &&
            highestWaveLeaderboardFound
        )
        {
            UploadHighestWave();
        }
    }


    private void UploadHighestWave()
    {
        if (uploadingScores)
        {
            return;
        }


        if (!highestWaveLeaderboardFound)
        {
            return;
        }


        uploadingScores = true;

        highestWaveUploadPending = false;


        int actualHighestWave =
            pendingHighestWave;


        // Highest Wave can legitimately be 0
        // because it represents completed waves.


        Debug.Log(
            "----------------------------------------------"
        );

        Debug.Log(
            "UPLOADING HIGHEST WAVE"
        );

        Debug.Log(
            "CURRENT STEAM USER BEFORE UPLOAD: " +
            SteamFriends.GetPersonaName()
        );

        Debug.Log(
            "CURRENT STEAM ID BEFORE UPLOAD: " +
            SteamUser.GetSteamID()
        );

        Debug.Log(
            "Steam Logged On: " +
            SteamUser.BLoggedOn()
        );

        Debug.Log(
            "Actual value: " +
            actualHighestWave
        );

        Debug.Log(
            "Steam score: " +
            actualHighestWave
        );

        Debug.Log(
            "Leaderboard: " +
            highestWaveLeaderboardName
        );

        Debug.Log(
            "----------------------------------------------"
        );


        SteamAPICall_t call =
            SteamUserStats.UploadLeaderboardScore(
                highestWaveLeaderboard,
                ELeaderboardUploadScoreMethod
                    .k_ELeaderboardUploadScoreMethodForceUpdate,
                actualHighestWave,
                null,
                0
            );


        highestWaveUploadResult.Set(
            call
        );
    }


    private void OnHighestWaveScoreUploaded(
        LeaderboardScoreUploaded_t result,
        bool ioFailure
    )
    {
        if (
            ioFailure ||
            result.m_bSuccess == 0
        )
        {
            Debug.LogError(
                "=============================================="
            );

            Debug.LogError(
                "HIGHEST WAVE UPLOAD FAILED"
            );

            Debug.LogError(
                "Steam IO Failure: " +
                ioFailure
            );

            Debug.LogError(
                "Steam Success: " +
                result.m_bSuccess
            );

            Debug.LogError(
                "=============================================="
            );


            highestWaveUploadPending = true;

            uploadingScores = false;

            return;
        }


        Debug.Log(
            "=============================================="
        );

        Debug.Log(
            "HIGHEST WAVE UPLOAD SUCCESSFUL!"
        );

        Debug.Log(
            "Steam Score: " +
            result.m_nScore
        );

        Debug.Log(
            "Score Changed: " +
            result.m_bScoreChanged
        );

        Debug.Log(
            "New Global Rank: " +
            result.m_nGlobalRankNew
        );

        Debug.Log(
            "=============================================="
        );


        uploadingScores = false;


        SteamLeaderboard leaderboardDisplay =
            FindFirstObjectByType<SteamLeaderboard>();


        if (leaderboardDisplay != null)
        {
            Debug.Log(
                "Refreshing leaderboard display after uploads..."
            );

            leaderboardDisplay.RefreshLeaderboards();
        }
        else
        {
            Debug.LogWarning(
                "SteamLeaderboard display component was not found in the current scene."
            );
        }


        Debug.Log(
            "=============================================="
        );

        Debug.Log(
            "GLOBAL LEADERBOARD UPDATE COMPLETE"
        );

        Debug.Log(
            "Net Worth + Highest Wave uploaded."
        );

        Debug.Log(
            "Leaderboard display refreshed."
        );

        Debug.Log(
            "Next upload: next global 3-minute boundary."
        );

        Debug.Log(
            "=============================================="
        );
    }
}