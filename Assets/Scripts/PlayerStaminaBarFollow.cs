using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerStaminaBarFollow : MonoBehaviour
{
    [Header("References")]
    public Player player;
    public Slider staminaSlider;

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
        // Wait one frame so the new scene and persistent
        // Player have finished reconnecting.
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
                "PlayerStaminaBarFollow: Could not find Player."
            );

            return;
        }


        if (staminaSlider == null)
        {
            staminaSlider =
                GetComponent<Slider>();
        }


        if (staminaSlider == null)
        {
            Debug.LogWarning(
                "PlayerStaminaBarFollow: No stamina Slider assigned."
            );

            return;
        }


        staminaSlider.maxValue =
            player.maxStamina;


        staminaSlider.value =
            player.stamina;


        // Reconnect fill image after scene reload.
        fillImage = null;


        if (staminaSlider.fillRect != null)
        {
            fillImage =
                staminaSlider.fillRect
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


        if (staminaSlider == null)
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


        staminaSlider.transform.position =
            screenPos;


        // ========================================================
        // UPDATE STAMINA
        // ========================================================

        staminaSlider.maxValue =
            player.maxStamina;


        staminaSlider.value =
            Mathf.Clamp(
                player.stamina,
                0f,
                player.maxStamina
            );


        // ========================================================
        // HIDE FILL AT ZERO
        // ========================================================

        if (fillImage != null)
        {
            fillImage.enabled =
                player.stamina > 0f;
        }
    }
}