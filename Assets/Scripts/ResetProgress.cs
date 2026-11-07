using UnityEngine;

public class ResetProgress : MonoBehaviour
{
    [Header("References")]
    public HouseUpgradeButton houseUpgradeButton;
    public HelmetBuyButton helmetBuyButton;


    // =========================================================
    // RESET EVERYTHING
    // =========================================================

    public void ResetEverything()
    {
        // =====================================================
        // HOUSE
        // =====================================================

        PlayerPrefs.SetInt("HouseLevel", 0);


        // =====================================================
        // ARMOR
        // =====================================================

        PlayerPrefs.SetInt("HelmetTier", 0);
        PlayerPrefs.SetInt("ChestplateTier", 0);
        PlayerPrefs.SetInt("LeggingsTier", 0);
        PlayerPrefs.SetInt("BootsTier", 0);

        PlayerPrefs.SetFloat("HelmetHPBonus", 0f);
        PlayerPrefs.SetFloat("ChestplateHPBonus", 0f);
        PlayerPrefs.SetFloat("LeggingsHPBonus", 0f);
        PlayerPrefs.SetFloat("BootsHPBonus", 0f);


        // =====================================================
        // MONEY
        // =====================================================

        PlayerPrefs.SetFloat("PlayerMoney", 0f);


        // =====================================================
        // SOULSTONES
        // =====================================================

        PlayerPrefs.SetInt("Soulstones", 0);


        // =====================================================
        // ATTACK SPEED
        // =====================================================

        PlayerPrefs.SetInt("AttackSpeedUpgradeLevel", 0);


        // =====================================================
        // ATTACK DAMAGE
        // =====================================================

        PlayerPrefs.SetInt("AttackDamageUpgradeLevel", 0);


        // =====================================================
        // ACCOUNT LEVEL + XP
        // =====================================================

        PlayerPrefs.SetInt("AccountLevel", 1);
        PlayerPrefs.SetInt("AccountXP", 0);


        // =====================================================
        // TIME PLAYED
        // =====================================================

        PlayerPrefs.SetFloat("TimePlayed", 0f);


        // =====================================================
        // NET WORTH
        // =====================================================

        PlayerPrefs.SetFloat("NetWorth", 0f);


        // =====================================================
        // HIGHEST WAVE
        // =====================================================

        PlayerPrefs.SetInt("HighestWave", 0);


        // =====================================================
        // STATS
        // =====================================================

        PlayerPrefs.SetInt("EnemiesKilled", 0);
        PlayerPrefs.SetInt("BossesKilled", 0);
        PlayerPrefs.SetInt("DamageDealt", 0);
        PlayerPrefs.SetInt("PlayerDeaths", 0);


        // =====================================================
        // ENEMY ENCYCLOPEDIA
        // =====================================================

        PlayerPrefs.SetInt("EnemyUnlocked_Scout", 0);
        PlayerPrefs.SetInt("EnemyUnlocked_Ravager", 0);
        PlayerPrefs.SetInt("EnemyUnlocked_Stalker", 0);
        PlayerPrefs.SetInt("EnemyUnlocked_Spitter", 0);
        PlayerPrefs.SetInt("EnemyUnlocked_Boss", 0);


        // =====================================================
        // INTRO
        // =====================================================

        PlayerPrefs.DeleteKey("IntroPlayed");

        PlayerPrefs.DeleteKey("HasLaunchedBefore");


        // =====================================================
        // REBIRTH
        // =====================================================

        PlayerPrefs.SetInt("RebirthCount", 0);
        PlayerPrefs.DeleteKey("RebirthIntroPlayed");


        // =====================================================
        // ACTIVE POWERUPS
        // =====================================================

        // -----------------------------------------------------
        // FREEZE
        // -----------------------------------------------------

        PlayerPrefs.SetInt(
            "Active_Freeze_Unlocked",
            0
        );

        PlayerPrefs.SetInt(
            "Active_Freeze_Level",
            0
        );


        // -----------------------------------------------------
        // POISON
        // -----------------------------------------------------

        PlayerPrefs.SetInt(
            "Active_Poison_Unlocked",
            0
        );

        PlayerPrefs.SetInt(
            "Active_Poison_Level",
            0
        );


        // -----------------------------------------------------
        // REPAIR
        // -----------------------------------------------------

        PlayerPrefs.SetInt(
            "Active_Repair_Unlocked",
            0
        );

        PlayerPrefs.SetInt(
            "Active_Repair_Level",
            0
        );


        // -----------------------------------------------------
        // ACTIVE EQUIPPED SLOTS
        // -----------------------------------------------------
        //
        // -1 = empty
        //  0 = Freeze
        //  1 = Poison
        //  2 = Repair
        //

        PlayerPrefs.SetInt("ActiveSlot_0", -1);
        PlayerPrefs.SetInt("ActiveSlot_1", -1);
        PlayerPrefs.SetInt("ActiveSlot_2", -1);


        Debug.Log("ACTIVE POWERUPS RESET.");


        // =====================================================
        // PASSIVE POWERUPS
        // =====================================================

        // -----------------------------------------------------
        // SOUL SIPHON
        // -----------------------------------------------------

        PlayerPrefs.SetInt(
            "Passive_SoulSiphon_Unlocked",
            0
        );

        PlayerPrefs.SetInt(
            "Passive_SoulSiphon_Level",
            0
        );

        PlayerPrefs.SetInt(
            "Passive_SoulSiphon_Equipped",
            0
        );


        // -----------------------------------------------------
        // ADRENALINE
        // -----------------------------------------------------

        PlayerPrefs.SetInt(
            "Passive_Adrenaline_Unlocked",
            0
        );

        PlayerPrefs.SetInt(
            "Passive_Adrenaline_Level",
            0
        );

        PlayerPrefs.SetInt(
            "Passive_Adrenaline_Equipped",
            0
        );


        // -----------------------------------------------------
        // WITHER
        // -----------------------------------------------------

        PlayerPrefs.SetInt(
            "Passive_Wither_Unlocked",
            0
        );

        PlayerPrefs.SetInt(
            "Passive_Wither_Level",
            0
        );

        PlayerPrefs.SetInt(
            "Passive_Wither_Equipped",
            0
        );


        // -----------------------------------------------------
        // RESET EQUIPPED PASSIVE INDEX
        // -----------------------------------------------------

        PlayerPrefs.SetInt(
            "EquippedPassiveIndex",
            -1
        );


        Debug.Log("ALL PASSIVE POWERUPS RESET.");


        // =====================================================
        // RESET DEMO JOURNEY
        // =====================================================

        // DemoJourneyManager is normally persistent and should
        // exist if the Demo Journey system has been initialized.
        //
        // ResetDemoJourney() handles:
        //
        // - All 10 claimed states
        // - All Demo Journey baselines
        // - Scout kills
        // - Money earned
        // - Soul Siphon activations
        // - Ravager kills
        // - Demo Finale completion
        // - Restarting the Journey at Scoutbreaker
        //

        if (DemoJourneyManager.Instance != null)
        {
            DemoJourneyManager.Instance.ResetDemoJourney();
        }
        else
        {
            // -------------------------------------------------
            // FALLBACK
            // -------------------------------------------------

            // CLAIMED STATES
            PlayerPrefs.DeleteKey("DemoJourneyClaimed_Scoutbreaker");
            PlayerPrefs.DeleteKey("DemoJourneyClaimed_RisingPower");
            PlayerPrefs.DeleteKey("DemoJourneyClaimed_FirstFortune");
            PlayerPrefs.DeleteKey("DemoJourneyClaimed_DeepFreeze");
            PlayerPrefs.DeleteKey("DemoJourneyClaimed_SoulHarvester");
            PlayerPrefs.DeleteKey("DemoJourneyClaimed_RavagersEnd");
            PlayerPrefs.DeleteKey("DemoJourneyClaimed_SecondChance");
            PlayerPrefs.DeleteKey("DemoJourneyClaimed_Fortified");
            PlayerPrefs.DeleteKey("DemoJourneyClaimed_Boneclad");
            PlayerPrefs.DeleteKey("DemoJourneyClaimed_IntoTheHollow");


            // BASELINE INITIALIZED STATES
            PlayerPrefs.DeleteKey("DemoJourneyBaselineInitialized_Scoutbreaker");
            PlayerPrefs.DeleteKey("DemoJourneyBaselineInitialized_RisingPower");
            PlayerPrefs.DeleteKey("DemoJourneyBaselineInitialized_FirstFortune");
            PlayerPrefs.DeleteKey("DemoJourneyBaselineInitialized_DeepFreeze");
            PlayerPrefs.DeleteKey("DemoJourneyBaselineInitialized_SoulHarvester");
            PlayerPrefs.DeleteKey("DemoJourneyBaselineInitialized_RavagersEnd");
            PlayerPrefs.DeleteKey("DemoJourneyBaselineInitialized_SecondChance");
            PlayerPrefs.DeleteKey("DemoJourneyBaselineInitialized_Fortified");
            PlayerPrefs.DeleteKey("DemoJourneyBaselineInitialized_Boneclad");
            PlayerPrefs.DeleteKey("DemoJourneyBaselineInitialized_IntoTheHollow");


            // INTEGER BASELINES
            PlayerPrefs.DeleteKey("DemoJourneyBaselineInt_Scoutbreaker");
            PlayerPrefs.DeleteKey("DemoJourneyBaselineInt_RisingPower");
            PlayerPrefs.DeleteKey("DemoJourneyBaselineInt_DeepFreeze");
            PlayerPrefs.DeleteKey("DemoJourneyBaselineInt_SoulHarvester");
            PlayerPrefs.DeleteKey("DemoJourneyBaselineInt_RavagersEnd");
            PlayerPrefs.DeleteKey("DemoJourneyBaselineInt_SecondChance");
            PlayerPrefs.DeleteKey("DemoJourneyBaselineInt_Fortified");
            PlayerPrefs.DeleteKey("DemoJourneyBaselineInt_Boneclad");


            // FLOAT BASELINE
            PlayerPrefs.DeleteKey(
                "DemoJourneyBaselineFloat_FirstFortune"
            );


            // TRACKING
            PlayerPrefs.DeleteKey(
                "DemoJourney_ScoutsKilled"
            );

            PlayerPrefs.DeleteKey(
                "DemoJourney_MoneyEarned"
            );

            PlayerPrefs.DeleteKey(
                "DemoJourney_SoulSiphonActivations"
            );

            PlayerPrefs.DeleteKey(
                "DemoJourney_RavagersKilled"
            );

            PlayerPrefs.DeleteKey(
                "DemoJourney_FinaleCompleted"
            );


            PlayerPrefs.Save();


            Debug.Log(
                "DEMO JOURNEY RESET USING FALLBACK."
            );
        }


        // =====================================================
        // SAVE EVERYTHING
        // =====================================================

        PlayerPrefs.Save();


        // =====================================================
        // RESET LIVE ACCOUNT LEVEL
        // =====================================================

        if (AccountLevel.Instance != null)
        {
            AccountLevel.Instance.currentLevel = 1;
            AccountLevel.Instance.currentXP = 0;
        }


        // =====================================================
        // RESET LIVE TIME PLAYED
        // =====================================================

        if (TimePlayed.Instance != null)
        {
            TimePlayed.Instance.ResetTime();
        }


        // =====================================================
        // RESET LIVE NET WORTH
        // =====================================================

        if (NetWorth.Instance != null)
        {
            NetWorth.Instance.ResetNetWorth();
        }


        // =====================================================
        // RESET LIVE HIGHEST WAVE
        // =====================================================

        if (HighestWave.Instance != null)
        {
            HighestWave.Instance.ResetHighestWave();
        }


        // =====================================================
        // RESET LIVE SOULSTONES
        // =====================================================

        if (Soulstones.Instance != null)
        {
            Soulstones.Instance.ResetSoulstones();
        }


        // =====================================================
        // RESET LIVE ENEMIES KILLED
        // =====================================================

        if (EnemiesKilled.Instance != null)
        {
            EnemiesKilled.Instance.ResetKills();
        }


        // =====================================================
        // RESET LIVE BOSSES KILLED
        // =====================================================

        if (BossesKilled.Instance != null)
        {
            BossesKilled.Instance.ResetBossKills();
        }


        // =====================================================
        // RESET LIVE DAMAGE DEALT
        // =====================================================

        if (DamageDealt.Instance != null)
        {
            DamageDealt.Instance.ResetDamage();
        }


        // =====================================================
        // RESET LIVE PLAYER DEATHS
        // =====================================================

        if (PlayerDeaths.Instance != null)
        {
            PlayerDeaths.Instance.ResetDeaths();
        }


        // =====================================================
        // RESET LIVE REBIRTH
        // =====================================================

        if (RebirthManager.Instance != null)
        {
            RebirthManager.Instance.ResetRebirthProgress();
        }


        // =====================================================
        // RESET MONEY IN LIVE PLAYER
        // =====================================================

        if (Player.Instance != null)
        {
            Player.Instance.AddMoney(
                -Player.Instance.Money
            );
        }


        // =====================================================
        // RESET LIVE ATTACK DAMAGE
        // =====================================================

        if (Player.Instance != null)
        {
            Player.Instance.attackDamage = 1f;
            Player.Instance.LoadAttackDamageStats();
        }


        // =====================================================
        // RESET LIVE ATTACK SPEED
        // =====================================================

        if (Player.Instance != null)
        {
            Player.Instance.LoadAttackSpeedStats();
        }


        // =====================================================
        // RESET ACTIVE POWERUP SYSTEM
        // =====================================================

        ResetActivePowerups();


        // =====================================================
        // RESET FREEZE EFFECTS
        // =====================================================

        ResetFreezeEffects();


        // =====================================================
        // RESET POISON PROGRESSION
        // =====================================================

        ResetPoisonProgress();


        // =====================================================
        // RESET REPAIR PROGRESSION
        // =====================================================

        ResetRepairProgress();


        // =====================================================
        // RESET ACHIEVEMENT STATS
        // =====================================================

        AchievementStats.ResetAllAchievementStats();


        // =====================================================
        // RESET ACHIEVEMENT LOGIC + UI
        // =====================================================

        if (AchievementManager.Instance != null)
        {
            AchievementManager.Instance.ResetAllAchievements();
        }


        // =====================================================
        // UPDATE DEMO JOURNEY UI
        // =====================================================

        if (DemoJourneyManager.Instance != null)
        {
            DemoJourneyManager.Instance.RefreshAllUI();
        }


        // =====================================================
        // UPDATE MONEY UI
        // =====================================================

        foreach (
            var moneyDisplay in
            Object.FindObjectsByType<MoneyDisplayTMP>(
                FindObjectsSortMode.None
            )
        )
        {
            moneyDisplay.UpdateTextFromPrefs();
        }


        // =====================================================
        // RESET HOUSE UI
        // =====================================================

        if (houseUpgradeButton != null)
        {
            houseUpgradeButton.currentLevel = 0;
            houseUpgradeButton.UpdateUI();
        }


        HouseUpgradeBars houseBars =
            FindFirstObjectByType<HouseUpgradeBars>();

        if (houseBars != null)
        {
            houseBars.UpdateBars();
        }


        // =====================================================
        // RESET HELMET UI
        // =====================================================

        if (helmetBuyButton != null)
        {
            helmetBuyButton.UpdateUI();
            helmetBuyButton.UpdateHelmetImage();
        }


        HelmetUpgradeBars helmetBars =
            FindFirstObjectByType<HelmetUpgradeBars>();

        if (helmetBars != null)
        {
            helmetBars.UpdateBars();
        }


        // =====================================================
        // RESET HOUSE OBJECT
        // =====================================================

        House house =
            FindFirstObjectByType<House>();

        if (house != null)
        {
            house.maxHealth = 10;
            house.health = 10;


            if (
                house.spriteRenderer != null &&
                house.houseLevelSprites != null &&
                house.houseLevelSprites.Length > 0
            )
            {
                house.spriteRenderer.sprite =
                    house.houseLevelSprites[0];
            }
        }


        // =====================================================
        // DEBUG
        // =====================================================

        Debug.Log("========================================");
        Debug.Log("FULL PROGRESS RESET COMPLETE");
        Debug.Log("========================================");

        Debug.Log(
            "IntroPlayed = " +
            PlayerPrefs.GetInt("IntroPlayed", 0)
        );

        Debug.Log(
            "RebirthCount = " +
            PlayerPrefs.GetInt("RebirthCount", 0)
        );

        Debug.Log(
            "RebirthIntroPlayed = " +
            PlayerPrefs.GetInt("RebirthIntroPlayed", 0)
        );

        Debug.Log(
            "AttackSpeedUpgradeLevel = " +
            PlayerPrefs.GetInt(
                "AttackSpeedUpgradeLevel",
                0
            )
        );

        Debug.Log(
            "AttackDamageUpgradeLevel = " +
            PlayerPrefs.GetInt(
                "AttackDamageUpgradeLevel",
                0
            )
        );

        Debug.Log(
            "Soulstones = " +
            PlayerPrefs.GetInt(
                "Soulstones",
                0
            )
        );

        Debug.Log(
            "HighestWave = " +
            PlayerPrefs.GetInt(
                "HighestWave",
                0
            )
        );

        Debug.Log(
            "Starting Wave = " +
            (
                PlayerPrefs.GetInt(
                    "HighestWave",
                    0
                ) + 1
            )
        );

        Debug.Log(
            "Enemy Encyclopedia Reset: All enemies locked"
        );

        Debug.Log("Achievement Stats Reset");
        Debug.Log("Achievement Claims Reset");
        Debug.Log("Achievement UI Reset");
        Debug.Log("Achievement Stars Reset");
        Debug.Log("Passive Powerups Reset");
        Debug.Log("Active Powerups Reset");
        Debug.Log("Freeze Reset");
        Debug.Log("Poison Reset");
        Debug.Log("Repair Reset");
        Debug.Log("Equipped Passive Reset");
        Debug.Log("Demo Journey Reset");


        // =====================================================
        // RESET STARTUP FLAGS FOR NEXT LAUNCH
        // =====================================================

        PlayerPrefs.DeleteKey("IntroPlayed");
        PlayerPrefs.DeleteKey("HasLaunchedBefore");
        PlayerPrefs.DeleteKey("RebirthIntroPlayed");
        PlayerPrefs.DeleteKey("TutorialCompleted");

        PlayerPrefs.Save();

        Debug.Log(
            "FULL PROGRESS RESET COMPLETE. Current scene preserved. " +
            "Tutorial will play on next startup."
        );

    }


