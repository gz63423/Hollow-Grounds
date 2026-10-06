using UnityEngine;
using UnityEngine.UI;

public class PassiveHUD : MonoBehaviour
{
    // =========================================================
    // HUD SLOT
    // =========================================================

    [Header("HUD Slot")]

    public int slotIndex = 0;

    public Image slotImage;

    public Sprite emptySlotSprite;


    // =========================================================
    // PASSIVE ICONS
    // =========================================================

    [Header("Passive Icons")]

    public Sprite[] passiveIcons;


    // =========================================================
    // SAVE
    // =========================================================

    private const string EquippedPassiveIndexKey =
        "EquippedPassiveIndex";


    // =========================================================
    // START
    // =========================================================

    private void Start()
    {
        UpdateHUD();
    }


    // =========================================================
    // ON ENABLE
    // =========================================================

    private void OnEnable()
    {
        UpdateHUD();
    }


    // =========================================================
    // UPDATE HUD
    // =========================================================

    public void UpdateHUD()
    {
        if (slotImage == null)
            return;


        slotImage.enabled = true;


        int equippedPassiveIndex = PlayerPrefs.GetInt(
            EquippedPassiveIndexKey,
            -1
        );


        if (equippedPassiveIndex == -1)
        {
            slotImage.sprite = emptySlotSprite;
            return;
        }


        if (equippedPassiveIndex != slotIndex)
        {
            slotImage.sprite = emptySlotSprite;
            return;
        }


        if (passiveIcons == null ||
            slotIndex < 0 ||
            slotIndex >= passiveIcons.Length)
        {
            slotImage.sprite = emptySlotSprite;
            return;
        }


        slotImage.sprite =
            passiveIcons[slotIndex];
    }
}