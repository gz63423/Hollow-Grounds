using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PassiveDragUnequip : MonoBehaviour,
    IBeginDragHandler,
    IDragHandler,
    IEndDragHandler
{
    [Header("Powerup")]
    public PassivePowerupManager powerup;

    public Sprite powerupIcon;

    [Header("Slot")]
    public Image slotImage;
    public Sprite emptySlotSprite;

    [Header("Drag Settings")]
    public Vector2 dragIconSize = new Vector2(50f, 50f);

    [Header("Canvas")]
    public Canvas canvas;

    private GameObject dragIcon;
    private RectTransform dragIconRect;
    private CanvasGroup originalCanvasGroup;

    private void Start()
    {
        if (canvas == null)
            canvas = GetComponentInParent<Canvas>();

        UpdateSlotImage();
    }

    private void OnEnable()
    {
        UpdateSlotImage();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (powerup == null)
            return;

        if (!powerup.IsEquipped())
            return;

        if (canvas == null)
        {
            Debug.LogError(gameObject.name + " could not find a Canvas.");
            return;
        }

        if (powerupIcon == null)
        {
            Debug.LogError(gameObject.name + " does not have a Powerup Icon assigned.");
            return;
        }

        dragIcon = new GameObject(powerup.name + "_DragIcon");

        dragIcon.transform.SetParent(canvas.transform, false);

        Image image = dragIcon.AddComponent<Image>();

        image.sprite = powerupIcon;
        image.preserveAspect = true;
        image.raycastTarget = false;

        dragIconRect = dragIcon.GetComponent<RectTransform>();
        dragIconRect.sizeDelta = dragIconSize;

        UpdateDragPosition(eventData);

        dragIcon.transform.SetAsLastSibling();

        originalCanvasGroup = GetComponent<CanvasGroup>();

        if (originalCanvasGroup == null)
            originalCanvasGroup = gameObject.AddComponent<CanvasGroup>();

        originalCanvasGroup.alpha = 0.5f;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (dragIcon == null)
            return;

        UpdateDragPosition(eventData);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (dragIcon == null)
        {
            RestoreOriginalButton();
            return;
        }

        bool releasedInsideButton = IsPointerInsideButton(eventData);

        if (!releasedInsideButton)
        {
            if (powerup != null && powerup.IsEquipped())
            {
                powerup.UnequipPowerup();
                UpdateSlotImage();
            }
        }

        Destroy(dragIcon);

        dragIcon = null;
        dragIconRect = null;

        RestoreOriginalButton();
    }

    private void UpdateDragPosition(PointerEventData eventData)
    {
        if (dragIconRect == null)
            return;

        RectTransform canvasRect = canvas.transform as RectTransform;

        Camera cam = null;

        if (canvas.renderMode != RenderMode.ScreenSpaceOverlay)
            cam = canvas.worldCamera;

        Vector2 localPosition;

        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            eventData.position,
            cam,
            out localPosition))
        {
            dragIconRect.localPosition = localPosition;
        }
    }

    private bool IsPointerInsideButton(PointerEventData eventData)
    {
        RectTransform buttonRect = GetComponent<RectTransform>();

        if (buttonRect == null)
            return false;

        Camera cam = null;

        if (canvas != null && canvas.renderMode != RenderMode.ScreenSpaceOverlay)
            cam = canvas.worldCamera;

        return RectTransformUtility.RectangleContainsScreenPoint(
            buttonRect,
            eventData.position,
            cam
        );
    }

    private void UpdateSlotImage()
    {
        if (slotImage == null)
            return;

        if (powerup != null && powerup.IsEquipped())
        {
            slotImage.sprite = powerupIcon;
        }
        else
        {
            slotImage.sprite = emptySlotSprite;
        }

        slotImage.enabled = true;
    }

    private void RestoreOriginalButton()
    {
        if (originalCanvasGroup != null)
            originalCanvasGroup.alpha = 1f;
    }
}