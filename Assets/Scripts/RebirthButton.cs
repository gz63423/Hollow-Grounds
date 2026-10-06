using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections.Generic;

public class RebirthButton : MonoBehaviour
{
    [Header("Button")]
    public Button rebirthButton;


    [Header("After Rebirth")]
    [Tooltip("Scene to load after the fast rebirth transition.")]
    public int rebirthResultSceneIndex = 13;


    [Header("Optional UI")]
    public TMP_Text statusText;


    private bool rebirthProcessing;


    // ============================================================
    // UI LOCK
    // ============================================================

    private readonly List<Selectable> lockedSelectables =
        new List<Selectable>();

    private readonly List<Graphic> lockedGraphics =
        new List<Graphic>();


    // ============================================================
    // START
    // ============================================================

    private void Start()
    {
        if (rebirthButton == null)
        {
            rebirthButton =
                GetComponent<Button>();
        }

        if (rebirthButton != null)
        {
            rebirthButton.interactable = true;
        }

        rebirthProcessing = false;

        UpdateButton();
    }


    // ============================================================
    // TRY REBIRTH
    // ============================================================

    public void TryRebirth()
    {
        if (rebirthProcessing)
            return;


        // ========================================================
        // REBIRTH MANAGER
        // ========================================================

        if (RebirthManager.Instance == null)
        {
            Debug.LogError(
                "RebirthManager not found!"
            );

            return;
        }


        // ========================================================
        // CHECK REQUIREMENTS
        // ========================================================

        if (!RebirthManager.Instance.CanRebirth())
        {
            Debug.Log(
                "Rebirth requirements not met."
            );

            UpdateButton();

            return;
        }


        // ========================================================
        // REMEMBER CURRENT REBIRTH
        // ========================================================

        int rebirthBefore =
            RebirthManager.Instance.CurrentRebirth;


        // ========================================================
        // PERFORM REBIRTH
        // ========================================================

        bool success =
            RebirthManager.Instance.PerformRebirth();


        if (!success)
        {
            Debug.Log(
                "Rebirth failed."
            );

            UpdateButton();

            return;
        }


        // ========================================================
        // NEW REBIRTH
        // ========================================================

        int rebirthAfter =
            RebirthManager.Instance.CurrentRebirth;


        rebirthProcessing = true;


        // ========================================================
        // DISABLE ALL UI INTERACTION
        // ========================================================

        DisableAllUIInteraction();


        // ========================================================
        // DEBUG
        // ========================================================

        Debug.Log(
            "============================================"
        );

        Debug.Log(
            "REBIRTH SUCCESSFUL!"
        );

        Debug.Log(
            rebirthBefore +
            " -> " +
            rebirthAfter
        );

        Debug.Log(
            "ALL UI INTERACTION DISABLED."
        );

        Debug.Log(
            "============================================"
        );


        // ========================================================
        // FIRST REBIRTH
        // ========================================================

        if (
            rebirthBefore == 0 &&
            rebirthAfter == 1
        )
        {
            Debug.Log(
                "FIRST REBIRTH PURCHASED!"
            );

            if (
                RebirthIntroManager.Instance != null
            )
            {
                RebirthIntroManager.Instance
                    .MarkFirstRebirthComplete();
            }
        }


        // ========================================================
        // ALL REBIRTHS
        // ========================================================

        StartFastTransition();
    }


    // ============================================================
    // DISABLE ALL UI INTERACTION
    // ============================================================

    private void DisableAllUIInteraction()
    {
        lockedSelectables.Clear();
        lockedGraphics.Clear();


        // ========================================================
        // DISABLE ALL SELECTABLE UI
        // ========================================================

        Selectable[] selectables =
            FindObjectsByType<Selectable>(
                FindObjectsInactive.Include,
                FindObjectsSortMode.None
            );

        foreach (Selectable selectable in selectables)
        {
            if (selectable == null)
                continue;

            if (selectable.interactable)
            {
                lockedSelectables.Add(
                    selectable
                );

                selectable.interactable = false;
            }
        }


        // ========================================================
        // DISABLE ALL GRAPHIC RAYCASTS
        // ========================================================

        Graphic[] graphics =
            FindObjectsByType<Graphic>(
                FindObjectsInactive.Include,
                FindObjectsSortMode.None
            );

        foreach (Graphic graphic in graphics)
        {
            if (graphic == null)
                continue;

            if (graphic.raycastTarget)
            {
                lockedGraphics.Add(
                    graphic
                );

                graphic.raycastTarget = false;
            }
        }


        // ========================================================
        // MAKE SURE REBIRTH BUTTON IS DISABLED
        // ========================================================

        if (rebirthButton != null)
        {
            rebirthButton.interactable = false;
        }
    }


    // ============================================================
    // RESTORE ALL UI INTERACTION
    // ============================================================

    private void RestoreAllUIInteraction()
    {
        // ========================================================
        // RESTORE SELECTABLES
        // ========================================================

        foreach (Selectable selectable in lockedSelectables)
        {
            if (selectable == null)
                continue;

            selectable.interactable = true;
        }

        lockedSelectables.Clear();


        // ========================================================
        // RESTORE GRAPHIC RAYCASTS
        // ========================================================

        foreach (Graphic graphic in lockedGraphics)
        {
            if (graphic == null)
                continue;

            graphic.raycastTarget = true;
        }

        lockedGraphics.Clear();
    }


    // ============================================================
    // START FAST TRANSITION
    // ============================================================

    private void StartFastTransition()
    {
        if (
            FastRebirthTransition.Instance != null
        )
        {
            Debug.Log(
                "Starting FAST rebirth transition to scene " +
                rebirthResultSceneIndex
            );

            FastRebirthTransition.Instance.StartTransition(
                rebirthResultSceneIndex
            );

            return;
        }


        // ========================================================
        // FALLBACK
        // ========================================================

        Debug.LogWarning(
            "FastRebirthTransition.Instance was not found. " +
            "Loading scene normally."
        );

        SceneManager.LoadScene(
            rebirthResultSceneIndex
        );
    }


    // ============================================================
    // UPDATE BUTTON
    // ============================================================

    public void UpdateButton()
    {
        if (RebirthManager.Instance == null)
            return;


        bool canRebirth =
            RebirthManager.Instance.CanRebirth();


        if (rebirthButton != null)
        {
            rebirthButton.interactable =
                !rebirthProcessing;
        }


        if (statusText != null)
        {
            if (
                RebirthManager.Instance.HasMaxRebirth
            )
            {
                statusText.text =
                    "MAX REBIRTH";
            }
            else if (canRebirth)
            {
                statusText.text =
                    "REBIRTH AVAILABLE";
            }
            else
            {
                statusText.text =
                    "REQUIREMENTS NOT MET";
            }
        }
    }


    // ============================================================
    // RESET BUTTON STATE
    // ============================================================

    public void ResetButtonState()
    {
        rebirthProcessing = false;

        RestoreAllUIInteraction();


        if (rebirthButton != null)
        {
            rebirthButton.interactable = true;
        }

        UpdateButton();
    }
}