using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ActiveEquip : MonoBehaviour
{
    // =========================================================
    // POWERUP
    // =========================================================

    [Header("Powerup")]

    public int powerupIndex;


    // =========================================================
    // EQUIP BUTTON
    // =========================================================

    [Header("Equip Button")]

    public Button equipButton;

    public TMP_Text equipButtonText;


    // =========================================================
    // UNEQUIP BUTTON
    // =========================================================

    [Header("Unequip Button")]

    public Button unequipButton;

    public TMP_Text unequipButtonText;


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
    // MANAGER SUBSCRIPTION
    // =========================================================

    private void Subscribe()
    {
        if (ActivePowerupManager.Instance != null)
        {
            ActivePowerupManager.Instance.OnSlotsChanged -= RefreshUI;
            ActivePowerupManager.Instance.OnSlotsChanged += RefreshUI;
        }
    }


    private void Unsubscribe()
    {
        if (ActivePowerupManager.Instance != null)
        {
            ActivePowerupManager.Instance.OnSlotsChanged -= RefreshUI;
        }
    }


    // =========================================================
    // EQUIP
    // =========================================================

    public void Equip()
    {
        if (ActivePowerupManager.Instance == null)
            return;

        if (!IsUnlocked())
            return;

        /*
         * IMPORTANT:
         *
         * Do NOT check HasEmptySlot().
         *
         * The new system intentionally replaces whatever is already
         * equipped.
         */

        bool equipped =
            ActivePowerupManager.Instance.EquipPowerup(
                powerupIndex,
                0
            );

        if (!equipped)
            return;

        RefreshUI();
    }


    // =========================================================
    // UNEQUIP
    // =========================================================

    public void Unequip()
    {
        if (ActivePowerupManager.Instance == null)
            return;

        ActivePowerupManager.Instance.UnequipPowerup(powerupIndex);

        RefreshUI();
    }


    // =========================================================
    // UNLOCK CHECK
    // =========================================================

    public bool IsUnlocked()
    {
        switch (powerupIndex)
        {
            case ActivePowerupManager.FreezeIndex:

                FreezePowerupManager freeze =
                    FindFirstObjectByType<FreezePowerupManager>(
                        FindObjectsInactive.Include
                    );

                return freeze != null && freeze.IsUnlocked;


            case ActivePowerupManager.PoisonIndex:

                PoisonPowerupManager poison =
                    FindFirstObjectByType<PoisonPowerupManager>(
                        FindObjectsInactive.Include
                    );

                return poison != null && poison.IsUnlocked;


            case ActivePowerupManager.RepairIndex:

                RepairPowerupManager repair =
                    FindFirstObjectByType<RepairPowerupManager>(
                        FindObjectsInactive.Include
                    );

                return repair != null && repair.IsUnlocked;


            default:
                return false;
        }
    }


    // =========================================================
    // REFRESH
    // =========================================================

    public void RefreshUI()
    {
        if (ActivePowerupManager.Instance == null)
        {
            SetEquipVisible(false);
            SetUnequipVisible(false);
            return;
        }

        bool unlocked = IsUnlocked();

        if (!unlocked)
        {
            SetEquipVisible(false);
            SetUnequipVisible(false);
            return;
        }

        bool thisPowerupEquipped =
            ActivePowerupManager.Instance.IsPowerupEquipped(powerupIndex);

        if (thisPowerupEquipped)
        {
            SetEquipVisible(false);
            SetUnequipVisible(true);
        }
        else
        {
            /*
             * Another powerup may already be equipped.
             *
             * That is completely fine.
             *
             * Clicking this Equip button will replace it.
             */

            SetEquipVisible(true);
            SetUnequipVisible(false);
        }
    }


    // =========================================================
    // BUTTON VISIBILITY
    // =========================================================

    private void SetEquipVisible(bool visible)
    {
        if (equipButton != null)
            equipButton.gameObject.SetActive(visible);
    }


    private void SetUnequipVisible(bool visible)
    {
        if (unequipButton != null)
            unequipButton.gameObject.SetActive(visible);
    }


    // =========================================================
    // SLOT
    // =========================================================

    public int GetCurrentSlot()
    {
        if (ActivePowerupManager.Instance == null)
            return -1;

        return ActivePowerupManager.Instance.GetSlotOfPowerup(powerupIndex);
    }


    // =========================================================
    // NAME
    // =========================================================

    public string GetPowerupName()
    {
        if (ActivePowerupManager.Instance == null)
            return "Unknown";

        return ActivePowerupManager.Instance.GetPowerupName(powerupIndex);
    }
}