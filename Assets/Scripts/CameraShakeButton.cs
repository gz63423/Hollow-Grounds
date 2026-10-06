using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CameraShakeButton : MonoBehaviour
{
    public TMP_Text stateText;
    public Image buttonImage;

    public Color offColor =
        new Color32(118, 0, 0, 255); // #760000

    public Color onColor =
        new Color32(76, 154, 76, 255); // green

    // ============================================================
    // START
    // ============================================================

    private void Start()
    {
        UpdateButton();
    }

    // ============================================================
    // TOGGLE CAMERA SHAKE
    // ============================================================

    public void ToggleCameraShake()
    {
        if (SettingsManager.Instance == null)
            return;

        SettingsManager.Instance.SetCameraShake(
            !SettingsManager.Instance.cameraShakeEnabled
        );

        UpdateButton();
    }

    // ============================================================
    // UPDATE BUTTON
    // ============================================================

    private void UpdateButton()
    {
        if (SettingsManager.Instance == null)
            return;

        bool enabled =
            SettingsManager.Instance.cameraShakeEnabled;

        if (stateText != null)
        {
            stateText.text =
                "Camera Shake: " +
                (enabled ? "On" : "Off");
        }

        if (buttonImage != null)
        {
            buttonImage.color =
                enabled
                ? onColor
                : offColor;
        }
    }
}