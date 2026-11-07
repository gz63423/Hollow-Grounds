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


        if (SteamLeaderboardUploader.Instance != null)
        {
            SteamLeaderboardUploader.Instance
                .OnNetWorthChanged(TotalNetWorth);
        }
    }



    private void SaveNetWorth()
    {
        PlayerPrefs.SetFloat(
            NetWorthKey,
            TotalNetWorth
        );

        PlayerPrefs.Save();
    }




    public void ResetNetWorth()
    {
        TotalNetWorth = 0f;

        SaveNetWorth();


        Debug.Log(
            "NET WORTH RESET TO $0.00"
        );


        if (SteamLeaderboardUploader.Instance != null)
        {
            SteamLeaderboardUploader.Instance
                .OnNetWorthChanged(0f);
        }
    }
}

