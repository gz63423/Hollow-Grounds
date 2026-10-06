using UnityEngine;

public class BossesKilled : MonoBehaviour
{
    public static BossesKilled Instance;

    public int TotalBossesKilled { get; private set; }


    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            LoadBossKills();
        }
        else
        {
            Destroy(gameObject);
        }
    }


    public void AddBossKill()
    {
        TotalBossesKilled++;

        // Update Achievement system
        AchievementStats.AddBossKilled();

        SaveBossKills();
    }


    void SaveBossKills()
    {
        PlayerPrefs.SetInt(
            "BossesKilled",
            TotalBossesKilled
        );

        PlayerPrefs.Save();
    }


    void LoadBossKills()
    {
        TotalBossesKilled =
            PlayerPrefs.GetInt(
                "BossesKilled",
                0
            );
    }


    public void ResetBossKills()
    {
        TotalBossesKilled = 0;

        PlayerPrefs.SetInt(
            "BossesKilled",
            0
        );

        PlayerPrefs.Save();
    }
}