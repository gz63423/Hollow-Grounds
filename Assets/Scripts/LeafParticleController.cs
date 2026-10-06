using UnityEngine;

public class LeafParticleController : MonoBehaviour
{
    void Start()
    {
        ApplySetting();
    }

    void ApplySetting()
    {
        if (SettingsManager.Instance != null)
        {
            gameObject.SetActive(SettingsManager.Instance.particlesEnabled);
        }
    }
}