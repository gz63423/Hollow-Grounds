using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ActiveEquip : MonoBehaviour
{
    [Header("Powerup")]

    public int powerupIndex;



    [Header("Equip Button")]

    public Button equipButton;

    public TMP_Text equipButtonText;



    [Header("Unequip Button")]

    public Button unequipButton;

    public TMP_Text unequipButtonText;




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



    //Equip


    public void Equip()
    {
        if (ActivePowerupManager.Instance == null)
            return;

        if (!IsUnlocked())
            return;

        bool equipped =
            ActivePowerupManager.Instance.EquipPowerup(
                powerupIndex,
                0
            );

        if (!equipped)
            return;

        RefreshUI();
    }




    public void Unequip()
    {
        if (ActivePowerupManager.Instance == null)
            return;

        ActivePowerupManager.Instance.UnequipPowerup(powerupIndex);

        RefreshUI();
    }



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



    // Refres


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
            SetEquipVisible(true);
            SetUnequipVisible(false);
        }
    }



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



    public int GetCurrentSlot()
    {
        if (ActivePowerupManager.Instance == null)
            return -1;

        return ActivePowerupManager.Instance.GetSlotOfPowerup(powerupIndex);
    }




    public string GetPowerupName()
    {
        if (ActivePowerupManager.Instance == null)
            return "Unknown";

        return ActivePowerupManager.Instance.GetPowerupName(powerupIndex);
    }
}