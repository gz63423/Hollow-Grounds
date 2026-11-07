using UnityEngine;

public class HighestWave : MonoBehaviour
{
    public static HighestWave Instance;

    public int HighestWaveReached { get; private set; }

    private const string SaveKey = "HighestWave";


    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;

            DontDestroyOnLoad(gameObject);

            HighestWaveReached =
                PlayerPrefs.GetInt(
                    SaveKey,
                    0
                );

            if (HighestWaveReached < 0)
            {
                HighestWaveReached = 0;

                PlayerPrefs.SetInt(
                    SaveKey,
                    HighestWaveReached
                );

                PlayerPrefs.Save();
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }


    public void ReachedWave(int wave)
    {
        if (wave <= HighestWaveReached)
            return;

        HighestWaveReached = wave;

        PlayerPrefs.SetInt(
            SaveKey,
            HighestWaveReached
        );

        PlayerPrefs.Save();

        Debug.Log(
            "HIGHEST COMPLETED WAVE CHANGED: " +
            HighestWaveReached
        );

        if (SteamLeaderboardUploader.Instance != null)
        {
            SteamLeaderboardUploader.Instance
                .OnHighestWaveChanged(
                    HighestWaveReached
                );
        }
    }


    public void ResetHighestWave()
    {
        HighestWaveReached = 0;

        PlayerPrefs.SetInt(
            SaveKey,
            0
        );

        PlayerPrefs.Save();

        Debug.Log(
            "HIGHEST WAVE RESET TO 0"
        );

        if (SteamLeaderboardUploader.Instance != null)
        {
            SteamLeaderboardUploader.Instance
                .OnHighestWaveChanged(
                    HighestWaveReached
                );
        }
    }
}