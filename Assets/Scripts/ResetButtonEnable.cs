using UnityEngine;
using UnityEngine.UI;

public class TutorialButtonUnlock : MonoBehaviour
{
    [Header("Button")]
    public Button targetButton;

    [Header("Tutorial Completion")]
    public string tutorialCompletedKey = "TutorialCompleted";

    private void Start()
    {
        RefreshButton();
    }

    public void RefreshButton()
    {
        if (targetButton == null)
        {
            Debug.LogWarning(
                "TutorialButtonUnlock: No button assigned!",
                this
            );
            return;
        }

        bool tutorialCompleted =
            PlayerPrefs.GetInt(tutorialCompletedKey, 0) == 1;

        // Completely hide or show the button.
        targetButton.gameObject.SetActive(tutorialCompleted);
    }
}