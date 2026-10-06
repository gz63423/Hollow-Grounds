using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class SoulstoneDisplayTMP : MonoBehaviour
{
    private TextMeshProUGUI tmpText;

    private int lastSoulstoneAmount = -1;

    void Awake()
    {
        tmpText = GetComponent<TextMeshProUGUI>();

        UpdateTextFromPrefs();
    }

    void OnEnable()
    {
        if (Soulstones.Instance != null)
        {
            Soulstones.Instance.OnSoulstonesChanged += UpdateText;
            UpdateText(Soulstones.Instance.SoulstoneCount);
        }
    }

    void OnDisable()
    {
        if (Soulstones.Instance != null)
        {
            Soulstones.Instance.OnSoulstonesChanged -= UpdateText;
        }
    }

    void Update()
    {
        int currentSoulstones = PlayerPrefs.GetInt(
            "Soulstones",
            0
        );

        // Only update the UI if the amount actually changed.
        if (currentSoulstones != lastSoulstoneAmount)
        {
            UpdateTextFromPrefs();
        }
    }

    public void UpdateTextFromPrefs()
    {
        int savedSoulstones = PlayerPrefs.GetInt(
            "Soulstones",
            0
        );

        lastSoulstoneAmount = savedSoulstones;

        tmpText.text = savedSoulstones.ToString("N0");
    }

    private void UpdateText(int amount)
    {
        lastSoulstoneAmount = amount;

        tmpText.text = amount.ToString("N0");
    }
}