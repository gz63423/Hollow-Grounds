using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class RebirthTabFadeIn : MonoBehaviour
{
    [Header("Timing")]
    public float delayBeforeFade = 34f;
    public float fadeDuration = 4f;

    private CanvasGroup canvasGroup;
    private Button[] buttons;

    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();

        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();

        // Find all buttons inside this tab
        buttons = GetComponentsInChildren<Button>(true);
    }

    void Start()
    {
        // Completely invisible
        canvasGroup.alpha = 0f;

        // Prevent interaction
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        // Make sure every button is disabled
        foreach (Button button in buttons)
        {
            button.interactable = false;
        }

        StartCoroutine(FadeIn());
    }

    IEnumerator FadeIn()
    {
        // Wait before appearing
        yield return new WaitForSeconds(delayBeforeFade);

        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;

            canvasGroup.alpha =
                Mathf.Clamp01(elapsed / fadeDuration);

            yield return null;
        }

        // Fully visible
        canvasGroup.alpha = 1f;

        // Enable interaction
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;

        // Enable buttons
        foreach (Button button in buttons)
        {
            button.interactable = true;
        }
    }
}