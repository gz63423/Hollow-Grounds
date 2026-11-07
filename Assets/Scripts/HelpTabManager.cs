using UnityEngine;
using UnityEngine.UI;

public class HelpTabManager : MonoBehaviour
{
    [Header("Help Buttons (In Order)")]
    public Button demoInfoButton;
    public Button objectiveButton;
    public Button upgradesButton;
    public Button powerupsButton;
    public Button tipsButton;
    public Button controlsButton;

    [Header("Corresponding Tab GameObjects (Same Order)")]
    public GameObject demoInfoTab;
    public GameObject objectiveTab;
    public GameObject upgradesTab;
    public GameObject powerupsTab;
    public GameObject tipsTab;
    public GameObject controlsTab;

    private GameObject[] tabs;

    private void Awake()
    {
        tabs = new GameObject[]
        {
            demoInfoTab,
            objectiveTab,
            upgradesTab,
            powerupsTab,
            tipsTab,
            controlsTab
        };
    }

    private void Start()
    {
        // Connect each button to its corresponding tab.
        demoInfoButton.onClick.AddListener(() => ShowTab(0));
        objectiveButton.onClick.AddListener(() => ShowTab(1));
        upgradesButton.onClick.AddListener(() => ShowTab(2));
        powerupsButton.onClick.AddListener(() => ShowTab(3));
        tipsButton.onClick.AddListener(() => ShowTab(4));
        controlsButton.onClick.AddListener(() => ShowTab(5));

        // Hide all tabs when the menu starts.
        HideAllTabs();
    }

    public void ShowTab(int index)
    {
        HideAllTabs();

        if (index < 0 || index >= tabs.Length)
        {
            Debug.LogWarning("HelpTabManager: Invalid tab index!");
            return;
        }

        if (tabs[index] != null)
        {
            tabs[index].SetActive(true);
        }
        else
        {
            Debug.LogWarning(
                "HelpTabManager: Tab " + index + " is not assigned!",
                this
            );
        }
    }

    private void HideAllTabs()
    {
        foreach (GameObject tab in tabs)
        {
            if (tab != null)
            {
                tab.SetActive(false);
            }
        }
    }
}