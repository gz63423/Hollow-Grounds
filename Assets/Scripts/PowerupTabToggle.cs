using UnityEngine;

public class PowerupTabButton : MonoBehaviour
{
    [Header("References")]
    public GameObject clickToViewText;
    public GameObject powerupTab;


    // =========================================================
    // START
    // =========================================================

    private void Start()
    {
        // All tabs start closed.

        if (powerupTab != null)
        {
            powerupTab.SetActive(false);
        }

        // "Click To View" starts visible.

        if (clickToViewText != null)
        {
            clickToViewText.SetActive(true);
        }
    }


    // =========================================================
    // OPEN POWERUP TAB
    // =========================================================

    public void OpenPowerupTab()
    {
        // Find every powerup tab button.

        PowerupTabButton[] allButtons =
            FindObjectsByType<PowerupTabButton>(
                FindObjectsInactive.Include,
                FindObjectsSortMode.None
            );


        // Close every OTHER tab.

        foreach (PowerupTabButton button in allButtons)
        {
            if (button == null)
                continue;

            if (button == this)
                continue;

            if (button.powerupTab != null)
            {
                button.powerupTab.SetActive(false);
            }

            if (button.clickToViewText != null)
            {
                button.clickToViewText.SetActive(true);
            }
        }


        // Open this tab.

        if (powerupTab != null)
        {
            powerupTab.SetActive(true);
        }


        // Hide this button's "Click To View" text.

        if (clickToViewText != null)
        {
            clickToViewText.SetActive(false);
        }
    }
}