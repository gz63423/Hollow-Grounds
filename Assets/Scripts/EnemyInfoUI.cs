using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class EnemyInfoUI : MonoBehaviour
{
    [Header("Enemy Images")]
    public Image[] enemyImages = new Image[5];

    [Header("Locked ? Images")]
    public GameObject[] lockedQuestionImages = new GameObject[5];

    [Header("Unlocked Enemy Images")]
    public GameObject[] unlockedEnemyImages = new GameObject[5];

    [Header("Enemy Buttons")]
    public Button[] enemyButtons = new Button[5];

    [Header("Enemy Info Parents")]
    public GameObject[] enemyInfoParents = new GameObject[5];

    [Header("Initial Message")]
    public GameObject clickMessage;


    // ============================================================
    // BOUNTY UI
    // ============================================================

    [Header("Enemy Bounty Text")]
    [Tooltip("Bounty TMPs in order: Scout, Ravager, Stalker, Spitter, Boss.")]
    public TMP_Text[] bountyTexts = new TMP_Text[5];

    [Header("Base Enemy Bounties")]
    [Tooltip("Base bounty for each enemy before rebirth multiplier.")]
    public float[] baseBounties =
    {
        0.01f, // Scout
        0.01f, // Ravager
        0.01f, // Stalker
        0.01f, // Spitter
        0.01f  // Boss
    };


    // ============================================================
    // COLORS
    // ============================================================

    [Header("Colors")]
    public Color normalColor =
        new Color32(0, 0, 0, 255);

    public Color hoverColor =
        new Color32(77, 77, 77, 255);

    public Color selectedColor =
        new Color32(148, 0, 14, 255);


    // ============================================================
    // UNLOCK POPUP
    // ============================================================

    [Header("Unlock Popup")]
    public EnemyUnlockPopup enemyUnlockPopupPrefab;


    // ============================================================
    // INTERNAL
    // ============================================================

    private int selectedEnemy = -1;

    public static EnemyInfoUI Instance { get; private set; }


    // ============================================================
    // PLAYER PREFS KEYS
    // ============================================================

    private const string ScoutKey =
        "EnemyUnlocked_Scout";

    private const string RavagerKey =
        "EnemyUnlocked_Ravager";

    private const string StalkerKey =
        "EnemyUnlocked_Stalker";

    private const string SpitterKey =
        "EnemyUnlocked_Spitter";

    private const string BossKey =
        "EnemyUnlocked_Boss";


    // ============================================================
    // AWAKE
    // ============================================================

    private void Awake()
    {
        Instance = this;
    }


    // ============================================================
    // START
    // ============================================================

    private void Start()
    {
        // Disable all enemy info panels
        for (int i = 0; i < enemyInfoParents.Length; i++)
        {
            if (enemyInfoParents[i] != null)
            {
                enemyInfoParents[i].SetActive(false);
            }
        }


        // Show initial message
        if (clickMessage != null)
        {
            clickMessage.SetActive(true);
        }


        // Set up all buttons
        for (int i = 0; i < enemyButtons.Length; i++)
        {
            if (enemyButtons[i] == null)
                continue;

            int index = i;


            // Click
            enemyButtons[i].onClick.AddListener(
                () => SelectEnemy(index)
            );


            // Hover
            AddHoverEvents(
                enemyButtons[i].gameObject,
                index
            );
        }


        // Update locked/unlocked visuals
        UpdateUnlockStates();


        // Update bounty values
        UpdateBountyTexts();
    }


    // ============================================================
    // UPDATE
    // ============================================================

    private void Update()
    {
        // Keeps the bounty synchronized if
        // the rebirth changes while this UI is open.
        UpdateBountyTexts();
    }


    // ============================================================
    // UPDATE BOUNTY TEXTS
    // ============================================================

    private void UpdateBountyTexts()
    {
        if (bountyTexts == null)
            return;

        if (baseBounties == null)
            return;


        float multiplier = 1f;


        if (RebirthManager.Instance != null)
        {
            multiplier =
                RebirthManager.Instance.CurrentMultiplier;
        }


        for (int i = 0; i < bountyTexts.Length; i++)
        {
            if (bountyTexts[i] == null)
                continue;

            if (i >= baseBounties.Length)
                continue;


            float bounty =
                baseBounties[i] * multiplier;


            // Number only — no "$"
            bountyTexts[i].text =
                bounty.ToString("0.00");
        }
    }


    // ============================================================
    // UNLOCK ENEMY
    // ============================================================

    public static void UnlockEnemy(
        int index,
        Vector3 deathPosition
    )
    {
        string key =
            GetUnlockKey(index);


        if (string.IsNullOrEmpty(key))
            return;


        // Already unlocked
        if (PlayerPrefs.GetInt(key, 0) == 1)
            return;


        // Save unlock
        PlayerPrefs.SetInt(key, 1);
        PlayerPrefs.Save();


        Debug.Log(
            "Enemy encyclopedia unlocked: " +
            index
        );


        // Find enemy info UI
        EnemyInfoUI infoUI =
            FindFirstObjectByType<EnemyInfoUI>();


        if (infoUI != null)
        {
            infoUI.SpawnUnlockPopup(
                index,
                deathPosition
            );

            infoUI.UpdateUnlockStates();
        }
    }


    // ============================================================
    // CHECK IF ENEMY IS UNLOCKED
    // ============================================================

    public static bool IsEnemyUnlocked(int index)
    {
        string key =
            GetUnlockKey(index);


        if (string.IsNullOrEmpty(key))
            return false;


        return PlayerPrefs.GetInt(
            key,
            0
        ) == 1;
    }


    // ============================================================
    // SPAWN UNLOCK POPUP
    // ============================================================

    private void SpawnUnlockPopup(
        int index,
        Vector3 deathPosition
    )
    {
        if (enemyUnlockPopupPrefab == null)
        {
            Debug.LogWarning(
                "EnemyInfoUI: Enemy Unlock Popup Prefab is not assigned!"
            );

            return;
        }


        EnemyUnlockPopup popup =
            Instantiate(
                enemyUnlockPopupPrefab,
                Vector3.zero,
                Quaternion.identity
            );


        popup.Setup(
            GetUnlockMessage(index),
            deathPosition
        );
    }


    // ============================================================
    // UNLOCK MESSAGE
    // ============================================================

    private string GetUnlockMessage(int index)
    {
        switch (index)
        {
            case 0:
                return "NEW SPECIMEN DISCOVERED";

            case 1:
                return "NEW SPECIMEN DISCOVERED";

            case 2:
                return "NEW SPECIMEN DISCOVERED";

            case 3:
                return "NEW SPECIMEN DISCOVERED";

            case 4:
                return "NEW SPECIMEN DISCOVERED";

            default:
                return "NEW SPECIMEN DISCOVERED";
        }
    }


    // ============================================================
    // GET UNLOCK KEY
    // ============================================================

    private static string GetUnlockKey(int index)
    {
        switch (index)
        {
            case 0:
                return ScoutKey;

            case 1:
                return RavagerKey;

            case 2:
                return StalkerKey;

            case 3:
                return SpitterKey;

            case 4:
                return BossKey;

            default:
                return null;
        }
    }


    // ============================================================
    // UPDATE UNLOCK STATES
    // ============================================================

    private void UpdateUnlockStates()
    {
        for (int i = 0; i < 5; i++)
        {
            bool unlocked =
                IsEnemyUnlocked(i);


            // ====================================================
            // QUESTION MARK IMAGE
            // ====================================================

            if (i < lockedQuestionImages.Length &&
                lockedQuestionImages[i] != null)
            {
                lockedQuestionImages[i].SetActive(
                    !unlocked
                );
            }


            // ====================================================
            // ACTUAL ENEMY IMAGE
            // ====================================================

            if (i < unlockedEnemyImages.Length &&
                unlockedEnemyImages[i] != null)
            {
                unlockedEnemyImages[i].SetActive(
                    unlocked
                );
            }


            // ====================================================
            // ENEMY BUTTON
            // ====================================================

            if (i < enemyButtons.Length &&
                enemyButtons[i] != null)
            {
                enemyButtons[i].interactable =
                    unlocked;
            }


            // ====================================================
            // ENEMY BUTTON IMAGE COLOR
            // ====================================================

            if (i < enemyImages.Length &&
                enemyImages[i] != null)
            {
                enemyImages[i].color =
                    normalColor;
            }
        }
    }


    // ============================================================
    // SELECT ENEMY
    // ============================================================

    private void SelectEnemy(int index)
    {
        if (index < 0 ||
            index >= enemyImages.Length)
            return;


        // Safety check
        if (!IsEnemyUnlocked(index))
            return;


        selectedEnemy = index;


        // Hide initial message
        if (clickMessage != null)
        {
            clickMessage.SetActive(false);
        }


        // Disable ALL enemy info panels
        for (int i = 0; i < enemyInfoParents.Length; i++)
        {
            if (enemyInfoParents[i] != null)
            {
                enemyInfoParents[i].SetActive(false);
            }
        }


        // Enable ONLY selected enemy info
        if (index < enemyInfoParents.Length &&
            enemyInfoParents[index] != null)
        {
            enemyInfoParents[index].SetActive(true);
        }


        // Update colors
        for (int i = 0; i < enemyImages.Length; i++)
        {
            if (enemyImages[i] == null)
                continue;


            if (i == selectedEnemy)
            {
                enemyImages[i].color =
                    selectedColor;
            }
            else
            {
                enemyImages[i].color =
                    normalColor;
            }
        }
    }


    // ============================================================
    // HOVER EVENTS
    // ============================================================

    private void AddHoverEvents(
        GameObject buttonObject,
        int index
    )
    {
        EventTrigger trigger =
            buttonObject.GetComponent<EventTrigger>();


        if (trigger == null)
        {
            trigger =
                buttonObject.AddComponent<EventTrigger>();
        }


        // Pointer Enter
        EventTrigger.Entry pointerEnter =
            new EventTrigger.Entry();


        pointerEnter.eventID =
            EventTriggerType.PointerEnter;


        pointerEnter.callback.AddListener(
            (data) =>
            {
                OnEnemyHoverEnter(index);
            }
        );


        trigger.triggers.Add(
            pointerEnter
        );


        // Pointer Exit
        EventTrigger.Entry pointerExit =
            new EventTrigger.Entry();


        pointerExit.eventID =
            EventTriggerType.PointerExit;


        pointerExit.callback.AddListener(
            (data) =>
            {
                OnEnemyHoverExit(index);
            }
        );


        trigger.triggers.Add(
            pointerExit
        );
    }


    // ============================================================
    // HOVER ENTER
    // ============================================================

    private void OnEnemyHoverEnter(int index)
    {
        if (index < 0 ||
            index >= enemyImages.Length)
            return;


        if (enemyImages[index] == null)
            return;


        // Locked enemies cannot hover
        if (!IsEnemyUnlocked(index))
            return;


        // Selected stays red
        if (index == selectedEnemy)
            return;


        // Hover = gray
        enemyImages[index].color =
            hoverColor;
    }


    // ============================================================
    // HOVER EXIT
    // ============================================================

    private void OnEnemyHoverExit(int index)
    {
        if (index < 0 ||
            index >= enemyImages.Length)
            return;


        if (enemyImages[index] == null)
            return;


        // Locked = black
        if (!IsEnemyUnlocked(index))
        {
            enemyImages[index].color =
                normalColor;

            return;
        }


        // Selected stays red
        if (index == selectedEnemy)
        {
            enemyImages[index].color =
                selectedColor;
        }
        else
        {
            enemyImages[index].color =
                normalColor;
        }
    }
}

