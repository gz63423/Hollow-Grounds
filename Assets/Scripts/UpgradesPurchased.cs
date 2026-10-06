using UnityEngine;

public class UpgradesPurchased : MonoBehaviour
{
    public static UpgradesPurchased Instance;

    public int TotalUpgradesPurchased { get; private set; }

    private const string SaveKey = "TotalUpgradesPurchased";

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;

            DontDestroyOnLoad(gameObject);

            TotalUpgradesPurchased =
                PlayerPrefs.GetInt(
                    SaveKey,
                    0
                );
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddUpgrade()
    {
        TotalUpgradesPurchased++;

        PlayerPrefs.SetInt(
            SaveKey,
            TotalUpgradesPurchased
        );

        PlayerPrefs.Save();
    }

    public void ResetUpgrades()
    {
        TotalUpgradesPurchased = 0;

        PlayerPrefs.SetInt(
            SaveKey,
            0
        );

        PlayerPrefs.Save();
    }
}