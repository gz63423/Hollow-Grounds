using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MusicSlider : MonoBehaviour
{
    public Slider slider;
    public TMP_Text volumeText;

    void Start()
    {
        slider.minValue = 0;
        slider.maxValue = 100;
        slider.wholeNumbers = true;

        slider.value = SettingsManager.Instance.GetVolumePercent();

        UpdateText(slider.value);

        slider.onValueChanged.AddListener(OnSliderChanged);
    }

    void OnSliderChanged(float value)
    {
        SettingsManager.Instance.SetMusicVolume(value);
        UpdateText(value);
    }

    void UpdateText(float value)
    {
        volumeText.text = "Music Volume: " + Mathf.RoundToInt(value) + "%";
    }
}