using System.Collections;
using UnityEngine;


[RequireComponent(typeof(AudioSource))]
public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance;


    // ============================================================
    // PLAYLIST
    // ============================================================

    [Header("Playlist")]

    public AudioClip[] songs;

    public bool shuffle = true;


    // ============================================================
    // FADE SETTINGS
    // ============================================================

    [Header("Fade Settings")]

    [Tooltip("Normal fade duration used when changing songs.")]
    public float fadeDuration = 1f;


    // ============================================================
    // STATE
    // ============================================================

    private AudioSource source;

    private int currentSong = -1;

    private bool changingSong = false;


    // ============================================================
    // MUSIC LOCK
    // ============================================================

    private bool musicLocked = false;


    // ============================================================
    // AWAKE
    // ============================================================

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;

            DontDestroyOnLoad(gameObject);

            source =
                GetComponent<AudioSource>();

            source.loop = false;

            source.playOnAwake = false;
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
        PreloadSongs();

        PlayNextSong();
    }


    // ============================================================
    // UPDATE
    // ============================================================

    private void Update()
    {
        // --------------------------------------------------------
        // MUSIC LOCKED
        // --------------------------------------------------------

        if (musicLocked)
            return;


        // --------------------------------------------------------
        // SONG FINISHED
        // --------------------------------------------------------

        if (!source.isPlaying && !changingSong)
        {
            StartCoroutine(
                ChangeSong()
            );
        }
    }


    // ============================================================
    // PRELOAD SONGS
    // ============================================================

    private void PreloadSongs()
    {
        if (songs == null)
            return;

        foreach (AudioClip song in songs)
        {
            if (song != null)
            {
                song.LoadAudioData();
            }
        }
    }


    // ============================================================
    // CHANGE SONG
    // ============================================================

    private IEnumerator ChangeSong()
    {
        changingSong = true;


        yield return new WaitForSecondsRealtime(
            0.2f
        );


        if (musicLocked)
        {
            changingSong = false;
            yield break;
        }


        // --------------------------------------------------------
        // FADE OUT
        // --------------------------------------------------------

        yield return StartCoroutine(
            FadeOut()
        );


        if (musicLocked)
        {
            changingSong = false;
            yield break;
        }


        // --------------------------------------------------------
        // NEXT SONG
        // --------------------------------------------------------

        PlayNextSong();


        // --------------------------------------------------------
        // FADE IN
        // --------------------------------------------------------

        if (!musicLocked)
        {
            yield return StartCoroutine(
                FadeIn()
            );
        }


        changingSong = false;
    }


    // ============================================================
    // PLAY NEXT SONG
    // ============================================================

    private void PlayNextSong()
    {
        PlayNextSong(false);
    }


    // ============================================================
    // PLAY NEXT SONG
    // ============================================================

    private void PlayNextSong(bool startSilent)
    {
        if (musicLocked)
            return;


        if (songs == null ||
            songs.Length == 0)
        {
            return;
        }


        // --------------------------------------------------------
        // SELECT SONG
        // --------------------------------------------------------

        if (shuffle)
        {
            int next;


            if (songs.Length == 1)
            {
                next = 0;
            }
            else
            {
                do
                {
                    next =
                        Random.Range(
                            0,
                            songs.Length
                        );
                }
                while (
                    next ==
                    currentSong
                );
            }


            currentSong =
                next;
        }
        else
        {
            currentSong++;


            if (currentSong >= songs.Length)
            {
                currentSong = 0;
            }
        }


        // --------------------------------------------------------
        // INVALID CLIP
        // --------------------------------------------------------

        if (songs[currentSong] == null)
        {
            return;
        }


        // --------------------------------------------------------
        // ASSIGN CLIP
        // --------------------------------------------------------

        source.clip =
            songs[currentSong];


        // --------------------------------------------------------
        // VOLUME
        // --------------------------------------------------------

        if (startSilent)
        {
            source.volume = 0f;
        }
        else
        {
            UpdateVolume();
        }


        // --------------------------------------------------------
        // PLAY
        // --------------------------------------------------------

        source.Play();
    }


    // ============================================================
    // FADE OUT
    // ============================================================

    private IEnumerator FadeOut()
    {
        float startVolume =
            source.volume;


        if (fadeDuration <= 0f)
        {
            source.volume = 0f;

            source.Stop();

            yield break;
        }


        float elapsed = 0f;


        while (elapsed < fadeDuration)
        {
            elapsed +=
                Time.unscaledDeltaTime;


            float progress =
                Mathf.Clamp01(
                    elapsed /
                    fadeDuration
                );


            source.volume =
                Mathf.Lerp(
                    startVolume,
                    0f,
                    progress
                );


            yield return null;
        }


        source.volume = 0f;

        source.Stop();
    }


    // ============================================================
    // FADE IN
    // ============================================================

    private IEnumerator FadeIn()
    {
        if (musicLocked)
            yield break;


        float targetVolume =
            GetTargetVolume();


        if (fadeDuration <= 0f)
        {
            source.volume =
                targetVolume;

            yield break;
        }


        float elapsed = 0f;

        float startVolume =
            source.volume;


        while (elapsed < fadeDuration)
        {
            if (musicLocked)
            {
                source.volume = 0f;

                yield break;
            }


            elapsed +=
                Time.unscaledDeltaTime;


            float progress =
                Mathf.Clamp01(
                    elapsed /
                    fadeDuration
                );


            float smoothProgress =
                Mathf.SmoothStep(
                    0f,
                    1f,
                    progress
                );


            source.volume =
                Mathf.Lerp(
                    startVolume,
                    targetVolume,
                    smoothProgress
                );


            yield return null;
        }


        source.volume =
            targetVolume;
    }


    // ============================================================
    // TUTORIAL MUSIC FADE
    // ============================================================

    public void StartTutorialMusicFade(
        float delay,
        float tutorialFadeDuration
    )
    {
        if (source == null)
            return;


        StopAllCoroutines();


        StartCoroutine(
            TutorialMusicRoutine(
                delay,
                tutorialFadeDuration
            )
        );
    }


    // ============================================================
    // TUTORIAL MUSIC ROUTINE
    // ============================================================

    private IEnumerator TutorialMusicRoutine(
        float delay,
        float tutorialFadeDuration
    )
    {
        // --------------------------------------------------------
        // LOCK MUSIC
        // --------------------------------------------------------

        musicLocked = true;

        changingSong = true;


        // --------------------------------------------------------
        // COMPLETELY SILENCE MUSIC
        // --------------------------------------------------------

        source.volume = 0f;


        // --------------------------------------------------------
        // WAIT
        // --------------------------------------------------------

        float safeDelay =
            Mathf.Max(
                0f,
                delay
            );


        if (safeDelay > 0f)
        {
            float elapsedDelay = 0f;


            while (elapsedDelay < safeDelay)
            {
                elapsedDelay +=
                    Time.unscaledDeltaTime;

                yield return null;
            }
        }


        // --------------------------------------------------------
        // CHECK WHETHER CURRENT SONG IS STILL PLAYING
        // --------------------------------------------------------

        if (!source.isPlaying)
        {
            // ----------------------------------------------------
            // CURRENT SONG ACTUALLY FINISHED
            // ----------------------------------------------------
            //
            // Since MusicManager was locked during the delay,
            // Update() could not advance the playlist.
            //
            // Only now do we start the next song.
            // ----------------------------------------------------

            musicLocked = false;

            PlayNextSong(true);
        }
        else
        {
            // ----------------------------------------------------
            // CURRENT SONG IS STILL PLAYING
            // ----------------------------------------------------
            //
            // DO NOT CALL Play().
            //
            // This preserves the exact position in the song.
            // ----------------------------------------------------

            musicLocked = false;
        }


        // --------------------------------------------------------
        // FADE FROM ZERO
        // --------------------------------------------------------

        float safeFadeDuration =
            Mathf.Max(
                0.01f,
                tutorialFadeDuration
            );


        float targetVolume =
            GetTargetVolume();


        float elapsed = 0f;


        while (elapsed < safeFadeDuration)
        {
            // ----------------------------------------------------
            // KEEP SONG FROM BEING REPLACED
            // ----------------------------------------------------

            changingSong = true;


            elapsed +=
                Time.unscaledDeltaTime;


            float progress =
                Mathf.Clamp01(
                    elapsed /
                    safeFadeDuration
                );


            float smoothProgress =
                Mathf.SmoothStep(
                    0f,
                    1f,
                    progress
                );


            source.volume =
                Mathf.Lerp(
                    0f,
                    targetVolume,
                    smoothProgress
                );


            yield return null;
        }


        // --------------------------------------------------------
        // FORCE FINAL VOLUME
        // --------------------------------------------------------

        source.volume =
            targetVolume;


        changingSong = false;


        // --------------------------------------------------------
        // FINAL MUSIC STATE
        // --------------------------------------------------------

        musicLocked = false;
    }


    // ============================================================
    // UPDATE VOLUME
    // ============================================================

    public void UpdateVolume()
    {
        if (source == null)
            return;


        if (musicLocked)
        {
            source.volume = 0f;

            return;
        }


        source.volume =
            GetTargetVolume();
    }


    // ============================================================
    // GET TARGET VOLUME
    // ============================================================

    private float GetTargetVolume()
    {
        if (SettingsManager.Instance != null)
        {
            return Mathf.Clamp01(
                SettingsManager.Instance.musicVolume
            );
        }


        return 1f;
    }


    // ============================================================
    // MUTE MUSIC
    // ============================================================

    public void MuteMusic()
    {
        musicLocked = true;


        if (source == null)
            return;


        source.volume = 0f;
    }


    // ============================================================
    // UNLOCK MUSIC
    // ============================================================

    public void UnlockMusic()
    {
        musicLocked = false;


        if (source == null)
            return;


        // --------------------------------------------------------
        // IMPORTANT:
        // If the current song is still playing, NEVER call
        // source.Play(). That would restart it from the beginning.
        // --------------------------------------------------------

        if (source.isPlaying)
        {
            UpdateVolume();

            return;
        }


        // --------------------------------------------------------
        // Current song actually finished.
        // --------------------------------------------------------

        PlayNextSong();
    }


    // ============================================================
    // RESTORE MUSIC
    // ============================================================

    public void RestoreMusic()
    {
        UnlockMusic();
    }


    // ============================================================
    // CHECK IF MUSIC IS LOCKED
    // ============================================================

    public bool IsMusicLocked()
    {
        return musicLocked;
    }
}