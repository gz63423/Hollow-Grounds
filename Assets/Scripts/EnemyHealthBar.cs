using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBar : MonoBehaviour
{
    [Header("References")]
    public Slider healthBarPrefab;
    public Transform anchor;

    [Header("Canvas")]
    public string healthBarCanvasName = "HealthBarCanvas";

    [Header("Visibility")]
    public float visibleDuration = 3f;
    public float fadeDuration = 1f;

    private Enemy enemy;
    private EnemyRanged rangedEnemy;
    private Boss1 boss;

    private Slider slider;
    private RectTransform sliderRect;
    private CanvasGroup canvasGroup;

    private Canvas healthBarCanvas;
    private RectTransform canvasRect;
    private Camera mainCamera;

    private float lastDamageTime;
    private bool isVisible;

    private GameObject spawnedHealthBar;


    void Start()
    {
        // Find enemy type
        enemy = GetComponent<Enemy>();
        rangedEnemy = GetComponent<EnemyRanged>();
        boss = GetComponent<Boss1>();

        // Make sure this is a supported enemy
        if (enemy == null &&
            rangedEnemy == null &&
            boss == null)
        {
            Debug.LogError(
                "EnemyHealthBar: No supported enemy script found on " +
                gameObject.name
            );

            enabled = false;
            return;
        }


        // Make sure health bar prefab exists
        if (healthBarPrefab == null)
        {
            Debug.LogError(
                "EnemyHealthBar: Health Bar Prefab is NULL on " +
                gameObject.name
            );

            enabled = false;
            return;
        }


        // Find HealthBarAnchor
        if (anchor == null)
        {
            anchor = transform.Find("HealthBarAnchor");
        }


        if (anchor == null)
        {
            Debug.LogError(
                "EnemyHealthBar: HealthBarAnchor was not found on " +
                gameObject.name
            );

            enabled = false;
            return;
        }


        // ---------------------------------------------------------
        // FIND HEALTH BAR CANVAS
        // ---------------------------------------------------------

        GameObject canvasObject =
            GameObject.Find(healthBarCanvasName);


        if (canvasObject == null)
        {
            Debug.LogError(
                "EnemyHealthBar: Could not find GameObject named '" +
                healthBarCanvasName +
                "' in the scene!"
            );

            enabled = false;
            return;
        }


        healthBarCanvas =
            canvasObject.GetComponent<Canvas>();


        if (healthBarCanvas == null)
        {
            Debug.LogError(
                "EnemyHealthBar: " +
                healthBarCanvasName +
                " does not have a Canvas component!"
            );

            enabled = false;
            return;
        }


        // ---------------------------------------------------------
        // FORCE SCREEN SPACE CAMERA
        // ---------------------------------------------------------

        healthBarCanvas.renderMode =
            RenderMode.ScreenSpaceCamera;


        // Use Main Camera
        mainCamera = Camera.main;


        if (mainCamera == null)
        {
            Debug.LogError(
                "EnemyHealthBar: Main Camera could not be found!"
            );

            enabled = false;
            return;
        }


        // Assign Main Camera to Canvas
        healthBarCanvas.worldCamera =
            mainCamera;


        // Make sure Canvas is reasonably in front of camera
        healthBarCanvas.planeDistance = 10f;


        // Get Canvas RectTransform
        canvasRect =
            healthBarCanvas.GetComponent<RectTransform>();


        if (canvasRect == null)
        {
            Debug.LogError(
                "EnemyHealthBar: HealthBarCanvas has no RectTransform!"
            );

            enabled = false;
            return;
        }


        // ---------------------------------------------------------
        // CREATE HEALTH BAR
        // ---------------------------------------------------------

        spawnedHealthBar =
            Instantiate(
                healthBarPrefab.gameObject,
                healthBarCanvas.transform
            );


        slider =
            spawnedHealthBar.GetComponent<Slider>();


        sliderRect =
            spawnedHealthBar.GetComponent<RectTransform>();


        if (slider == null)
        {
            Debug.LogError(
                "EnemyHealthBar: Health Bar Prefab does not contain " +
                "a Slider component!"
            );

            Destroy(spawnedHealthBar);
            enabled = false;
            return;
        }


        if (sliderRect == null)
        {
            Debug.LogError(
                "EnemyHealthBar: Health Bar Prefab does not contain " +
                "a RectTransform!"
            );

            Destroy(spawnedHealthBar);
            enabled = false;
            return;
        }


        // ---------------------------------------------------------
        // CANVAS GROUP
        // ---------------------------------------------------------

        canvasGroup =
            spawnedHealthBar.GetComponent<CanvasGroup>();


        if (canvasGroup == null)
        {
            canvasGroup =
                spawnedHealthBar.AddComponent<CanvasGroup>();
        }


        // Start hidden
        canvasGroup.alpha = 0f;


        // ---------------------------------------------------------
        // SET HEALTH VALUES
        // ---------------------------------------------------------

        if (enemy != null)
        {
            slider.maxValue =
                enemy.maxHealth;

            slider.value =
                enemy.Health;

            enemy.OnEnemyDestroyed +=
                DestroyHealthBar;
        }
        else if (rangedEnemy != null)
        {
            slider.maxValue =
                rangedEnemy.maxHealth;

            slider.value =
                rangedEnemy.Health;

            rangedEnemy.OnEnemyDestroyed +=
                DestroyHealthBar;
        }
        else if (boss != null)
        {
            slider.maxValue =
                boss.maxHealth;

            slider.value =
                boss.Health;

            boss.OnEnemyDestroyed +=
                DestroyHealthBar;
        }


        // ---------------------------------------------------------
        // DEBUG
        // ---------------------------------------------------------

        Debug.Log(
            "EnemyHealthBar successfully created for " +
            gameObject.name +
            " | Canvas: " +
            healthBarCanvas.name +
            " | Camera: " +
            mainCamera.name
        );

    }


    void LateUpdate()
    {
        if (slider == null ||
        sliderRect == null ||
        anchor == null ||
        mainCamera == null ||
        healthBarCanvas == null)
        {
            return;
        }

        // Get the enemy's position in screen space
        Vector3 screenPos =
            mainCamera.WorldToScreenPoint(
                anchor.position
            );


        // If the enemy is behind the camera, hide the bar
        if (screenPos.z < 0f)
        {
            slider.gameObject.SetActive(false);
            return;
        }


        slider.gameObject.SetActive(true);


        // ---------------------------------------------------------
        // SCREEN POSITION -> HEALTH BAR CANVAS WORLD POSITION
        // ---------------------------------------------------------

        RectTransform canvasRect =
            healthBarCanvas.GetComponent<RectTransform>();


        if (canvasRect == null)
            return;


        Vector3 canvasWorldPosition;


        RectTransformUtility.ScreenPointToWorldPointInRectangle(
            canvasRect,
            screenPos,
            healthBarCanvas.worldCamera,
            out canvasWorldPosition
        );


        sliderRect.position =
            canvasWorldPosition;


        // ---------------------------------------------------------
        // UPDATE HEALTH
        // ---------------------------------------------------------

        if (enemy != null)
        {
            slider.value =
                Mathf.Clamp(
                    enemy.Health,
                    0f,
                    enemy.maxHealth
                );
        }
        else if (rangedEnemy != null)
        {
            slider.value =
                Mathf.Clamp(
                    rangedEnemy.Health,
                    0f,
                    rangedEnemy.maxHealth
                );
        }
        else if (boss != null)
        {
            slider.value =
                Mathf.Clamp(
                    boss.Health,
                    0f,
                    boss.maxHealth
                );
        }


        // ---------------------------------------------------------
        // VISIBILITY
        // ---------------------------------------------------------

        if (!isVisible)
            return;


        float elapsed =
            Time.time - lastDamageTime;


        if (elapsed < visibleDuration)
        {
            canvasGroup.alpha = 1f;
        }
        else
        {
            float fadeProgress =
                (elapsed - visibleDuration) /
                fadeDuration;


            canvasGroup.alpha =
                Mathf.Lerp(
                    1f,
                    0f,
                    fadeProgress
                );


            if (canvasGroup.alpha <= 0f)
            {
                canvasGroup.alpha = 0f;
                isVisible = false;
            }
        }

    }


    public void Show()
    {
        if (canvasGroup == null)
            return;


        lastDamageTime =
            Time.time;


        isVisible = true;


        canvasGroup.alpha = 1f;
    }


    private void DestroyHealthBar()
    {
        if (spawnedHealthBar != null)
        {
            Destroy(spawnedHealthBar);
            spawnedHealthBar = null;
        }
    }


    void OnDestroy()
    {
        if (enemy != null)
            enemy.OnEnemyDestroyed -= DestroyHealthBar;

        if (rangedEnemy != null)
            rangedEnemy.OnEnemyDestroyed -= DestroyHealthBar;

        if (boss != null)
            boss.OnEnemyDestroyed -= DestroyHealthBar;
    }

}