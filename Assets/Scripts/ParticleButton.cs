using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ParticleButton : MonoBehaviour
{
    public TMP_Text stateText;
    public Image buttonImage;

    public Color offColor = new Color32(118, 0, 0, 255); // #760000
    public Color onColor = new Color32(76, 154, 76, 255); // green

    void Start()
    {
        UpdateButton();
    }

    public void ToggleParticles()
    {
        SettingsManager.Instance.SetParticles(!SettingsManager.Instance.particlesEnabled);

        UpdateButton();
    }

    void UpdateButton()
    {
        bool enabled = SettingsManager.Instance.particlesEnabled;

        stateText.text = "Leaf Particles: " + (enabled ? "On" : "Off");

        if (buttonImage != null)
        {
            buttonImage.color = enabled ? onColor : offColor;
        }
    }
}