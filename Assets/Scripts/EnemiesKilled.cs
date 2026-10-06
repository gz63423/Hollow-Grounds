using UnityEngine;

public class EnemiesKilled : MonoBehaviour
{
    public static EnemiesKilled Instance;

    public int TotalEnemiesKilled { get; private set; }

    private const string SaveKey = "EnemiesKilled";


    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;

            DontDestroyOnLoad(gameObject);

            TotalEnemiesKilled =
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


    public void AddKill()
    {
        TotalEnemiesKilled++;

        PlayerPrefs.SetInt(
            SaveKey,
            TotalEnemiesKilled
        );

        PlayerPrefs.Save();
    }


    public void ResetKills()
    {
        TotalEnemiesKilled = 0;

        PlayerPrefs.SetInt(
            SaveKey,
            0
        );

        PlayerPrefs.Save();
    }
}