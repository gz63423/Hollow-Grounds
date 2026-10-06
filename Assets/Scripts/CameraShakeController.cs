using UnityEngine;

public class CameraShakeController : MonoBehaviour
{
    void Start()
    {
        ApplySetting();
    }

    void ApplySetting()
    {
        if (SettingsManager.Instance != null)
        {
            bool enabled =
                SettingsManager.Instance.cameraShakeEnabled;

            gameObject.SetActive(enabled);
        }
    }
}