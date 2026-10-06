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


        // ============================================================
        // ACHIEVEMENT: BRUTALITY
        // ============================================================
        //
        // Tracks ALL damage dealt over the lifetime of the account.
        //
        // This does NOT reset on rebirth.
        //
        // Every source that calls AddDamage() is automatically counted:
        // - Normal enemies
        // - Ranged enemies
        // - Bosses
        //
        // DamageDealt.AddDamage() is the single central damage tracker,
        // so damage cannot accidentally be counted twice by each enemy.
        // ============================================================

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