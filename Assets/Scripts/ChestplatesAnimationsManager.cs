using UnityEngine;

public class ChestplateAnimationsManager : MonoBehaviour
{
    [Header("Chestplate")]
    public GameObject chestplateObject;
    public Animator chestplateAnimator;

    [Header("Chestplate Rarity Overrides")]
    public AnimatorOverrideController[] chestplateOverrides =
        new AnimatorOverrideController[5];

    private int currentChestplateTier;


    void Awake()
    {
        currentChestplateTier =
            PlayerPrefs.GetInt(
                "ChestplateTier",
                0
            );

        ApplyChestplate();
    }


    void LateUpdate()
    {
        if (chestplateAnimator == null)
            return;

        SyncParameters();
    }


    void ApplyChestplate()
    {
        if (chestplateAnimator == null)
            return;


        // No chestplate equipped
        if (currentChestplateTier == 0)
        {
            if (chestplateObject != null)
                chestplateObject.SetActive(false);

            return;
        }


        // Enable chestplate
        if (chestplateObject != null)
            chestplateObject.SetActive(true);


        // Tier 1-5
        int index = Mathf.Clamp(
            currentChestplateTier - 1,
            0,
            chestplateOverrides.Length - 1
        );


        if (chestplateOverrides[index] != null)
        {
            chestplateAnimator.runtimeAnimatorController =
                chestplateOverrides[index];
        }
    }


    void SyncParameters()
    {
        Player player =
            Player.Instance;


        if (player == null)
            return;


        Animator playerAnimator =
            player.animator;


        if (playerAnimator == null)
            return;


        // =========================
        // SYNC PARAMETERS
        // =========================

        chestplateAnimator.SetFloat(
            "InputX",
            playerAnimator.GetFloat("InputX")
        );


        chestplateAnimator.SetFloat(
            "InputY",
            playerAnimator.GetFloat("InputY")
        );


        chestplateAnimator.SetFloat(
            "LastInputX",
            playerAnimator.GetFloat("LastInputX")
        );


        chestplateAnimator.SetFloat(
            "LastInputY",
            playerAnimator.GetFloat("LastInputY")
        );


        chestplateAnimator.SetBool(
            "isWalking",
            playerAnimator.GetBool("isWalking")
        );


        chestplateAnimator.SetBool(
            "isRunning",
            playerAnimator.GetBool("isRunning")
        );


        // =========================
        // FORCE ANIMATION SYNC
        // =========================

        AnimatorStateInfo playerState =
            playerAnimator.GetCurrentAnimatorStateInfo(0);


        AnimatorStateInfo chestState =
            chestplateAnimator.GetCurrentAnimatorStateInfo(0);


        if (playerState.fullPathHash !=
            chestState.fullPathHash)
        {
            chestplateAnimator.Play(
                playerState.fullPathHash,
                0,
                playerState.normalizedTime
            );


            chestplateAnimator.Update(0f);
        }


        // Keep playback identical
        chestplateAnimator.speed =
            playerAnimator.speed;
    }


    // =========================
    // TRIGGERS
    // =========================

    public void TriggerAttack()
    {
        if (chestplateAnimator == null)
            return;


        chestplateAnimator.ResetTrigger(
            "Attack"
        );


        chestplateAnimator.SetTrigger(
            "Attack"
        );
    }


    public void TriggerHurt()
    {
        if (chestplateAnimator == null)
            return;


        chestplateAnimator.ResetTrigger(
            "Hurt"
        );


        chestplateAnimator.SetTrigger(
            "Hurt"
        );
    }


    public void TriggerDie()
    {
        if (chestplateAnimator == null)
            return;


        chestplateAnimator.ResetTrigger(
            "Die"
        );


        chestplateAnimator.SetTrigger(
            "Die"
        );
    }


    // =========================
    // RESPAWN RESET
    // =========================

    public void ResetAnimator()
    {
        if (chestplateAnimator == null)
            return;

        currentChestplateTier =
            PlayerPrefs.GetInt(
                "ChestplateTier",
                0
            );

        // If no chestplate is equipped, keep it disabled
        if (currentChestplateTier == 0)
        {
            if (chestplateObject != null)
                chestplateObject.SetActive(false);

            return;
        }

        // Make sure the GameObject is active before using the Animator
        if (chestplateObject != null)
            chestplateObject.SetActive(true);

        ApplyChestplate();

        chestplateAnimator.Rebind();
        chestplateAnimator.Update(0f);

        chestplateAnimator.Play(
            "Idle",
            0,
            0f
        );

        chestplateAnimator.speed = 1f;
    }


    public void DealAttackDamage()
    {
        // Intentionally empty
    }


    public void EndAttack()
    {
        // Intentionally empty
    }
}