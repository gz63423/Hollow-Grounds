using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ActiveDragUnequip : MonoBehaviour,
    IBeginDragHandler,
    IDragHandler,
    IEndDragHandler
{
    // =========================================================
    // SLOT
    // =========================================================

    [Header("Slot")]

    public int slotIndex = 0;

    public Image slotImage;

    public Sprite emptySlotSprite;


    // =========================================================
    // POWERUP SPRITES
    // =========================================================

    [Header("Powerup Sprites")]

    public Sprite freezeSprite;
    public Sprite poisonSprite;
    public Sprite repairSprite;
    public Sprite cloneSprite;
    public Sprite reanimateSprite;
    public Sprite shockwaveSprite;
    public Sprite decoySprite;


    // =========================================================
    // DRAG
    // =========================================================

    [Header("Drag")]

    public float dragIconSize = 80f;

    public Canvas canvas;


    // =========================================================
    // INTERNAL
    // =========================================================

    private GameObject dragObject;

    private int draggedPowerupIndex = -1;

    private Sprite draggedSprite;

    private bool isDragging;


    // =========================================================
    // UNITY
    // =========================================================

    private void OnEnable()
    {
        Subscribe();

        RefreshUI();
    }


    private void Start()
    {
        Subscribe();

        RefreshUI();
    }


    private void OnDisable()
    {
        Unsubscribe();
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
        if (ActivePowerupManager.Instance == null)
            return;


        // -----------------------------------------------------
        // READ ACTUAL SLOT
        // -----------------------------------------------------

        draggedPowerupIndex =
            ActivePowerupManager.Instance.GetPowerupInSlot(
                slotIndex
            );


        if (draggedPowerupIndex == -1)
            return;


        // -----------------------------------------------------
        // GET DRAG SPRITE
        // -----------------------------------------------------

        draggedSprite =
            GetPowerupSprite(draggedPowerupIndex);


        if (draggedSprite == null)
        {
            draggedSprite =
                ActivePowerupManager.Instance.GetPowerupIcon(
                    draggedPowerupIndex
                );
        }


        if (draggedSprite == null)
            return;


        // -----------------------------------------------------
        // CANVAS
        // -----------------------------------------------------

        if (canvas == null)
            canvas = GetComponentInParent<Canvas>();

        if (canvas == null)
            return;


        // -----------------------------------------------------
        // CREATE DRAG OBJECT
        // -----------------------------------------------------

        isDragging = true;

        dragObject = new GameObject(
            "ActivePowerupDragIcon",
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


        Image image =
            dragObject.GetComponent<Image>();

        image.sprite = draggedSprite;
        image.preserveAspect = true;
        image.raycastTarget = false;


        CanvasGroup group =
            dragObject.GetComponent<CanvasGroup>();

        group.alpha = 0.85f;


        UpdateDragPosition(eventData);
    }


    // =========================================================
    // DRAG
    // =========================================================

    public void OnDrag(PointerEventData eventData)
    {
        if (!isDragging)
            return;

        UpdateDragPosition(eventData);
    }


    private void UpdateDragPosition(
        PointerEventData eventData)
    {
        if (dragObject == null || canvas == null)
            return;

        RectTransform canvasRect =
            canvas.transform as RectTransform;

        Camera cameraToUse = null;

        if (canvas.renderMode != RenderMode.ScreenSpaceOverlay)
        {
            cameraToUse = canvas.worldCamera;
        }

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            eventData.position,
            cameraToUse,
            out Vector2 localPoint
        );

        dragObject.GetComponent<RectTransform>().localPosition =
            localPoint;
    }


    // =========================================================
    // END DRAG
    // =========================================================

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!isDragging)
            return;

        isDragging = false;


        bool droppedBackOnSlot =
            IsPointerOverSlot(eventData);


        /*
         * If released outside the slot:
         *     unequip.
         *
         * If released back on the slot:
         *     keep equipped.
         */

        if (!droppedBackOnSlot)
        {
            UnequipDraggedPowerup();
        }


        DestroyDragObject();

        draggedPowerupIndex = -1;

        draggedSprite = null;

        RefreshUI();
    }


    // =========================================================
    // UNEQUIP
    // =========================================================

    private void UnequipDraggedPowerup()
    {
        if (ActivePowerupManager.Instance == null)
            return;

        if (draggedPowerupIndex == -1)
            return;


        ActivePowerupManager.Instance.UnequipPowerup(
            draggedPowerupIndex
        );
    }


    // =========================================================
    // SLOT CHECK
    // =========================================================

    private bool IsPointerOverSlot(
        PointerEventData eventData)
    {
        if (slotImage == null)
            return false;

        Camera cameraToUse = null;

        if (canvas != null &&
            canvas.renderMode != RenderMode.ScreenSpaceOverlay)
        {
            cameraToUse = canvas.worldCamera;
        }

        return RectTransformUtility.RectangleContainsScreenPoint(
            slotImage.rectTransform,
            eventData.position,
            cameraToUse
        );
    }


    // =========================================================
    // DESTROY DRAG
    // =========================================================

    private void DestroyDragObject()
    {
        if (dragObject != null)
        {
            Destroy(dragObject);
        }

        dragObject = null;
    }


    // =========================================================
    // REFRESH
    // =========================================================

    public void RefreshUI()
    {
        UpdateSlotImage();
    }


    // =========================================================
    // SLOT IMAGE
    // =========================================================

    private void UpdateSlotImage()
    {
        if (slotImage == null)
            return;


        // -----------------------------------------------------
        // NO MANAGER
        // -----------------------------------------------------

        if (ActivePowerupManager.Instance == null)
        {
            SetEmpty();
            return;
        }


        // -----------------------------------------------------
        // READ ACTUAL SHARED SLOT
        // -----------------------------------------------------

        int equippedIndex =
            ActivePowerupManager.Instance.GetPowerupInSlot(
                slotIndex
            );


        // -----------------------------------------------------
        // ACTUALLY EMPTY
        // -----------------------------------------------------

        if (equippedIndex == -1)
        {
            SetEmpty();
            return;
        }


        // -----------------------------------------------------
        // OCCUPIED
        // -----------------------------------------------------

        Sprite sprite =
            GetPowerupSprite(equippedIndex);


        // Try the central manager if the local sprite wasn't
        // assigned.

        if (sprite == null)
        {
            sprite =
                ActivePowerupManager.Instance.GetPowerupIcon(
                    equippedIndex
                );
        }


        // -----------------------------------------------------
        // ICON FOUND
        // -----------------------------------------------------

        if (sprite != null)
        {
            slotImage.sprite = sprite;
            slotImage.enabled = true;
        }


        // -----------------------------------------------------
        // ICON NOT FOUND
        // -----------------------------------------------------
        //
        // IMPORTANT:
        // Do NOT call SetEmpty().
        //
        // The slot is occupied according to the manager.
        // -----------------------------------------------------
    }


    // =========================================================
    // EMPTY
    // =========================================================

    private void SetEmpty()
    {
        if (slotImage == null)
            return;

        if (emptySlotSprite != null)
        {
            slotImage.sprite =
                emptySlotSprite;

            slotImage.enabled = true;
        }
    }


    // =========================================================
    // SPRITE LOOKUP
    // =========================================================

    private Sprite GetPowerupSprite(int index)
    {
        switch (index)
        {
            case ActivePowerupManager.FreezeIndex:
                return freezeSprite;

            case ActivePowerupManager.PoisonIndex:
                return poisonSprite;

            case ActivePowerupManager.RepairIndex:
                return repairSprite;

            case ActivePowerupManager.CloneIndex:
                return cloneSprite;

            case ActivePowerupManager.ReanimateIndex:
                return reanimateSprite;

            case ActivePowerupManager.ShockwaveIndex:
                return shockwaveSprite;

            case ActivePowerupManager.DecoyIndex:
                return decoySprite;

            default:
                return null;
        }
    }
}