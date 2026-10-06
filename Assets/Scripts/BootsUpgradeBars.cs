using UnityEngine;
using UnityEngine.UI;

public class BootsUpgradeBars : MonoBehaviour
{
    [Header("Assign the 6 boots upgrade bars")]
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
        int bootsTier =
            PlayerPrefs.GetInt(
                "BootsTier",
                0
            );


        bootsTier =
            Mathf.Clamp(
                bootsTier,
                0,
                5
            );


        int unlockedBars =
            bootsTier + 1;



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