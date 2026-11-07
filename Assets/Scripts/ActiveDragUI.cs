using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ActiveDragUI : MonoBehaviour,
    IBeginDragHandler,
    IDragHandler,
    IEndDragHandler
{
    // =========================================================
    // POWERUP
    // =========================================================

    [Header("Powerup")]

    [Tooltip("Assign the FreezePowerupManager, PoisonPowerupManager, or RepairPowerupManager.")]
    public MonoBehaviour powerup;

    [Tooltip("Automatically determined from the assigned powerup manager.")]
    public int powerupIndex;

    public Image powerupIcon;


    // =========================================================
    // ACTIVE EQUIP
    // =========================================================

    [Header("Active Equip")]

    public ActiveEquip activeEquip;


    // =========================================================
    // DRAG
    // =========================================================

    [Header("Drag")]

    public float dragIconSize = 80f;

    public Canvas canvas;


    // =========================================================
    // ACTIVE SLOT DISPLAY
    // =========================================================

    [Header("Active Slot Display")]

    public Image activeSlotImage;

    public Sprite emptySlotSprite;


    // =========================================================
    // INTERNAL
    // =========================================================

    private GameObject dragObject;

    private Image dragImage;

    private bool isDragging;


    // =========================================================
    // UNITY
    // =========================================================

    private void OnEnable()
    {
        UpdatePowerupIndex();

        Subscribe();

        RefreshUI();
    }


    private void Start()
    {
        UpdatePowerupIndex();

        Subscribe();

        RefreshUI();
    }


    private void OnDisable()
    {
        Unsubscribe();
    }


    // =========================================================
    // POWERUP INDEX
    // =========================================================

    private void UpdatePowerupIndex()
    {
        // -----------------------------------------------------
        // FREEZE
        // -----------------------------------------------------

        if (powerup is FreezePowerupManager)
        {
            powerupIndex =
                ActivePowerupManager.FreezeIndex;

            return;
        }


        // -----------------------------------------------------
        // POISON
        // -----------------------------------------------------

        if (powerup is PoisonPowerupManager)
        {
            powerupIndex =
                ActivePowerupManager.PoisonIndex;

            return;
        }


        // -----------------------------------------------------
        // REPAIR
        // -----------------------------------------------------

        if (powerup is RepairPowerupManager)
        {
            powerupIndex =
                ActivePowerupManager.RepairIndex;

            return;
        }


        // -----------------------------------------------------
        // UNKNOWN
        // -----------------------------------------------------

        powerupIndex = -1;
    }


    // =========================================================
    // SUBSCRIBE
    // =========================================================

    private void Subscribe()
    {
        if (ActivePowerupManager.Instance == null)
            return;

        ActivePowerupManager.Instance.OnSlotsChanged -= RefreshUI;

        ActivePowerupManager.Instance.OnSlotsChanged += RefreshUI;
    }


    // =========================================================
    // UNSUBSCRIBE
    // =========================================================

    private void Unsubscribe()
    {
        if (ActivePowerupManager.Instance == null)
            return;

        ActivePowerupManager.Instance.OnSlotsChanged -= RefreshUI;
    }


    // =========================================================
    // BEGIN DRAG
    // =========================================================

    public void OnBeginDrag(PointerEventData eventData)
    {
        UpdatePowerupIndex();

        if (!IsUnlocked())
            return;


        if (canvas == null)
            canvas = GetComponentInParent<Canvas>();


        if (canvas == null)
            return;


        if (powerupIcon == null)
            return;


        isDragging = true;


        dragObject =
            new GameObject(
                "PowerupDragIcon",
                typeof(RectTransform),
                typeof(CanvasGroup),
                typeof(Image)
            );


        dragObject.transform.SetParent(
            canvas.transform,
            false
        );


        RectTransform rect =
            dragObject.GetComponent<RectTransform>();


        rect.sizeDelta =
            new Vector2(
                dragIconSize,
                dragIconSize
            );


        dragImage =
            dragObject.GetComponent<Image>();


        dragImage.sprite =
            powerupIcon.sprite;


        dragImage.preserveAspect =
            true;


        dragImage.raycastTarget =
            false;


        CanvasGroup group =
            dragObject.GetComponent<CanvasGroup>();


        group.alpha =
            0.85f;


        UpdateDragPosition(eventData);
    }


    // =========================================================
    // DRAG
    // =========================================================

    public void OnDrag(
        PointerEventData eventData
    )
    {
        if (!isDragging)
            return;

        UpdateDragPosition(eventData);
    }


    private void UpdateDragPosition(
        PointerEventData eventData
    )
    {
        if (dragObject == null ||
            canvas == null)
        {
            return;
        }


        RectTransform canvasRect =
            canvas.transform as RectTransform;


        Camera cameraToUse = null;


        if (canvas.renderMode !=
            RenderMode.ScreenSpaceOverlay)
        {
            cameraToUse =
                canvas.worldCamera;
        }


        RectTransformUtility
            .ScreenPointToLocalPointInRectangle(
                canvasRect,
                eventData.position,
                cameraToUse,
                out Vector2 localPoint
            );


        dragObject
            .GetComponent<RectTransform>()
            .localPosition =
            localPoint;
    }


    // =========================================================
    // END DRAG
    // =========================================================

    public void OnEndDrag(
        PointerEventData eventData
    )
    {
        if (!isDragging)
            return;


        isDragging = false;


        bool droppedOnSlot =
            IsPointerOverActiveSlot(eventData);


        if (droppedOnSlot)
        {
            EquipPowerup();
        }


        DestroyDragObject();

        RefreshUI();
    }


    // =========================================================
    // EQUIP
    // =========================================================

    public void EquipPowerup()
    {
        if (ActivePowerupManager.Instance == null)
            return;


        UpdatePowerupIndex();


        if (!IsUnlocked())
            return;


        ActivePowerupManager.Instance.EquipPowerup(
            powerupIndex,
            0
        );


        RefreshUI();
    }


    // =========================================================
    // ACTIVE SLOT CHECK
    // =========================================================

    private bool IsPointerOverActiveSlot(
        PointerEventData eventData
    )
    {
        if (activeSlotImage == null)
            return false;


        RectTransform rect =
            activeSlotImage.rectTransform;


        Camera cameraToUse = null;


        if (canvas != null &&
            canvas.renderMode !=
            RenderMode.ScreenSpaceOverlay)
        {
            cameraToUse =
                canvas.worldCamera;
        }


        return RectTransformUtility
            .RectangleContainsScreenPoint(
                rect,
                eventData.position,
                cameraToUse
            );
    }


    // =========================================================
    // DESTROY DRAG OBJECT
    // =========================================================

    private void DestroyDragObject()
    {
        if (dragObject != null)
        {
            Destroy(dragObject);
        }


        dragObject = null;

        dragImage = null;
    }


    // =========================================================
    // UNLOCK CHECK
    // =========================================================

    private bool IsUnlocked()
    {
        if (powerup == null)
            return false;


        FreezePowerupManager freeze =
            powerup as FreezePowerupManager;


        if (freeze != null)
        {
            return freeze.IsUnlocked;
        }


        PoisonPowerupManager poison =
            powerup as PoisonPowerupManager;


        if (poison != null)
        {
            return poison.IsUnlocked;
        }


        RepairPowerupManager repair =
            powerup as RepairPowerupManager;


        if (repair != null)
        {
            return repair.IsUnlocked;
        }


        return false;
    }


    // =========================================================
    // REFRESH UI
    // =========================================================


    public void RefreshUI()
    {
        UpdatePowerupIndex();

        // Only update this tab's equip buttons here.
        // The shared bottom slot should have one owner.
        if (activeEquip != null)
        {
            activeEquip.RefreshUI();
        }
    }



    // =========================================================
    // UPDATE SLOT IMAGE
    // =========================================================


    private void UpdateSlotImage()
    {
        // Intentionally do not update the shared bottom slot here.
        // ActiveDragUnequip is responsible for displaying the
        // currently equipped powerup.
    }



    // =========================================================
    // EMPTY SLOT
    // =========================================================

    private void SetEmptySlot()
    {
        if (activeSlotImage == null)
            return;


        if (emptySlotSprite != null)
        {
            activeSlotImage.sprite =
                emptySlotSprite;

            activeSlotImage.enabled =
                true;
        }
    }
}