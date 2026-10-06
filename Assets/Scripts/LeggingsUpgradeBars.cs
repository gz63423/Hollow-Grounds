using UnityEngine;
using UnityEngine.UI;

public class LeggingsUpgradeBars : MonoBehaviour
{
    [Header("Assign the 6 leggings upgrade bars")]
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
        int leggingsTier =
            PlayerPrefs.GetInt(
                "LeggingsTier",
                0
            );


        leggingsTier =
            Mathf.Clamp(
                leggingsTier,
                0,
                5
            );


        int unlockedBars =
            leggingsTier + 1;



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