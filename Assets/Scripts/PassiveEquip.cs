using UnityEngine;
using UnityEngine.UI;

public class PassiveEquip : MonoBehaviour
{
    [Header("Powerup")]
    public PassivePowerupManager powerup;

    [Header("Equip UI")]
    public GameObject equipButton;
    public GameObject equipButtonText;
    public GameObject unequipButton;
    public GameObject unequipButtonText;

    [Header("Passive Slot")]
    public int slotIndex = 0;
    public Image passiveSlotImage;
    public Sprite emptySlotSprite;
    public Sprite passiveSlotIcon;

    private void Start()
    {
        if (powerup != null)
            powerup.ReloadPowerup();

        UpdateUI();
    }

    private void OnEnable()
    {
        if (powerup != null)
            powerup.ReloadPowerup();

        UpdateUI();
    }

    public void Equip()
    {
        if (powerup == null)
        {
            Debug.LogError(
                gameObject.name +
                " is missing a PassivePowerupManager reference."
            );
            return;
        }

        powerup.ReloadPowerup();

        if (!powerup.IsUnlocked())
            return;

        powerup.EquipPowerup();

        UpdateUI();
    }

    public void Unequip()
    {
        if (powerup == null)
        {
            Debug.LogError(
                gameObject.name +
                " is missing a PassivePowerupManager reference."
            );
            return;
        }

        powerup.ReloadPowerup();

        if (!powerup.IsEquipped())
            return;

        powerup.UnequipPowerup();

        UpdateUI();
    }

    private void UpdateUI()
    {
        if (powerup == null)
            return;

        if (!powerup.IsUnlocked())
        {
            SetEquipButtons(false, false);
            SetSlotEmpty();
            return;
        }

        if (powerup.IsEquipped())
        {
            SetEquipButtons(false, true);
            SetSlotIcon();
            return;
        }

        SetEquipButtons(true, false);
        SetSlotEmpty();
    }

    private void SetEquipButtons(bool showEquip, bool showUnequip)
    {
        if (equipButton != null)
            equipButton.SetActive(showEquip);

        if (equipButtonText != null)
            equipButtonText.SetActive(showEquip);

        if (unequipButton != null)
            unequipButton.SetActive(showUnequip);

        if (unequipButtonText != null)
            unequipButtonText.SetActive(showUnequip);
    }

    private void SetSlotIcon()
    {
        if (passiveSlotImage == null)
            return;

        passiveSlotImage.sprite = passiveSlotIcon;
        passiveSlotImage.enabled = true;
    }

    private void SetSlotEmpty()
    {
        if (passiveSlotImage == null)
            return;

        passiveSlotImage.sprite = emptySlotSprite;
        passiveSlotImage.enabled = true;
    }
}