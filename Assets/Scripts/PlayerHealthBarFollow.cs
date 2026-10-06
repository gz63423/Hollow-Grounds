using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerHealthBarFollow : MonoBehaviour
{
    [Header("References")]
    public Player player;
    public Slider healthSlider;

    public Vector3 offset = new Vector3(0, 2f, 0);

    private Image fillImage;


    // ============================================================
    // ENABLE / DISABLE
    // ============================================================

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }


    // ============================================================
    // START
    // ============================================================

    void Start()
    {
        FindPlayer();

        SetupBar();
    }


    // ============================================================
    // SCENE LOADED
    // ============================================================

    private void OnSceneLoaded(
        Scene scene,
        LoadSceneMode mode)
    {
        // Wait until the persistent Player has reconnected
        // to the new scene.
        Invoke(
            nameof(ReconnectAfterSceneLoad),
            0.05f
        );
    }


    private void ReconnectAfterSceneLoad()
    {
        FindPlayer();

        SetupBar();
    }


    // ============================================================
    // FIND PLAYER
    // ============================================================

    private void FindPlayer()
    {
        if (Player.Instance != null)
        {
            player =
                Player.Instance;
        }
        else
        {
            player =
                FindFirstObjectByType<Player>();
        }
    }


    // ============================================================
    // SETUP BAR
    // ============================================================

    private void SetupBar()
    {
        if (player == null)
        {
            Debug.LogWarning(
                "PlayerHealthBarFollow: Could not find Player."
            );

            return;
        }


        if (healthSlider == null)
        {
            healthSlider =
                GetComponent<Slider>();
        }


        if (healthSlider == null)
        {
            Debug.LogWarning(
                "PlayerHealthBarFollow: No health Slider assigned."
            );

            return;
        }


        healthSlider.maxValue =
            player.maxHealth;


        healthSlider.value =
            player.health;


        // Reconnect fill image after scene reload.
        fillImage = null;


        if (healthSlider.fillRect != null)
        {
            fillImage =
                healthSlider.fillRect
                    .GetComponent<Image>();
        }
    }


    // ============================================================
    // LATE UPDATE
    // ============================================================

    void LateUpdate()
    {
        if (player == null)
        {
            FindPlayer();

            if (player == null)
                return;
        }


        if (healthSlider == null)
            return;


        Camera cam =
            Camera.main;


        if (cam == null)
            return;


        // ========================================================
        // FOLLOW PLAYER
        // ========================================================

        Vector3 screenPos =
            cam.WorldToScreenPoint(
                player.transform.position +
                offset
            );


        healthSlider.transform.position =
            screenPos;


        // ========================================================
        // UPDATE HEALTH
        // ========================================================

        healthSlider.maxValue =
            player.maxHealth;


        healthSlider.value =
            Mathf.Clamp(
                player.health,
                0f,
                player.maxHealth
            );


        // ========================================================
        // HIDE FILL AT ZERO
        // ========================================================

        if (fillImage != null)
        {
            fillImage.enabled =
                player.health > 0f;
        }
    }
}