using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SFXSlider : MonoBehaviour
{
    public Slider slider;
    public TMP_Text volumeText;

    // ============================================================
    // START
    // ============================================================

    private void Start()
    {
        if (SettingsManager.Instance == null)
            return;

        slider.minValue = 0;
        slider.maxValue = 100;
        slider.wholeNumbers = true;

        slider.value =
            SettingsManager.Instance.GetSFXVolumePercent();

        UpdateText(slider.value);

        slider.onValueChanged.AddListener(
            OnSliderChanged
        );
    }

    // ============================================================
    // SLIDER CHANGED
    // ============================================================

    private void OnSliderChanged(float value)
    {
        if (SettingsManager.Instance == null)
            return;

        SettingsManager.Instance.SetSFXVolume(
            value
        );

        UpdateText(value);
    }

    // ============================================================
    // UPDATE TEXT
    // ============================================================

    private void UpdateText(float value)
    {
        if (volumeText != null)
        {
            volumeText.text =
                "SFX Volume: " +
                Mathf.RoundToInt(value) +
                "%";
        }
    }

    // ============================================================
    // CLEANUP
    // ============================================================

    private void OnDestroy()
    {
        if (slider != null)
        {
            slider.onValueChanged.RemoveListener(
                OnSliderChanged
            );
        }
    }
}