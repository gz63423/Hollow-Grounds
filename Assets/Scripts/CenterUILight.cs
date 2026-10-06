using UnityEngine;
using UnityEngine.UI;


[RequireComponent(typeof(Image))]
public class CenterUILight : MonoBehaviour
{
    // ============================================================
    // LIGHT
    // ============================================================

    [Header("Center Light")]

    [Tooltip("Color of the center illumination.")]
    public Color lightColor = Color.white;

    [Range(0f, 2f)]
    [Tooltip("Base brightness of the center illumination.")]
    public float intensity = 1f;

    [Range(0.1f, 2f)]
    [Tooltip("Size of the illuminated center. Higher values make the light larger.")]
    public float radius = 1f;

    [Range(0.1f, 5f)]
    [Tooltip("How softly the light fades toward the edges.")]
    public float softness = 2f;


    // ============================================================
    // PULSE
    // ============================================================

    [Header("Optional Pulse")]

    [Tooltip("Makes the center light slowly brighten and dim.")]
    public bool pulse = false;

    [Tooltip("Speed of the pulse.")]
    public float pulseSpeed = 0.5f;

    [Range(0f, 1f)]
    [Tooltip("How much the intensity changes above and below the base intensity.")]
    public float pulseAmount = 0.1f;


    // ============================================================
    // CLICK BOOST
    // ============================================================

    [Header("Click Intensity Boost")]

    [Tooltip("Additional intensity added after Begin is clicked.")]
    public float clickedIntensityBoost = 1f;


    [Tooltip("How long it takes to fade from the normal intensity to the boosted intensity.")]
    public float clickedBoostFadeDuration = 1f;


    // ============================================================
    // INTERNAL CLICK BOOST
    // ============================================================

    private float currentIntensityBoost = 0f;

    private Coroutine intensityBoostCoroutine;


    // ============================================================
    // INTERNAL
    // ============================================================

    private Image image;

    private Texture2D gradientTexture;

    private Sprite gradientSprite;

    private const int TextureSize = 256;


    // ============================================================
    // AWAKE
    // ============================================================

    private void Awake()
    {
        image =
            GetComponent<Image>();

        GenerateGradient();

        UpdateLight();
    }


    // ============================================================
    // UPDATE
    // ============================================================

    private void Update()
    {
        float pulseValue = 0f;

        // --------------------------------------------------------
        // CALCULATE PULSE
        // --------------------------------------------------------

        if (pulse)
        {
            pulseValue =
                Mathf.Sin(
                    Time.unscaledTime *
                    pulseSpeed
                );

            // Convert -1..1 into -1..1 using pulseAmount.
            pulseValue *=
                pulseAmount;
        }


        // --------------------------------------------------------
        // FINAL INTENSITY
        // --------------------------------------------------------

        float currentIntensity =
            intensity +
            pulseValue +
            currentIntensityBoost;


        // --------------------------------------------------------
        // APPLY
        // --------------------------------------------------------

        ApplyLight(
            currentIntensity
        );
    }


    // ============================================================
    // ADD CLICK BOOST
    // ============================================================

    public void ActivateClickedBoost()
    {
        if (intensityBoostCoroutine != null)
        {
            StopCoroutine(
                intensityBoostCoroutine
            );
        }

        intensityBoostCoroutine =
            StartCoroutine(
                ClickedBoostRoutine()
            );
    }


    // ============================================================
    // CLICK BOOST ROUTINE
    // ============================================================

    private System.Collections.IEnumerator ClickedBoostRoutine()
    {
        float duration =
            Mathf.Max(
                0.01f,
                clickedBoostFadeDuration
            );

        float startBoost =
            currentIntensityBoost;

        float targetBoost =
            clickedIntensityBoost;

        float elapsed = 0f;


        while (elapsed < duration)
        {
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

            currentIntensityBoost =
                Mathf.Lerp(
                    startBoost,
                    targetBoost,
                    smoothProgress
                );

            yield return null;
        }


        currentIntensityBoost =
            targetBoost;

        intensityBoostCoroutine = null;
    }


    // ============================================================
    // GENERATE GRADIENT
    // ============================================================

