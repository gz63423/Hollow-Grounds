using UnityEngine;

public class PlayerDeaths : MonoBehaviour
{
    public static PlayerDeaths Instance;

    public int TotalDeaths { get; private set; }

    private const string SaveKey = "PlayerDeaths";


    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;

            DontDestroyOnLoad(gameObject);


            TotalDeaths =
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



    public void AddDeath()
    {
        TotalDeaths++;


        PlayerPrefs.SetInt(
            SaveKey,
            TotalDeaths
        );


        PlayerPrefs.Save();
    }



    public void ResetDeaths()
    {
        TotalDeaths = 0;


        PlayerPrefs.SetInt(
            SaveKey,
            0
        );


        PlayerPrefs.Save();
    }
}