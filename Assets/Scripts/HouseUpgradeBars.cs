using UnityEngine;
using UnityEngine.UI;

public class HouseUpgradeBars : MonoBehaviour
{
    [Header("Assign the 10 UI Images in order")]
    public Image[] bars = new Image[10];

    [Header("Your 10 hex colors")]
    public string[] barHexColors = new string[10]
    {
        "#5B5B5B",
        "#8A9F88",
        "#9AC08F",
        "#94C371",
        "#B7DD57",
        "#DFEE5D",
        "#E7DA2D",
        "#F1B92F",
        "#FF8408",
        "#FF3800"
    };

    private Color[] colors;


    void Awake()
    {
        ConvertColors();
    }


    void Start()
    {
        UpdateBars();
    }


    void ConvertColors()
    {
        colors = new Color[barHexColors.Length];

        for (int i = 0; i < barHexColors.Length; i++)
        {
            if (!ColorUtility.TryParseHtmlString(barHexColors[i], out colors[i]))
            {
                colors[i] = Color.white;
                Debug.LogWarning("Invalid hex color: " + barHexColors[i]);
            }
        }
    }


    public void UpdateBars()
    {
        int level = PlayerPrefs.GetInt("HouseLevel", 0);

        // Level 1 = 1 bar
        // Level 10 = 10 bars
        // Level 11 = reset
        int unlockedBars = (level % 10) + 1;


        Debug.Log("House Level: " + (level + 1));
        Debug.Log("Unlocked Bars: " + unlockedBars);


        for (int i = 0; i < bars.Length; i++)
        {
            if (bars[i] == null)
            {
                Debug.LogWarning("Missing bar at index " + i);
                continue;
            }


            if (i < unlockedBars)
            {
                bars[i].color = colors[i];
            }
            else
            {
                bars[i].color = Color.black;
            }
        }
    }
}