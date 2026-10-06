using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayButton : MonoBehaviour
{
    // ============================================================
    // SCENES
    // ============================================================

    [Header("Normal Play Scene")]
    [Tooltip("Normal Waves scene.")]
    public int wavesSceneIndex = 2;

    [Header("First Rebirth Save Scene")]
    [Tooltip("Scene used ONLY when Rebirth 0 has completed the Rebirth 1 requirements.")]
    public int savesSceneIndex = 12;


    // ============================================================
    // STATE
    // ============================================================

    private bool loading = false;


    // ============================================================
    // PLAY BUTTON
    // ============================================================

    public void Play()
    {
        if (loading)
            return;

        loading = true;


        // ========================================================
        // REBIRTH MANAGER REQUIRED
        // ========================================================

        if (RebirthManager.Instance == null)
        {
            Debug.LogWarning(
                "PLAY BUTTON: RebirthManager not found. " +
                "Loading Waves Scene."
            );

            SceneManager.LoadScene(
                wavesSceneIndex
            );

            return;
        }


        // ========================================================
        // CURRENT REBIRTH
        // ========================================================

        int currentRebirth =
            RebirthManager.Instance.CurrentRebirth;


        Debug.Log(
            "PLAY BUTTON: Current Rebirth = " +
            currentRebirth
        );


        // ========================================================
        // IMPORTANT:
        //
        // THE SPECIAL REBIRTH INTRO SYSTEM ONLY EXISTS
        // FOR REBIRTH 0.
        //
        // Once the player has Rebirth 1 or higher,
        // COMPLETELY IGNORE:
        //
        // - RebirthIntroManager
        // - Rebirth Save Scene
        // - First Rebirth requirements
        // - IntroCompleted flags
        //
        // The player simply goes to the Waves Scene.
        // ========================================================

        if (currentRebirth >= 1)
        {
            Debug.Log(
                "PLAY BUTTON: Player already has Rebirth 1+. " +
                "Ignoring all first-rebirth intro logic."
            );

            Debug.Log(
                "PLAY BUTTON: Loading Waves Scene: " +
                wavesSceneIndex
            );

            SceneManager.LoadScene(
                wavesSceneIndex
            );

            return;
        }


        // ========================================================
        // REBIRTH 0
        // ========================================================
        //
        // At this point the player has NEVER rebirthed.
        //
        // Now we are allowed to check the Rebirth 1 requirements.
        // ========================================================

        bool requirementsComplete =
            RebirthManager.Instance.CanRebirth();


        // ========================================================
        // FIRST REBIRTH REQUIREMENTS COMPLETE
        // ========================================================

        if (requirementsComplete)
        {
            Debug.Log(
                "PLAY BUTTON: Rebirth 1 requirements met."
            );

            Debug.Log(
                "PLAY BUTTON: Loading First Rebirth Save Scene: " +
                savesSceneIndex
            );

            SceneManager.LoadScene(
                savesSceneIndex
            );

            return;
        }


        // ========================================================
        // REBIRTH 0 BUT REQUIREMENTS NOT COMPLETE
        // ========================================================

        Debug.Log(
            "PLAY BUTTON: Rebirth 1 requirements not met."
        );

        Debug.Log(
            "PLAY BUTTON: Loading Waves Scene: " +
            wavesSceneIndex
        );

        SceneManager.LoadScene(
            wavesSceneIndex
        );
    }
}