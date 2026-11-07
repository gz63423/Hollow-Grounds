using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AccountLevel : MonoBehaviour
{
    public static AccountLevel Instance;


    [Header("Account Data")]
    public int currentLevel = 1;
    public int currentXP = 0;


    [Header("XP Settings")]
    public int baseXPRequired = 100;
    public float xpMultiplier = 1.25f;


    //UI
    private TMP_Text levelText;
    private TMP_Text xpText;
    private Slider xpBar;



    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;

            DontDestroyOnLoad(gameObject);

            LoadXP();

            Debug.Log("AccountLevel Loaded");
        }
        else
        {
            Destroy(gameObject);
        }
    }



    public void SetUI(
        TMP_Text newLevelText,
        TMP_Text newXPText,
        Slider newXPBar
    )
    {
        levelText = newLevelText;
        xpText = newXPText;
        xpBar = newXPBar;

        UpdateUI();

        Debug.Log("Account UI Connected");
    }



    public void AddXP(int amount)
    {
        Debug.Log("Adding XP: " + amount);


        currentXP += amount;


        while (currentXP >= GetXPRequired(currentLevel))
        {
            currentXP -= GetXPRequired(currentLevel);
            currentLevel++;

            Debug.Log("LEVEL UP!");
        }


        SaveXP();

        UpdateUI();
    }



    public int GetXPRequired(int level)
    {
        return Mathf.RoundToInt(
            baseXPRequired *
            Mathf.Pow(
                xpMultiplier,
                level - 1
            )
        );
    }



    private void UpdateUI()
    {
        if (levelText != null)
        {
            levelText.text =
                "" + currentLevel;
        }


        if (xpText != null)
        {
            xpText.text =
                currentXP +
                " / " +
                GetXPRequired(currentLevel) +
                " XP";
        }


        if (xpBar != null)
        {
            xpBar.maxValue =
                GetXPRequired(currentLevel);

            xpBar.value =
                currentXP;
        }
    }



    private void SaveXP()
    {
        PlayerPrefs.SetInt(
            "AccountLevel",
            currentLevel
        );

        PlayerPrefs.SetInt(
            "AccountXP",
            currentXP
        );

        PlayerPrefs.Save();

        Debug.Log("XP Saved");
    }



    private void LoadXP()
    {
        currentLevel =
            PlayerPrefs.GetInt(
                "AccountLevel",
                1
            );

        currentXP =
            PlayerPrefs.GetInt(
                "AccountXP",
                0
            );

        Debug.Log(
            "Loaded XP: " +
            currentXP
        );
    }
}