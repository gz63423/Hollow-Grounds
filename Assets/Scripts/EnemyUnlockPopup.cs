using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class EnemyUnlockPopup : MonoBehaviour
{
    [Header("References")]
    public Canvas canvas;
    public TMP_Text messageText;
    public Image enemyImage;

    [Header("Spawn Position")]
    public float yOffset = 1f;

    [Header("Movement")]
    public float floatDistance = 60f;
    public float duration = 1f;

    [Header("Pop Effect")]
    public float startingScale = 1.2f;

    private Camera mainCamera;
    private RectTransform canvasRect;
    private RectTransform popupRect;

    public void Setup(
        string message,
        Vector3 popupWorldPosition)
    {
        // ==========================================
        // CAMERA
        // ==========================================

        mainCamera = Camera.main;

        if (mainCamera == null)
        {
            Debug.LogError(
                "EnemyUnlockPopup: Main Camera not found!"
            );

            return;
        }

        // ==========================================
        // CANVAS
        // ==========================================

        if (canvas == null)
        {
            Debug.LogError(
                "EnemyUnlockPopup: Canvas is not assigned!"
            );

            return;
        }

        canvasRect =
            canvas.GetComponent<RectTransform>();

        if (canvasRect == null)
        {
            Debug.LogError(
                "EnemyUnlockPopup: Canvas has no RectTransform!"
            );

            return;
        }

        // ==========================================
        // MESSAGE TEXT
        // ==========================================

        if (messageText == null)
        {
            Debug.LogError(
                "EnemyUnlockPopup: Message Text is not assigned!"
            );

            return;
        }

        // ==========================================
        // APPLY Y OFFSET
        // ==========================================

        popupWorldPosition.y += yOffset;

        // ==========================================
        // USE THE TEXT RECT
        // ==========================================

        popupRect =
            messageText.GetComponent<RectTransform>();

        if (popupRect == null)
        {
            Debug.LogError(
                "EnemyUnlockPopup: Message Text has no RectTransform!"
            );

            return;
        }

        // ==========================================
        // SET MESSAGE
        // ==========================================

        messageText.text = message;

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
        // MESSAGE TEXT
        // ==========================================

        if (messageText != null)
        {
            Color textColor =
                messageText.color;

            messageText.color =
                new Color(
                    textColor.r,
                    textColor.g,
                    textColor.b,
                    alpha
                );
        }

        // ==========================================
        // ENEMY IMAGE
        // ==========================================

        if (enemyImage != null)
        {
            Color imageColor =
                enemyImage.color;

            enemyImage.color =
                new Color(
                    imageColor.r,
                    imageColor.g,
                    imageColor.b,
                    alpha
                );
        }
    }
}

