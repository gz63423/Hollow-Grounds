using UnityEngine;

public class NetWorth : MonoBehaviour
{
    public static NetWorth Instance;

    public float TotalNetWorth { get; private set; }

    private const string NetWorthKey = "NetWorth";


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;

            DontDestroyOnLoad(gameObject);

            LoadNetWorth();
        }
        else
        {
            Destroy(gameObject);
        }
    }



    // ============================================================
    // LOAD NET WORTH
    // ============================================================

    private void LoadNetWorth()
    {
        TotalNetWorth =
            PlayerPrefs.GetFloat(
                NetWorthKey,
                0f
            );


        Debug.Log(
            "NET WORTH LOADED: " +
            MoneyFormatter.FormatMoney(TotalNetWorth)
        );
    }



    // ============================================================
    // ADD MONEY EARNED
    // ============================================================

    public void AddMoneyEarned(float amount)
    {
        if (amount <= 0f)
        {
            return;
        }


        TotalNetWorth += amount;

        SaveNetWorth();


        Debug.Log(
            "NET WORTH CHANGED: " +
            MoneyFormatter.FormatMoney(TotalNetWorth)
        );


        // Tell Steam immediately that Net Worth changed.
        if (SteamLeaderboardUploader.Instance != null)
        {
            SteamLeaderboardUploader.Instance
                .OnNetWorthChanged(TotalNetWorth);
        }
    }



    // ============================================================
    // SAVE NET WORTH
    // ============================================================

    private void SaveNetWorth()
    {
        PlayerPrefs.SetFloat(
            NetWorthKey,
            TotalNetWorth
        );

        PlayerPrefs.Save();
    }



    // ============================================================
    // RESET NET WORTH
    // ============================================================

    public void ResetNetWorth()
    {
        TotalNetWorth = 0f;

        SaveNetWorth();


        Debug.Log(
            "NET WORTH RESET TO $0.00"
        );


        // NOTE:
        // This does NOT happen during rebirth unless
        // something specifically calls ResetNetWorth().
        //
        // If you intentionally reset Net Worth, Steam
        // will be notified of the reset.

        if (SteamLeaderboardUploader.Instance != null)
        {
            SteamLeaderboardUploader.Instance
                .OnNetWorthChanged(0f);
        }
    }
}

