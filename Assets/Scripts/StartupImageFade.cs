using UnityEngine;
using System.Collections;


public class StartupImageFade : MonoBehaviour
{
    [Header("Color Transition Settings")]

    [Tooltip("How long the light takes to transition from black to red.")]
    public float fadeDuration = 1f;

    [Tooltip("Automatically play the color transition when the scene starts.")]
    public bool playOnStart = true;


    // ============================================================
    // CENTER UI LIGHT
    // ============================================================

    [Header("Center UI Light")]

    [Tooltip("Center UI Light that will change from black to red.")]
    public CenterUILight centerUILight;

    [Tooltip("Starting color of the light.")]
    public Color startColor = Color.black;

    [Tooltip("Ending color of the light.")]
    public Color endColor = new Color32(
        255,
        0,
        12,
        255
    );


    // ============================================================
    // START
    // ============================================================

    private void Start()
    {
        if (centerUILight == null)
            return;


        // --------------------------------------------------------
        // INITIAL COLOR
        // --------------------------------------------------------

        centerUILight.lightColor =
            startColor;


        // --------------------------------------------------------
        // PLAY
        // --------------------------------------------------------

        if (playOnStart)
        {
            StartCoroutine(
                ColorTransitionRoutine()
            );
        }
    }


    // ============================================================
    // COLOR TRANSITION
    // ============================================================

    public void FadeIn()
    {
        if (centerUILight == null)
            return;


        StartCoroutine(
            ColorTransitionRoutine()
        );
    }


    // ============================================================
    // COLOR TRANSITION ROUTINE
    // ============================================================

    private IEnumerator ColorTransitionRoutine()
    {
        float duration =
            Mathf.Max(
                0.01f,
                fadeDuration
            );


        // --------------------------------------------------------
        // START COLOR
        // --------------------------------------------------------

        centerUILight.lightColor =
            startColor;


        // --------------------------------------------------------
        // TRANSITION
        // --------------------------------------------------------

        float elapsed =
            0f;


        while (elapsed < duration)
        {
            if (centerUILight == null)
                yield break;


            elapsed +=
                Time.unscaledDeltaTime;


            float progress =
                Mathf.Clamp01(
                    elapsed /
                    duration
                );


            float smoothProgress =
                Mathf.SmoothStep(
                    0f,
                    1f,
                    progress
                );


            // ----------------------------------------------------
            // SHIFT LIGHT COLOR
            // ----------------------------------------------------

            centerUILight.lightColor =
                Color.Lerp(
                    startColor,
                    endColor,
                    smoothProgress
                );


            yield return null;
        }


        // --------------------------------------------------------
        // GUARANTEE FINAL COLOR
        // --------------------------------------------------------

        if (centerUILight != null)
        {
            centerUILight.lightColor =
                endColor;
        }
    }
}
