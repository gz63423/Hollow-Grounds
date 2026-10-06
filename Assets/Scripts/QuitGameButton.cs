using UnityEngine;

public class QuitGameButton : MonoBehaviour
{
    [Header("Quit Confirmation Tab")]
    public GameObject quitConfirmationTab;

    // Opens the "Are you sure you want to quit?" tab
    public void OpenQuitTab()
    {
        quitConfirmationTab.SetActive(true);
    }

    // Closes the confirmation tab
    public void CloseQuitTab()
    {
        quitConfirmationTab.SetActive(false);
    }

    // Actually closes the game
    public void QuitGame()
    {
        Application.Quit();
    }
}

