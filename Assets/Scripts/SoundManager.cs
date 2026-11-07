using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }


    // ============================================================
    // SOUND CLIPS
    // ============================================================

    [Header("Player")]
    public AudioClip[] playerAttack = new AudioClip[5];
    public AudioClip[] playerHit = new AudioClip[5];
    public AudioClip[] playerDamaged = new AudioClip[5];
    public AudioClip[] playerDeath = new AudioClip[5];
    public AudioClip[] playerRespawn = new AudioClip[5];


    [Header("Enemy")]
    public AudioClip[] enemyHit = new AudioClip[5];
    public AudioClip[] enemyDeath = new AudioClip[5];
    public AudioClip[] rangedEnemyShot = new AudioClip[5];
    public AudioClip[] enemyUnlocked = new AudioClip[5];


    [Header("Upgrades")]
    public AudioClip[] upgradePurchased = new AudioClip[5];


    [Header("Wave")]
    public AudioClip[] waveStarted = new AudioClip[5];


    [Header("Boss")]
    public AudioClip[] boss = new AudioClip[5];


    [Header("UI")]
    public AudioClip[] uiClick = new AudioClip[5];


    [Header("Rebirth / Cutscene")]
    public AudioClip rebirthIntroSFX;


    // ============================================================
    // MASTER VOLUME
    // ============================================================

    [Header("Master SFX Volume")]
    [Range(0f, 2f)]
    public float sfxVolume = 1f;


    // ============================================================
    // SECTION VOLUMES
    // ============================================================

    [Header("Player Volume")]
    [Range(0f, 2f)]
    public float playerVolume = 1f;


    [Header("Enemy Volume")]
    [Range(0f, 2f)]
    public float enemyVolume = 1f;


    [Header("Upgrade Volume")]
    [Range(0f, 2f)]
    public float upgradeVolume = 1f;


    [Header("Wave Volume")]
    [Range(0f, 2f)]
    public float waveVolume = 1f;


    [Header("Boss Volume")]
    [Range(0f, 2f)]
    public float bossVolume = 1f;


    [Header("UI Volume")]
    [Range(0f, 2f)]
    public float uiVolume = 1f;


    [Header("Rebirth Volume")]
    [Range(0f, 2f)]
    public float rebirthVolume = 1f;


    // ============================================================
    // GLOBAL SFX PITCH
    // ============================================================

    [Header("Global SFX Pitch")]
    public bool randomizeSFXPitch = true;

    [Range(0.5f, 1.5f)]
    public float minSFXPitch = 0.9f;

    [Range(0.5f, 1.5f)]
    public float maxSFXPitch = 1.1f;


    private AudioSource audioSource;


    // ============================================================
    // LAST PLAYED CLIPS
    // ============================================================

    private AudioClip lastPlayerAttack;
    private AudioClip lastPlayerHit;
    private AudioClip lastPlayerDamaged;
    private AudioClip lastPlayerDeath;
    private AudioClip lastPlayerRespawn;

    private AudioClip lastEnemyHit;
    private AudioClip lastEnemyDeath;
    private AudioClip lastRangedEnemyShot;
    private AudioClip lastEnemyUnlocked;

    private AudioClip lastUpgradePurchased;

    private AudioClip lastWaveStarted;

    private AudioClip lastBoss;

    private AudioClip lastUIClick;


    // ============================================================
    // UNITY
    // ============================================================

    private void Awake()
    {
        if (Instance != null &&
            Instance != this)
        {
            Destroy(gameObject);
            return;
        }


        Instance = this;

        DontDestroyOnLoad(gameObject);


        audioSource =
            GetComponent<AudioSource>();


        if (audioSource == null)
        {
            audioSource =
                gameObject.AddComponent<AudioSource>();
        }


        audioSource.playOnAwake = false;

        audioSource.loop = false;

        audioSource.pitch = 1f;


        SceneManager.sceneLoaded +=
            OnSceneLoaded;


        SetupAllButtons();


        if (SettingsManager.Instance != null)
        {
            sfxVolume =
                SettingsManager.Instance.sfxVolume;
        }
    }


    private void OnDestroy()
    {
        if (Instance == this)
        {
            SceneManager.sceneLoaded -=
                OnSceneLoaded;
        }
    }


    // ============================================================
    // SCENE LOADED
    // ============================================================

    private void OnSceneLoaded(
        Scene scene,
        LoadSceneMode mode)
    {
        SetupAllButtons();


        if (SettingsManager.Instance != null)
        {
            sfxVolume =
                SettingsManager.Instance.sfxVolume;
        }
    }


    // ============================================================
    // BUTTON SETUP
    // ============================================================

    private void SetupAllButtons()
    {
        Button[] buttons =
            FindObjectsByType<Button>(
                FindObjectsInactive.Include,
                FindObjectsSortMode.None
            );


        foreach (Button button in buttons)
        {
            if (button == null)
                continue;


            button.onClick.RemoveListener(
                PlayUIClick
            );


            button.onClick.AddListener(
                PlayUIClick
            );
        }
    }


    // ============================================================
    // RANDOM SOUND
    // ============================================================

    private void PlayRandomSound(
        AudioClip[] clips,
        ref AudioClip lastPlayed,
        float sectionVolume)
    {
        if (audioSource == null)
            return;


        if (clips == null ||
            clips.Length == 0)
        {
            return;
        }


        int validClipCount = 0;


        for (
            int i = 0;
            i < clips.Length;
            i++
        )
        {
            if (clips[i] != null)
            {
                validClipCount++;
            }
        }


        if (validClipCount == 0)
            return;


        float finalVolume =
            sfxVolume *
            sectionVolume;


        AudioClip selectedClip = null;


        if (validClipCount == 1)
        {
            for (
                int i = 0;
                i < clips.Length;
                i++
            )
            {
                if (clips[i] != null)
                {
                    selectedClip =
                        clips[i];

                    break;
                }
            }
        }
        else
        {
            int attempts = 0;


            while (
                selectedClip == null &&
                attempts < 20
            )
            {
                int randomIndex =
                    Random.Range(
                        0,
                        clips.Length
                    );


                if (
                    clips[randomIndex] != null &&
                    clips[randomIndex] != lastPlayed
                )
                {
                    selectedClip =
                        clips[randomIndex];
                }


                attempts++;
            }


            if (selectedClip == null)
            {
                for (
                    int i = 0;
                    i < clips.Length;
                    i++
                )
                {
                    if (
                        clips[i] != null &&
                        clips[i] != lastPlayed
                    )
                    {
                        selectedClip =
                            clips[i];

                        break;
                    }
                }
            }
        }


        if (selectedClip == null)
            return;


        float originalPitch =
            audioSource.pitch;


        if (randomizeSFXPitch)
        {
            audioSource.pitch =
                Random.Range(
                    minSFXPitch,
                    maxSFXPitch
                );
        }
        else
        {
            audioSource.pitch = 1f;
        }


        audioSource.PlayOneShot(
            selectedClip,
            finalVolume
        );


        lastPlayed =
            selectedClip;


        audioSource.pitch =
            originalPitch;
    }


    // ============================================================
    // REBIRTH INTRO SFX
    // ============================================================

    public void PlayRebirthIntroSFX()
    {
        if (audioSource == null)
            return;


        if (rebirthIntroSFX == null)
            return;


        float finalVolume =
            sfxVolume *
            rebirthVolume;


        float originalPitch =
            audioSource.pitch;


        audioSource.pitch = 1f;


        audioSource.PlayOneShot(
            rebirthIntroSFX,
            finalVolume
        );


        audioSource.pitch =
            originalPitch;
    }


    // ============================================================
    // PLAYER
    // ============================================================

    public void PlayPlayerAttack()
    {
        PlayRandomSound(
            playerAttack,
            ref lastPlayerAttack,
            playerVolume
        );
    }


    public void PlayPlayerHit()
    {
        PlayRandomSound(
            playerHit,
            ref lastPlayerHit,
            playerVolume
        );
    }


    public void PlayPlayerDamaged()
    {
        PlayRandomSound(
            playerDamaged,
            ref lastPlayerDamaged,
            playerVolume
        );
    }


    public void PlayPlayerDeath()
    {
        PlayRandomSound(
            playerDeath,
            ref lastPlayerDeath,
            playerVolume
        );
    }


    public void PlayPlayerRespawn()
    {
        PlayRandomSound(
            playerRespawn,
            ref lastPlayerRespawn,
            playerVolume
        );
    }


    // ============================================================
    // ENEMY
    // ============================================================

    public void PlayEnemyHit()
    {
        PlayRandomSound(
            enemyHit,
            ref lastEnemyHit,
            enemyVolume
        );
    }


    public void PlayEnemyDeath()
    {
        PlayRandomSound(
            enemyDeath,
            ref lastEnemyDeath,
            enemyVolume
        );
    }


    public void PlayRangedEnemyShot()
    {
        PlayRandomSound(
            rangedEnemyShot,
            ref lastRangedEnemyShot,
            enemyVolume
        );
    }


    public void PlayEnemyUnlocked()
    {
        PlayRandomSound(
            enemyUnlocked,
            ref lastEnemyUnlocked,
            enemyVolume
        );
    }


    // ============================================================
    // UPGRADES
    // ============================================================

    public void PlayUpgradePurchased()
    {
        PlayRandomSound(
            upgradePurchased,
            ref lastUpgradePurchased,
            upgradeVolume
        );
    }


    // ============================================================
    // WAVES
    // ============================================================

    public void PlayWaveStarted()
    {
        PlayRandomSound(
            waveStarted,
            ref lastWaveStarted,
            waveVolume
        );
    }


    // ============================================================
    // BOSS
    // ============================================================

    public void PlayBoss()
    {
        PlayRandomSound(
            boss,
            ref lastBoss,
            bossVolume
        );
    }


    // ============================================================
    // UI
    // ============================================================

    public void PlayUIClick()
    {
        PlayRandomSound(
            uiClick,
            ref lastUIClick,
            uiVolume
        );
    }


    // ============================================================
    // MASTER VOLUME
    // ============================================================

    public void SetSFXVolume(float volume)
    {
        sfxVolume =
            Mathf.Clamp(
                volume,
                0f,
                2f
            );
    }

    // ============================================================
    // POWERUP ACTIVATION SFX
    // ============================================================

    public void PlayPowerupSFX(AudioClip clip, float volume = 1f)
    {
        if (audioSource == null || clip == null)
            return;

        float masterVolume = sfxVolume;

        if (SettingsManager.Instance != null)
        {
            masterVolume =
                SettingsManager.Instance.GetSFXVolumePercent() / 100f;
        }

        audioSource.PlayOneShot(
            clip,
            Mathf.Clamp01(masterVolume) *
            Mathf.Clamp01(volume)
        );
    }
}
