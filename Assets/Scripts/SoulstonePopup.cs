using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class SoulstonePopup : MonoBehaviour
{
    [Header("References")]
    public Canvas canvas;
    public TMP_Text amountText;
    public Image soulstoneImage;

    [Header("Spawn Position")]
    public float yOffset = 1f;
    public float horizontalOffset = 0f;

    [Header("Movement")]
    public float floatDistance = 60f;
    public float duration = 1f;

    [Header("Pop Effect")]
    public float startingScale = 1.2f;

    private Camera mainCamera;
    private RectTransform canvasRect;
    private RectTransform popupRect;


    public void Setup(
        int amount,
        Vector3 popupWorldPosition)
    {
        // ==========================================
        // CAMERA
        // ==========================================

        mainCamera = Camera.main;

        if (mainCamera == null)
        {
            Debug.LogError(
                "SoulstonePopup: Main Camera not found!"
            );

            return;
        }


        // ==========================================
        // CANVAS
        // ==========================================

        if (canvas == null)
        {
            Debug.LogError(
                "SoulstonePopup: Canvas is not assigned!"
            );

            return;
        }

        canvasRect =
            canvas.GetComponent<RectTransform>();

        if (canvasRect == null)
        {
            Debug.LogError(
                "SoulstonePopup: Canvas has no RectTransform!"
            );

            return;
        }


        // ==========================================
        // AMOUNT TEXT
        // ==========================================

        if (amountText == null)
        {
            Debug.LogError(
                "SoulstonePopup: Amount Text is not assigned!"
            );

            return;
        }


        // ==========================================
        // APPLY OFFSETS
        // ==========================================

        popupWorldPosition.x += horizontalOffset;
        popupWorldPosition.y += yOffset;


        // ==========================================
        // USE THE TEXT RECT
        // ==========================================

        popupRect =
            amountText.GetComponent<RectTransform>();

        if (popupRect == null)
        {
            Debug.LogError(
                "SoulstonePopup: Amount Text has no RectTransform!"
            );

            return;
        }


        // ==========================================
        // SET SOULSTONE AMOUNT
        // ==========================================

        amountText.text =
            amount.ToString();


        // ==========================================
        // MAKE VISIBLE
        // ==========================================

        SetAlpha(1f);


        // ==========================================
        // WORLD → SCREEN
        // ==========================================

        Vector3 screenPosition =
            mainCamera.WorldToScreenPoint(
                popupWorldPosition
            );


        // ==========================================
        // SCREEN → CANVAS
        // ==========================================

        Vector2 canvasPosition;

        Camera canvasCamera =
            canvas.renderMode ==
            RenderMode.ScreenSpaceOverlay
                ? null
                : canvas.worldCamera;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            screenPosition,
            canvasCamera,
            out canvasPosition
        );


        // ==========================================
        // POSITION
        // ==========================================

        popupRect.anchoredPosition =
            canvasPosition;


        // ==========================================
        // START SCALE
        // ==========================================

        popupRect.localScale =
            Vector3.one * startingScale;


        // ==========================================
        // START ANIMATION
        // ==========================================

        StartCoroutine(
            AnimatePopup()
        );
    }


    private IEnumerator AnimatePopup()
    {
        float elapsed = 0f;

        Vector2 startPosition =
            popupRect.anchoredPosition;


        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            float progress =
                Mathf.Clamp01(
                    elapsed / duration
                );


            // ==========================================
            // FLOAT UP
            // ==========================================

            popupRect.anchoredPosition =
                startPosition +
                Vector2.up *
                (floatDistance * progress);


            // ==========================================
            // POP SCALE
            // ==========================================

            float scaleProgress =
                Mathf.Clamp01(
                    progress / 0.15f
                );

            popupRect.localScale =
                Vector3.Lerp(
                    Vector3.one * startingScale,
                    Vector3.one,
                    scaleProgress
                );


            // ==========================================
            // FADE
            // ==========================================

            float alpha =
                Mathf.Lerp(
                    1f,
                    0f,
                    progress
                );

            SetAlpha(alpha);

            yield return null;
        }


        Destroy(gameObject);
    }


    private void SetAlpha(float alpha)
    {
        // ==========================================
        // AMOUNT TEXT
        // ==========================================

        if (amountText != null)
        {
            Color textColor =
                amountText.color;

            amountText.color =
                new Color(
                    textColor.r,
                    textColor.g,
                    textColor.b,
                    alpha
                );
        }


        // ==========================================
        // SOULSTONE IMAGE
        // ==========================================

        if (soulstoneImage != null)
        {
            Color imageColor =
                soulstoneImage.color;

            soulstoneImage.color =
                new Color(
                    imageColor.r,
                    imageColor.g,
                    imageColor.b,
                    alpha
                );
        }
    }
}