using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayButton : MonoBehaviour
{
    [Header("Waves Scene")]

    [Tooltip("Scene index for the normal Waves scene.")]
    public int wavesSceneIndex = 2;


    private bool loading = false;


    public void Play()
    {
        if (loading)
            return;

        loading = true;

        Debug.Log(
            "PLAY BUTTON: Loading Waves Scene: " +
            wavesSceneIndex
        );

        SceneManager.LoadScene(wavesSceneIndex);
    }
}