    // =========================================================
    // RESET ACTIVE POWERUPS
    // =========================================================

    private void ResetActivePowerups()
    {
        // -----------------------------------------------------
        // RESET ACTUAL ACTIVE POWERUP MANAGER
        // -----------------------------------------------------

        if (ActivePowerupManager.Instance != null)
        {
            ActivePowerupManager.Instance.ResetAllSlots();
        }
        else
        {
            PlayerPrefs.SetInt("ActiveSlot_0", -1);
            PlayerPrefs.SetInt("ActiveSlot_1", -1);
            PlayerPrefs.SetInt("ActiveSlot_2", -1);

            PlayerPrefs.Save();
        }


        // -----------------------------------------------------
        // RESET EVERY ACTIVE HUD
        // -----------------------------------------------------

        ActiveHUD[] huds =
            Object.FindObjectsByType<ActiveHUD>(
                FindObjectsInactive.Include,
                FindObjectsSortMode.None
            );


        for (int i = 0; i < huds.Length; i++)
        {
            if (huds[i] == null)
                continue;

            huds[i].UpdateHUD();
        }


        // -----------------------------------------------------
        // RESET ACTIVE EQUIP MENUS
        // -----------------------------------------------------

        ActiveEquip[] equips =
            Object.FindObjectsByType<ActiveEquip>(
                FindObjectsInactive.Include,
                FindObjectsSortMode.None
            );


        for (int i = 0; i < equips.Length; i++)
        {
            if (equips[i] == null)
                continue;

            equips[i].RefreshUI();
        }


        // -----------------------------------------------------
        // RESET DRAG-TO-EQUIP UI
        // -----------------------------------------------------

        ActiveDragUI[] dragUIs =
            Object.FindObjectsByType<ActiveDragUI>(
                FindObjectsInactive.Include,
                FindObjectsSortMode.None
            );


        for (int i = 0; i < dragUIs.Length; i++)
        {
            if (dragUIs[i] == null)
                continue;

            dragUIs[i].RefreshUI();
        }


        // -----------------------------------------------------
        // RESET DRAG-TO-UNEQUIP UI
        // -----------------------------------------------------

        ActiveDragUnequip[] dragUnequips =
            Object.FindObjectsByType<ActiveDragUnequip>(
                FindObjectsInactive.Include,
                FindObjectsSortMode.None
            );


        for (int i = 0; i < dragUnequips.Length; i++)
        {
            if (dragUnequips[i] == null)
                continue;

            dragUnequips[i].RefreshUI();
        }


        Debug.Log(
            "ACTIVE POWERUP SLOTS + ALL ACTIVE SLOT UI RESET."
        );
    }


