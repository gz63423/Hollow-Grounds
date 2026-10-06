using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ActiveHUD : MonoBehaviour
{
    // =========================================================
    // HUD SLOT
    // =========================================================

    [Header("HUD Slot")]

    [Tooltip("The active powerup slot this HUD displays.")]
    public int slotIndex = 0;

    [Tooltip("BOTTOM layer. Displays the grey version of the equipped powerup.")]
    public Image greySlotImage;

    [Tooltip("TOP layer. Displays the filled/color version of the equipped powerup.")]
    public Image powerupImage;

    [Tooltip("Displayed on the slot when no powerup is equipped.")]
    public Sprite emptySlotSprite;


    // =========================================================
    // FREEZE SPRITES
    // =========================================================

    [Header("Freeze Sprites")]

    public Sprite freezeGreySprite;
    public Sprite freezeFillSprite;


    // =========================================================
    // POISON SPRITES
    // =========================================================

    [Header("Poison Sprites")]

    public Sprite poisonGreySprite;
    public Sprite poisonFillSprite;


    // =========================================================
    // REPAIR SPRITES
    // =========================================================

    [Header("Repair Sprites")]

    public Sprite repairGreySprite;
    public Sprite repairFillSprite;


    // =========================================================
    // CLONE SPRITES
    // =========================================================

    [Header("Clone Sprites")]

    public Sprite cloneGreySprite;
    public Sprite cloneFillSprite;


    // =========================================================
    // REANIMATE SPRITES
    // =========================================================

    [Header("Reanimate Sprites")]

    public Sprite reanimateGreySprite;
    public Sprite reanimateFillSprite;


    // =========================================================
    // SHOCKWAVE SPRITES
    // =========================================================

    [Header("Shockwave Sprites")]

    public Sprite shockwaveGreySprite;
    public Sprite shockwaveFillSprite;


    // =========================================================
    // DECOY SPRITES
    // =========================================================

    [Header("Decoy Sprites")]

    public Sprite decoyGreySprite;
    public Sprite decoyFillSprite;


    // =========================================================
    // COOLDOWN
    // =========================================================

    [Header("Cooldown")]

    [Tooltip("Image used to display the cooldown fill.")]
    public Image colorFillImage;

    [Tooltip("Text displayed when the powerup is ready.")]
    public TMP_Text readyText;


    // =========================================================
    // RUNTIME
    // =========================================================

    private bool hasPowerupEquipped = false;

    private int displayedPowerupIndex = -2;


    // =========================================================
    // AWAKE
    // =========================================================

    private void Awake()
    {
        // -----------------------------------------------------
        // GREY SLOT
        // -----------------------------------------------------

        if (greySlotImage != null)
        {
            greySlotImage.enabled = true;

            greySlotImage.sprite =
                emptySlotSprite;
        }


        // -----------------------------------------------------
        // FILL / POWERUP IMAGE
        // -----------------------------------------------------

        if (powerupImage != null)
        {
            powerupImage.enabled = false;
        }


        // -----------------------------------------------------
        // COOLDOWN
        // -----------------------------------------------------

        if (colorFillImage != null)
        {
            colorFillImage.fillAmount = 1f;
            colorFillImage.enabled = false;
        }


        // -----------------------------------------------------
        // READY TEXT
        // -----------------------------------------------------

        if (readyText != null)
        {
            readyText.gameObject.SetActive(false);
        }
    }


    // =========================================================
    // START
    // =========================================================

    private void Start()
    {
        SubscribeToManager();

        UpdateHUD();
    }


    // =========================================================
    // ENABLE
    // =========================================================

    private void OnEnable()
    {
        SubscribeToManager();

        UpdateHUD();
    }


    // =========================================================
    // DISABLE
    // =========================================================

    private void OnDisable()
    {
        if (ActivePowerupManager.Instance != null)
        {
            ActivePowerupManager.Instance.OnSlotsChanged -= UpdateHUD;
        }
    }


    // =========================================================
    // SUBSCRIBE
    // =========================================================

    private void SubscribeToManager()
    {
        if (ActivePowerupManager.Instance == null)
            return;

        ActivePowerupManager.Instance.OnSlotsChanged -= UpdateHUD;

        ActivePowerupManager.Instance.OnSlotsChanged += UpdateHUD;
    }


    // =========================================================
    // UPDATE HUD
    // =========================================================

    public void UpdateHUD()
    {
        if (ActivePowerupManager.Instance == null)
        {
            ClearPowerup();
            return;
        }


        int equippedIndex =
            ActivePowerupManager.Instance.GetPowerupInSlot(
                slotIndex
            );


        // =====================================================
        // EMPTY
        // =====================================================

        if (equippedIndex < 0)
        {
            ClearPowerup();
            return;
        }


        // =====================================================
        // POWERUP EQUIPPED
        // =====================================================

        hasPowerupEquipped = true;


        // -----------------------------------------------------
        // GET BOTH SPRITES
        // -----------------------------------------------------

        Sprite greySprite;
        Sprite fillSprite;

        GetPowerupSprites(
            equippedIndex,
            out greySprite,
            out fillSprite
        );


        // -----------------------------------------------------
        // BOTTOM — GREY SPRITE
        // -----------------------------------------------------

        if (greySlotImage != null)
        {
            if (greySprite != null)
            {
                greySlotImage.sprite =
                    greySprite;

                greySlotImage.enabled =
                    true;
            }
            else
            {
                greySlotImage.enabled =
                    false;
            }
        }


        // -----------------------------------------------------
        // TOP — FILL SPRITE
        // -----------------------------------------------------

        if (powerupImage != null)
        {
            if (fillSprite != null)
            {
                powerupImage.sprite =
                    fillSprite;

                powerupImage.enabled =
                    true;
            }
            else
            {
                powerupImage.enabled =
                    false;
            }
        }


        // -----------------------------------------------------
        // TRACK DISPLAYED POWERUP
        // -----------------------------------------------------

        displayedPowerupIndex =
            equippedIndex;


        // -----------------------------------------------------
        // RESET COOLDOWN VISUALS
        // -----------------------------------------------------

        if (colorFillImage != null)
        {
            colorFillImage.fillAmount =
                1f;

            colorFillImage.enabled =
                true;
        }

        SetReady(true);
    }


    // =========================================================
    // CLEAR POWERUP
    // =========================================================

    public void ClearPowerup()
    {
        hasPowerupEquipped = false;

        displayedPowerupIndex = -1;


        // -----------------------------------------------------
        // SHOW EMPTY SLOT
        // -----------------------------------------------------

        if (greySlotImage != null)
        {
            greySlotImage.sprite =
                emptySlotSprite;

            greySlotImage.enabled =
                true;
        }


        // -----------------------------------------------------
        // HIDE FILLED POWERUP SPRITE
        // -----------------------------------------------------

        if (powerupImage != null)
        {
            powerupImage.enabled =
                false;
        }


        // -----------------------------------------------------
        // COOLDOWN
        // -----------------------------------------------------

        if (colorFillImage != null)
        {
            colorFillImage.fillAmount =
                1f;

            colorFillImage.enabled =
                false;
        }


        // -----------------------------------------------------
        // READY
        // -----------------------------------------------------

        SetReady(false);
    }


    // =========================================================
    // START COOLDOWN
    // =========================================================

    public void StartCooldown()
    {
        if (!hasPowerupEquipped)
            return;

        if (colorFillImage != null)
        {
            colorFillImage.enabled =
                true;

            colorFillImage.fillAmount =
                0f;
        }

        SetReady(false);
    }


    // =========================================================
    // UPDATE COOLDOWN
    // =========================================================

    public void UpdateCooldown(float elapsedTime)
    {
        if (!hasPowerupEquipped)
            return;

        if (colorFillImage == null)
            return;


        float normalized =
            Mathf.Clamp01(
                elapsedTime / 60f
            );


        colorFillImage.fillAmount =
            normalized;


        if (normalized >= 1f)
        {
            SetReady(true);
        }
        else
        {
            SetReady(false);
        }
    }


    // =========================================================
    // SET COOLDOWN FILL
    // =========================================================

    public void SetCooldownFill(float value)
    {
        if (colorFillImage == null)
            return;


        colorFillImage.enabled =
            hasPowerupEquipped;


        colorFillImage.fillAmount =
            Mathf.Clamp01(value);
    }


    // =========================================================
    // SET READY
    // =========================================================

    public void SetReady(bool ready)
    {
        if (readyText == null)
            return;


        readyText.gameObject.SetActive(
            ready &&
            hasPowerupEquipped
        );
    }


    // =========================================================
    // COMPATIBILITY VERSION
    // =========================================================

    public void SetReady()
    {
        SetReady(true);
    }


    // =========================================================
    // SET READY TEXT
    // =========================================================

    public void SetReadyText(bool visible)
    {
        if (readyText == null)
            return;


        readyText.gameObject.SetActive(
            visible &&
            hasPowerupEquipped
        );
    }


    // =========================================================
    // REFRESH UI
    // =========================================================

    public void RefreshUI()
    {
        UpdateHUD();
    }


    // =========================================================
    // GET POWERUP SPRITES
    // =========================================================

    private void GetPowerupSprites(
        int powerupIndex,
        out Sprite greySprite,
        out Sprite fillSprite)
    {
        greySprite = null;
        fillSprite = null;


        switch (powerupIndex)
        {
            // -------------------------------------------------
            // FREEZE
            // -------------------------------------------------

            case ActivePowerupManager.FreezeIndex:

                greySprite =
                    freezeGreySprite;

                fillSprite =
                    freezeFillSprite;

                break;


            // -------------------------------------------------
            // POISON
            // -------------------------------------------------

            case ActivePowerupManager.PoisonIndex:

                greySprite =
                    poisonGreySprite;

                fillSprite =
                    poisonFillSprite;

                break;


            // -------------------------------------------------
            // REPAIR
            // -------------------------------------------------

            case ActivePowerupManager.RepairIndex:

                greySprite =
                    repairGreySprite;

                fillSprite =
                    repairFillSprite;

                break;


            // -------------------------------------------------
            // CLONE
            // -------------------------------------------------

            case ActivePowerupManager.CloneIndex:

                greySprite =
                    cloneGreySprite;

                fillSprite =
                    cloneFillSprite;

                break;


            // -------------------------------------------------
            // REANIMATE
            // -------------------------------------------------

            case ActivePowerupManager.ReanimateIndex:

                greySprite =
                    reanimateGreySprite;

                fillSprite =
                    reanimateFillSprite;

                break;


            // -------------------------------------------------
            // SHOCKWAVE
            // -------------------------------------------------

            case ActivePowerupManager.ShockwaveIndex:

                greySprite =
                    shockwaveGreySprite;

                fillSprite =
                    shockwaveFillSprite;

                break;


            // -------------------------------------------------
            // DECOY
            // -------------------------------------------------

            case ActivePowerupManager.DecoyIndex:

                greySprite =
                    decoyGreySprite;

                fillSprite =
                    decoyFillSprite;

                break;
        }
    }
}

