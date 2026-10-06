using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class MoneyPopup : MonoBehaviour
{
    [Header("References")]
    public Canvas canvas;
    public TMP_Text amountText;
    public Image coinImage;

    [Header("Movement")]
    public float floatDistance = 60f;
    public float duration = 1f;

    [Header("Pop Effect")]
    public float startingScale = 1.2f;

    private Camera mainCamera;
    private RectTransform canvasRect;
    private RectTransform popupRect;


    public void Setup(float amount, Vector3 popupWorldPosition)
    {
        mainCamera = Camera.main;

        if (mainCamera == null)
        {
            Debug.LogError(
                "MoneyPopup: Main Camera not found!"
            );

            return;
        }

        if (canvas == null)
        {
            Debug.LogError(
                "MoneyPopup: Canvas is not assigned!"
            );

            return;
        }

        if (amountText == null)
        {
            Debug.LogError(
                "MoneyPopup: Amount Text is not assigned!"
            );

            return;
        }


        // Get the RectTransform from the Canvas
        canvasRect =
            canvas.GetComponent<RectTransform>();


        // Get the RectTransform from the actual UI element
        popupRect =
            amountText.GetComponent<RectTransform>();


        // Set money amount
        amountText.text =
    MoneyFormatter.FormatMoney(amount).TrimStart('$');


        // Make everything visible
        SetAlpha(1f);


        // Convert popup anchor world position to screen position
        Vector3 screenPosition =
            mainCamera.WorldToScreenPoint(
                popupWorldPosition
            );


        // Convert screen position into Canvas local position
        Vector2 canvasPosition;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            screenPosition,
            canvas.worldCamera,
            out canvasPosition
        );


        // Position popup
        popupRect.anchoredPosition =
            canvasPosition;


        // Start slightly larger
        popupRect.localScale =
            Vector3.one * startingScale;


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
                elapsed / duration;


            // Float upward
            popupRect.anchoredPosition =
                startPosition +
                Vector2.up *
                (floatDistance * progress);


            // Shrink from pop size to normal size
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


            // Fade out
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


        if (coinImage != null)
        {
            Color coinColor =
                coinImage.color;

            coinImage.color =
                new Color(
                    coinColor.r,
                    coinColor.g,
                    coinColor.b,
                    alpha
                );
        }
    }
}

