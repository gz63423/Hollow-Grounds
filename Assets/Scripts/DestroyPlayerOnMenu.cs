using UnityEngine;
using UnityEngine.SceneManagement;

public class DestroyPlayerOnMenu : MonoBehaviour
{
    [Header("Set your main menu scene name here")]
    public string mainMenuSceneName = "Main Menu"; // Change this to match your main menu scene

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // If the loaded scene is the main menu, destroy the Player instance
        if (scene.name == mainMenuSceneName)
        {
            if (Player.Instance != null)
            {
                Destroy(Player.Instance.gameObject);
                Debug.Log("Player destroyed because Main Menu loaded.");
            }
        }
    }
}
