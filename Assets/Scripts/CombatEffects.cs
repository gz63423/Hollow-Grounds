using UnityEngine;
using System.Collections;

public class CombatEffects : MonoBehaviour
{
    public static CombatEffects Instance { get; private set; }

    [Header("Freeze Frame")]
    [Tooltip("How long the game freezes when a hit connects.")]
    public float hitStopDuration = 0.05f;

    private Coroutine freezeCoroutine;
    private float originalTimeScale = 1f;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        DontDestroyOnLoad(gameObject);
    }

    public void FreezeFrame()
    {
        if (freezeCoroutine != null)
            return;

        freezeCoroutine =
            StartCoroutine(
                FreezeFrameRoutine()
            );
    }


    private IEnumerator FreezeFrameRoutine()
    {
        originalTimeScale =
            Time.timeScale;

        Time.timeScale = 0f;

        float elapsed = 0f;

        while (elapsed < hitStopDuration)
        {
            elapsed +=
                Time.unscaledDeltaTime;

            yield return null;
        }

        Time.timeScale =
            originalTimeScale;

        freezeCoroutine = null;
    }
}