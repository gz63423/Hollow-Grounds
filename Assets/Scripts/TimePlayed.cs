using UnityEngine;

public class TimePlayed : MonoBehaviour
{
    public static TimePlayed Instance;

    public float TotalSeconds { get; private set; }

    private const string TimeKey = "TimePlayed";


    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;

            DontDestroyOnLoad(gameObject);

            LoadTime();
        }
        else
        {
            Destroy(gameObject);
        }
    }


    void Update()
    {
        TotalSeconds += Time.deltaTime;

        PlayerPrefs.SetFloat(
            TimeKey,
            TotalSeconds
        );
    }


    void LoadTime()
    {
        TotalSeconds =
            PlayerPrefs.GetFloat(
                TimeKey,
                0f
            );
    }


    public string GetFormattedTime()
    {
        int total =
            Mathf.FloorToInt(TotalSeconds);


        int days = total / 86400;

        total %= 86400;


        int hours = total / 3600;

        total %= 3600;


        int minutes = total / 60;

        int seconds = total % 60;



        string result = "";


        if (days > 0)
            result += days + " D, ";


        if (hours > 0 || days > 0)
            result += hours + " H, ";


        if (minutes > 0 || hours > 0 || days > 0)
            result += minutes + " M, ";


        result += seconds + " S";


        return result;
    }



    public void ResetTime()
    {
        TotalSeconds = 0f;


        PlayerPrefs.SetFloat(
            TimeKey,
            0f
        );


        PlayerPrefs.Save();
    }
}