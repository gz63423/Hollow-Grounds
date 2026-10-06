using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AccountLevelUI : MonoBehaviour
{
    public TMP_Text levelText;
    public TMP_Text xpText;
    public Slider xpBar;


    private void Start()
    {
        if (AccountLevel.Instance != null)
        {
            AccountLevel.Instance.SetUI(
                levelText,
                xpText,
                xpBar
            );
        }
        else
        {
            Debug.LogError(
                "No AccountLevel found!"
            );
        }
    }
}