using UnityEngine;


// =============================================================
// REPAIR POWERUP
// =============================================================

public class RepairPowerup : MonoBehaviour
{
    // =========================================================
    // POWERUP
    // =========================================================

    [Header("Powerup")]

    [Tooltip("Repair = 2")]
    public int powerupIndex = 2;

    [Tooltip("Repair cooldown in seconds.")]
    public float cooldown = 30f;


    // =========================================================
    // REPAIR SETTINGS
    // =========================================================

    [Header("Repair Settings")]

    [Tooltip("Maximum Repair level.")]
    public int maxLevel = 10;


    // =========================================================
    // REPAIR VISUALS
    // =========================================================

    [Header("Repair Visuals")]

    [Tooltip("Visual effects played when Repair successfully activates.")]
    public RepairPowerupEffects repairEffects;


    // =========================================================
    // HUD
    // =========================================================

    [Header("HUD")]

    public ActiveHUD activeHUD;


    // =========================================================
    // INTERNAL
    // =========================================================

    private float cooldownTimer = 0f;

    private bool isRepairActive = false;


    // =========================================================
    // UNITY
    // =========================================================

    private void Start()
    {
        RefreshHUD();
    }


    private void Update()
    {
        // =====================================================
        // COOLDOWN
        // =====================================================

        UpdateCooldown();


        // =====================================================
        // ACTIVATION
        // =====================================================

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            TryActivate();
        }
    }


    // =========================================================
    // COOLDOWN
    // =========================================================

    private void UpdateCooldown()
    {
        if (cooldownTimer <= 0f)
        {
            return;
        }


        cooldownTimer -=
            Time.deltaTime;


        if (cooldownTimer <= 0f)
        {
            cooldownTimer = 0f;


            if (activeHUD != null)
            {
                activeHUD.SetReady();
            }


            return;
        }


        if (activeHUD != null &&
            cooldown > 0f)
        {
            float elapsedTime =
                cooldown -
                cooldownTimer;


            float fillAmount =
                elapsedTime /
                cooldown;


            fillAmount =
                Mathf.Clamp01(
                    fillAmount
                );


            activeHUD.SetCooldownFill(
                fillAmount
            );
        }
    }


    // =========================================================
    // TRY ACTIVATE
    // =========================================================

    private void TryActivate()
    {
        // =====================================================
        // MANAGER CHECK
        // =====================================================

        if (ActivePowerupManager.Instance == null)
        {
            Debug.LogWarning(
                "[RepairPowerup] Cannot activate Repair because " +
                "ActivePowerupManager.Instance is null."
            );

            return;
        }


        // =====================================================
        // GET SLOT 0
        // =====================================================

        int equippedPowerup =
            ActivePowerupManager.Instance.GetPowerupInSlot(0);


        Debug.Log(
            "[RepairPowerup] Alpha1 pressed. " +
            "Slot 0 contains powerup index: " +
            equippedPowerup
        );


        // =====================================================
        // EMPTY SLOT
        // =====================================================

        if (equippedPowerup == -1)
        {
            Debug.Log(
                "[RepairPowerup] Slot 0 is empty."
            );

            return;
        }


        // =====================================================
        // WRONG POWERUP
        // =====================================================

        if (equippedPowerup != powerupIndex)
        {
            Debug.Log(
                "[RepairPowerup] Slot 0 contains " +
                ActivePowerupManager.Instance.GetPowerupName(
                    equippedPowerup
                ) +
                " (index " +
                equippedPowerup +
                "), not Repair (index " +
                powerupIndex +
                ")."
            );

            return;
        }


        // =====================================================
        // FIND HUD
        // =====================================================

        FindActiveHUD();


        // =====================================================
        // ACTIVATE
        // =====================================================

        ActivateRepair();
    }


    // =========================================================
    // FIND HUD
    // =========================================================

    private void FindActiveHUD()
    {
        ActiveHUD[] huds =
            FindObjectsByType<ActiveHUD>(
                FindObjectsInactive.Include,
                FindObjectsSortMode.None
            );


        for (int i = 0;
             i < huds.Length;
             i++)
        {
            if (huds[i] == null)
            {
                continue;
            }


            if (huds[i].slotIndex == 0)
            {
                activeHUD =
                    huds[i];

                return;
            }
        }
    }


    // =========================================================
    // ACTIVATE REPAIR
    // =========================================================

    private void ActivateRepair()
    {
        // =====================================================
        // ALREADY ACTIVE
        // =====================================================

        if (isRepairActive)
        {
            Debug.Log(
                "[RepairPowerup] Repair is already active."
            );

            return;
        }


        // =====================================================
        // COOLDOWN
        // =====================================================

        if (cooldownTimer > 0f)
        {
            Debug.Log(
                "[RepairPowerup] Repair is on cooldown: " +
                cooldownTimer.ToString("0.0") +
                " seconds remaining."
            );

            return;
        }


        // =====================================================
        // CHECK UNLOCK
        // =====================================================

        const string saveKey =
            "Active_Repair";


        bool unlocked =
            PlayerPrefs.GetInt(
                saveKey + "_Unlocked",
                0
            ) == 1;


        if (!unlocked)
        {
            Debug.Log(
                "[RepairPowerup] Repair is not unlocked."
            );

            return;
        }


        // =====================================================
        // FIND HOUSE
        // =====================================================

        House house =
            FindFirstObjectByType<House>(
                FindObjectsInactive.Include
            );


        if (house == null)
        {
            Debug.LogWarning(
                "[RepairPowerup] Could not find House."
            );

            return;
        }


        // =====================================================
        // HOUSE ALREADY FULL
        // =====================================================

        if (house.health >= house.maxHealth)
        {
            Debug.Log(
                "[RepairPowerup] Cannot use Repair. " +
                "House is already at full health: " +
                house.health +
                "/" +
                house.maxHealth
            );

            return;
        }


        // =====================================================
        // GET LEVEL
        // =====================================================

        int level =
            PlayerPrefs.GetInt(
                saveKey + "_Level",
                0
            );


        // =====================================================
        // LEVEL 0 SAFETY
        // =====================================================

        if (level <= 0)
        {
            level = 1;

            PlayerPrefs.SetInt(
                saveKey + "_Level",
                level
            );

            PlayerPrefs.Save();


            Debug.Log(
                "[RepairPowerup] Saved Repair level was 0. " +
                "Setting it to Level 1."
            );
        }


        level =
            Mathf.Clamp(
                level,
                1,
                maxLevel
            );


        // =====================================================
        // GET REPAIR AMOUNT
        // =====================================================

        int repairAmount =
            GetRepairAmountFromLevel(
                level
            );


        // =====================================================
        // INVALID REPAIR AMOUNT
        // =====================================================

        if (repairAmount <= 0)
        {
            Debug.LogWarning(
                "[RepairPowerup] Repair amount is 0. " +
                "Level: " +
                level
            );

            return;
        }


        // =====================================================
        // START COOLDOWN
        // =====================================================

        cooldownTimer =
            cooldown;


        if (activeHUD != null)
        {
            activeHUD.StartCooldown();
        }


        // =====================================================
        // DEBUG BEFORE HEAL
        // =====================================================

        Debug.Log(
            "[RepairPowerup] REPAIR ACTIVATED | " +
            "Level: " +
            level +
            " | Repair: " +
            repairAmount +
            " HP | " +
            "House HP Before: " +
            house.health +
            "/" +
            house.maxHealth +
            " | Cooldown: " +
            cooldown.ToString("0.0") +
            "s"
        );


        // =====================================================
        // HEAL HOUSE
        // =====================================================

        isRepairActive = true;


        int healthBefore =
            house.health;


        house.Heal(
            repairAmount
        );


        int actualHealing =
            house.health -
            healthBefore;


        isRepairActive = false;


        // =====================================================
        // PLAY REPAIR VISUAL
        // =====================================================

        if (actualHealing > 0)
        {
            if (repairEffects != null)
            {
                repairEffects.PlayRepairEffect();
            }
            else
            {
                Debug.LogWarning(
                    "[RepairPowerup] Repair activated successfully, " +
                    "but RepairPowerupEffects is not assigned."
                );
            }
        }


        // =====================================================
        // DEBUG AFTER HEAL
        // =====================================================

        Debug.Log(
            "[RepairPowerup] HOUSE REPAIRED | " +
            "Requested: " +
            repairAmount +
            " HP | " +
            "Actual: " +
            actualHealing +
            " HP | " +
            "House HP After: " +
            house.health +
            "/" +
            house.maxHealth
        );
    }


    // =========================================================
    // GET REPAIR AMOUNT
    // =========================================================

    private int GetRepairAmountFromLevel(
        int level)
    {
        // =====================================================
        // CLAMP LEVEL
        // =====================================================

        level =
            Mathf.Clamp(
                level,
                1,
                maxLevel
            );


        // =====================================================
        // REPAIR VALUES
        // =====================================================

        int repairAmount =
            level * 5;


        // =====================================================
        // DEBUG
        // =====================================================

        Debug.Log(
            "[RepairPowerup] Repair lookup | " +
            "Level: " +
            level +
            " | Value: " +
            repairAmount +
            " HP"
        );


        // =====================================================
        // RETURN INTEGER HP
        // =====================================================

        return Mathf.Max(
            0,
            repairAmount
        );
    }


    // =========================================================
    // RESET REPAIR
    // =========================================================

    public void ResetRepair()
    {
        isRepairActive =
            false;

        cooldownTimer =
            0f;


        RefreshHUD();


        Debug.Log(
            "[RepairPowerup] REPAIR FULLY RESET."
        );
    }


    // =========================================================
    // HUD REFRESH
    // =========================================================

    private void RefreshHUD()
    {
        if (activeHUD == null)
        {
            FindActiveHUD();
        }


        if (activeHUD != null)
        {
            if (cooldownTimer <= 0f)
            {
                activeHUD.SetReady();
            }
            else
            {
                activeHUD.StartCooldown();
            }
        }
    }
}