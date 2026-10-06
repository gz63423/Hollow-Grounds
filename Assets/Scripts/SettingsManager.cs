using UnityEngine;

public class SettingsManager : MonoBehaviour
{
    public static SettingsManager Instance;

    // ============================================================
    // AUDIO
    // ============================================================

    [Header("Audio")]

    [Range(0f, 1f)]
    public float musicVolume = 1f;

    [Range(0f, 2f)]
    public float sfxVolume = 2f;


    // ============================================================
    // PARTICLES
    // ============================================================

    [Header("Particles")]

    public bool particlesEnabled = true;


    // ============================================================
    // CAMERA SHAKE
    // ============================================================

    [Header("Camera Shake")]

    public bool cameraShakeEnabled = true;


    // ============================================================
    // PLAYER PREF KEYS
    // ============================================================

    private const string MusicVolumeKey =
        "MusicVolume";

    private const string SFXVolumeKey =
        "SFXVolume";

    private const string ParticlesKey =
        "Particles";

    private const string CameraShakeKey =
        "CameraShake";


    // ============================================================
    // AWAKE
    // ============================================================

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;

            DontDestroyOnLoad(gameObject);

            LoadSettings();
        }
        else
        {
            Destroy(gameObject);
        }
    }


    // ============================================================
    // START
    // ============================================================

    private void Start()
    {
        if (MusicManager.Instance != null)
        {
            MusicManager.Instance.UpdateVolume();
        }


        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.sfxVolume =
                sfxVolume;
        }
    }


    // ============================================================
    // MUSIC VOLUME
    // ============================================================

    public void SetMusicVolume(
        float percent
    )
    {
        percent =
            Mathf.Clamp(
                percent,
                0f,
                100f
            );


        musicVolume =
            percent / 100f;


        PlayerPrefs.SetFloat(
            MusicVolumeKey,
            percent
        );


        PlayerPrefs.Save();


        if (MusicManager.Instance != null)
        {
            MusicManager.Instance.UpdateVolume();
        }
    }


    public float GetVolumePercent()
    {
        return musicVolume * 100f;
    }


    // ============================================================
    // SFX VOLUME
    // ============================================================

    public void SetSFXVolume(
        float percent
    )
    {
        percent =
            Mathf.Clamp(
                percent,
                0f,
                100f
            );


        sfxVolume =
            (percent / 100f) * 2f;


        PlayerPrefs.SetFloat(
            SFXVolumeKey,
            percent
        );


        PlayerPrefs.Save();


        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.sfxVolume =
                sfxVolume;
        }
    }


    public float GetSFXVolumePercent()
    {
        return (sfxVolume / 2f) * 100f;
    }


    // ============================================================
    // PARTICLES
    // ============================================================

    public void SetParticles(
        bool enabled
    )
    {
        particlesEnabled =
            enabled;


        PlayerPrefs.SetInt(
            ParticlesKey,
            enabled ? 1 : 0
        );


        PlayerPrefs.Save();
    }


    // ============================================================
    // CAMERA SHAKE
    // ============================================================

    public void SetCameraShake(
        bool enabled
    )
    {
        cameraShakeEnabled =
            enabled;


        PlayerPrefs.SetInt(
            CameraShakeKey,
            enabled ? 1 : 0
        );


        PlayerPrefs.Save();
    }


    // ============================================================
    // LOAD SETTINGS
    // ============================================================

    private void LoadSettings()
    {
        // --------------------------------------------------------
        // MUSIC
        // --------------------------------------------------------

        float savedMusicVolume =
            PlayerPrefs.GetFloat(
                MusicVolumeKey,
                100f
            );


        musicVolume =
            savedMusicVolume / 100f;


        // --------------------------------------------------------
        // SFX
        // --------------------------------------------------------

        float savedSFXVolume =
            PlayerPrefs.GetFloat(
                SFXVolumeKey,
                100f
            );


        sfxVolume =
            (savedSFXVolume / 100f) * 2f;


        // --------------------------------------------------------
        // PARTICLES
        // --------------------------------------------------------

        particlesEnabled =
            PlayerPrefs.GetInt(
                ParticlesKey,
                1
            ) == 1;


        // --------------------------------------------------------
        // CAMERA SHAKE
        // --------------------------------------------------------

        cameraShakeEnabled =
            PlayerPrefs.GetInt(
                CameraShakeKey,
                1
            ) == 1;
    }
}