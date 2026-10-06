using NUnit.Framework;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ActivePowerupManager : MonoBehaviour
{
    // =========================================================
    // SINGLETON
    // =========================================================

    public static ActivePowerupManager Instance;


    // =========================================================
    // SAVE KEYS
    // =========================================================

    private const string ActiveSlot0Key = "ActiveSlot_0";
    private const string ActiveSlot1Key = "ActiveSlot_1";
    private const string ActiveSlot2Key = "ActiveSlot_2";


    // =========================================================
    // POWERUP INDEXES
    // =========================================================

    public const int FreezeIndex = 0;
    public const int PoisonIndex = 1;
    public const int RepairIndex = 2;
    public const int CloneIndex = 3;
    public const int ReanimateIndex = 4;
    public const int ShockwaveIndex = 5;
    public const int DecoyIndex = 6;


    // =========================================================
    // ICON DATA
    // =========================================================

    [Serializable]
    public class PowerupIconData
    {
        public int powerupIndex;
        public string powerupName;

        public Sprite normalIcon;
        public Sprite greyedIcon;
    }

    [Header("Powerup Icons")]

    [Tooltip("Assign one entry for each powerup index. The index must match the powerup's powerupIndex.")]
    public PowerupIconData[] powerupIcons;


    // =========================================================
    // SLOT DATA
    // =========================================================

    /*
     * Only Slot 0 is currently used.
     *
     * -1 = Empty
     *  0 = Freeze
     *  1 = Poison
     *  2 = Repair
     *  3 = Clone
     *  4 = Reanimate
     *  5 = Shockwave
     *  6 = Decoy
     */

    private int[] equippedSlots = new int[1];

    public event Action OnSlotsChanged;


    // =========================================================
    // UNITY
    // =========================================================

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        DontDestroyOnLoad(gameObject);

        LoadSlots();

        SceneManager.sceneLoaded += OnSceneLoaded;

        // Refresh once after Awake.
        RefreshAllActiveUI();
    }


    private void OnDestroy()
    {
        if (Instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }


    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        /*
         * UI objects may not have finished enabling yet when
         * sceneLoaded fires.
         *
         * Wait one frame so every UI object has been created,
         * then refresh ALL powerup UI, including inactive tabs.
         */

        StartCoroutine(RefreshAfterSceneLoad());
    }


    private IEnumerator RefreshAfterSceneLoad()
    {
        yield return null;

        RefreshAllActiveUI();

        yield return null;

        RefreshAllActiveUI();
    }


    // =========================================================
    // LOAD / SAVE
    // =========================================================

    private void LoadSlots()
    {
        equippedSlots[0] =
            PlayerPrefs.GetInt(
                ActiveSlot0Key,
                -1
            );

        // Old versions had three active slots.
        // Make absolutely sure they cannot affect the current system.
        PlayerPrefs.SetInt(
            ActiveSlot1Key,
            -1
        );

        PlayerPrefs.SetInt(
            ActiveSlot2Key,
            -1
        );

        if (!IsValidPowerupIndex(equippedSlots[0]))
        {
            equippedSlots[0] = -1;
        }
    }


    private void SaveSlots()
    {
        PlayerPrefs.SetInt(
            ActiveSlot0Key,
            equippedSlots[0]
        );

        // Keep old save keys cleared.
        PlayerPrefs.SetInt(
            ActiveSlot1Key,
            -1
        );

        PlayerPrefs.SetInt(
            ActiveSlot2Key,
            -1
        );

        PlayerPrefs.Save();

        NotifySlotsChanged();
    }


    // =========================================================
    // EQUIP
    // =========================================================

    public bool EquipPowerup(
        int powerupIndex,
        int slotIndex = 0
    )
    {
        if (!IsValidPowerupIndex(powerupIndex))
            return false;

        if (slotIndex != 0)
            return false;

        // Already equipped.
        if (equippedSlots[0] == powerupIndex)
        {
            NotifySlotsChanged();

            return true;
        }

        /*
         * Only one active slot exists.
         *
         * Equipping a new powerup automatically replaces
         * the currently equipped powerup.
         */

        equippedSlots[0] =
            powerupIndex;

        SaveSlots();

        Debug.Log(
            $"[ActivePowerupManager] Equipped {GetPowerupName(powerupIndex)} in Slot 0."
        );

        return true;
    }


    // =========================================================
    // UNEQUIP
    // =========================================================

    public bool UnequipPowerup(
        int powerupIndex
    )
    {
        if (!IsValidPowerupIndex(powerupIndex))
            return false;

        if (equippedSlots[0] != powerupIndex)
        {
            NotifySlotsChanged();

            return false;
        }

        equippedSlots[0] = -1;

        SaveSlots();

        Debug.Log(
            $"[ActivePowerupManager] Unequipped {GetPowerupName(powerupIndex)}."
        );

        return true;
    }


    // =========================================================
    // SLOT ACCESS
    // =========================================================

    public int GetPowerupInSlot(
        int slotIndex
    )
    {
        if (slotIndex != 0)
            return -1;

        return equippedSlots[0];
    }


    public int GetSlotOfPowerup(
        int powerupIndex
    )
    {
        if (!IsValidPowerupIndex(powerupIndex))
            return -1;

        return equippedSlots[0] == powerupIndex
            ? 0
            : -1;
    }


    public bool IsPowerupEquipped(
        int powerupIndex
    )
    {
        return equippedSlots[0] == powerupIndex;
    }


    public bool IsSlotOccupied(
        int slotIndex
    )
    {
        if (slotIndex != 0)
            return false;

        return equippedSlots[0] != -1;
    }


    public bool HasEmptySlot()
    {
        return equippedSlots[0] == -1;
    }


    public int GetFirstEmptySlot()
    {
        return HasEmptySlot()
            ? 0
            : -1;
    }


    public int GetSlotCount()
    {
        return 1;
    }


    public void ClearSlot(
        int slotIndex
    )
    {
        if (slotIndex != 0)
            return;

        if (equippedSlots[0] == -1)
            return;

        equippedSlots[0] = -1;

        SaveSlots();
    }


    public void ResetAllSlots()
    {
        equippedSlots[0] = -1;

        SaveSlots();
    }


    // =========================================================
    // ICON LOOKUP
    // =========================================================

    public Sprite GetPowerupIcon(
        int powerupIndex
    )
    {
        if (powerupIcons == null)
            return null;

        for (int i = 0;
             i < powerupIcons.Length;
             i++)
        {
            if (powerupIcons[i] == null)
                continue;

            if (powerupIcons[i].powerupIndex ==
                powerupIndex)
            {
                return powerupIcons[i].normalIcon;
            }
        }

        return null;
    }


    public Sprite GetPowerupGreyIcon(
        int powerupIndex
    )
    {
        if (powerupIcons == null)
            return null;

        for (int i = 0;
             i < powerupIcons.Length;
             i++)
        {
            if (powerupIcons[i] == null)
                continue;

            if (powerupIcons[i].powerupIndex ==
                powerupIndex)
            {
                return powerupIcons[i].greyedIcon;
            }
        }

        return null;
    }


    // =========================================================
    // POWERUP NAME
    // =========================================================

    public string GetPowerupName(
        int powerupIndex
    )
    {
        switch (powerupIndex)
        {
            case FreezeIndex:
                return "Freeze";

            case PoisonIndex:
                return "Poison";

            case RepairIndex:
                return "Repair";

            case CloneIndex:
                return "Clone";

            case ReanimateIndex:
                return "Reanimate";

            case ShockwaveIndex:
                return "Shockwave";

            case DecoyIndex:
                return "Decoy";

            default:
                return "Unknown";
        }
    }


    // =========================================================
    // VALIDATION
    // =========================================================

    public bool IsValidPowerupIndex(
        int powerupIndex
    )
    {
        return powerupIndex >= FreezeIndex &&
               powerupIndex <= DecoyIndex;
    }


    // =========================================================
    // UI NOTIFICATION
    // =========================================================

    public void NotifySlotsChanged()
    {
        OnSlotsChanged?.Invoke();

        RefreshAllActiveUI();
    }


    // =========================================================
    // REFRESH ALL POWERUP UI
    // =========================================================

    public void RefreshAllActiveUI()
    {
        // =====================================================
        // FREEZE MANAGERS
        // =====================================================

        FreezePowerupManager[] freezeManagers =
            FindObjectsByType<FreezePowerupManager>(
                FindObjectsInactive.Include,
                FindObjectsSortMode.None
            );

        foreach (FreezePowerupManager freeze in freezeManagers)
        {
            if (freeze == null)
                continue;

            freeze.LoadPowerup();

            freeze.UpdateUI();
        }


        // =====================================================
        // POISON MANAGERS
        // =====================================================

        PoisonPowerupManager[] poisonManagers =
            FindObjectsByType<PoisonPowerupManager>(
                FindObjectsInactive.Include,
                FindObjectsSortMode.None
            );

        foreach (PoisonPowerupManager poison in poisonManagers)
        {
            if (poison == null)
                continue;

            poison.LoadPowerup();

            poison.UpdateUI();
        }


        // =====================================================
        // REPAIR MANAGERS
        // =====================================================

        RepairPowerupManager[] repairManagers =
            FindObjectsByType<RepairPowerupManager>(
                FindObjectsInactive.Include,
                FindObjectsSortMode.None
            );

        foreach (RepairPowerupManager repair in repairManagers)
        {
            if (repair == null)
                continue;

            repair.LoadPowerup();

            repair.UpdateUI();
        }


        // =====================================================
        // ACTIVE HUD
        // =====================================================

        ActiveHUD[] huds =
            FindObjectsByType<ActiveHUD>(
                FindObjectsInactive.Include,
                FindObjectsSortMode.None
            );

        foreach (ActiveHUD hud in huds)
        {
            if (hud != null)
                hud.RefreshUI();
        }


        // =====================================================
        // EQUIP BUTTONS
        // =====================================================

        ActiveEquip[] equips =
            FindObjectsByType<ActiveEquip>(
                FindObjectsInactive.Include,
                FindObjectsSortMode.None
            );

        foreach (ActiveEquip equip in equips)
        {
            if (equip != null)
                equip.RefreshUI();
        }


        // =====================================================
        // DRAG UI
        // =====================================================

        ActiveDragUI[] dragUIs =
            FindObjectsByType<ActiveDragUI>(
                FindObjectsInactive.Include,
                FindObjectsSortMode.None
            );

        foreach (ActiveDragUI dragUI in dragUIs)
        {
            if (dragUI != null)
                dragUI.RefreshUI();
        }


        // =====================================================
        // DRAG UNEQUIP UI
        // =====================================================

        ActiveDragUnequip[] dragUnequips =
            FindObjectsByType<ActiveDragUnequip>(
                FindObjectsInactive.Include,
                FindObjectsSortMode.None
            );

        foreach (ActiveDragUnequip dragUnequip in dragUnequips)
        {
            if (dragUnequip != null)
                dragUnequip.RefreshUI();
        }
    }
}