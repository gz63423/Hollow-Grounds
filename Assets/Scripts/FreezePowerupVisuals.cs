using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FreezePowerupEffects : MonoBehaviour
{
    [Header("Sound")]
    [Tooltip("Sound played when Freeze successfully activates.")]
    public AudioClip freezeActivationSound;

    [Range(0f, 1f)]
    public float freezeActivationVolume = 1f;

    [Header("Screen Tint")]
    [Tooltip("Full-screen Image used for the blue Freeze tint.")]
    public Image freezeTintImage;

    [Tooltip("Color of the Freeze screen tint.")]
    public Color freezeTintColor =
        new Color(
            0.2f,
            0.65f,
            1f,
            0.22f
        );

    [Tooltip("Speed of the tint fading in.")]
    public float tintFadeInSpeed = 6f;

    [Tooltip("Speed of the tint fading out.")]
    public float tintFadeOutSpeed = 0.5f;

    [Header("Falling White Circles")]
    [Tooltip("Parent RectTransform covering the screen.")]
    public RectTransform fallingCircleParent;

    [Tooltip("White circle sprite.")]
    public Sprite circleSprite;

    [Tooltip("Number of falling circles.")]
    public int circleCount = 25;

    [Tooltip("Minimum circle size.")]
    public float minCircleSize = 4f;

    [Tooltip("Maximum circle size.")]
    public float maxCircleSize = 12f;

    [Tooltip("Minimum falling speed.")]
    public float minCircleSpeed = 40f;

    [Tooltip("Maximum falling speed.")]
    public float maxCircleSpeed = 100f;

    [Tooltip("Minimum starting transparency.")]
    [Range(0f, 1f)]
    public float minCircleAlpha = 0.6f;

    [Tooltip("Maximum starting transparency.")]
    [Range(0f, 1f)]
    public float maxCircleAlpha = 1f;

    private Coroutine effectCoroutine;

    private List<GameObject> fallingCircles =
        new List<GameObject>();

    private void Start()
    {
        if (freezeTintImage != null)
        {
            Color color =
                freezeTintImage.color;

            color.a = 0f;

            freezeTintImage.color =
                color;
        }
    }

    public void PlayFreezeEffect(float duration)
    {
        PlayActivationSound();

        if (effectCoroutine != null)
        {
            StopCoroutine(effectCoroutine);

            ClearFallingCircles();
        }

        effectCoroutine =
            StartCoroutine(
                FreezeEffectCoroutine(duration)
            );
    }

    private void PlayActivationSound()
    {
        if (freezeActivationSound == null)
            return;

        AudioSource.PlayClipAtPoint(
            freezeActivationSound,
            Camera.main != null
                ? Camera.main.transform.position
                : transform.position,
            freezeActivationVolume
        );
    }

    private IEnumerator FreezeEffectCoroutine(
        float duration)
    {
        // =====================================================
        // 1. CREATE ALL SNOW IMMEDIATELY
        // =====================================================

        CreateFallingCircles();

        // =====================================================
        // 2. FADE TINT IN
        // =====================================================

        yield return StartCoroutine(
            FadeTint(
                freezeTintColor.a,
                tintFadeInSpeed
            )
        );

        // =====================================================
        // 3. KEEP SNOW + TINT ACTIVE FOR FULL DURATION
        // =====================================================

        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;

            UpdateFallingCircles();

            yield return null;
        }

        // =====================================================
        // 4. FADE EVERYTHING OUT
        // =====================================================

        yield return StartCoroutine(
            FadeOutEverything()
        );

        // =====================================================
        // 5. REMOVE SNOW
        // =====================================================

        ClearFallingCircles();

        effectCoroutine = null;
    }

    // =========================================================
    // TINT FADE IN
    // =========================================================

    private IEnumerator FadeTint(
        float targetAlpha,
        float speed)
    {
        if (freezeTintImage == null)
            yield break;

        float startAlpha =
            freezeTintImage.color.a;

        float fadeDuration =
            speed > 0f
                ? 1f / speed
                : 0f;

        float timer = 0f;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;

            float progress =
                Mathf.Clamp01(
                    timer / fadeDuration
                );

            progress =
                Mathf.SmoothStep(
                    0f,
                    1f,
                    progress
                );

            Color color =
                freezeTintImage.color;

            color.a =
                Mathf.Lerp(
                    startAlpha,
                    targetAlpha,
                    progress
                );

            freezeTintImage.color =
                color;

            // IMPORTANT:
            // Snow continues falling while tint fades in.
            UpdateFallingCircles();

            yield return null;
        }

        Color finalColor =
            freezeTintImage.color;

        finalColor.a =
            targetAlpha;

        freezeTintImage.color =
            finalColor;
    }

    // =========================================================
    // FADE OUT TINT + SNOW
    // =========================================================

    private IEnumerator FadeOutEverything()
    {
        float startTintAlpha =
            freezeTintImage != null
                ? freezeTintImage.color.a
                : 0f;

        // Store each snow particle's ORIGINAL alpha.
        List<float> startingAlphas =
            new List<float>();

        for (int i = 0;
             i < fallingCircles.Count;
             i++)
        {
            GameObject circle =
                fallingCircles[i];

            if (circle == null)
            {
                startingAlphas.Add(0f);
                continue;
            }

            Image image =
                circle.GetComponent<Image>();

            if (image != null)
            {
                startingAlphas.Add(
                    image.color.a
                );
            }
            else
            {
                startingAlphas.Add(0f);
            }
        }

        // =====================================================
        // FADE DURATION
        // =====================================================

        float fadeDuration =
            tintFadeOutSpeed > 0f
                ? 1f / tintFadeOutSpeed
                : 0f;

        float timer = 0f;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;

            float progress =
                Mathf.Clamp01(
                    timer / fadeDuration
                );

            progress =
                Mathf.SmoothStep(
                    0f,
                    1f,
                    progress
                );

            // =================================================
            // TINT
            // =================================================

            if (freezeTintImage != null)
            {
                Color color =
                    freezeTintImage.color;

                color.a =
                    Mathf.Lerp(
                        startTintAlpha,
                        0f,
                        progress
                    );

                freezeTintImage.color =
                    color;
            }

            // =================================================
            // SNOW
            // =================================================

            for (int i = 0;
                 i < fallingCircles.Count;
                 i++)
            {
                GameObject circle =
                    fallingCircles[i];

                if (circle == null)
                    continue;

                Image image =
                    circle.GetComponent<Image>();

                if (image == null)
                    continue;

                float originalAlpha =
                    i < startingAlphas.Count
                        ? startingAlphas[i]
                        : 0f;

                Color color =
                    image.color;

                color.a =
                    Mathf.Lerp(
                        originalAlpha,
                        0f,
                        progress
                    );

                image.color =
                    color;
            }

            // Keep snow moving while it fades.
            UpdateFallingCircles();

            yield return null;
        }

        // =====================================================
        // FORCE EVERYTHING TO ZERO
        // =====================================================

        if (freezeTintImage != null)
        {
            Color color =
                freezeTintImage.color;

            color.a = 0f;

            freezeTintImage.color =
                color;
        }

        for (int i = 0;
             i < fallingCircles.Count;
             i++)
        {
            GameObject circle =
                fallingCircles[i];

            if (circle == null)
                continue;

            Image image =
                circle.GetComponent<Image>();

            if (image == null)
                continue;

            Color color =
                image.color;

            color.a = 0f;

            image.color =
                color;
        }
    }

    // =========================================================
    // CREATE SNOW
    // =========================================================

    private void CreateFallingCircles()
    {
        ClearFallingCircles();

        if (fallingCircleParent == null)
        {
            Debug.LogWarning(
                "FreezePowerupEffects: Falling Circle Parent is not assigned."
            );

            return;
        }

        if (circleSprite == null)
        {
            Debug.LogWarning(
                "FreezePowerupEffects: Circle Sprite is not assigned."
            );

            return;
        }

        Rect rect =
            fallingCircleParent.rect;

        for (int i = 0;
             i < circleCount;
             i++)
        {
            GameObject circle =
                new GameObject(
                    "FreezeCircle"
                );

            circle.transform.SetParent(
                fallingCircleParent,
                false
            );

            Image image =
                circle.AddComponent<Image>();

            image.sprite =
                circleSprite;

            image.raycastTarget =
                false;

            // =================================================
            // SIZE
            // =================================================

            float size =
                Random.Range(
                    minCircleSize,
                    maxCircleSize
                );

            RectTransform rectTransform =
                circle.GetComponent<
                    RectTransform
                >();

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

            rectTransform.sizeDelta =
                new Vector2(
                    size,
                    size
                );

            // =================================================
            // RANDOM STARTING POSITION
            // =================================================

            float x =
                Random.Range(
                    rect.xMin,
                    rect.xMax
                );

            float y =
                Random.Range(
                    rect.yMin,
                    rect.yMax
                );

            rectTransform.anchoredPosition =
                new Vector2(
                    x,
                    y
                );

            // =================================================
            // STARTING TRANSPARENCY
            // =================================================

            float alpha =
                Random.Range(
                    minCircleAlpha,
                    maxCircleAlpha
                );

            image.color =
                new Color(
                    1f,
                    1f,
                    1f,
                    alpha
                );

            // =================================================
            // FALLING SPEED
            // =================================================

            FreezeCircleData data =
                circle.AddComponent<
                    FreezeCircleData
                >();

            data.speed =
                Random.Range(
                    minCircleSpeed,
                    maxCircleSpeed
                );

            fallingCircles.Add(
                circle
            );
        }
    }

    // =========================================================
    // MOVE SNOW
    // =========================================================

    private void UpdateFallingCircles()
    {
        if (fallingCircleParent == null)
            return;

        Rect rect =
            fallingCircleParent.rect;

        for (int i =
             fallingCircles.Count - 1;
             i >= 0;
             i--)
        {
            GameObject circle =
                fallingCircles[i];

            if (circle == null)
            {
                fallingCircles.RemoveAt(i);
                continue;
            }

            FreezeCircleData data =
                circle.GetComponent<
                    FreezeCircleData
                >();

            RectTransform rectTransform =
                circle.GetComponent<
                    RectTransform
                >();

            if (data == null ||
                rectTransform == null)
            {
                continue;
            }

            rectTransform.anchoredPosition +=
                Vector2.down *
                data.speed *
                Time.deltaTime;

            // =================================================
            // LOOP BACK TO TOP
            // =================================================

            if (
                rectTransform.anchoredPosition.y <
                rect.yMin - maxCircleSize
            )
            {
                float newX =
                    Random.Range(
                        rect.xMin,
                        rect.xMax
                    );

                float newY =
                    rect.yMax + maxCircleSize;

                rectTransform.anchoredPosition =
                    new Vector2(
                        newX,
                        newY
                    );
            }
        }
    }

    // =========================================================
    // CLEAR SNOW
    // =========================================================

    private void ClearFallingCircles()
    {
        for (int i = 0;
             i < fallingCircles.Count;
             i++)
        {
            if (fallingCircles[i] != null)
            {
                Destroy(
                    fallingCircles[i]
                );
            }
        }

        fallingCircles.Clear();
    }

    // =========================================================
    // SNOW DATA
    // =========================================================

    private class FreezeCircleData : MonoBehaviour
    {
        public float speed;
    }
}