using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PoisonPowerupEffects : MonoBehaviour
{
    // =========================================================
    // SOUND
    // =========================================================

    [Header("Sound")]

    [Tooltip("Sound played when Poison successfully activates.")]
    public AudioClip poisonActivationSound;

    [Range(0f, 1f)]
    public float poisonActivationVolume = 1f;


    // =========================================================
    // POISON SPRITE
    // =========================================================

    [Header("Poison Sprite")]

    [Tooltip("Sprite used for each Poison visual.")]
    public Sprite poisonSprite;


    // =========================================================
    // SPAWN SETTINGS
    // =========================================================

    [Header("Spawn Settings")]

    [Tooltip("How many Poison visuals spawn on each poisoned enemy.")]
    public int visualsPerEnemy = 3;

    [Tooltip("Time between each Poison visual spawning on an enemy.")]
    public float spawnInterval = 0.12f;


    // =========================================================
    // SPAWN ZONE
    // =========================================================

    [Header("Spawn Zone")]

    [Tooltip("Width of the area where Poison visuals can initially spawn.")]
    public float spawnZoneWidth = 100f;

    [Tooltip("Height of the area where Poison visuals can initially spawn.")]
    public float spawnZoneHeight = 100f;

    [Tooltip("Moves the entire Poison spawn zone vertically relative to the enemy. Positive = up, negative = down.")]
    public float verticalSpawnOffset = 0f;


    // =========================================================
    // SIZE
    // =========================================================

    [Header("Size")]

    [Tooltip("Minimum size of each Poison visual.")]
    public float minSize = 20f;

    [Tooltip("Maximum size of each Poison visual.")]
    public float maxSize = 35f;


    // =========================================================
    // FADE
    // =========================================================

    [Header("Fade")]

    [Tooltip("Starting transparency of each Poison visual.")]
    [Range(0f, 1f)]
    public float startingAlpha = 1f;

    [Tooltip("Time it takes to fade into view.")]
    public float fadeInDuration = 0.05f;

    [Tooltip("Time it takes to fade out.")]
    public float fadeOutDuration = 0.4f;


    // =========================================================
    // FLOATING
    // =========================================================

    [Header("Floating")]

    [Tooltip("Minimum distance the Poison visual floats upward.")]
    public float minFloatDistance = 50f;

    [Tooltip("Maximum distance the Poison visual floats upward.")]
    public float maxFloatDistance = 100f;

    [Tooltip("Time it takes to float upward.")]
    public float floatDuration = 0.8f;


    // =========================================================
    // HORIZONTAL MOVEMENT
    // =========================================================

    [Header("Horizontal Movement")]

    [Tooltip("Minimum horizontal movement while floating.")]
    public float minHorizontalMovement = -15f;

    [Tooltip("Maximum horizontal movement while floating.")]
    public float maxHorizontalMovement = 15f;


    // =========================================================
    // ROTATION
    // =========================================================

    [Header("Rotation")]

    [Tooltip("Allow Poison visuals to randomly rotate.")]
    public bool randomRotation = true;

    [Tooltip("Minimum starting rotation.")]
    public float minRotation = -15f;

    [Tooltip("Maximum starting rotation.")]
    public float maxRotation = 15f;


    // =========================================================
    // ACTIVE VISUALS
    // =========================================================

    private List<GameObject> activePoisonVisuals =
        new List<GameObject>();


    // =========================================================
    // PLAY ACTIVATION SOUND
    // =========================================================

    public void PlayPoisonActivationSound()
    {
        if (poisonActivationSound == null)
            return;

        AudioSource.PlayClipAtPoint(
            poisonActivationSound,
            Camera.main != null
                ? Camera.main.transform.position
                : transform.position,
            poisonActivationVolume
        );
    }


    // =========================================================
    // PLAY POISON EFFECT
    // =========================================================

    public void PlayPoisonEffect(GameObject target)
    {
        if (target == null)
            return;

        if (poisonSprite == null)
        {
            Debug.LogWarning(
                "[PoisonPowerupEffects] Poison Sprite is not assigned."
            );

            return;
        }

        StartCoroutine(
            SpawnPoisonVisuals(target)
        );
    }


    // =========================================================
    // SPAWN POISON VISUALS
    // =========================================================

    private IEnumerator SpawnPoisonVisuals(GameObject target)
    {
        for (int i = 0; i < visualsPerEnemy; i++)
        {
            if (target == null)
                yield break;

            SpawnPoisonVisual(target);

            if (i < visualsPerEnemy - 1)
                yield return new WaitForSeconds(spawnInterval);
        }
    }


    // =========================================================
    // SPAWN SINGLE POISON VISUAL
    // =========================================================

    private void SpawnPoisonVisual(GameObject target)
    {
        if (target == null)
            return;

        Canvas canvas =
            FindFirstObjectByType<Canvas>();

        if (canvas == null)
        {
            Debug.LogWarning(
                "[PoisonPowerupEffects] Could not find Canvas."
            );

            return;
        }

        // ---------------------------------------------------------
        // CREATE UI OBJECT
        // ---------------------------------------------------------

        GameObject poisonVisual =
            new GameObject("PoisonVisual");

        poisonVisual.transform.SetParent(
            canvas.transform,
            false
        );

        activePoisonVisuals.Add(
            poisonVisual
        );


        // ---------------------------------------------------------
        // IMAGE
        // ---------------------------------------------------------

        Image image =
            poisonVisual.AddComponent<Image>();

        image.sprite =
            poisonSprite;

        image.raycastTarget =
            false;


        // ---------------------------------------------------------
        // RECT TRANSFORM
        // ---------------------------------------------------------

        RectTransform rectTransform =
            poisonVisual.GetComponent<RectTransform>();

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


        // ---------------------------------------------------------
        // CAMERA
        // ---------------------------------------------------------

        Camera camera =
            Camera.main;

        if (camera == null)
        {
            Destroy(poisonVisual);

            activePoisonVisuals.Remove(
                poisonVisual
            );

            return;
        }


        // ---------------------------------------------------------
        // TARGET WORLD POSITION
        // ---------------------------------------------------------

        Vector3 worldPosition =
            target.transform.position;

        // Move the entire spawn area vertically.
        worldPosition.y +=
            verticalSpawnOffset;


        // ---------------------------------------------------------
        // WORLD TO SCREEN
        // ---------------------------------------------------------

        Vector3 screenPosition =
            camera.WorldToScreenPoint(
                worldPosition
            );


        // ---------------------------------------------------------
        // CANVAS POSITION
        // ---------------------------------------------------------

        RectTransform canvasRect =
            canvas.GetComponent<RectTransform>();

        Vector2 canvasPosition;

        Camera canvasCamera =
            canvas.renderMode ==
            RenderMode.ScreenSpaceOverlay
                ? null
                : canvas.worldCamera;

        RectTransformUtility
            .ScreenPointToLocalPointInRectangle(
                canvasRect,
                screenPosition,
                canvasCamera,
                out canvasPosition
            );


        // ---------------------------------------------------------
        // RANDOM RECTANGULAR SPAWN AREA
        // ---------------------------------------------------------
        //
        // The visual can spawn anywhere inside:
        //
        //        LEFT          RIGHT
        //          |             |
        //          v             v
        //
        //       +-------------------+
        //       |                   |
        //       |        ENEMY      |
        //       |          X        |
        //       |                   |
        //       +-------------------+
        //
        // spawnZoneWidth  = horizontal range
        // spawnZoneHeight = vertical range
        //
        // The zone is centered around the enemy.
        // ---------------------------------------------------------

        float randomX =
            Random.Range(
                -spawnZoneWidth * 0.5f,
                 spawnZoneWidth * 0.5f
            );

        float randomY =
            Random.Range(
                -spawnZoneHeight * 0.5f,
                 spawnZoneHeight * 0.5f
            );

        canvasPosition.x +=
            randomX;

        canvasPosition.y +=
            randomY;


        // ---------------------------------------------------------
        // APPLY POSITION
        // ---------------------------------------------------------

        rectTransform.anchoredPosition =
            canvasPosition;


        // ---------------------------------------------------------
        // RANDOM ROTATION
        // ---------------------------------------------------------

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


        // ---------------------------------------------------------
        // INITIAL ALPHA
        // ---------------------------------------------------------

        Color color =
            image.color;

        color.a =
            fadeInDuration > 0f
                ? 0f
                : startingAlpha;

        image.color =
            color;


        // ---------------------------------------------------------
        // FLOATING MOVEMENT
        // ---------------------------------------------------------

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


        // ---------------------------------------------------------
        // START ANIMATION
        // ---------------------------------------------------------

        StartCoroutine(
            AnimatePoisonVisual(
                poisonVisual,
                floatDistance,
                horizontalMovement
            )
        );
    }


    // =========================================================
    // ANIMATE POISON VISUAL
    // =========================================================

    private IEnumerator AnimatePoisonVisual(
        GameObject poisonVisual,
        float floatDistance,
        float horizontalMovement)
    {
        if (poisonVisual == null)
            yield break;

        RectTransform rectTransform =
            poisonVisual.GetComponent<RectTransform>();

        Image image =
            poisonVisual.GetComponent<Image>();

        if (rectTransform == null ||
            image == null)
        {
            yield break;
        }


        // ---------------------------------------------------------
        // START / END POSITIONS
        // ---------------------------------------------------------

        Vector2 startPosition =
            rectTransform.anchoredPosition;

        Vector2 endPosition =
            startPosition +
            new Vector2(
                horizontalMovement,
                floatDistance
            );


        // =========================================================
        // FADE IN
        // =========================================================

        if (fadeInDuration > 0f)
        {
            float timer = 0f;

            while (timer < fadeInDuration)
            {
                if (poisonVisual == null)
                    yield break;

                timer +=
                    Time.deltaTime;

                float progress =
                    Mathf.Clamp01(
                        timer /
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


        // =========================================================
        // FLOAT UPWARD
        // =========================================================

        float floatTimer = 0f;

        float duration =
            Mathf.Max(
                0.01f,
                floatDuration
            );

        while (floatTimer < duration)
        {
            if (poisonVisual == null)
                yield break;

            floatTimer +=
                Time.deltaTime;

            float progress =
                Mathf.Clamp01(
                    floatTimer /
                    duration
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


        // =========================================================
        // FADE OUT
        // =========================================================

        if (fadeOutDuration > 0f)
        {
            float timer = 0f;

            float startingFadeAlpha =
                image.color.a;

            while (timer < fadeOutDuration)
            {
                if (poisonVisual == null)
                    yield break;

                timer +=
                    Time.deltaTime;

                float progress =
                    Mathf.Clamp01(
                        timer /
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


        // =========================================================
        // CLEAN UP
        // =========================================================

        if (poisonVisual != null)
        {
            activePoisonVisuals.Remove(
                poisonVisual
            );

            Destroy(
                poisonVisual
            );
        }
    }


    // =========================================================
    // CLEAR ALL POISON VISUALS
    // =========================================================

    public void ClearPoisonVisuals()
    {
        for (int i = 0;
             i < activePoisonVisuals.Count;
             i++)
        {
            if (activePoisonVisuals[i] != null)
            {
                Destroy(
                    activePoisonVisuals[i]
                );
            }
        }

        activePoisonVisuals.Clear();
    }


    // =========================================================
    // DISABLE
    // =========================================================

    private void OnDisable()
    {
        ClearPoisonVisuals();
    }
}
