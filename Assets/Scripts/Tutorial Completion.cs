using UnityEngine;

public class TutorialCompletion : MonoBehaviour
{
    private const string TutorialCompletedKey = "TutorialCompleted";

    public void CompleteTutorial()
    {
        PlayerPrefs.SetInt(TutorialCompletedKey, 1);
        PlayerPrefs.Save();

        Debug.Log("Tutorial completed. It will be skipped on future launches.");
    }

    public void ResetTutorialCompletion()
    {
        PlayerPrefs.SetInt(TutorialCompletedKey, 0);
        PlayerPrefs.Save();

        Debug.Log("Tutorial completion reset.");
    }
}