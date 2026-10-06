using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PassiveDragUI : MonoBehaviour,
    IBeginDragHandler,
    IDragHandler,
    IEndDragHandler
{
    // =========================================================
    // POWERUP
    // =========================================================

    [Header("Powerup")]

    [Tooltip("The PassivePowerupManager for this powerup.")]
    public PassivePowerupManager powerup;

    [Tooltip("The icon that will be created when dragging.")]
    public Sprite powerupIcon;


    // =========================================================
    // DRAG SETTINGS
    // =========================================================

    [Header("Drag Settings")]

    [Tooltip("Size of the created drag icon.")]
    public Vector2 dragIconSize = new Vector2(50f, 50f);


    // =========================================================
    // CANVAS
    // =========================================================

    [Header("Canvas")]

    [Tooltip("Canvas where the drag icon will be created.")]
    public Canvas canvas;


    // =========================================================
    // PASSIVE SLOT
    // =========================================================

    [Header("Passive Slot")]

    [Tooltip("The Image component of the passive slot.")]
    public Image passiveSlotImage;


    // =========================================================
    // INTERNAL
    // =========================================================

    private GameObject dragIcon;
    private RectTransform dragIconRect;

    private CanvasGroup originalCanvasGroup;


    // =========================================================
    // START
    // =========================================================

    private void Start()
    {
        // Automatically find the parent canvas if one
        // wasn't assigned in the Inspector.
        if (canvas == null)
        {
            canvas = GetComponentInParent<Canvas>();
        }
    }


    // =========================================================
    // BEGIN DRAG
    // =========================================================

    public void OnBeginDrag(PointerEventData eventData)
    {
        // -----------------------------------------------------
        // SAFETY
        // -----------------------------------------------------

        if (powerup == null)
            return;

        if (!powerup.IsUnlocked())
            return;

        if (canvas == null)
        {
            Debug.LogError(
                gameObject.name +
                " could not find a Canvas."
            );

            return;
        }

        if (powerupIcon == null)
        {
            Debug.LogError(
                gameObject.name +
                " does not have a Powerup Icon assigned."
            );

            return;
        }


        // -----------------------------------------------------
        // CREATE DRAG ICON
        // -----------------------------------------------------

        dragIcon = new GameObject(
            powerup.name + "_DragIcon"
        );


        // Make it a child of the canvas
        dragIcon.transform.SetParent(
            canvas.transform,
            false
        );


        // -----------------------------------------------------
        // IMAGE
        // -----------------------------------------------------

        Image image =
            dragIcon.AddComponent<Image>();

        image.sprite = powerupIcon;

        image.preserveAspect = true;

        image.raycastTarget = false;


        // -----------------------------------------------------
        // RECT TRANSFORM
        // -----------------------------------------------------

        dragIconRect =
            dragIcon.GetComponent<RectTransform>();

        dragIconRect.sizeDelta =
            dragIconSize;


        // -----------------------------------------------------
        // POSITION
        // -----------------------------------------------------

        UpdateDragPosition(eventData);


        // -----------------------------------------------------
        // SORT TO FRONT
        // -----------------------------------------------------

        dragIcon.transform.SetAsLastSibling();


        // -----------------------------------------------------
        // MAKE ORIGINAL BUTTON TRANSPARENT
        // -----------------------------------------------------

        originalCanvasGroup =
            GetComponent<CanvasGroup>();

        if (originalCanvasGroup == null)
        {
            originalCanvasGroup =
                gameObject.AddComponent<CanvasGroup>();
        }

        originalCanvasGroup.alpha = 0.5f;
    }


    // =========================================================
    // DRAG
    // =========================================================

    public void OnDrag(PointerEventData eventData)
    {
        if (dragIcon == null)
            return;

        UpdateDragPosition(eventData);
    }


    // =========================================================
    // END DRAG
    // =========================================================

    public void OnEndDrag(PointerEventData eventData)
    {
        if (dragIcon == null)
        {
            RestoreOriginalButton();
            return;
        }


        // -----------------------------------------------------
        // CHECK WHAT WE DROPPED ON
        // -----------------------------------------------------

        bool droppedOnSlot =
            IsPointerOverPassiveSlot(eventData);


        // -----------------------------------------------------
        // DROPPED ON SLOT
        // -----------------------------------------------------

        if (droppedOnSlot)
        {
            EquipPowerup();
        }

        // -----------------------------------------------------
        // DRAGGED OFF SLOT
        // -----------------------------------------------------

        else
        {
            // Only unequip if this powerup was equipped.
            if (powerup != null &&
                powerup.IsEquipped())
            {
                UnequipPowerup();
            }
        }


        // -----------------------------------------------------
        // CLEAN UP
        // -----------------------------------------------------

        Destroy(dragIcon);

        dragIcon = null;
        dragIconRect = null;

        RestoreOriginalButton();
    }


    // =========================================================
    // UPDATE DRAG POSITION
    // =========================================================

    private void UpdateDragPosition(
        PointerEventData eventData)
    {
        if (dragIconRect == null)
            return;

        RectTransform canvasRect =
            canvas.transform as RectTransform;

        Camera cam = null;

        if (canvas.renderMode != RenderMode.ScreenSpaceOverlay)
        {
            cam = canvas.worldCamera;
        }


        Vector2 localPosition;

        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            eventData.position,
            cam,
            out localPosition))
        {
            dragIconRect.localPosition =
                localPosition;
        }
    }


    // =========================================================
    // CHECK PASSIVE SLOT
    // =========================================================

    private bool IsPointerOverPassiveSlot(
        PointerEventData eventData)
    {
        if (passiveSlotImage == null)
            return false;


        RectTransform slotRect =
            passiveSlotImage.rectTransform;


        Camera cam = null;

        if (canvas != null &&
            canvas.renderMode != RenderMode.ScreenSpaceOverlay)
        {
            cam = canvas.worldCamera;
        }


        return RectTransformUtility.RectangleContainsScreenPoint(
            slotRect,
            eventData.position,
            cam
        );
    }


    // =========================================================
    // EQUIP
    // =========================================================

    private void EquipPowerup()
    {
        if (powerup == null)
            return;

        if (!powerup.IsUnlocked())
            return;

        powerup.EquipPowerup();


        // Update slot image
        if (passiveSlotImage != null)
        {
            passiveSlotImage.sprite =
                powerupIcon;

            passiveSlotImage.enabled = true;
        }
    }


    // =========================================================
    // UNEQUIP
    // =========================================================

    private void UnequipPowerup()
    {
        if (powerup == null)
            return;

        powerup.UnequipPowerup();


        // We intentionally do not change the slot
        // sprite here. The PassiveEquip script can
        // handle the normal empty-slot appearance.
    }


    // =========================================================
    // RESTORE BUTTON
    // =========================================================

    private void RestoreOriginalButton()
    {
        if (originalCanvasGroup != null)
        {
            originalCanvasGroup.alpha = 1f;
        }
    }
}