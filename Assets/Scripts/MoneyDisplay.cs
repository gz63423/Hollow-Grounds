using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(TextMeshProUGUI))]
public class MoneyDisplayTMP : MonoBehaviour
{
    private TextMeshProUGUI tmpText;


    // ============================================================
    // AWAKE
    // ============================================================

    private void Awake()
    {
        tmpText = GetComponent<TextMeshProUGUI>();

        UpdateTextFromPrefs();
    }


    // ============================================================
    // ENABLE
    // ============================================================

    private void OnEnable()
    {
        // Listen for scene changes.
        SceneManager.sceneLoaded += OnSceneLoaded;


        if (Player.Instance != null)
        {
            Player.Instance.OnMoneyChanged += UpdateText;

            UpdateText(
                Player.Instance.Money
            );
        }
        else
        {
            StartCoroutine(
                WaitForPlayer()
            );
        }
    }


    // ============================================================
    // DISABLE
    // ============================================================

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;


        if (Player.Instance != null)
        {
            Player.Instance.OnMoneyChanged -= UpdateText;
        }
    }


    // ============================================================
    // SCENE LOADED
    // ============================================================

    private void OnSceneLoaded(
        Scene scene,
        LoadSceneMode mode
    )
    {
        /*
         * Rebirth can change PlayerPrefs without firing
         * OnMoneyChanged.
         *
         * Refresh directly from the saved value after the
         * new scene has loaded so the Home UI cannot remain
         * visually stale.
         */

        StartCoroutine(
            RefreshAfterSceneLoad()
        );
    }


    // ============================================================
    // REFRESH AFTER SCENE LOAD
    // ============================================================

    private IEnumerator RefreshAfterSceneLoad()
    {
        // Wait one frame so Player / other persistent systems
        // have time to finish their scene-load initialization.
        yield return null;

        UpdateTextFromPrefs();


        // If the Player exists, make sure we're subscribed.
        if (Player.Instance != null)
        {
            Player.Instance.OnMoneyChanged -= UpdateText;
            Player.Instance.OnMoneyChanged += UpdateText;
        }
    }


    // ============================================================
    // WAIT FOR PLAYER
    // ============================================================

    private IEnumerator WaitForPlayer()
    {
        while (Player.Instance == null)
        {
            UpdateTextFromPrefs();

            yield return null;
        }


        Player.Instance.OnMoneyChanged -= UpdateText;
        Player.Instance.OnMoneyChanged += UpdateText;


        UpdateText(
            Player.Instance.Money
        );
    }


    // ============================================================
    // DISPLAY SAVED MONEY
    // ============================================================

    public void UpdateTextFromPrefs()
    {
        if (tmpText == null)
            return;


        float savedMoney =
            PlayerPrefs.GetFloat(
                "PlayerMoney",
                0f
            );


        tmpText.text =
            MoneyFormatter.FormatMoney(savedMoney)
                .TrimStart('$');
    }


    // ============================================================
    // UPDATE DISPLAY
    // ============================================================

    private void UpdateText(float money)
    {
        if (tmpText == null)
            return;


        tmpText.text =
            MoneyFormatter.FormatMoney(money)
                .TrimStart('$');
    }


    // ============================================================
    // PUBLIC REFRESH
    // ============================================================

    public void RefreshMoneyDisplay()
    {
        UpdateTextFromPrefs();
    }
}

