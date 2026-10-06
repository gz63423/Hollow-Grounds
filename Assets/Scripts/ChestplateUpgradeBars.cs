using UnityEngine;
using UnityEngine.UI;

public class ChestplateUpgradeBars : MonoBehaviour
{
    [Header("Assign the 6 chestplate bars")]
    public Image[] bars = new Image[6];


    [Header("Colors")]
    public Color lockedColor = Color.black;
    public Color unlockedColor = Color.white;



    void Start()
    {
        UpdateBars();
    }



    public void UpdateBars()
    {
        int chestplateTier =
            PlayerPrefs.GetInt(
                "ChestplateTier",
                0
            );


        chestplateTier =
            Mathf.Clamp(
                chestplateTier,
                0,
                5
            );


        // Tier 0 counts as the first visual bar
        int unlockedBars =
            chestplateTier + 1;



        for (int i = 0; i < bars.Length; i++)
        {
            if (bars[i] == null)
                continue;


            if (i < unlockedBars)
            {
                bars[i].color =
                    unlockedColor;
            }
            else
            {
                bars[i].color =
                    lockedColor;
            }
        }
    }
}