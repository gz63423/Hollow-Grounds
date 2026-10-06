using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RebirthSceneButton : MonoBehaviour
{
    [Header("Button")]
    public Button button;


    [Header("Rebirth Feature Scene")]
    [Tooltip("Build Index of the scene containing the rebirth feature.")]
    public int rebirthFeatureSceneIndex = 0;


    // ============================================================
    // AWAKE
    // ============================================================

    private void Awake()
    {
        if (button == null)
        {
            button =
                GetComponent<Button>();
        }
    }


    // ============================================================
    // START
    // ============================================================

    private void Start()
    {
        UpdateButtonVisibility();
    }


    // ============================================================
    // UPDATE VISIBILITY
    // ============================================================

    private void UpdateButtonVisibility()
    {
        if (RebirthManager.Instance == null)
        {
            gameObject.SetActive(false);
            return;
        }


        // ========================================================
        // REBIRTH 1+
        // ========================================================

        if (
            RebirthManager.Instance.CurrentRebirth >= 1
        )
        {
            gameObject.SetActive(true);

            if (button != null)
            {
                button.interactable = true;
            }

            return;
        }


        // ========================================================
        // REBIRTH 0
        // ========================================================

        gameObject.SetActive(false);
    }


    // ============================================================
    // OPEN REBIRTH FEATURE
    // ============================================================

    public void OpenRebirthFeature()
    {
        if (RebirthManager.Instance == null)
            return;


        // ========================================================
        // PLAYER MUST OWN REBIRTH 1
        // ========================================================

        if (
            RebirthManager.Instance.CurrentRebirth < 1
        )
        {
            Debug.Log(
                "Rebirth Feature locked. " +
                "Rebirth 1 is required."
            );

            return;
        }


        Debug.Log(
            "Opening Rebirth Feature Scene: " +
            rebirthFeatureSceneIndex
        );


        SceneManager.LoadScene(
            rebirthFeatureSceneIndex
        );
    }
}