    // =========================================================
    // RESET FREEZE EFFECTS
    // =========================================================

    private void ResetFreezeEffects()
    {
        // -----------------------------------------------------
        // RESET FREEZE POWERUP RUNTIME STATE
        // -----------------------------------------------------

        FreezePowerup[] freezePowerups =
            Object.FindObjectsByType<FreezePowerup>(
                FindObjectsInactive.Include,
                FindObjectsSortMode.None
            );


        for (int i = 0; i < freezePowerups.Length; i++)
        {
            if (freezePowerups[i] == null)
                continue;

            freezePowerups[i].ResetFreeze();
        }


        // -----------------------------------------------------
        // RESET FREEZE VISUAL EFFECTS
        // -----------------------------------------------------

        FreezePowerupEffects[] effects =
            Object.FindObjectsByType<FreezePowerupEffects>(
                FindObjectsInactive.Include,
                FindObjectsSortMode.None
            );


        for (int i = 0; i < effects.Length; i++)
        {
            if (effects[i] == null)
                continue;

            effects[i].StopAllCoroutines();
        }


        // -----------------------------------------------------
        // REMOVE ALL DYNAMIC FREEZE SNOW
        // -----------------------------------------------------

        GameObject[] allObjects =
            Resources.FindObjectsOfTypeAll<GameObject>();


        for (int i = 0; i < allObjects.Length; i++)
        {
            GameObject obj =
                allObjects[i];

            if (obj == null)
                continue;


            if (obj.name.StartsWith("FreezeCircle"))
            {
                if (obj.scene.IsValid())
                {
                    Destroy(obj);
                }
            }
        }


        // -----------------------------------------------------
        // RESET FREEZE TINT
        // -----------------------------------------------------

        FreezePowerupEffects[] freezeEffects =
            Object.FindObjectsByType<FreezePowerupEffects>(
                FindObjectsInactive.Include,
                FindObjectsSortMode.None
            );


        for (int i = 0; i < freezeEffects.Length; i++)
        {
            if (freezeEffects[i] == null)
                continue;


            if (freezeEffects[i].freezeTintImage != null)
            {
                Color color =
                    freezeEffects[i]
                        .freezeTintImage
                        .color;

                color.a = 0f;

                freezeEffects[i]
                    .freezeTintImage
                    .color = color;
            }
        }


        Debug.Log(
            "FREEZE RUNTIME + VISUAL EFFECTS RESET."
        );
    }


