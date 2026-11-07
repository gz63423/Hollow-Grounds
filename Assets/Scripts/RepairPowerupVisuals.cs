using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RepairPowerupEffects : MonoBehaviour
{
    [Header("Sound")]
    [Tooltip("Sound played when Repair successfully activates.")]
    public AudioClip repairActivationSound;

    [Range(0f, 1f)]
    public float repairActivationVolume = 1f;


    [Header("Repair Sprite")]
    [Tooltip("Sprite used for each Repair visual.")]
    public Sprite repairSprite;

    [Tooltip("Parent UI RectTransform where the Repair visuals will spawn.")]
    public RectTransform repairSpawnZone;


    [Header("Spawn Settings")]
    [Tooltip("Total number of Repair visuals spawned.")]
    public int repairVisualCount = 7;

    [Tooltip("Time between each Repair visual spawning.")]
    public float spawnInterval = 0.12f;


    [Header("Size")]
    [Tooltip("Minimum size of each Repair visual.")]
    public float minSize = 20f;

    [Tooltip("Maximum size of each Repair visual.")]
    public float maxSize = 35f;


    [Header("Fade")]
    [Tooltip("Starting alpha of each Repair visual.")]
    [Range(0f, 1f)]
    public float startingAlpha = 1f;

    [Tooltip("Time it takes to fade into view.")]
    public float fadeInDuration = 0.05f;

    [Tooltip("Time it takes to fade out.")]
    public float fadeOutDuration = 0.4f;


    [Header("Floating")]
    [Tooltip("Minimum distance the visual floats upward.")]
    public float minFloatDistance = 50f;

    [Tooltip("Maximum distance the visual floats upward.")]
    public float maxFloatDistance = 100f;

    [Tooltip("Time it takes to float upward.")]
    public float floatDuration = 0.8f;


    [Header("Horizontal Movement")]
    [Tooltip("Minimum horizontal movement while floating.")]
    public float minHorizontalMovement = -15f;

    [Tooltip("Maximum horizontal movement while floating.")]
    public float maxHorizontalMovement = 15f;


    [Header("Rotation")]
    [Tooltip("Allow Repair visuals to randomly rotate.")]
    public bool randomRotation = true;

    [Tooltip("Minimum starting rotation.")]
    public float minRotation = -15f;

    [Tooltip("Maximum starting rotation.")]
    public float maxRotation = 15f;


    private Coroutine effectCoroutine;

    private List<GameObject> activeRepairVisuals =
        new List<GameObject>();


    // =========================================================
    // PLAY REPAIR EFFECT
    // =========================================================

    public void PlayRepairEffect()
    {
        PlayActivationSound();

        if (effectCoroutine != null)
        {
            StopCoroutine(effectCoroutine);

            ClearRepairVisuals();
        }

        effectCoroutine =
            StartCoroutine(
                RepairEffectCoroutine()
            );
    }


    // =========================================================
    // SOUND
    // =========================================================

    private void PlayActivationSound()
    {
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlayPowerupSFX(
                repairActivationSound,
                repairActivationVolume
            );
        }
    }


    // =========================================================
    // MAIN EFFECT
    // =========================================================

    private IEnumerator RepairEffectCoroutine()
    {
        for (int i = 0;
             i < repairVisualCount;
             i++)
        {
            SpawnRepairVisual();

            yield return new WaitForSeconds(
                spawnInterval
            );
        }

        // Give the final visual time to finish.
        yield return new WaitForSeconds(
            floatDuration +
            fadeOutDuration
        );

        ClearRepairVisuals();

        effectCoroutine = null;
    }


    // =========================================================
    // SPAWN VISUAL
    // =========================================================

    private void SpawnRepairVisual()
    {
        if (repairSprite == null)
        {
            Debug.LogWarning(
                "RepairPowerupEffects: Repair Sprite is not assigned."
            );

            return;
        }

        if (repairSpawnZone == null)
        {
            Debug.LogWarning(
                "RepairPowerupEffects: Repair Spawn Zone is not assigned."
            );

            return;
        }

        // =====================================================
        // CREATE UI OBJECT
        // =====================================================

        GameObject repairVisual =
            new GameObject(
                "RepairVisual"
            );

        repairVisual.transform.SetParent(
            repairSpawnZone,
            false
        );

        activeRepairVisuals.Add(
            repairVisual
        );

        Image image =
            repairVisual.AddComponent<Image>();

        image.sprite =
            repairSprite;

        image.raycastTarget =
            false;


        RectTransform rectTransform =
            repairVisual.GetComponent<RectTransform>();


        // =====================================================
        // RANDOM SIZE
        // =====================================================

        float size =
            Random.Range(
                minSize,
                maxSize
            );

        rectTransform.sizeDelta =
            new Vector2(
                size,
                size
            );


        // =====================================================
        // RANDOM SPAWN POSITION
        // =====================================================

        Rect zone =
            repairSpawnZone.rect;

        float x =
            Random.Range(
                zone.xMin,
                zone.xMax
            );

        float y =
            Random.Range(
                zone.yMin,
                zone.yMax
            );

        rectTransform.anchorMin =
            new Vector2(
                0.5f,
                0.5f
            );

        rectTransform.anchorMax =
            new Vector2(
                0.5f,
                0.5f
            );

        rectTransform.pivot =
            new Vector2(
                0.5f,
                0.5f
            );

        rectTransform.anchoredPosition =
            new Vector2(
                x,
                y
            );


        // =====================================================
        // RANDOM ROTATION
        // =====================================================

        if (randomRotation)
        {
            float rotation =
                Random.Range(
                    minRotation,
                    maxRotation
                );

            rectTransform.localRotation =
                Quaternion.Euler(
                    0f,
                    0f,
                    rotation
                );
        }


        // =====================================================
        // START TRANSPARENT
        // =====================================================

        Color color =
            image.color;

        color.a =
            fadeInDuration > 0f
                ? 0f
                : startingAlpha;

        image.color =
            color;


        // =====================================================
        // FLOAT SETTINGS
        // =====================================================

        float floatDistance =
            Random.Range(
                minFloatDistance,
                maxFloatDistance
            );

        float horizontalMovement =
            Random.Range(
                minHorizontalMovement,
                maxHorizontalMovement
            );


        // =====================================================
        // START ANIMATION
        // =====================================================

        StartCoroutine(
            AnimateRepairVisual(
                repairVisual,
                floatDistance,
                horizontalMovement
            )
        );
    }


    // =========================================================
    // ANIMATE VISUAL
    // =========================================================

    private IEnumerator AnimateRepairVisual(
        GameObject repairVisual,
        float floatDistance,
        float horizontalMovement)
    {
        if (repairVisual == null)
            yield break;

        RectTransform rectTransform =
            repairVisual.GetComponent<RectTransform>();

        Image image =
            repairVisual.GetComponent<Image>();

        if (rectTransform == null ||
            image == null)
        {
            yield break;
        }


        // =====================================================
        // POSITIONS
        // =====================================================

        Vector2 startPosition =
            rectTransform.anchoredPosition;

        Vector2 endPosition =
            startPosition +
            new Vector2(
                horizontalMovement,
                floatDistance
            );


        // =====================================================
        // TIMING
        // =====================================================

        float totalDuration =
            Mathf.Max(
                0.01f,
                floatDuration
            );

        float timer = 0f;


        // =====================================================
        // FADE IN
        // =====================================================

        if (fadeInDuration > 0f)
        {
            float fadeTimer = 0f;

            while (
                fadeTimer <
                fadeInDuration
            )
            {
                if (repairVisual == null)
                    yield break;

                fadeTimer +=
                    Time.deltaTime;

                float progress =
                    Mathf.Clamp01(
                        fadeTimer /
                        fadeInDuration
                    );

                progress =
                    Mathf.SmoothStep(
                        0f,
                        1f,
                        progress
                    );

                Color color =
                    image.color;

                color.a =
                    Mathf.Lerp(
                        0f,
                        startingAlpha,
                        progress
                    );

                image.color =
                    color;

                yield return null;
            }
        }


        // =====================================================
        // FLOAT UPWARD
        // =====================================================

        timer = 0f;

        while (
            timer <
            totalDuration
        )
        {
            if (repairVisual == null)
                yield break;

            timer +=
                Time.deltaTime;

            float progress =
                Mathf.Clamp01(
                    timer /
                    totalDuration
                );

            float smoothProgress =
                Mathf.SmoothStep(
                    0f,
                    1f,
                    progress
                );

            rectTransform.anchoredPosition =
                Vector2.Lerp(
                    startPosition,
                    endPosition,
                    smoothProgress
                );

            yield return null;
        }


        // =====================================================
        // FADE OUT
        // =====================================================

        if (fadeOutDuration > 0f)
        {
            float fadeTimer = 0f;

            float startingFadeAlpha =
                image.color.a;

            while (
                fadeTimer <
                fadeOutDuration
            )
            {
                if (repairVisual == null)
                    yield break;

                fadeTimer +=
                    Time.deltaTime;

                float progress =
                    Mathf.Clamp01(
                        fadeTimer /
                        fadeOutDuration
                    );

                progress =
                    Mathf.SmoothStep(
                        0f,
                        1f,
                        progress
                    );

                Color color =
                    image.color;

                color.a =
                    Mathf.Lerp(
                        startingFadeAlpha,
                        0f,
                        progress
                    );

                image.color =
                    color;

                yield return null;
            }
        }


        // =====================================================
        // DESTROY
        // =====================================================

        if (repairVisual != null)
        {
            activeRepairVisuals.Remove(
                repairVisual
            );

            Destroy(
                repairVisual
            );
        }
    }


    // =========================================================
    // CLEAR
    // =========================================================

    private void ClearRepairVisuals()
    {
        for (int i = 0;
             i < activeRepairVisuals.Count;
             i++)
        {
            if (
                activeRepairVisuals[i] != null
            )
            {
                Destroy(
                    activeRepairVisuals[i]
                );
            }
        }

        activeRepairVisuals.Clear();
    }


    // =========================================================
    // CLEANUP
    // =========================================================

    private void OnDisable()
    {
        if (effectCoroutine != null)
        {
            StopCoroutine(
                effectCoroutine
            );

            effectCoroutine = null;
        }

        ClearRepairVisuals();
    }
}