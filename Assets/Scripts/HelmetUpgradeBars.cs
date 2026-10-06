using UnityEngine;
using UnityEngine.UI;

public class HelmetUpgradeBars : MonoBehaviour
{
    [Header("Assign the 6 helmet upgrade bars")]
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
        int helmetTier =
            PlayerPrefs.GetInt("HelmetTier", 0);


        helmetTier = Mathf.Clamp(
            helmetTier,
            0,
            5
        );


        // Tier 0 always has the first visual bar unlocked
        int unlockedBars = helmetTier + 1;


        for (int i = 0; i < bars.Length; i++)
        {
            if (bars[i] == null)
                continue;


            if (i < unlockedBars)
            {
                bars[i].color = unlockedColor;
            }
            else
            {
                bars[i].color = lockedColor;
            }
        }
    }
}