    // =========================================================
    // RESET POISON PROGRESSION
    // =========================================================

    private void ResetPoisonProgress()
    {
        // -----------------------------------------------------
        // RESET POISON SAVE DATA
        // -----------------------------------------------------

        PlayerPrefs.SetInt(
            "Active_Poison_Unlocked",
            0
        );

        PlayerPrefs.SetInt(
            "Active_Poison_Level",
            0
        );


        // -----------------------------------------------------
        // RESET POISON MANAGER UI IF PRESENT
        // -----------------------------------------------------

        PoisonPowerupManager[] managers =
            Object.FindObjectsByType<PoisonPowerupManager>(
                FindObjectsInactive.Include,
                FindObjectsSortMode.None
            );


        for (int i = 0; i < managers.Length; i++)
        {
            if (managers[i] == null)
                continue;

            managers[i].ReloadPowerup();
        }


        // -----------------------------------------------------
        // RESET POISON SAVE AGAIN
        // -----------------------------------------------------

        PlayerPrefs.Save();


        Debug.Log(
            "POISON PROGRESSION RESET."
        );
    }


    // =========================================================
    // RESET REPAIR PROGRESSION
    // =========================================================

    private void ResetRepairProgress()
    {
        // -----------------------------------------------------
        // RESET REPAIR SAVE DATA
        // -----------------------------------------------------

        PlayerPrefs.SetInt(
            "Active_Repair_Unlocked",
            0
        );

        PlayerPrefs.SetInt(
            "Active_Repair_Level",
            0
        );


        // -----------------------------------------------------
        // RESET REPAIR POWERUP RUNTIME STATE
        // -----------------------------------------------------

        RepairPowerup[] repairPowerups =
            Object.FindObjectsByType<RepairPowerup>(
                FindObjectsInactive.Include,
                FindObjectsSortMode.None
            );


        for (int i = 0; i < repairPowerups.Length; i++)
        {
            if (repairPowerups[i] == null)
                continue;

            repairPowerups[i].ResetRepair();
        }


        // -----------------------------------------------------
        // RESET REPAIR MANAGER UI IF PRESENT
        // -----------------------------------------------------

        RepairPowerupManager[] managers =
            Object.FindObjectsByType<RepairPowerupManager>(
                FindObjectsInactive.Include,
                FindObjectsSortMode.None
            );


        for (int i = 0; i < managers.Length; i++)
        {
            if (managers[i] == null)
                continue;

            managers[i].ReloadPowerup();
        }


        // -----------------------------------------------------
        // RESET REPAIR SAVE AGAIN
        // -----------------------------------------------------

        PlayerPrefs.Save();


        Debug.Log(
            "REPAIR PROGRESSION + RUNTIME STATE RESET."
        );
    }
}