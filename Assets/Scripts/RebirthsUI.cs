using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class RebirthUIManager : MonoBehaviour
{
    // ============================================================
    // DEMO
    // ============================================================

    [Header("Demo")]

    [Tooltip("If enabled, the Max Rebirth tab is shown after reaching Rebirth 2 instead of Rebirth 5.")]
    public bool demo = false;


    // ============================================================
    // REBIRTH TABS
    // ============================================================

    [Header("Rebirth Requirement Tabs")]

    [Tooltip("Entire UI tab for Rebirth 1.")]
    public GameObject rebirth1Tab;

    [Tooltip("Entire UI tab for Rebirth 2.")]
    public GameObject rebirth2Tab;

    [Tooltip("Entire UI tab for Rebirth 3.")]
    public GameObject rebirth3Tab;

    [Tooltip("Entire UI tab for Rebirth 4.")]
    public GameObject rebirth4Tab;

    [Tooltip("Entire UI tab for Rebirth 5.")]
    public GameObject rebirth5Tab;

    [Tooltip("Shown after the maximum rebirth has been reached.")]
    public GameObject maxRebirthTab;


    // ============================================================
    // MISC UI
    // ============================================================

    [Header("Misc UI")]

    public GameObject miscObject1;
    public GameObject miscObject2;
    public GameObject miscObject3;
    public GameObject miscObject4;


    // ============================================================
    // REBIRTH BUTTONS
    // ============================================================

    [Header("Rebirth Buttons")]

    public Button rebirth1Button;
    public Button rebirth2Button;
    public Button rebirth3Button;
    public Button rebirth4Button;
    public Button rebirth5Button;


    // ============================================================
    // CHECKMARK / X SPRITES
    // ============================================================

    [Header("Requirement Sprites")]

    public Sprite checkmarkSprite;
    public Sprite xSprite;


    // ============================================================
    // REBIRTH 1 INDICATORS
    // ============================================================

    [Header("Rebirth 1 Requirement Indicators")]

    public Image rebirth1HouseIndicator;
    public Image rebirth1HelmetIndicator;
    public Image rebirth1ChestplateIndicator;
    public Image rebirth1LeggingsIndicator;
    public Image rebirth1BootsIndicator;


    // ============================================================
    // REBIRTH 2 INDICATORS
    // ============================================================

    [Header("Rebirth 2 Requirement Indicators")]

    public Image rebirth2HouseIndicator;
    public Image rebirth2HelmetIndicator;
    public Image rebirth2ChestplateIndicator;
    public Image rebirth2LeggingsIndicator;
    public Image rebirth2BootsIndicator;


    // ============================================================
    // REBIRTH 3 INDICATORS
    // ============================================================

    [Header("Rebirth 3 Requirement Indicators")]

    public Image rebirth3HouseIndicator;
    public Image rebirth3HelmetIndicator;
    public Image rebirth3ChestplateIndicator;
    public Image rebirth3LeggingsIndicator;
    public Image rebirth3BootsIndicator;


    // ============================================================
    // REBIRTH 4 INDICATORS
    // ============================================================

    [Header("Rebirth 4 Requirement Indicators")]

    public Image rebirth4HouseIndicator;
    public Image rebirth4HelmetIndicator;
    public Image rebirth4ChestplateIndicator;
    public Image rebirth4LeggingsIndicator;
    public Image rebirth4BootsIndicator;


    // ============================================================
    // REBIRTH 5 INDICATORS
    // ============================================================

    [Header("Rebirth 5 Requirement Indicators")]

    public Image rebirth5HouseIndicator;
    public Image rebirth5HelmetIndicator;
    public Image rebirth5ChestplateIndicator;
    public Image rebirth5LeggingsIndicator;
    public Image rebirth5BootsIndicator;


    // ============================================================
    // START
    // ============================================================

    private IEnumerator Start()
    {
        while (RebirthManager.Instance == null)
        {
            yield return null;
        }

        RefreshUI();
    }


    // ============================================================
    // REFRESH EVERYTHING
    // ============================================================

    public void RefreshUI()
    {
        if (RebirthManager.Instance == null)
        {
            Debug.LogError(
                "RebirthUIManager: RebirthManager.Instance is missing!"
            );

            return;
        }

        int currentRebirth =
            RebirthManager.Instance.CurrentRebirth;


        // ========================================================
        // DEMO MAX REBIRTH
        // ========================================================

        if (demo && currentRebirth >= 2)
        {
            ShowMaxRebirthUI();
            return;
        }


        // ========================================================
        // EXTERNAL REBIRTH 2 UNLOCK
        // ========================================================
        // If another script has already unlocked Rebirth 2,
        // Rebirth 1 must not hide or override that tab.

        if (currentRebirth == 0 &&
            rebirth2Tab != null &&
            rebirth2Tab.activeSelf)
        {
            SetMiscUI(true);

            if (rebirth1Tab != null)
                rebirth1Tab.SetActive(false);

            return;
        }


        // ========================================================
        // HIDE ALL TABS FIRST
        // ========================================================

        if (rebirth1Tab != null)
            rebirth1Tab.SetActive(false);

        if (rebirth2Tab != null)
            rebirth2Tab.SetActive(false);

        if (rebirth3Tab != null)
            rebirth3Tab.SetActive(false);

        if (rebirth4Tab != null)
            rebirth4Tab.SetActive(false);

        if (rebirth5Tab != null)
            rebirth5Tab.SetActive(false);

        if (maxRebirthTab != null)
            maxRebirthTab.SetActive(false);


        // ========================================================
        // SHOW MISC UI
        // ========================================================

        SetMiscUI(true);


        // ========================================================
        // SHOW REBIRTH 1
        // CURRENT REBIRTH = 0
        // ========================================================

        if (currentRebirth == 0)
        {
            if (rebirth1Tab != null)
                rebirth1Tab.SetActive(true);

            UpdateRebirth1UI();
        }


        // ========================================================
        // SHOW REBIRTH 2
        // CURRENT REBIRTH = 1
        // ========================================================

        else if (currentRebirth == 1)
        {
            if (rebirth2Tab != null)
                rebirth2Tab.SetActive(true);

            UpdateRebirth2UI();
        }


        // ========================================================
        // SHOW REBIRTH 3
        // CURRENT REBIRTH = 2
        // ========================================================

        else if (currentRebirth == 2)
        {
            if (rebirth3Tab != null)
                rebirth3Tab.SetActive(true);

            UpdateRebirth3UI();
        }


        // ========================================================
        // SHOW REBIRTH 4
        // CURRENT REBIRTH = 3
        // ========================================================

        else if (currentRebirth == 3)
        {
            if (rebirth4Tab != null)
                rebirth4Tab.SetActive(true);

            UpdateRebirth4UI();
        }


        // ========================================================
        // SHOW REBIRTH 5
        // CURRENT REBIRTH = 4
        // ========================================================

        else if (currentRebirth == 4)
        {
            if (rebirth5Tab != null)
                rebirth5Tab.SetActive(true);

            UpdateRebirth5UI();
        }


        // ========================================================
        // MAX REBIRTH
        // CURRENT REBIRTH = 5
        // ========================================================

        else if (currentRebirth >= 5)
        {
            ShowMaxRebirthUI();
        }
    }


    // ============================================================
    // REBIRTH 1 UI
    // ============================================================

    private void UpdateRebirth1UI()
    {
        int requiredHouse =
            RebirthManager.Instance.requiredHouseLevels[1];

        int requiredArmor =
            RebirthManager.Instance.requiredArmorTiers[1];

        UpdateIndicator(
            rebirth1HouseIndicator,
            PlayerPrefs.GetInt("HouseLevel", 0) >= requiredHouse
        );

        UpdateIndicator(
            rebirth1HelmetIndicator,
            PlayerPrefs.GetInt("HelmetTier", 0) >= requiredArmor
        );

        UpdateIndicator(
            rebirth1ChestplateIndicator,
            PlayerPrefs.GetInt("ChestplateTier", 0) >= requiredArmor
        );

        UpdateIndicator(
            rebirth1LeggingsIndicator,
            PlayerPrefs.GetInt("LeggingsTier", 0) >= requiredArmor
        );

        UpdateIndicator(
            rebirth1BootsIndicator,
            PlayerPrefs.GetInt("BootsTier", 0) >= requiredArmor
        );

        if (rebirth1Button != null)
            rebirth1Button.interactable = true;
    }


    // ============================================================
    // REBIRTH 2 UI
    // ============================================================

    private void UpdateRebirth2UI()
    {
        int requiredHouse =
            RebirthManager.Instance.requiredHouseLevels[2];

        int requiredArmor =
            RebirthManager.Instance.requiredArmorTiers[2];

        UpdateIndicator(
            rebirth2HouseIndicator,
            PlayerPrefs.GetInt("HouseLevel", 0) >= requiredHouse
        );

        UpdateIndicator(
            rebirth2HelmetIndicator,
            PlayerPrefs.GetInt("HelmetTier", 0) >= requiredArmor
        );

        UpdateIndicator(
            rebirth2ChestplateIndicator,
            PlayerPrefs.GetInt("ChestplateTier", 0) >= requiredArmor
        );

        UpdateIndicator(
            rebirth2LeggingsIndicator,
            PlayerPrefs.GetInt("LeggingsTier", 0) >= requiredArmor
        );

        UpdateIndicator(
            rebirth2BootsIndicator,
            PlayerPrefs.GetInt("BootsTier", 0) >= requiredArmor
        );

        if (rebirth2Button != null)
            rebirth2Button.interactable = true;
    }


    // ============================================================
    // REBIRTH 3 UI
    // ============================================================

    private void UpdateRebirth3UI()
    {
        int requiredHouse =
            RebirthManager.Instance.requiredHouseLevels[3];

        int requiredArmor =
            RebirthManager.Instance.requiredArmorTiers[3];

        UpdateIndicator(
            rebirth3HouseIndicator,
            PlayerPrefs.GetInt("HouseLevel", 0) >= requiredHouse
        );

        UpdateIndicator(
            rebirth3HelmetIndicator,
            PlayerPrefs.GetInt("HelmetTier", 0) >= requiredArmor
        );

        UpdateIndicator(
            rebirth3ChestplateIndicator,
            PlayerPrefs.GetInt("ChestplateTier", 0) >= requiredArmor
        );

        UpdateIndicator(
            rebirth3LeggingsIndicator,
            PlayerPrefs.GetInt("LeggingsTier", 0) >= requiredArmor
        );

        UpdateIndicator(
            rebirth3BootsIndicator,
            PlayerPrefs.GetInt("BootsTier", 0) >= requiredArmor
        );

        if (rebirth3Button != null)
            rebirth3Button.interactable = true;
    }


    // ============================================================
    // REBIRTH 4 UI
    // ============================================================

    private void UpdateRebirth4UI()
    {
        int requiredHouse =
            RebirthManager.Instance.requiredHouseLevels[4];

        int requiredArmor =
            RebirthManager.Instance.requiredArmorTiers[4];

        UpdateIndicator(
            rebirth4HouseIndicator,
            PlayerPrefs.GetInt("HouseLevel", 0) >= requiredHouse
        );

        UpdateIndicator(
            rebirth4HelmetIndicator,
            PlayerPrefs.GetInt("HelmetTier", 0) >= requiredArmor
        );

        UpdateIndicator(
            rebirth4ChestplateIndicator,
            PlayerPrefs.GetInt("ChestplateTier", 0) >= requiredArmor
        );

        UpdateIndicator(
            rebirth4LeggingsIndicator,
            PlayerPrefs.GetInt("LeggingsTier", 0) >= requiredArmor
        );

        UpdateIndicator(
            rebirth4BootsIndicator,
            PlayerPrefs.GetInt("BootsTier", 0) >= requiredArmor
        );

        if (rebirth4Button != null)
            rebirth4Button.interactable = true;
    }


    // ============================================================
    // REBIRTH 5 UI
    // ============================================================

    private void UpdateRebirth5UI()
    {
        int requiredHouse =
            RebirthManager.Instance.requiredHouseLevels[5];

        int requiredArmor =
            RebirthManager.Instance.requiredArmorTiers[5];

        UpdateIndicator(
            rebirth5HouseIndicator,
            PlayerPrefs.GetInt("HouseLevel", 0) >= requiredHouse
        );

        UpdateIndicator(
            rebirth5HelmetIndicator,
            PlayerPrefs.GetInt("HelmetTier", 0) >= requiredArmor
        );

        UpdateIndicator(
            rebirth5ChestplateIndicator,
            PlayerPrefs.GetInt("ChestplateTier", 0) >= requiredArmor
        );

        UpdateIndicator(
            rebirth5LeggingsIndicator,
            PlayerPrefs.GetInt("LeggingsTier", 0) >= requiredArmor
        );

        UpdateIndicator(
            rebirth5BootsIndicator,
            PlayerPrefs.GetInt("BootsTier", 0) >= requiredArmor
        );

        if (rebirth5Button != null)
            rebirth5Button.interactable = true;
    }


    // ============================================================
    // UPDATE CHECKMARK / X
    // ============================================================

    private void UpdateIndicator(Image indicator, bool requirementMet)
    {
        if (indicator == null)
            return;

        indicator.sprite = requirementMet
            ? checkmarkSprite
            : xSprite;
    }


    // ============================================================
    // MISC UI
    // ============================================================

    private void SetMiscUI(bool active)
    {
        if (miscObject1 != null)
            miscObject1.SetActive(active);

        if (miscObject2 != null)
            miscObject2.SetActive(active);

        if (miscObject3 != null)
            miscObject3.SetActive(active);

        if (miscObject4 != null)
            miscObject4.SetActive(active);
    }


    // ============================================================
    // MAX REBIRTH UI
    // ============================================================

    private void ShowMaxRebirthUI()
    {
        if (rebirth1Tab != null)
            rebirth1Tab.SetActive(false);

        if (rebirth2Tab != null)
            rebirth2Tab.SetActive(false);

        if (rebirth3Tab != null)
            rebirth3Tab.SetActive(false);

        if (rebirth4Tab != null)
            rebirth4Tab.SetActive(false);

        if (rebirth5Tab != null)
            rebirth5Tab.SetActive(false);


        if (rebirth1Button != null)
            rebirth1Button.gameObject.SetActive(false);

        if (rebirth2Button != null)
            rebirth2Button.gameObject.SetActive(false);

        if (rebirth3Button != null)
            rebirth3Button.gameObject.SetActive(false);

        if (rebirth4Button != null)
            rebirth4Button.gameObject.SetActive(false);

        if (rebirth5Button != null)
            rebirth5Button.gameObject.SetActive(false);


        SetMiscUI(false);

        if (maxRebirthTab != null)
            maxRebirthTab.SetActive(true);
    }


    // ============================================================
    // PERFORM REBIRTH
    // ============================================================

    public void PerformRebirth()
    {
        if (RebirthManager.Instance == null)
        {
            Debug.LogError(
                "RebirthUIManager: RebirthManager.Instance is missing!"
            );

            return;
        }

        if (demo &&
            RebirthManager.Instance.CurrentRebirth >= 2)
        {
            Debug.Log(
                "Demo Rebirth limit reached. Rebirth 2 is the maximum."
            );

            RefreshUI();
            return;
        }

        if (!RebirthManager.Instance.CanRebirth())
        {
            Debug.Log(
                "Cannot rebirth yet. Requirements are not met."
            );

            return;
        }

        bool success =
            RebirthManager.Instance.PerformRebirth();

        if (success)
        {
            Debug.Log("Rebirth successful!");
            RefreshUI();
        }
    }


    // ============================================================
    // MANUAL REFRESH
    // ============================================================

    public void Refresh()
    {
        RefreshUI();
    }
}