    private void GenerateGradient()
    {
        if (image == null)
        {
            image =
                GetComponent<Image>();
        }


        // --------------------------------------------------------
        // CLEAN UP OLD SPRITE
        // --------------------------------------------------------

        if (gradientSprite != null)
        {
            DestroyGeneratedObject(
                gradientSprite
            );

            gradientSprite = null;
        }


        // --------------------------------------------------------
        // CLEAN UP OLD TEXTURE
        // --------------------------------------------------------

        if (gradientTexture != null)
        {
            DestroyGeneratedObject(
                gradientTexture
            );

            gradientTexture = null;
        }


        // --------------------------------------------------------
        // CREATE TEXTURE
        // --------------------------------------------------------

        gradientTexture =
            new Texture2D(
                TextureSize,
                TextureSize,
                TextureFormat.RGBA32,
                false
            );

        gradientTexture.name =
            "Center UI Light Gradient";

        gradientTexture.wrapMode =
            TextureWrapMode.Clamp;

        gradientTexture.filterMode =
            FilterMode.Bilinear;


        // --------------------------------------------------------
        // CREATE RADIAL GRADIENT
        // --------------------------------------------------------

        Vector2 center =
            new Vector2(
                0.5f,
                0.5f
            );


        for (int y = 0;
             y < TextureSize;
             y++)
        {
            for (int x = 0;
                 x < TextureSize;
                 x++)
            {
                float normalizedX =
                    (float)x /
                    (TextureSize - 1);

                float normalizedY =
                    (float)y /
                    (TextureSize - 1);

                Vector2 position =
                    new Vector2(
                        normalizedX,
                        normalizedY
                    );

                float distance =
                    Vector2.Distance(
                        position,
                        center
                    );

                float adjustedDistance =
                    distance /
                    Mathf.Max(
                        radius,
                        0.01f
                    );

                float alpha =
                    1f -
                    Mathf.Clamp01(
                        adjustedDistance
                    );

                alpha =
                    Mathf.Pow(
                        alpha,
                        softness
                    );

                Color pixelColor =
                    new Color(
                        1f,
                        1f,
                        1f,
                        alpha
                    );

                gradientTexture.SetPixel(
                    x,
                    y,
                    pixelColor
                );
            }
        }


        gradientTexture.Apply();


        // --------------------------------------------------------
        // CREATE SPRITE
        // --------------------------------------------------------

        gradientSprite =
            Sprite.Create(
                gradientTexture,
                new Rect(
                    0,
                    0,
                    TextureSize,
                    TextureSize
                ),
                new Vector2(
                    0.5f,
                    0.5f
                ),
                100f
            );

        gradientSprite.name =
            "Center UI Light Sprite";


        // --------------------------------------------------------
        // APPLY TO IMAGE
        // --------------------------------------------------------

        image.sprite =
            gradientSprite;

        image.type =
            Image.Type.Simple;

        image.preserveAspect =
            false;

        image.raycastTarget =
            false;

        UpdateLight();
    }


    // ============================================================
    // UPDATE LIGHT
    // ============================================================

    private void UpdateLight()
    {
        ApplyLight(
            intensity +
            currentIntensityBoost
        );
    }


    // ============================================================
    // APPLY LIGHT
    // ============================================================

    private void ApplyLight(
        float currentIntensity
    )
    {
        if (image == null)
            return;

        Color color =
            lightColor;

        color.a =
            Mathf.Clamp01(
                currentIntensity
            );

        image.color =
            color;
    }


    // ============================================================
    // DESTROY GENERATED OBJECT
    // ============================================================

    private void DestroyGeneratedObject(
        Object obj
    )
    {
        if (obj == null)
            return;


#if UNITY_EDITOR

        if (!Application.isPlaying)
        {
            DestroyImmediate(
                obj
            );

            return;
        }

#endif


        Destroy(
            obj
        );
    }


    // ============================================================
    // INSPECTOR CHANGES
    // ============================================================

#if UNITY_EDITOR

    private void OnValidate()
    {
        if (image == null)
        {
            image =
                GetComponent<Image>();
        }

        if (image == null)
            return;

        if (this == null)
            return;

        GenerateGradient();

        UpdateLight();
    }

#endif


    // ============================================================
    // CLEANUP
    // ============================================================

    private void OnDestroy()
    {
        if (intensityBoostCoroutine != null)
        {
            StopCoroutine(
                intensityBoostCoroutine
            );

            intensityBoostCoroutine = null;
        }


        if (gradientSprite != null)
        {
            DestroyGeneratedObject(
                gradientSprite
            );

            gradientSprite = null;
        }


        if (gradientTexture != null)
        {
            DestroyGeneratedObject(
                gradientTexture
            );

            gradientTexture = null;
        }
    }
}

