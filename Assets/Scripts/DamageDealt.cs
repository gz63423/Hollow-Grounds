using UnityEngine;

public class DamageDealt : MonoBehaviour
{
    public static DamageDealt Instance;

    public float TotalDamageDealt { get; private set; }

    private const string SaveKey = "DamageDealt";


    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;

            DontDestroyOnLoad(gameObject);


            TotalDamageDealt =
                PlayerPrefs.GetFloat(
                    SaveKey,
                    0f
                );
        }
        else
        {
            Destroy(gameObject);
        }
    }



    public void AddDamage(float amount)
    {
        if (amount <= 0f)
            return;


        TotalDamageDealt += amount;


        PlayerPrefs.SetFloat(
            SaveKey,
            TotalDamageDealt
        );



        AchievementStats.AddDamageDealt(
            amount
        );


        PlayerPrefs.Save();
    }



    public void ResetDamage()
    {
        TotalDamageDealt = 0f;


        PlayerPrefs.SetFloat(
            SaveKey,
            0f
        );


        PlayerPrefs.Save();
    }
}