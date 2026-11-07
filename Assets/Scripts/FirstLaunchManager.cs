using UnityEngine;
using UnityEngine.SceneManagement;

public class FirstLaunchManager : MonoBehaviour
{
    [Header("Scene Indexes")]
    public int introSceneIndex = 1;
    public int mainMenuSceneIndex = 2;

    private void Awake()
    {
        //If intro has already been watched, skip
        if (PlayerPrefs.GetInt("IntroPlayed", 0) == 1)
        {
            SceneManager.LoadScene(mainMenuSceneIndex);
        }
    }

    //call at the end of the intro cutscene
    public static void MarkIntroAsPlayed()
    {
        PlayerPrefs.SetInt("IntroPlayed", 1);
        PlayerPrefs.Save();
    }
}