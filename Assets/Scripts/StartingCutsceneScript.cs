using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CutsceneProgress : MonoBehaviour
{
    [Header("Frames")]
    public Image[] images = new Image[9];

    [Header("Dialogue")]
    public TMP_Text[] texts = new TMP_Text[9];

    [Header("Buttons")]
    public Button nextButton;
    public Button newButton;

    private int currentIndex = 0;

    void Start()
    {
        // Make all images invisible
        foreach (Image img in images)
        {
            if (img != null)
            {
                Color c = img.color;
                c.a = 0f;
                img.color = c;
            }
        }

        // Make all text invisible
        foreach (TMP_Text txt in texts)
        {
            if (txt != null)
            {
                Color c = txt.color;
                c.a = 0f;
                txt.color = c;
            }
        }

        // Hide the next button
        if (newButton != null)
            newButton.gameObject.SetActive(false);
    }

    public void NextFrame()
    {
        if (currentIndex >= images.Length || currentIndex >= texts.Length)
            return;

        // Show the next image
        if (images[currentIndex] != null)
        {
            Color c = images[currentIndex].color;
            c.a = 1f;
            images[currentIndex].color = c;
        }

        // Show the matching text
        if (texts[currentIndex] != null)
        {
            Color c = texts[currentIndex].color;
            c.a = 1f;
            texts[currentIndex].color = c;
        }

        currentIndex++;

        // Finished?
        if (currentIndex >= images.Length || currentIndex >= texts.Length)
        {
            // Mark the intro as played forever
            FirstLaunchManager.MarkIntroAsPlayed();

            if (nextButton != null)
                nextButton.gameObject.SetActive(false);

            if (newButton != null)
                newButton.gameObject.SetActive(true);
        }
    }
}