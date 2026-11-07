using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class DemoJourneyClaimArrow : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Optional: Drag the DemoJourneyManager here. If empty, it finds the singleton automatically.")]
    public DemoJourneyManager demoJourneyManager;

    [Tooltip("Drag the arrow's UI Image here.")]
    public Image arrowImage;

    [Tooltip("Optional: Assign the left-facing arrow sprite.")]
    public Sprite arrowSprite;

    [Header("Arrow Size")]
    public Vector2 arrowSize = new Vector2(50f, 50f);

    [Header("Horizontal Movement")]
    public float horizontalDistance = 20f;
    public float horizontalSpeed = 2f;

    [Header("Vertical Bounce")]
    public float bounceHeight = 8f;
    public float bounceSpeed = 5f;

    [Header("Claim Check")]
    public float checkInterval = 0.25f;

    private readonly string[] objectiveIDs =
    {
        "Scoutbreaker",
        "RisingPower",
        "FirstFortune",
        "DeepFreeze",
        "SoulHarvester",
        "RavagersEnd",
        "SecondChance",
        "Boneclad",
        "Fortified",
        "IntoTheHollow"
    };

    private RectTransform arrowRect;
    private Vector2 startingPosition;
    private float checkTimer;
    private bool referencesReady;

    private void Awake()
    {
        if (arrowImage == null)
        {
            Debug.LogError(
                "DemoJourneyClaimArrow: Assign the Arrow Image in the Inspector.",
                this
            );

            enabled = false;
            return;
        }

        arrowRect = arrowImage.rectTransform;
        startingPosition = arrowRect.anchoredPosition;

        if (arrowSprite != null)
            arrowImage.sprite = arrowSprite;

        arrowRect.sizeDelta = arrowSize;

        // Hide only the Image component, not its GameObject.
        arrowImage.enabled = false;

        referencesReady = true;
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;

        // Reconnect and check whenever this object becomes active.
        ReconnectManager();
        RefreshClaimStatus();
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Start()
    {
        ReconnectManager();
        RefreshClaimStatus();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // The manager may need a moment to be available after a scene change.
        ReconnectManager();
        RefreshClaimStatus();

        checkTimer = 0f;
    }

    private void Update()
    {
        if (!referencesReady)
            return;

        // Keep reconnecting if the previous manager reference was destroyed.
        ReconnectManager();

        if (arrowImage.enabled)
        {
            float time = Time.unscaledTime;

            float horizontalOffset =
                Mathf.Cos(time * horizontalSpeed) * horizontalDistance;

            float verticalOffset =
                Mathf.Abs(Mathf.Sin(time * bounceSpeed)) * bounceHeight;

            arrowRect.anchoredPosition =
                startingPosition +
                new Vector2(horizontalOffset, verticalOffset);
        }

        checkTimer += Time.unscaledDeltaTime;

        if (checkTimer >= checkInterval)
        {
            checkTimer = 0f;
            RefreshClaimStatus();
        }
    }

    private void ReconnectManager()
    {
        // Always prefer the current persistent singleton.
        if (DemoJourneyManager.Instance != null)
        {
            demoJourneyManager = DemoJourneyManager.Instance;
        }
    }

    public void RefreshClaimStatus()
    {
        if (!referencesReady)
            return;

        ReconnectManager();

        if (demoJourneyManager == null)
        {
            arrowImage.enabled = false;
            return;
        }

        bool claimable = false;

        foreach (string id in objectiveIDs)
        {
            if (demoJourneyManager.IsClaimed(id))
                continue;

            if (!demoJourneyManager.IsUnlocked(id))
                continue;

            if (!demoJourneyManager.IsCompleted(id))
                continue;

            claimable = true;
            break;
        }

        arrowImage.enabled = claimable;

        if (!claimable)
        {
            arrowRect.anchoredPosition = startingPosition;
        }
    }
}
